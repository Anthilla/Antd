
using antdlib.MountPoint;
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
using antdlib.Systemd;
using System;
using System.IO;
using System.Linq;
using System.Threading;

namespace antdlib.Apps {

    public class AnthillaSP {

        private static string AnthillaSPAppDir = $"{Folder.Apps}/Anthilla_AnthillaSP";
        private static string AnthillaSPFrameworkDir = $"/framework/anthillasp";

        public static void SetApp() {
            CreateUnits();
            Thread.Sleep(20);
            SetDirectories();
            Thread.Sleep(20);
            Start();
        }

        public static void CreateUnits() {
            if (Units.CheckFiles() == false) {
                Units.SetAnthillaSP();
                Units.MountFramework();
                Units.LaunchAnthillaSP();
                Units.LaunchAnthillaServer();
            }
            Systemctl.DaemonReload();
        }

        private static void SetDirectories() {
            var app = Management.DetectApps().Where(a => a.Name == "anthillasp").FirstOrDefault();
            if (app != null) {
                var dirs = Management.GetWantedDirectories(app);
                if (dirs.Length > 0) {
                    foreach (var dir in dirs) {
                        Directory.CreateDirectory(dir.Trim());
                        Directory.CreateDirectory(Mount.SetDIRSPath(dir.Trim()));
                        Mount.Dir(dir.Trim());
                    }
                }
                else {
                    ConsoleLogger.Warn("no app directory found");
                }
            }
            else {
                ConsoleLogger.Warn("no appinfo detected");
            }
        }

        public static void Start() {
            Systemctl.Start(Units.Name.FileName.Prepare);
            Thread.Sleep(20);
            Systemctl.Start(Units.Name.FileName.Mount);
            Thread.Sleep(20);
            Systemctl.Start(Units.Name.FileName.LaunchSP);
            Thread.Sleep(20);
            Systemctl.Start(Units.Name.FileName.LaunchServer);
        }

        //public static void Start(string app) {
        //    Systemctl.Start(app);
        //}

        public static void StartSP() {
            Systemctl.Start(Units.Name.FileName.LaunchSP);
        }

        public static void StartServer() {
            Systemctl.Start(Units.Name.FileName.LaunchServer);
        }

        public static void StopSP() {
            Systemctl.Stop(Units.Name.FileName.LaunchSP);
        }

        public static void StopServer() {
            Systemctl.Stop(Units.Name.FileName.LaunchServer);
        }

        public class Status {
            public static string AnthillaSP() {
                return Systemctl.Status(Units.Name.FileName.LaunchSP).output;
            }

            public static string AnthillaServer() {
                return Systemctl.Status(Units.Name.FileName.LaunchServer).output;
            }

            public static bool IsActiveAnthillaSP() {
                return (Systemctl.Status(Units.Name.FileName.LaunchSP).output.Contains("Active: active")) ? true : false;
            }

            public static bool IsActiveAnthillaServer() {
                return (Systemctl.Status(Units.Name.FileName.LaunchServer).output.Contains("Active: active")) ? true : false;
            }
        }

        public class Setting {

            public static bool CheckSquash() {
                Directory.CreateDirectory(AnthillaSPAppDir);
                var result = false;
                var filePaths = Directory.EnumerateFiles(Folder.Apps, "*.squashfs.xz*", SearchOption.AllDirectories);
                foreach (string t in filePaths) {
                    if (t.Contains("anthillasp")) {
                        result = true;
                    }
                }
                return result;
            }

            public static void CreateSquash() {
                Terminal.Execute($"mksquashfs {AnthillaSPAppDir}/anthillasp {AnthillaSPAppDir}/DIR_framework_anthillasp-{DateTime.Now.ToString(AssemblyInfo.dateFormat)}.squashfs.xz -comp xz -Xbcj x86 -Xdict-size 75%");
            }

            public static void MountSquash(string version = null) {
                Directory.CreateDirectory("/framework/anthillasp");
                var squashList = Directory.EnumerateFiles(AnthillaSPAppDir, "*.squashfs.xz", SearchOption.TopDirectoryOnly);
                var newest = "";
                if (squashList.Count() > 0) {
                    newest = squashList.OrderByDescending(f => f).LastOrDefault();
                }
                var file = (version != null) ? $"DIR_framework_anthillasp-{version}.squashfs.xz" : Path.GetFileName(newest);
                if (file.Length > 0) {
                    Terminal.Execute($"mount {AnthillaSPAppDir}/{file} {AnthillaSPFrameworkDir}");
                }
            }
        }

        public class Units {

            public class Name {

                public class FileName {
                    public static string Prepare { get { return "anthillasp-prepare.service"; } }
                    public static string Mount { get { return "framework-anthillasp.mount"; } }
                    public static string LaunchSP { get { return "anthillasp-launcher.service"; } }
                    public static string LaunchServer { get { return "anthillaserver-launcher.service"; } }
                }

                public static string Prepare { get { return Path.Combine(Folder.AppsUnits, "anthillasp-prepare.service"); } }
                public static string Mount { get { return Path.Combine(Folder.AppsUnits, "framework-anthillasp.mount"); } }
                public static string LaunchSP { get { return Path.Combine(Folder.AppsUnits, "anthillasp-launcher.service"); } }
                public static string LaunchServer { get { return Path.Combine(Folder.AppsUnits, "anthillaserver-launcher.service"); } }
            }

            public static bool CheckFiles() {
                return (File.Exists(Name.Prepare) && File.Exists(Name.Mount) && File.Exists(Name.LaunchSP) && File.Exists(Name.LaunchServer)) ? true : false;
            }

            public static void SetAnthillaSP() {
                var path = Name.Prepare;
                if (!File.Exists(path)) {
                    using (StreamWriter sw = File.CreateText(path)) {
                        sw.WriteLine("[Unit]");
                        sw.WriteLine("Description=External Volume Unit, Application: AnthillaSP Prepare Service");
                        sw.WriteLine("Requires=local-fs.target sysinit.target");
                        sw.WriteLine("Before=framework-anthillasp.mount");
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
                    using (StreamWriter sw = File.CreateText(path)) {
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

            public static void LaunchAnthillaSP() {
                var path = Name.LaunchSP;
                if (!File.Exists(path)) {
                    using (StreamWriter sw = File.CreateText(path)) {
                        sw.WriteLine("[Unit]");
                        sw.WriteLine("Description=External Volume Unit, Application: AnthillaSP Launcher Service");
                        sw.WriteLine("Requires=local-fs.target sysinit.target");
                        sw.WriteLine("After=framework-anthillasp.mount");
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
                    using (StreamWriter sw = File.CreateText(path)) {
                        sw.WriteLine("[Unit]");
                        sw.WriteLine("Description=External Volume Unit, Application: AnthillaServer Launcher Service");
                        sw.WriteLine("Requires=local-fs.target sysinit.target");
                        sw.WriteLine("After=framework-anthillasp.mount");
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

        public static string AnthillaServerPID { get { return Proc.GetPID("AnthillaServer.exe"); } }

        public static string AnthillaSPPID { get { return Proc.GetPID("AnthillaSP.exe"); } }
    }
}