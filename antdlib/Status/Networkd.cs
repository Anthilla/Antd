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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using antdlib.Common;

namespace antdlib.Status {

    public class Networkd {
        private const string CoreFileName = "antdConfig";

        private static readonly string[] Files = {
                CoreFileName + "Current",
                CoreFileName + "001",
                CoreFileName + "002"
            };

        private static readonly ParameterXmlWriter XmlWriter = new ParameterXmlWriter(Files);

        public static void SetConfiguration() {
            var check = CheckConfiguration();
            if (check) {
                EnableRequiredServices();
                MountNetworkdDir();
                CreateFirstUnit();
                RestartNetworkdDir();
                ConsoleLogger.Log(StatusNetworkdDir());
                ConsoleLogger.Log("    networkd -> loaded");
            }
            else {
                ConsoleLogger.Warn("----------------------------------+");
                ConsoleLogger.Warn("networkd -> not configured yet    |");
                ConsoleLogger.Warn("----------------------------------+");
            }
        }

        private static bool CheckConfiguration() {
            return XmlWriter.CheckValue("networkd");
        }

        private static void EnableRequiredServices() {
            Systemctl.Start("systemd-networkd.service");
            Systemctl.Start("systemd-resolved.service");
            Systemctl.Enable("systemd-networkd.service");
            Systemctl.Enable("systemd-resolved.service");
        }

        private static void MountNetworkdDir() {
            Terminal.Terminal.Execute("mount --bind /etc/systemd/network " + Folder.Networkd);
        }

        private static void RestartNetworkdDir() {
            Systemctl.Restart("systemd-networkd");
        }

        private static string StatusNetworkdDir() {
            var r = Systemctl.Status("systemd-networkd");
            return r.output;
        }

        private static void CreateUnit(string filename, string matchName, string matchHost, string matchVirtualization,
                                      string networkDhcp, string networkDns, string networkBridge, string networkIpForward,
                                      string addressAddress, string routeGateway) {
            var path = Path.Combine(Folder.Networkd, filename + ".network");
            using (var sw = File.CreateText(path)) {
                sw.WriteLine("[Match]");
                sw.WriteLine("Name=" + matchName);
                if (matchHost != "") { sw.WriteLine("Host=" + matchHost); }
                if (matchVirtualization != "") { sw.WriteLine("Virtualization=" + matchVirtualization); }
                sw.WriteLine("");
                sw.WriteLine("[Network]");
                if (networkDhcp != "")
                    sw.WriteLine("DHCP=" + networkDhcp);
                if (networkDns != "")
                    sw.WriteLine("DNS=" + networkDns);
                if (networkBridge != "")
                    sw.WriteLine("Bridge=" + networkBridge);
                if (networkIpForward != "")
                    sw.WriteLine("IPForward=" + networkIpForward);
                sw.WriteLine("");
                sw.WriteLine("[Address]");
                sw.WriteLine("Address=" + addressAddress);
                if (routeGateway != "")
                    sw.WriteLine("");
                if (routeGateway != "")
                    sw.WriteLine("[Route]");
                if (routeGateway != "")
                    sw.WriteLine("Gateway=" + routeGateway);
                sw.WriteLine("");
            }
        }

        private static void CreateFirstUnit() {
            CreateUnit("antd", "eth0", "", "", "", "192.168.56.1", "", "", "192.168.56.101/24", "192.168.56.1");
        }

        public static string ReadAntdUnit() {
            var path = Path.Combine(Folder.Networkd, "antd.network");
            return !File.Exists(path) ? "Unit file doesn't exist!" : File.ReadAllText(path);
        }

        public static List<string> ReadUnits() {
            var list = new List<string>();
            var dirContainer = Folder.Networkd;
            if (Directory.Exists(dirContainer)) {
                var dirs = Directory.GetFiles(Folder.Networkd);
                list.AddRange(dirs.Select(file => Path.Combine(Folder.Networkd, file)).Select(path => !File.Exists(path) ? "Unit file does not exist!" : File.ReadAllText(path)));
                return list;
            }
            return list;
        }

        public static void CreateCustomUnit(string text, string fname) {
            var path = Path.Combine(Folder.Networkd, fname + ".network");
            if (File.Exists(path)) {
                File.Delete(path);
            }
            using (var sw = File.CreateText(path)) {
                sw.Write(text);
            }
        }
    }
}