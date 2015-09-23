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
/// 

using antdlib;
using antdlib.Boot;
using Microsoft.Owin.Hosting;
using Owin;
using System;
using System.Threading;

namespace Antd {

    internal static class Program {

        private static void Main(string[] args) {
            var startTime = DateTime.Now;
            Console.Title = "ANTD";

            AntdBoot.CheckIfGlobalRepositoryIsWriteable();
            AntdBoot.SetWorkingDirectories();
            AntdBoot.SetCoreParameters();
            AntdBoot.InitAuthentication();

            var uri = CoreParametersConfig.GetHostUri();
            var stop = new ManualResetEvent(false);
            Console.CancelKeyPress +=
                (sender, e) => {
                    Console.WriteLine("^C");
                    Environment.Exit(1);
                    stop.Set();
                    e.Cancel = true;
                };

            using (WebApp.Start<Startup>(uri)) {
                ConsoleLogger.Log("loading service");
                ConsoleLogger.Log("    server url -> {0}", uri);

                ConsoleLogger.Log("antd is running");
                ConsoleLogger.Log("loaded in: {0}", DateTime.Now - startTime);
                stop.WaitOne();
            }
        }
    }

    internal class Startup {

        public void Configuration(IAppBuilder app) {
            ConsoleLogger.Log("loading core service configuration");

            AntdBoot.StartDatabase();
            AntdBoot.SetMounts();
            AntdBoot.SetUsersMount();
            AntdBoot.StartSignalR(app, true);
            AntdBoot.StartNancy(app);

            AntdBoot.StartScheduler(false);
            AntdBoot.StartDirectoryWatcher(true);
            AntdBoot.CheckSysctl(false);
        }
    }
}