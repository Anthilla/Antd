///-------------------------------------------------------------------------------------
///     Copyright (c) 2014, Anthilla S.r.l. (http://www.anthilla.com)
///     All rights reserved.
///
///     Redistribution and use in source and binary forms, with or without
///     modification, are permitted provided that the following conditions are met:
///         * Redistributions of source code must retain the above copyright
///           notice, this list of conditions and the following disclaimer.
///         * Redistributions in binary form must reproduce the above copyright
///           notice, this list of conditions and the following disclaimer in the
///           documentation and/or other materials provided with the distribution.
///         * Neither the name of the Anthilla S.r.l. nor the
///           names of its contributors may be used to endorse or promote products
///           derived from this software without specific prior written permission.
///
///     THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
///     ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
///     WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
///     DISCLAIMED. IN NO EVENT SHALL ANTHILLA S.R.L. BE LIABLE FOR ANY
///     DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
///     (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
///     LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
///     ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
///     (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
///     SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
///
///     20141110
///-------------------------------------------------------------------------------------

using Antd.Common;
using Antd.Status;
using Microsoft.AspNet.SignalR;
using Microsoft.Owin.Hosting;
using Nancy;
using Owin;
using System;
using System.IO;
using System.Threading;

namespace Antd {

    internal static class Program {

        private static void Main(string[] args) {
            DateTime startTime = DateTime.Now;
            Console.Title = "ANTD";
            ConsoleLogger.Log("loading application...");

            SystemSetupBoot.Start();
            ConsoleLogger.Log("applying system configuration...");

            var stop = new ManualResetEvent(false);
            Console.CancelKeyPress +=
                (sender, e) => {
                    Console.WriteLine("^C");
                    Database.ShutDown();
                    stop.Set();
                    e.Cancel = true;
                };
            string uri = SelfConfig.GetAntdUri();
            ConsoleLogger.Log("initializing antd");
            //try {
            using (WebApp.Start<Startup>(uri)) {
                ConsoleLogger.Log("loading service");
                ConsoleLogger.Log("    service type -> server");
                ConsoleLogger.Log("                 -> server url -> {0}", uri);
                ConsoleLogger.Log("service is now running");
                var elapsed = DateTime.Now - startTime;
                ConsoleLogger.Log("loaded in: {0}", elapsed);

                //ConsoleLogger.Log("");
                //ServiceUnitInfo.SetDefaultUnitInfo();
                //ConsoleLogger.Log("misc -> default unit info saved to database");

                //UnitFile.WriteForSelf();
                //ConsoleLogger.Log("self -> unit file created");

                //Console.WriteLine("");
                //string[] watchThese = new string[] {
                //    "/cfg",
                //    "/proc/sys",
                //    "/sys/class/net"
                //};
                //foreach (string folder in watchThese) {
                //    new DirectoryWatcher(folder).Watch();
                //    ConsoleLogger.Log("watcher enabled for {0}", folder);
                //}

                stop.WaitOne();
            }
            /*} catch (System.Reflection.TargetInvocationException ex) {
                ConsoleLogger.Warn(ex.Message);
                ConsoleLogger.Warn("Register +: urlacl");
                ConsoleLogger.Warn("on windows:");
                ConsoleLogger.Warn("netsh http add urlacl url=http://+:7777/ user=UserName");
            }*/
        }
    }

    internal class Startup {

        public void Configuration(IAppBuilder app) {
            ConsoleLogger.Log("setting default configuration...");
            SelfConfig.WriteDefaults();
            ConsoleLogger.Log("    antd configuration -> saved");

            //JobScheduler.Start(false);
            //ConsoleLogger.Log("     scheduler -> loaded");

            //Sysctl.WriteConfig();
            //ConsoleLogger.Log("     sysctl.config -> created");
            //Sysctl.LoadConfig();
            //ConsoleLogger.Log("     sysctl.config -> loaded");

            //Mount.WriteConfig();
            //ConsoleLogger.Log("     mounts -> created");

            Networkd.EnableRequiredServices();
            ConsoleLogger.Log("    networkd -> enabled");
            Networkd.MountNetworkdDir();
            ConsoleLogger.Log("    networkd -> mounted");
            Networkd.CreateFirstUnit();
            ConsoleLogger.Log("    networkd -> unit created");
            Networkd.RestartNetworkdDir();
            ConsoleLogger.Log("    networkd -> applied");
            ConsoleLogger.Log(Networkd.StatusNetworkdDir());

            Command.Launch("chmod", "777 *.xml");
            ConsoleLogger.Log("    check configuration...");
            SystemSetupBoot.Start();
            ConsoleLogger.Log("    save configuration...");

            ConsoleLogger.Log("loading service configuration");
            var hubConfiguration = new HubConfiguration { EnableDetailedErrors = false };
            app.MapSignalR(hubConfiguration);
            ConsoleLogger.Log("    signalR -> loaded");
            bool errorTrace = false;
            StaticConfiguration.DisableErrorTraces = errorTrace;
            ConsoleLogger.Log("    disableerrortraces -> {0}", errorTrace);
            Database.Start();
            ConsoleLogger.Log("    denso-db -> loaded");
            app.UseNancy();
            ConsoleLogger.Log("    nancy-fx -> loaded");
        }
    }

    public class Database {

        public static void Start() {
            string[] databases;

            string root = SelfConfig.GetAntdDb();
            if (!Directory.Exists(root)) {
                Directory.CreateDirectory(root);
            }
            databases = new string[] { root };
            ConsoleLogger.Log("        database path(s): {0}", String.Join(", ", databases));

            DeNSo.Configuration.BasePath = databases;
            DeNSo.Configuration.EnableJournaling = true;
            DeNSo.Configuration.DBCheckTimeSpan = new System.TimeSpan(0, 1, 0);
            DeNSo.Configuration.ReindexCheck = new System.TimeSpan(0, 1, 0);
            DeNSo.Configuration.EnableOperationsLog = false;
            string db = "AntDB";
            DeNSo.Session.DefaultDataBase = db;
            DeNSo.Session.Start();
        }

        public static void ShutDown() {
            DeNSo.Session.ShutDown();
        }
    }
}