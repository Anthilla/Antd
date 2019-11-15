using NWebDav.Server;
using NWebDav.Server.Http;
using NWebDav.Server.HttpListener;
using NWebDav.Server.Stores;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;

namespace Antd2.Storage {
    public class WebDavServer : IDisposable {

        private readonly string Realm;
        private readonly string WebdavIp;
        private readonly int WebdavPort;
        private readonly bool UseHttps;

        private readonly IDictionary<string, WebDavDispatcher> WEBDAV_DISPATCHERS = new Dictionary<string, WebDavDispatcher>();

        private readonly CancellationTokenSource CancellationTokenSource;
        private HttpListener HttpListener;

        public WebDavServer(string realm, string webdavIp, int webdavPort, bool useHttps = false) {
            Realm = realm;
            WebdavIp = webdavIp;
            WebdavPort = webdavPort;
            UseHttps = useHttps;
            CancellationTokenSource = new CancellationTokenSource();
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

                while (Console.ReadKey().KeyChar != 'x')
                    ;
            }
            Console.WriteLine($"[webdav] closing");
        }

        public void Stop() {
            CancellationTokenSource.Cancel();
            CancellationTokenSource.Dispose();
        }

        private WebdavUser AuthenticateUser(string username, string password) {
            if (string.IsNullOrEmpty(username)) {
                throw new ArgumentException("message", nameof(username));
            }

            if (string.IsNullOrEmpty(password)) {
                throw new ArgumentException("message", nameof(password));
            }
            Console.WriteLine($"[webdav] auth {username}");

            if (username != "visor") {
                Console.WriteLine($"[webdav] auth {username} failed");
                return null;
            }
            if (password != "AnthillaDev2019!!") {
                Console.WriteLine($"[webdav] auth {password} failed");
                return null;
            }

            return new WebdavUser() {
                Alias = "visor",
                Root = "/Data/Data01"
            };
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
                //if (httpListenerContext.Request.IsAuthenticated) {
                try {
                    var id = httpListenerContext.User.Identity as HttpListenerBasicIdentity;
                    var webdavUser = AuthenticateUser(id.Name, id.Password);
                    if (webdavUser != null) {
                        Console.WriteLine($"[webdav] manage {id.Name}");
                        if (!WEBDAV_DISPATCHERS.ContainsKey(id.Name)) {
                            Console.WriteLine($"[webdav] init dispatcher for {id.Name} at {webdavUser.Root}");
                            WEBDAV_DISPATCHERS[id.Name] = new WebDavDispatcher(new DiskStore(webdavUser.Root), requestHandlerFactory);
                        }
                        httpContext = new HttpBasicContext(httpListenerContext,
                           checkIdentity: i => i.Name == id.Name && i.Password == id.Password);
                        webDavDispatcher = WEBDAV_DISPATCHERS[id.Name];
                        //await WEBDAV_DISPATCHERS[webdavUser.UserGuid].DispatchRequestAsync(httpContext).ConfigureAwait(false);
                    }
                    else {
                        httpContext = new HttpContext(httpListenerContext);
                        webDavDispatcher = nullDispatcher;
                    }
                    //}
                    //else {
                    //httpContext = new HttpContext(httpListenerContext);
                    //webDavDispatcher = nullDispatcher;
                    //}
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
