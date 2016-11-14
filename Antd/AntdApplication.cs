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
using antdlib;
using antdlib.common;
using Nancy;
using Nancy.Hosting.Self;

namespace Antd {
    internal static class AntdApplication {
        public static RaptorDB.RaptorDB Database;

        private static readonly AntdBoot Boot = new AntdBoot();
        private static readonly ApplicationSetting ApplicationSetting = new ApplicationSetting();

        private static void Main() {
            ConsoleLogger.Log("");
            ConsoleLogger.Log("starting antd");
            var startTime = DateTime.Now;
            Boot.RemoveLimits();
            Boot.StartOverlayWatcher();

            if(Parameter.IsUnix == false) {
                Directory.CreateDirectory("/cfg/antd");
                Directory.CreateDirectory("/cfg/antd/database");
                Directory.CreateDirectory("/mnt/cdrom/DIRS");
                ConsoleLogger.Warn("This application is not running on an Anthilla OS Linux, some functions may be disabled");
            }

            Boot.CheckOsIsRw();
            Boot.SetWorkingDirectories();
            Boot.SetCoreParameters();
            Database = Boot.StartDatabase();
            Boot.PrepareConfiguration();

            Boot.SetOsMount();
            ConsoleLogger.Log($"loaded in: {DateTime.Now - startTime}");
            Boot.SetOsParametersLocal();
            ConsoleLogger.Log($"loaded in: {DateTime.Now - startTime}");
            Boot.LoadModules();
            ConsoleLogger.Log($"loaded in: {DateTime.Now - startTime}");
            Boot.SetMounts();
            ConsoleLogger.Log($"loaded in: {DateTime.Now - startTime}");
            Boot.ImportCommands();
            ConsoleLogger.Log($"loaded in: {DateTime.Now - startTime}");
            Boot.ReloadUsers();
            ConsoleLogger.Log($"loaded in: {DateTime.Now - startTime}");
            Boot.CommandExecuteLocal();
            ConsoleLogger.Log($"loaded in: {DateTime.Now - startTime}");
            Boot.ImportNetworkConfiguration();
            ConsoleLogger.Log($"loaded in: {DateTime.Now - startTime}");
            Boot.Ssh();
            ConsoleLogger.Log($"loaded in: {DateTime.Now - startTime}");
            Boot.LoadServices();
            ConsoleLogger.Log($"loaded in: {DateTime.Now - startTime}");
            Boot.SetSyslogNg();
            ConsoleLogger.Log($"loaded in: {DateTime.Now - startTime}");
            Boot.InitAvahi();
            ConsoleLogger.Log($"loaded in: {DateTime.Now - startTime}");
            Boot.ImportPools();
            ConsoleLogger.Log($"loaded in: {DateTime.Now - startTime}");
            Boot.StartScheduler();
            ConsoleLogger.Log($"loaded in: {DateTime.Now - startTime}");
            Boot.InitGlusterfs();
            ConsoleLogger.Log($"loaded in: {DateTime.Now - startTime}");
            Boot.StartDirectoryWatcher();
            ConsoleLogger.Log($"loaded in: {DateTime.Now - startTime}");
            Boot.LaunchInternalTimers();
            ConsoleLogger.Log($"loaded in: {DateTime.Now - startTime}");
            Boot.LaunchApps();
            ConsoleLogger.Log($"loaded in: {DateTime.Now - startTime}");

            var port = ApplicationSetting.HttpPort();
            var uri = $"http://localhost:{port}/";
            var host = new NancyHost(new Uri(uri));
            host.Start();
            ConsoleLogger.Log("host ready");
            StaticConfiguration.DisableErrorTraces = false;
            ConsoleLogger.Log($"http port: {port}");
            ConsoleLogger.Log("antd is running");
            ConsoleLogger.Log($"loaded in: {DateTime.Now - startTime}");
            KeepAlive();
            ConsoleLogger.Log("antd is closing");
            host.Stop();
            Database.Shutdown();
        }

        private static void KeepAlive() {
            var r = Console.ReadLine();
            while(r != "quit") {
                r = Console.ReadLine();
            }
        }
    }
}