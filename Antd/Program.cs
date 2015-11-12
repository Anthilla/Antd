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
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using antdlib;
using antdlib.Boot;
using antdlib.Common;
using Microsoft.Owin.Builder;
using Nowin;
using Owin;

namespace Antd {
    internal static class Program {
        private static void Main() {
            var startTime = DateTime.Now;
            Console.Title = "ANTD";

            if (AssemblyInfo.IsUnix == false) {
                Directory.CreateDirectory("/cfg/antd");
                Directory.CreateDirectory("/cfg/antd/database");
                Directory.CreateDirectory("/mnt/cdrom/DIRS");
                ConsoleLogger.Warn("This application is not running on an Unix OS:");
                ConsoleLogger.Warn("some functions may be disabled!");
            }

            var stop = new ManualResetEvent(false);
            Console.CancelKeyPress +=
                (sender, e) => {
                    Console.WriteLine("^C");
                    Environment.Exit(1);
                    stop.Set();
                    e.Cancel = true;
                };

            //01 - controlo lo stato di lettura/scrittura
            AntdBoot.CheckIfGlobalRepositoryIsWriteable();
            //02 - controllo e creo le cartelle di lavoro di antd
            AntdBoot.SetWorkingDirectories();
            //03 - configuro i parametri di base di antd
            AntdBoot.SetCoreParameters();

            var owinbuilder = new AppBuilder();
            Microsoft.Owin.Host.HttpListener.OwinServerFactory.Initialize(owinbuilder.Properties);
            new Startup().Configuration(owinbuilder);
            var port = Convert.ToInt32(CoreParametersConfig.GetPort());
            var builder = ServerBuilder.New()
                .SetOwinApp(owinbuilder.Build())
                .SetEndPoint(new IPEndPoint(IPAddress.Any, port))
                .SetCertificate(new X509Certificate2("certificate/certificate.pfx"));
            //.RequireClientCertificate();

            using (var server = builder.Build()) {
                Task.Run(() => server.Start());
                ConsoleLogger.Log("Applying configuration...");
                Configuration();
                ConsoleLogger.Log("loading service");
                ConsoleLogger.Log("    server port -> {0}", port);
                ConsoleLogger.Log("antd is running");
                ConsoleLogger.Log("loaded in: {0}", DateTime.Now - startTime);
                stop.WaitOne();
            }
        }

        private static void Configuration() {
            //07 - load config degli utenti
            AntdBoot.ReloadUsers();
            //08 - load config dell'ssh
            //AntdBoot.ReloadSsh();
            //09 - load config di network
            AntdBoot.SetBootConfiguration();
            //10 - mount system directories
            AntdBoot.SetMounts();
            //11 - mount os directories
            AntdBoot.SetOsMount();
            //12 - install websocketd
            AntdBoot.SetWebsocketd();
            //13 - set journald
            AntdBoot.SetSystemdJournald();
            //14 - check resolv.conf
            AntdBoot.CheckResolvd();
            //15 - start scheduler
            AntdBoot.StartScheduler(true);
            //16 - start directory watcher
            AntdBoot.StartDirectoryWatcher(new[] { Folder.Config }, false);
            //17 - set authentication method
            AntdBoot.InitAuthentication();
            //18 - setup and launch all apps
            AntdBoot.LaunchApps();
            //19 - download files
            AntdBoot.DownloadDefaultRepoFiles();
        }
    }

    internal class Startup {
        public void Configuration(IAppBuilder app) {
            //04 - avvio il database
            AntdBoot.StartDatabase();
            ConsoleLogger.Log("loading core service configuration");
            //05 - avvio signalr
            AntdBoot.StartSignalR(app, true, true);
            //06 - avvio nancy
            AntdBoot.StartNancy(app);
        }
    }
}