using System;
using antdlib.common;
using AntdUi.AppConfig;
using Nancy.Hosting.Self;

namespace AntdUi {
    internal class Application {
        public static void Main() {
            ConsoleLogger.Log("starting antdui");
            var startTime = DateTime.Now;
            var app = new AppConfiguration().Get();
            var port = app.AntdUiPort;
            var uri = $"http://localhost:{app.AntdUiPort}/";
            var host = new NancyHost(new Uri(uri));
            host.Start();
            ConsoleLogger.Log("host ready");
            ConsoleLogger.Log($"http port: {port}");
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
