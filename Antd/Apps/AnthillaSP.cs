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
using System.IO;

namespace Antd.Apps {

    public class AnthillaSP {

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

            public static void MountSquash() {
                Directory.CreateDirectory("/framework/anthillasp");
                Command.Launch("mount", "/mnt/cdrom/Apps/DIR_framework_anthillasp.squashfs.xz /framework/anthillasp");
            }

            public static void CreateUnits() {
                ConsoleLogger.Warn("Your attempt to write in Overlays will be lost! (tmpsf)");
                Command.Launch("mount", "-t tmpfs tmpfs /mnt/cdrom/Overlay/");
                ConsoleLogger.Warn("Your anthillasp units be written in tmpfs!");
                Directory.CreateDirectory("/mnt/cdrom/Overlay/anthillasp/");
                Command.Launch("mount", "-t tmpfs tmpfs /mnt/cdrom/Overlay/anthillasp/");
                UnitPrepareAnthillasp();
                UnitFrameworkMount();
                UnitLaunchAnthillaSP();
                UnitLaunchAnthillaServer();
            }

            private static void UnitPrepareAnthillasp() {
                var unitName = "anthillasp-prepare.service";
                var folder = "/mnt/cdrom/Overlay/anthillasp";
                var path = Path.Combine(folder, unitName);
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
                Command.Launch("chmod", "777 " + path);
            }

            private static void UnitFrameworkMount() {
                var unitName = "framework-anthillasp.mount";
                var folder = "/mnt/cdrom/Overlay/anthillasp";
                var path = Path.Combine(folder, unitName);
                if (!File.Exists(path)) {
                    using (StreamWriter sw = File.CreateText(path)) {
                        sw.WriteLine("[Unit]");
                        sw.WriteLine("Description=External Volume Unit, Application: DIR_framework_anthillasp Mount ");
                        sw.WriteLine("ConditionPathExists=/framework/anthillasp");
                        sw.WriteLine("");
                        sw.WriteLine("[Mount]");
                        sw.WriteLine("What=/mnt/cdrom/Apps/DIR_framework_anthillasp.squashfs.xz");
                        sw.WriteLine("Where=/framework/anthillasp/");
                        sw.WriteLine("");
                        sw.WriteLine("[Install]");
                        sw.WriteLine("WantedBy=multi-user.target");
                    }
                }
                Command.Launch("chmod", "777 " + path);
            }

            private static void UnitLaunchAnthillaSP() {
                var unitName = "anthillasp-launcher.service";
                var folder = "/mnt/cdrom/Overlay/anthillasp";
                var path = Path.Combine(folder, unitName);
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
                Command.Launch("chmod", "777 " + path);
            }

            private static void UnitLaunchAnthillaServer() {
                var unitName = "anthillaserver-launcher.service";
                var folder = "/mnt/cdrom/Overlay/anthillasp";
                var path = Path.Combine(folder, unitName);
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
                Command.Launch("chmod", "777 " + path);
            }
        }

        public static string AnthillaServerPID { get { return Proc.GetPID("AnthillaServer.exe"); } }

        public static string AnthillaSPPID { get { return Proc.GetPID("AnthillaSP.exe"); } }
    }
}