using System;
using AntdUi.AppConfig;
using Nancy;
using Nancy.Hosting.Self;

namespace AntdUi {
    internal class Application {
        public static void Main() {
            var app = new AppConfiguration().Get();
            var uri = $"http://localhost:{app.Port}/";

            var host = new NancyHost(new Uri(uri));
            host.Start();
            StaticConfiguration.DisableErrorTraces = false;
            Console.WriteLine($"Application running on {uri}");

            Configuration.Setup();

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
