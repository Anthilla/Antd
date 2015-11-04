using antdlib.MountPoint;
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
using antdlib.Systemd;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using antdlib.Common;
using antdlib.Contexts;

namespace antdlib.Apps {

    public class AnthillaSp {

        private static readonly string AnthillaSpAppDir = $"{Folder.Apps}/Anthilla_AnthillaSP";
        private static string _anthillaSpFrameworkDir = "/framework/anthillasp";

        public static void SetApp() {
            CreateUnits();
            Thread.Sleep(20);
            SetDirectories();
            Thread.Sleep(20);
            Start();
        }

        public static void CreateUnits() {
            if (Units.CheckFiles() == false) {
                Units.SetAnthillaSp();
                Units.MountFramework();
                Units.LaunchAnthillaSp();
                Units.LaunchAnthillaServer();
            }
            Systemctl.DaemonReload();
        }

        private static void SetDirectories() {
            var app = Management.DetectApps().FirstOrDefault(a => a.Name == "anthillasp");
            if (app == null) {
                ConsoleLogger.Warn("no appinfo detected");
            }
            else {
                var dirs = Management.GetWantedDirectories(app);
                if (dirs.Length <= 0) {
                    ConsoleLogger.Warn("no app directory found");
                }
                else {
                    foreach (var dir in dirs) {
                        Directory.CreateDirectory(dir.Trim());
                        Directory.CreateDirectory(Mount.SetDirsPath(dir.Trim()));
                        Mount.Dir(dir.Trim());
                    }
                }
            }
        }

        public static void Start() {
            if (!Systemctl.Status("app-anthillasp-01-prepare.service").output.Contains("Active: active (running)")) {
                Systemctl.Start("app-anthillasp-01-prepare.service");
            }

            if (!Systemctl.Status("app-anthillasp-02-mount.service").output.Contains("Active: active (running)")) {
                Systemctl.Start("app-anthillasp-02-mount.service");
            }

            if (!Systemctl.Status("app-anthillasp-03-srv-launcher.service").output.Contains("Active: active (running)")) {
                Systemctl.Start("app-anthillasp-03-srv-launcher.service");
            }

            if (!Systemctl.Status("app-anthillasp-04-wui-launcher.service").output.Contains("Active: active (running)")) {
                Systemctl.Start("app-anthillasp-04-wui-launcher.service");
            }
        }

        public static void StartSp() {
            Systemctl.Start("app-anthillasp-04-wui-launcher.service");
        }

        public static void StartServer() {
            Systemctl.Start("app-anthillasp-03-srv-launcher.service");
        }

        public static void StopSp() {
            Systemctl.Stop("app-anthillasp-04-wui-launcher.service");
        }

        public static void StopServer() {
            Systemctl.Stop("app-anthillasp-03-srv-launcher.service");
        }

        public class Status {
            public static string AnthillaSp() {
                return Systemctl.Status("app-anthillasp-04-wui-launcher.service").output;
            }

            public static string AnthillaServer() {
                return Systemctl.Status("app-anthillasp-03-srv-launcher.service").output;
            }

            public static bool IsActiveAnthillaSp() {
                return (Systemctl.Status("app-anthillasp-04-wui-launcher.service").output.Contains("Active: active"));
            }

            public static bool IsActiveAnthillaServer() {
                return (Systemctl.Status("app-anthillasp-03-srv-launcher.service").output.Contains("Active: active"));
            }
        }

        public class Setting {

            public static bool CheckSquash() {
                Directory.CreateDirectory(AnthillaSpAppDir);
                var filePaths = Directory.EnumerateFiles(Folder.Apps, "*.squashfs.xz*", SearchOption.AllDirectories);
                return filePaths.Any(t => t.Contains("anthillasp"));
            }

            public static void CreateSquash() {
                Terminal.Execute($"mksquashfs {AnthillaSpAppDir}/anthillasp {AnthillaSpAppDir}/DIR_framework_anthillasp-{DateTime.Now.ToString(AssemblyInfo.dateFormat)}.squashfs.xz -comp xz -Xbcj x86 -Xdict-size 75%");
            }

            public static void MountSquash(string version = null) {
                Directory.CreateDirectory("/framework/anthillasp");
                var squashList = Directory.EnumerateFiles(AnthillaSpAppDir, "*.squashfs.xz", SearchOption.TopDirectoryOnly);
                var enumerable = squashList as IList<string> ?? squashList.ToList();
                var file = (version != null && enumerable.Any()) ? $"DIR_framework_anthillasp-{version}.squashfs.xz" : Path.GetFileName(enumerable.OrderByDescending(f => f).LastOrDefault());
                if (string.IsNullOrEmpty(file))
                    return;
                Terminal.Execute($"mount {AnthillaSpAppDir}/{file} {_anthillaSpFrameworkDir}");
            }
        }

        public class Units {
            public class Name {
                public static string Prepare => Path.Combine(Folder.AppsUnits, "app-anthillasp-01-prepare.service");
                public static string Mount => Path.Combine(Folder.AppsUnits, "app-anthillasp-02-mount.service");
                public static string LaunchSp => Path.Combine(Folder.AppsUnits, "app-anthillasp-04-wui-launcher.service");
                public static string LaunchServer => Path.Combine(Folder.AppsUnits, "app-anthillasp-03-srv-launcher.service");
            }

            public static bool CheckFiles() {
                return (File.Exists(Name.Prepare) && File.Exists(Name.Mount) && File.Exists(Name.LaunchSp) && File.Exists(Name.LaunchServer));
            }

            public static void SetAnthillaSp() {
                var path = Name.Prepare;
                if (!File.Exists(path)) {
                    using (var sw = File.CreateText(path)) {
                        sw.WriteLine("[Unit]");
                        sw.WriteLine("Description=External Volume Unit, Application: AnthillaSP Prepare Service");
                        sw.WriteLine("Requires=local-fs.target sysinit.target");
                        sw.WriteLine("Before=app-anthillasp-02-mount.service");
                        sw.WriteLine("");
                        sw.WriteLine("[Service]");
                        sw.WriteLine("ExecStart=/bin/mkdir -p /framework/anthillasp");
                        sw.WriteLine("");
                        sw.WriteLine("[Install]");
                        sw.WriteLine("WantedBy=applicative.target");
                    }
                }
                Systemctl.DaemonReload();
            }

            public static void MountFramework() {
                var path = Name.Mount;
                if (!File.Exists(path)) {
                    using (var sw = File.CreateText(path)) {
                        sw.WriteLine("[Unit]");
                        sw.WriteLine("Description=External Volume Unit, Application: DIR_framework_anthillasp Mount ");
                        sw.WriteLine("ConditionPathExists=/framework/anthillasp");
                        sw.WriteLine("");
                        sw.WriteLine("[Mount]");
                        sw.WriteLine("What=/mnt/cdrom/Apps/Anthilla_AnthillaSP/active-version");
                        sw.WriteLine("Where=/framework/anthillasp/");
                        sw.WriteLine("");
                        sw.WriteLine("[Install]");
                        sw.WriteLine("WantedBy=applicative.target");
                    }
                }
                Systemctl.DaemonReload();
            }

            public static void LaunchAnthillaSp() {
                var path = Name.LaunchSp;
                if (!File.Exists(path)) {
                    using (var sw = File.CreateText(path)) {
                        sw.WriteLine("[Unit]");
                        sw.WriteLine("Description=External Volume Unit, Application: AnthillaSP Launcher Service");
                        sw.WriteLine("Requires=local-fs.target sysinit.target");
                        sw.WriteLine("After=app-anthillasp-02-mount.service");
                        sw.WriteLine("");
                        sw.WriteLine("[Service]");
                        sw.WriteLine("ExecStart=/usr/bin/mono /framework/anthillasp/anthillasp/AnthillaSP.exe");
                        sw.WriteLine("");
                        sw.WriteLine("[Install]");
                        sw.WriteLine("WantedBy=applicative.target");
                    }
                }
                Systemctl.DaemonReload();
            }

            public static void LaunchAnthillaServer() {
                var path = Name.LaunchServer;
                if (!File.Exists(path)) {
                    using (var sw = File.CreateText(path)) {
                        sw.WriteLine("[Unit]");
                        sw.WriteLine("Description=External Volume Unit, Application: AnthillaServer Launcher Service");
                        sw.WriteLine("Requires=local-fs.target sysinit.target");
                        sw.WriteLine("After=app-anthillasp-02-mount.service");
                        sw.WriteLine("");
                        sw.WriteLine("[Service]");
                        sw.WriteLine("ExecStart=/usr/bin/mono /framework/anthillasp/anthillaserver/AnthillaServer.exe");
                        sw.WriteLine("");
                        sw.WriteLine("[Install]");
                        sw.WriteLine("WantedBy=applicative.target");
                    }
                }
                Systemctl.DaemonReload();
            }
        }

        public static string AnthillaServerPid => Proc.GetPid("AnthillaServer.exe");

        public static string AnthillaSppid => Proc.GetPid("AnthillaSP.exe");
    }
}