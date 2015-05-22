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

using Antd.Boot;
using Antd.Common;
using Antd.Status;
using Microsoft.AspNet.SignalR;
using Microsoft.Owin.Hosting;
using Nancy;
using Owin;
using System;
using System.IO;

namespace Antd {

    internal static class Program {

        private static void Main(string[] args) {
            DateTime startTime = DateTime.Now;
            Console.Title = "ANTD";
            ConsoleLogger.Log("loading application...");

            string uri = SelfConfig.GetAntdUri();
            //try {
            using (WebApp.Start<Startup>(uri)) {
                ConsoleLogger.Log("loading service");
                ConsoleLogger.Log("    service type -> server");
                ConsoleLogger.Log("                 -> server url -> {0}", uri);
                ConsoleLogger.Log("service is now running");
                ConsoleLogger.Log("loaded in: {0}", DateTime.Now - startTime);

                Console.ReadLine();
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
            ConsoleLogger.Log("loading service configuration");
            AntdBoot.CheckDirectories();
            AntdBoot.SetCoreParameters();
            AntdBoot.StartScheduler(false);
            AntdBoot.StartNetworkd();

            //Sysctl.WriteConfig();
            //ConsoleLogger.Log("     sysctl.config -> created");
            //Sysctl.LoadConfig();
            //ConsoleLogger.Log("     sysctl.config -> loaded");

            var hubConfiguration = new HubConfiguration { EnableDetailedErrors = false };
            app.MapSignalR(hubConfiguration);
            ConsoleLogger.Log("    signalR -> loaded");
            StaticConfiguration.DisableErrorTraces = false;
            AntdBoot.StartDatabase();
            app.UseNancy();
            ConsoleLogger.Log("    nancy -> loaded");
        }
    }
}