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

using Antd.Systemd;
using System.IO;

namespace Antd.Apps {

    public class AnthillaSP {

        public static void CreateUnits() {
            if (AnthillaSP.Units.CheckFiles() == false) {
                Units.SetAnthillaSP();
                Units.MountFramework();
                Units.LaunchAnthillaSP();
                Units.LaunchAnthillaServer();
            }
            Systemctl.DaemonReload();
        }

        public static void Start() {
            Systemctl.Start(Units.Name.FileName.Prepare);
            Systemctl.Start(Units.Name.FileName.Mount);
            Systemctl.Start(Units.Name.FileName.LaunchSP);
            Systemctl.Start(Units.Name.FileName.LaunchServer);
        }

        public static void Start(string app) {
            Systemctl.Start(app);
        }

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
                var result = false;
                var lookInto = "/mnt/cdrom/Apps";
                var filePaths = Directory.GetFiles(lookInto);
                foreach (string t in filePaths) {
                    if (t.Contains("DIR_framework_anthillasp.squashfs.xz")) {
                        result = true;
                    }
                }
                return result;
            }

            public static void CreateSquash() {
                Terminal.Execute("mksquashfs /mnt/cdrom/Apps/anthillasp /mnt/cdrom/Apps/DIR_framework_anthillasp.squashfs.xz -comp xz -Xbcj x86 -Xdict-size 75%");
            }

            public static void MountSquash() {
                Directory.CreateDirectory("/framework/anthillasp");
                Terminal.Execute("mount /mnt/cdrom/Apps/DIR_framework_anthillasp.squashfs.xz /framework/anthillasp");
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
                        sw.WriteLine("");
                        sw.WriteLine("[Service]");
                        sw.WriteLine("ExecStart=/bin/mkdir -p /framework/anthillasp");
                        sw.WriteLine("Requires=local-fs.target sysinit.target");
                        sw.WriteLine("Before=framework-anthillasp.mount System.mount");
                        sw.WriteLine("");
                        sw.WriteLine("[Install]");
                        sw.WriteLine("WantedBy=multi-user.target");
                    }
                }
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
                        sw.WriteLine("What=/mnt/cdrom/DIRS/DIR_framework_anthillasp/running");
                        sw.WriteLine("Where=/framework/anthillasp/");
                        sw.WriteLine("");
                        sw.WriteLine("[Install]");
                        sw.WriteLine("WantedBy=multi-user.target");
                    }
                }
            }

            public static void LaunchAnthillaSP() {
                var path = Name.LaunchSP;
                if (!File.Exists(path)) {
                    using (StreamWriter sw = File.CreateText(path)) {
                        sw.WriteLine("[Unit]");
                        sw.WriteLine("Description=External Volume Unit, Application: AnthillaSP Launcher Service");
                        sw.WriteLine("");
                        sw.WriteLine("[Service]");
                        sw.WriteLine("ExecStart=/usr/bin/mono /framework/anthillasp/anthillasp/AnthillaSP.exe");
                        sw.WriteLine("Requires=local-fs.target sysinit.target");
                        sw.WriteLine("After=framework-anthillasp.mount");
                        sw.WriteLine("");
                        sw.WriteLine("[Install]");
                        sw.WriteLine("WantedBy=multi-user.target");
                    }
                }
            }

            public static void LaunchAnthillaServer() {
                var path = Name.LaunchServer;
                if (!File.Exists(path)) {
                    using (StreamWriter sw = File.CreateText(path)) {
                        sw.WriteLine("[Unit]");
                        sw.WriteLine("Description=External Volume Unit, Application: AnthillaServer Launcher Service");
                        sw.WriteLine("");
                        sw.WriteLine("[Service]");
                        sw.WriteLine("ExecStart=/usr/bin/mono /framework/anthillasp/anthillaserver/AnthillaServer.exe");
                        sw.WriteLine("Requires=local-fs.target sysinit.target");
                        sw.WriteLine("After=framework-anthillasp.mount");
                        sw.WriteLine("");
                        sw.WriteLine("[Install]");
                        sw.WriteLine("WantedBy=multi-user.target");
                    }
                }
            }
        }

        public static string AnthillaServerPID { get { return Proc.GetPID("AnthillaServer.exe"); } }

        public static string AnthillaSPPID { get { return Proc.GetPID("AnthillaSP.exe"); } }
    }
}