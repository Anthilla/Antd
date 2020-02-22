using Antd2.Configuration;
using NWebDav.Server;
using NWebDav.Server.Http;
using NWebDav.Server.HttpListener;
using NWebDav.Server.Stores;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;

namespace Antd2.Storage {
    public class WebDavServer : IDisposable {

        private readonly string Realm;
        private readonly string WebdavIp;
        private readonly int WebdavPort;
        private readonly bool UseHttps;

        /// <summary>
        /// Target directory
        /// </summary>
        private readonly string Root;

        private readonly List<WebdavUser> Users;

        private readonly IDictionary<string, WebDavDispatcher> WEBDAV_DISPATCHERS = new Dictionary<string, WebDavDispatcher>();

        private readonly CancellationTokenSource CancellationTokenSource;
        private HttpListener HttpListener;

        public WebDavServer(string realm, string webdavIp, int webdavPort,
            string root, string user, string password,
            bool useHttps = false) {
            if (!Directory.Exists(root)) {
                throw new DirectoryNotFoundException($"Cannot init Webdav server on this root {root}");
            }
            Realm = realm;
            WebdavIp = webdavIp;
            WebdavPort = webdavPort;
            UseHttps = useHttps;
            CancellationTokenSource = new CancellationTokenSource();

            Root = root;

            Users.Add(new WebdavUser() { User = user, Password = password });
        }

        public WebDavServer(WebdavParameters webdavConf, bool useHttps = false) {

            if (!Directory.Exists(webdavConf.Target)) {
                throw new DirectoryNotFoundException($"Cannot init Webdav server on this root {webdavConf.Target}");
            }
            Realm = webdavConf.Target.Replace("/", "_");
            WebdavIp = webdavConf.Address;
            WebdavPort = webdavConf.Port;
            UseHttps = useHttps;
            CancellationTokenSource = new CancellationTokenSource();

            Root = webdavConf.Target;
            Users = webdavConf.MappedUsers;
        }

        public void Start() {
            using (HttpListener = new System.Net.HttpListener()) {
                var webdavProtocol = UseHttps ? "https" : "http";
                HttpListener.Prefixes.Add($"{webdavProtocol}://{WebdavIp}:{WebdavPort}/");
                Console.WriteLine($"[webdav] listening on {webdavProtocol}://{WebdavIp}:{WebdavPort}/");
                HttpListener.AuthenticationSchemes = AuthenticationSchemes.Basic;
                HttpListener.Realm = Realm;
                Console.WriteLine($"[webdav] starting");
                HttpListener.Start();
                Console.WriteLine($"[webdav] started");
                DispatchHttpRequestsAsync(HttpListener, CancellationTokenSource.Token);
                while (true)
                    ;
            }
        }

        public void Stop() {
            CancellationTokenSource.Cancel();
            CancellationTokenSource.Dispose();
            Console.WriteLine($"[webdav] stop listening on http://{WebdavIp}:{WebdavPort}/");
        }

        private async void DispatchHttpRequestsAsync(System.Net.HttpListener httpListener, CancellationToken cancellationToken) {
            var requestHandlerFactory = new RequestHandlerFactory();

            var nullDispatcher = new WebDavDispatcher(new DiskStore("/tmp/empty"), requestHandlerFactory);

            WebDavDispatcher webDavDispatcher;
            HttpListenerContext httpListenerContext;
            while (!cancellationToken.IsCancellationRequested &&
                (httpListenerContext = await httpListener.GetContextAsync().ConfigureAwait(false)) != null) {
                IHttpContext httpContext;
                Console.WriteLine(httpListenerContext.Request.IsAuthenticated);
                try {
                    var id = httpListenerContext.User.Identity as HttpListenerBasicIdentity;

                    var confUser = Users.FirstOrDefault(_ => _.User == id.Name && _.Password == id.Password);

                    if (confUser != null) {
                        Console.WriteLine($"[webdav] manage {id.Name}");
                        if (!WEBDAV_DISPATCHERS.ContainsKey(id.Name)) {
                            Console.WriteLine($"[webdav] init dispatcher for {id.Name} at {Root}");
                            WEBDAV_DISPATCHERS[id.Name] = new WebDavDispatcher(new DiskStore(Root), requestHandlerFactory);
                        }
                        httpContext = new HttpBasicContext(httpListenerContext,
                           checkIdentity: i => i.Name == confUser.User && i.Password == confUser.Password);
                        webDavDispatcher = WEBDAV_DISPATCHERS[id.Name];
                    }
                    else {
                        httpContext = new HttpContext(httpListenerContext);
                        webDavDispatcher = nullDispatcher;
                    }
                    await webDavDispatcher.DispatchRequestAsync(httpContext).ConfigureAwait(false);
                }
                catch (Exception ex) {
                    Console.WriteLine(ex.ToString());
                }
            }
        }

        private static readonly object _syncLock = new object();
        private readonly bool _disposed = false;

        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private protected virtual void Dispose(bool disposing) {
            if (_disposed)
                return;
            if (disposing) {
                lock (_syncLock) {
                    CancellationTokenSource.Cancel();
                    CancellationTokenSource.Dispose();
                    HttpListener.Close();
                }
            }
        }

    }
}
