using Nancy.Hosting.Self;
using System;
using anthilla.core;
using System.Threading;
using System.Diagnostics;
using Nancy;
using System.IO;
using Antd;
using anthilla.scheduler;

namespace AntdUi {
    internal class Application {

        public static string KeyName = "antdui";

        public static MachineConfig CurrentConfiguration;

        public static JobManager Scheduler;
        public static Stopwatch STOPWATCH;

        public static string ServerUrl;

        public static void Main() {
            var resetEvent = new AutoResetEvent(initialState: false);
            Console.CancelKeyPress += (s, e) => { e.Cancel = true; resetEvent.Set(); };
            STOPWATCH = new Stopwatch();
            STOPWATCH.Start();
            ConsoleLogger.Log($"[{KeyName}] start");
            Scheduler = new JobManager();
            var urlFile = $"{Parameter.AntdCfg}/host_reference";
            while(!File.Exists(urlFile)) {
                Thread.Sleep(500);
                ConsoleLogger.Warn($"[{KeyName}] waiting for server");
            }
            while(string.IsNullOrEmpty(File.ReadAllText(urlFile))) {
                Thread.Sleep(500);
                ConsoleLogger.Warn($"[{KeyName}] waiting for server");
            }
            ServerUrl = File.ReadAllText(urlFile);
            CurrentConfiguration = Help.GetCurrentConfiguration();
            while(CurrentConfiguration == null) {
                ConsoleLogger.Warn($"[{KeyName}] waiting for server");
                Thread.Sleep(500);
                CurrentConfiguration = Help.GetCurrentConfiguration();
            }
            var port = CurrentConfiguration.WebService.GuiWebServicePort;
            var uri = $"http://localhost:{port}/";
            var webService = new NancyHost(new Uri(uri));
            webService.Start();
            StaticConfiguration.DisableErrorTraces = false;
            ConsoleLogger.Log($"[{KeyName}] web service is listening on port {port}");
            ConsoleLogger.Log($"[{KeyName}] loaded in: {STOPWATCH.ElapsedMilliseconds} ms");

            #region [    Set Parameters    ]
            ServerUrl = CommonString.Append(CurrentConfiguration.WebService.Protocol, "://", CurrentConfiguration.WebService.Host, ":", CurrentConfiguration.WebService.Port.ToString());
            #endregion

            resetEvent.WaitOne();
            webService.Stop();
            STOPWATCH.Stop();
            ConsoleLogger.Log($"[{KeyName}] stop");
            Environment.Exit(0);
        }
    }
}
