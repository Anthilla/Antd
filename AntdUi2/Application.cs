using AntdUi2.Web;
using Microsoft.AspNetCore.Hosting;
using System;
using System.IO;
using System.Threading;

namespace AntdUi2 {
    /// <summary>
    /// Contiene il metodo Main dell'applicazione
    /// </summary>
    public partial class Application {

        public const string SessionCookieKey = "session-antd";
        public const string SessionRequestCookieKey = "session-antd-req";

        public string ServerUrl { get; set; }
        public static RestConsumer RestConsumer { get; set; }

        public static void Main(string[] args) {
            var resetEvent = new AutoResetEvent(false);
            Console.CancelKeyPress += (s, e) => { e.Cancel = true; resetEvent.Set(); };

            RestConsumer = new RestConsumer("http://localhost:8085", SessionCookieKey, "Antd");

            var url = $"http://localhost:8006";
            var host = new WebHostBuilder()
              .UseContentRoot(Directory.GetCurrentDirectory())
              .UseKestrel()
              .UseStartup<Startup>()
              .UseUrls(url)
              .Build();
            host.Run();

            resetEvent.WaitOne();
            host.Dispose();
            resetEvent.Dispose();
        }
    }
}
