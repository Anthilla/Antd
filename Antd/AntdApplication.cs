//-------------------------------------------------------------------------------------
//     Copyright (c) 2014, Anthilla S.r.l. (http://www.anthilla.com)
//     All rights reserved.
//
//     Redistribution and use in source and binary forms, with or without
//     modification, are permitted provided that the following conditions are met:
//         * Redistributions of source code must retain the above copyright
//           notice, this list of conditions and the following disclaimer.
//         * Redistributions in binary form must reproduce the above copyright
//           notice, this list of conditions and the following disclaimer in the
//           documentation and/or other materials provided with the distribution.
//         * Neither the name of the Anthilla S.r.l. nor the
//           names of its contributors may be used to endorse or promote products
//           derived from this software without specific prior written permission.
//
//     THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
//     ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
//     WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
//     DISCLAIMED. IN NO EVENT SHALL ANTHILLA S.R.L. BE LIABLE FOR ANY
//     DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
//     (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
//     LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
//     ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
//     (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
//     SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
//
//     20141110
//-------------------------------------------------------------------------------------

using System;
using System.IO;
using System.Net;
using System.Threading;
using antdlib;
using antdlib.Common;
using antdlib.Log;
using Owin;
using Microsoft.Owin.Hosting;
using Antd.Middleware;
using Nancy.Owin;

namespace Antd {
    internal static class AntdApplication {
        private static void Main() {
            try {
                ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
                var startTime = DateTime.Now;
                Console.Title = "antd";
                if (Parameter.IsUnix == false) {
                    Directory.CreateDirectory("/cfg/antd");
                    Directory.CreateDirectory("/cfg/antd/database");
                    Directory.CreateDirectory("/mnt/cdrom/DIRS");
                    ConsoleLogger.Warn("This application is not running on an Anthilla OS Linux, some functions may be disabled");
                }
                Configuration();
                var port = Convert.ToInt32(ApplicationSetting.HttpPort());
                using (WebApp.Start<Startup>($"http://+:{port}/")) {
                    ConsoleLogger.Log("loading service");
                    ConsoleLogger.Log($"http port: {port}");
                    ConsoleLogger.Log("antd is running");
                    ConsoleLogger.Log($"loaded in: {DateTime.Now - startTime}");
                    do {
                        Thread.Sleep(60000);
                    } while (!Console.KeyAvailable);
                }
            }
            catch (Exception ex) {
                Directory.CreateDirectory($"{Parameter.AntdCfgReport}");
                File.WriteAllText($"{Parameter.AntdCfgReport}/{Timestamp.Now}-crash-report.txt", ex.ToString());
                DeNSo.Session.ShutDown();
            }
        }

        private static void Configuration() {
            AntdBoot.CheckOsIsRw();
            AntdBoot.SetWorkingDirectories();
            AntdBoot.SetCoreParameters();
            AntdBoot.StartDatabase();
            AntdBoot.CheckCertificate();
            AntdBoot.ReloadUsers();
            AntdBoot.ReloadSsh();
            //AntdBoot.SetOverlayDirectories();
            //AntdBoot.SetSystemdJournald();
            AntdBoot.SetMounts();
            AntdBoot.SetOsMount();
            AntdBoot.LaunchDefaultOsConfiguration();
            //AntdBoot.SetWebsocketd();
            AntdBoot.CheckResolv();
            AntdBoot.SetFirewall();
            AntdBoot.ImportSystemInformation();
            //AntdBoot.StartScheduler(true);
            AntdBoot.StartDirectoryWatcher();
            AntdBoot.LaunchApps();
            AntdBoot.StartWebsocketServer();
            //AntdBoot.DownloadDefaultRepoFiles();
        }
    }

    internal class Startup {
        public void Configuration(IAppBuilder app) {
            ConsoleLogger.Log("loading app configuration");
            object httpListener;
            if (app.Properties.TryGetValue(typeof(HttpListener).FullName, out httpListener) && httpListener is HttpListener) {
                ((HttpListener)httpListener).IgnoreWriteExceptions = true;
            }
            //var hubConfiguration = new HubConfiguration { EnableDetailedErrors = true };
            //app.MapSignalR(hubConfiguration);
            //ConsoleLogger.Log("signalR ready");
            app.UseDebugMiddleware();
            app.UseNancy();
            app.UseDebugMiddleware(new DebugMiddlewareOptions() {
                OnIncomingRequest = context => context.Response.WriteAsync("## Beginning ##"),
                OnOutGoingRequest = context => context.Response.WriteAsync("## End ##")
            });
            //StaticConfiguration.DisableErrorTraces = false;
            //var options = new NancyOptions { EnableClientCertificates = true };
            app.UseNancy(options => options.PassThroughWhenStatusCodesAre(Nancy.HttpStatusCode.NotFound));
            ConsoleLogger.Log("nancyfx ready");
        }
    }
}