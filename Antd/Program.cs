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

using System;
using System.Threading;
using Microsoft.Owin.Hosting;
using Nancy;
using Owin;

namespace Antd {
    internal static class Program {
        private static StartupConfig appConfig = new StartupConfig();

        private static void Main(string[] args) {
            Console.Title = "ANTD";
            var stop = new ManualResetEvent(false);
            Console.CancelKeyPress +=
                (sender, e) => {
                    Console.WriteLine("^C");
                    stop.Set();
                    e.Cancel = true;
                };

            string port;
            bool portExist;
            bool dbrootExist;
            string uri;

            portExist = appConfig.CheckValue("server", "port");
            if (portExist == false) {
                appConfig.WriteValue("server", "port", "7777");
            }

            dbrootExist = appConfig.CheckValue("server", "dbroot");
            if (dbrootExist == false) {
                appConfig.WriteValue("server", "dbroot", "/database/directory");
            }

            port = appConfig.ReadValue("server", "port");

            uri = "http://+:" + port + "/";
            Console.WriteLine(ConsoleTime.GetTime(DateTime.Now) + "initializing");
            using (WebApp.Start<Startup>(uri)) {
                Console.WriteLine(ConsoleTime.GetTime(DateTime.Now) + "loading service");
                Console.WriteLine(ConsoleTime.GetTime(DateTime.Now) + "    service type -> server");
                Console.WriteLine(ConsoleTime.GetTime(DateTime.Now) + "                 -> server port -> {0}", port);
                Console.WriteLine(ConsoleTime.GetTime(DateTime.Now) + "                 -> server url  -> {0}", uri);
                Console.WriteLine(ConsoleTime.GetTime(DateTime.Now) + "service is now running");
                Console.WriteLine("");
                stop.WaitOne();
            }
        }
    }

    internal class Startup {
        public void Configuration(IAppBuilder app) {
            Console.WriteLine(ConsoleTime.GetTime(DateTime.Now) + "loading service configuration");
            StaticConfiguration.DisableErrorTraces = false;
            Console.WriteLine(ConsoleTime.GetTime(DateTime.Now) + "    disableerrortraces -> false");
            Database.Start();
            Console.WriteLine(ConsoleTime.GetTime(DateTime.Now) + "    denso-db -> loaded");
            app.UseNancy();
            Console.WriteLine(ConsoleTime.GetTime(DateTime.Now) + "    nancy-fx -> loaded");
        }
    }

    public class Database {
        private static StartupConfig appConfig = new StartupConfig();

        public static void Start() {
            string root;
            root = appConfig.ReadValue("server", "dbroot");
            Start(root);
        }

        public static void Start(string root) {
            DeNSo.Configuration.BasePath = new string[] { System.IO.Path.Combine(root, "Database") };
            DeNSo.Configuration.EnableJournaling = true;
            DeNSo.Configuration.DBCheckTimeSpan = new System.TimeSpan(0, 1, 0);
            DeNSo.Configuration.ReindexCheck = new System.TimeSpan(0, 1, 0);

            DeNSo.Session.DefaultDataBase = "Data";
            DeNSo.Session.Start();
        }

        public static void ShutDown() {
            DeNSo.Session.ShutDown();
        }
    }
}
