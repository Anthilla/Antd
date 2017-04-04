using antdlib.common;
using antdlib.config.shared;
using Nancy.Hosting.Self;
using System;

namespace AntdUi {
    internal class Application {

        public static int Port;
        public static int ServerPort;

        public static void Main() {
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
            KeepAlive();
            Console.WriteLine("Stopping...");
            host.Stop();
        }

        private static void KeepAlive() {
            var r = Console.ReadLine();
            while(r != "quit") {
                r = Console.ReadLine();
            }
        }
    }
}
