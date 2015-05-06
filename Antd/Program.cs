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

using Antd.Scheduler;
using Antd.UnitFiles;
using Microsoft.AspNet.SignalR;
using Microsoft.Owin.Hosting;
using Nancy;
using Owin;
using System;
using System.Threading;

namespace Antd {

    internal static class Program {

        private static void Main(string[] args) {
            DateTime startTime = DateTime.Now;
            Console.Title = "ANTD";
            Console.WriteLine(ConsoleTime.GetTime(DateTime.Now) + "loading application...");

            SystemConfig.FirstLaunchDefaults();
            Console.WriteLine(ConsoleTime.GetTime(DateTime.Now) + "setting core system configuration...");

            Cfg.FirstLaunchDefaults();
            Console.WriteLine(ConsoleTime.GetTime(DateTime.Now) + "setting core cfg configuration...");

            var stop = new ManualResetEvent(false);
            Console.CancelKeyPress +=
                (sender, e) => {
                    Console.WriteLine("^C");
                    Database.ShutDown();
                    stop.Set();
                    e.Cancel = true;
                };
            string uri = SelfConfig.GetAntdUri();
            Console.WriteLine(ConsoleTime.GetTime(DateTime.Now) + "initializing antd");
            using (WebApp.Start<Startup>(uri)) {
                Console.WriteLine(ConsoleTime.GetTime(DateTime.Now) + "loading service");
                Console.WriteLine(ConsoleTime.GetTime(DateTime.Now) + "    service type -> server");
                Console.WriteLine(ConsoleTime.GetTime(DateTime.Now) + "                 -> server url -> {0}", uri);
                Console.WriteLine(ConsoleTime.GetTime(DateTime.Now) + "service is now running");
                var elapsed = DateTime.Now - startTime;
                Console.WriteLine(ConsoleTime.GetTime(DateTime.Now) + "loaded in: " + elapsed);



                //create basic custom sysctl.config -> atm is == to local sysctl.config
                Sysctl.Sysctl.WriteConfig();
                Console.WriteLine(ConsoleTime.GetTime(DateTime.Now) + "XX sysctl.config -> created");
                //load custom sysctl.config
                Sysctl.Sysctl.LoadConfig();
                Console.WriteLine(ConsoleTime.GetTime(DateTime.Now) + "XX sysctl.config -> loaded");
                
                //Console.WriteLine("");
                //ServiceUnitInfo.SetDefaultUnitInfo();
                //Console.WriteLine(ConsoleTime.GetTime(DateTime.Now) + "misc -> default unit info saved to database");

                //UnitFile.WriteForSelf();
                //Console.WriteLine(ConsoleTime.GetTime(DateTime.Now) + "self -> unit file created");

                //Systemctl.Enable("antd.service");
                //Console.WriteLine(ConsoleTime.GetTime(DateTime.Now) + "self -> unit file enabled");

                //Console.WriteLine("");
                //string[] watchThese = new string[] { 
                //    "/cfg",
                //    "/proc/sys",
                //    "/sys/class/net"
                //};
                //foreach (string folder in watchThese) {
                //    new DirectoryWatcher(folder).Watch();
                //    Console.WriteLine(ConsoleTime.GetTime(DateTime.Now) + "watcher enabled for {0}", folder);
                //}

                stop.WaitOne();
            }
        }
    }

    internal class Startup {

        public void Configuration(IAppBuilder app) {
            //write defaults and stuff
            Console.WriteLine(ConsoleTime.GetTime(DateTime.Now) + "setting default configuration...");
            SelfConfig.WriteDefaults();
            Console.WriteLine(ConsoleTime.GetTime(DateTime.Now) + "    set configuration for: antd...");
            SystemConfig.WriteDefaults();
            Console.WriteLine(ConsoleTime.GetTime(DateTime.Now) + "    set configuration for: system...");
            Cfg.LaunchDefaults();
            Console.WriteLine(ConsoleTime.GetTime(DateTime.Now) + "    set configuration for: cfg...");
            Network.LaunchDefaults();
            Console.WriteLine(ConsoleTime.GetTime(DateTime.Now) + "    set configuration for: network...");
            SystemDataRepo.LaunchDefaults();
            Console.WriteLine(ConsoleTime.GetTime(DateTime.Now) + "    set configuration for: systemDataRepo...");
            ZfsMount.LaunchDefaults();
            Console.WriteLine(ConsoleTime.GetTime(DateTime.Now) + "    set configuration for: zfsMount...");
            Command.Launch("chmod", "777 *.xml");
            Console.WriteLine(ConsoleTime.GetTime(DateTime.Now) + "    check configuration...");

            Console.WriteLine(ConsoleTime.GetTime(DateTime.Now) + "loading service configuration");
            var hubConfiguration = new HubConfiguration { EnableDetailedErrors = false };
            app.MapSignalR(hubConfiguration);
            Console.WriteLine(ConsoleTime.GetTime(DateTime.Now) + "    signalR -> loaded");
            bool errorTrace = false;
            StaticConfiguration.DisableErrorTraces = errorTrace;
            Console.WriteLine(ConsoleTime.GetTime(DateTime.Now) + "    disableerrortraces -> {0}", errorTrace);
            Database.Start();
            Console.WriteLine(ConsoleTime.GetTime(DateTime.Now) + "    denso-db -> loaded");
            app.UseNancy();
            Console.WriteLine(ConsoleTime.GetTime(DateTime.Now) + "    nancy-fx -> loaded");
            JobScheduler.Start(true);
            Console.WriteLine(ConsoleTime.GetTime(DateTime.Now) + "    scheduler -> loaded");
        }
    }

    public class Database {

        public static void Start() {
            string[] databases;

            string root = SelfConfig.GetAntdDb();
            databases = new string[] { root };

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