using System;
using AntdUi.AppConfig;
using Nancy.Hosting.Self;

namespace AntdUi {
    internal class Application {
        public static void Main() {
            var app = new AppConfiguration().Get();
            var uri = $"http://localhost:{app.AntdUiPort}/";
            var host = new NancyHost(new Uri(uri));
            host.Start();
            Console.WriteLine($"Application running on {uri}");
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
