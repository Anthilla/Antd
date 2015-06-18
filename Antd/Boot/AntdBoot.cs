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
using Antd.Scheduler;
using Antd.Status;
using Microsoft.AspNet.SignalR;
using Nancy;
using Owin;
using System;
using System.IO;

namespace Antd.Boot {

    public class AntdBoot {
        //private readonly static string[] Directories =
        //{
        //    "/antd",
        //    "/framework/antd",
        //    "/framework/anthillasp",
        //    "/framework/anthillaas"
        //};

        //public static void CheckDirectories() {
        //    foreach (var path in Directories) {
        //        if (!Directory.Exists(path)) {
        //            Directory.CreateDirectory(path);
        //            ConsoleLogger.Info("    directories -> {0} created", path);
        //        }
        //    }
        //    ConsoleLogger.Success("    directories -> checked");
        //}

        public static void SetCoreParameters() {
            CoreParametersConfig.WriteDefaults();
            ConsoleLogger.Success("    antd core parameters -> loaded");
        }

        public static void CheckSysctl(bool isActive) {
            if (isActive) {
                Sysctl.WriteConfig();
                Sysctl.LoadConfig();
                ConsoleLogger.Success("    sysctl -> loaded");
            }
            else {
                ConsoleLogger.Info("    sysctl -> skipped");
            }
        }

        public static void StartNetworkd() {
            Networkd.SetConfiguration();
        }

        public static void StartScheduler(bool loadFromDatabase) {
            JobScheduler.Start(loadFromDatabase);
            ConsoleLogger.Success("    scheduler -> loaded");
        }

        public static void StartDirectoryWatcher(bool isActive, string[] foldersToWatch) {
            if (isActive) {
                ConsoleLogger.Success("    directory watcher -> enabled");
                foreach (string folder in foldersToWatch) {
                    if (Directory.Exists(folder)) {
                        new DirectoryWatcher(folder).Watch();
                        ConsoleLogger.Info("    directory watcher -> enabled for {0}", folder);
                    }
                    else {
                        ConsoleLogger.Info("    directory watcher -> {0} does not exist", folder);
                    }
                }
            }
            else {
                ConsoleLogger.Info("    directory watcher -> skipped");
            }
        }

        public static void StartDatabase() {
            string[] databases;

            string applicationRoot;
            string applicationDatabaseFolder;
            string applicationDatabasePath;
            applicationRoot = AppDomain.CurrentDomain.BaseDirectory;
            ConsoleLogger.Info("root info -> application root: {0}", applicationRoot);

            applicationDatabaseFolder = "database";
            ConsoleLogger.Info("root info -> application config folder: {0}", applicationDatabaseFolder);
            applicationDatabasePath = Path.Combine(applicationRoot, applicationDatabaseFolder);
            ConsoleLogger.Info("root info -> application config path: {0}", applicationDatabasePath);
            if (!Directory.Exists(applicationDatabasePath)) {
                ConsoleLogger.Info("root info -> application config path does not exist");
                Directory.CreateDirectory(applicationDatabasePath);
                ConsoleLogger.Info("root info -> application config path created");
            }

            databases = new[] { applicationDatabasePath };
            DatabaseBoot.Start(databases, true);
            ConsoleLogger.Success("    database -> loaded");
        }

        public static void StartSignalR(IAppBuilder app, bool isActive) {
            if (isActive) {
                var hubConfiguration = new HubConfiguration { EnableDetailedErrors = false };
                app.MapSignalR(hubConfiguration);
                ConsoleLogger.Success("    signalR -> loaded");
            }
            else {
                ConsoleLogger.Info("    signalR -> skipped");
            }
        }

        public static void StartNancy(IAppBuilder app) {
            StaticConfiguration.DisableErrorTraces = false;
            app.UseNancy();
            ConsoleLogger.Success("    nancy -> loaded");
        }
    }
}