using System;
using System.Diagnostics;
using Nancy;
using Nancy.Conventions;
using Nancy.Hosting.Self;
using anthilla.core;

namespace AnthillaTest {
    internal class Application {

        public static void Main(string[] args) {
            string port = "8080";
            if(args.Length > 0) {
                if(!string.IsNullOrEmpty(args[0])) {
                    port = args[0];
                }
            }
            var host = new NancyHost(new Uri($"http://localhost:{port}/"));
            host.Start();
            Console.WriteLine($"host listening on port: {port}");
            KeepAlive();
            host.Stop();
        }

        private static void KeepAlive() {
            var r = Console.ReadLine();
            while(r != "quit") {
                r = Console.ReadLine();
            }
        }
    }

    public class Bootstrapper : DefaultNancyBootstrapper {
        protected override void ConfigureConventions(NancyConventions conv) {
            base.ConfigureConventions(conv);
            conv.StaticContentsConventions.Clear();
            conv.StaticContentsConventions.AddDirectory("f", @"src");
        }
    }

    public class Times {
        public string App { get; set; }
        public string Hw { get; set; }
    }

    public class HomeModule : NancyModule {
        public HomeModule() {
            Get["/"] = _ => View["home"];

            Get["/host"] = _ => {
                return Response.AsText(Hostname());
            };

            Get["/date"] = _ => {
                return Response.AsJson(new Times { App = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), Hw = Hwclock() });
            };

            Get["/cls"] = x => {
                return ApiConsumer.GetJson("http://localhost:8086/cluster");
            };

            Post["/ok"] = x => {
                return ApiConsumer.Post("http://localhost:8086/device/ok");
            };

            Get["/chk"] = x => {
                return ApiConsumer.Post("http://localhost:8086/device/checklist");
            };

            Get["/clschk"] = x => {
                return ApiConsumer.Post("http://localhost:8086/device/clusterchecklist");
            };
        }

        public static string Hwclock() {
            try {
                var proc = new ProcessStartInfo {
                    FileName = "/bin/bash",
                    Arguments = "-c \"date \'+%Y/%m/%d %H:%M:%S\'\"",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false
                };
                using(var p = new Process()) {
                    p.StartInfo = proc;
                    p.Start();
                    string output;
                    using(var streamReader = p.StandardOutput) {
                        output = streamReader.ReadToEnd();
                    }
                    p.WaitForExit(1000 * 30);
                    return output;
                }
            }
            catch(Exception) {
                return string.Empty;
            }
        }

        public static string Hostname() {
            try {
                var proc = new ProcessStartInfo {
                    FileName = "/bin/bash",
                    Arguments = "-c \"hostname\"",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false
                };
                using(var p = new Process()) {
                    p.StartInfo = proc;
                    p.Start();
                    string output;
                    using(var streamReader = p.StandardOutput) {
                        output = streamReader.ReadToEnd();
                    }
                    p.WaitForExit(1000 * 30);
                    return output;
                }
            }
            catch(Exception) {
                return "no-name";
            }
        }
    }
}
