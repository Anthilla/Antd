using antdlib.config.shared;
using Nancy.Hosting.Self;
using System;
using anthilla.core;
using System.Linq;
using System.Threading;

namespace AntdUi {
    internal class Application {

        public static string KeyName = "antdui";
        public static int Port;
        public static int ServerPort;

        public static void Main() {
            var resetEvent = new AutoResetEvent(initialState: false);
            Console.CancelKeyPress += (s, e) => { e.Cancel = true; resetEvent.Set(); };
            ConsoleLogger.Log("starting antdui");
            var startTime = DateTime.Now;
            var app = new AppConfiguration().Get();
            Port = app.AntdUiPort;
            ServerPort = app.AntdPort;
            var uri = $"http://localhost:{Port}/";
            var host = new NancyHost(new Uri(uri));
            host.Start();
            ConsoleLogger.Log("host ready");
            ConsoleLogger.Log($"http port: {Port}");
            ConsoleLogger.Log("antdui is running");
            ConsoleLogger.Log($"loaded in: {DateTime.Now - startTime}");
            Test();
            resetEvent.WaitOne();
            //Common.KeepAlive();
            Console.WriteLine("Stopping...");
            host.Stop();
        }

        private static void Test() {
       
        }
    }
}
