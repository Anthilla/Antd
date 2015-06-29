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

using Antd.UnitFiles;
using System;
using System.Collections.Generic;
using System.IO;

namespace Antd.Status {

    public class Networkd {
        private static string coreFileName = "antdConfig";

        private static string[] _files = new string[] {
                coreFileName + "Current",
                coreFileName + "001",
                coreFileName + "002"
            };

        private static XmlWriter xmlWriter = new XmlWriter(_files);

        public static void SetConfiguration() {
            var check = CheckConfiguration();
            if (check == true) {
                //il file esiste
                //la configurazione esiste
                // -> applica la configurazione
                EnableRequiredServices();
                MountNetworkdDir();
                CreateFirstUnit();
                RestartNetworkdDir();
                ConsoleLogger.Log(StatusNetworkdDir());
                ConsoleLogger.Log("    networkd -> loaded");
            }
            else {
                //il file NON esiste
                //la configurazione NON esiste
                // -> NON applica la configurazione
                ConsoleLogger.Warn("----------------------------------+");
                ConsoleLogger.Warn("networkd -> not configured yet    |");
                ConsoleLogger.Warn("----------------------------------+");
            }
        }

        private static bool CheckConfiguration() {
            return xmlWriter.CheckValue("networkd");
        }

        private static void EnableRequiredServices() {
            Systemctl.Start("systemd-networkd.service");
            Systemctl.Start("systemd-resolved.service");
            Systemctl.Enable("systemd-networkd.service");
            Systemctl.Enable("systemd-resolved.service");
        }

        private static void MountNetworkdDir() {
            Command.Launch("mount", "--bind /etc/systemd/network /antd/networkd");
        }

        private static string RestartNetworkdDir() {
            var r = Systemctl.Restart("systemd-networkd");
            return r.output;
        }

        private static string StatusNetworkdDir() {
            var r = Systemctl.Status("systemd-networkd");
            return r.output;
        }

        private static void CreateUnit(string filename, string matchName, string matchHost, string matchVirtualization,
                                      string networkDHCP, string networkDNS, string networkBridge, string networkIPForward,
                                      string addressAddress, string routeGateway) {
            Directory.CreateDirectory("/antd/networkd");
            string path = Path.Combine("/antd/networkd", filename + ".network");
            //if (File.Exists(path)) {
            //    File.Delete(path);
            //}
            using (StreamWriter sw = File.CreateText(path)) {
                sw.WriteLine("[Match]");
                sw.WriteLine("Name=" + matchName);
                if (matchHost != "") { sw.WriteLine("Host=" + matchHost); }
                if (matchVirtualization != "") { sw.WriteLine("Virtualization=" + matchVirtualization); }
                sw.WriteLine("");
                sw.WriteLine("[Network]");
                if (networkDHCP != "") sw.WriteLine("DHCP=" + networkDHCP);
                if (networkDNS != "") sw.WriteLine("DNS=" + networkDNS);
                if (networkBridge != "") sw.WriteLine("Bridge=" + networkBridge);
                if (networkIPForward != "") sw.WriteLine("IPForward=" + networkIPForward);
                sw.WriteLine("");
                sw.WriteLine("[Address]");
                sw.WriteLine("Address=" + addressAddress);
                if (routeGateway != "") sw.WriteLine("");
                if (routeGateway != "") sw.WriteLine("[Route]");
                if (routeGateway != "") sw.WriteLine("Gateway=" + routeGateway);
                sw.WriteLine("");
            }
        }

        private static void CreateFirstUnit() {
            CreateUnit("antd", "eth0", "", "", "", "192.168.56.1", "", "", "192.168.56.101/24", "192.168.56.1");
        }

        public static string ReadAntdUnit() {
            string path = Path.Combine("/antd/networkd", "antd.network");
            string text;
            if (!File.Exists(path)) {
                text = "Unit file doesn't exist!";
            }
            else {
                text = File.ReadAllText(path);
            }
            return text;
        }

        public static dynamic ReadUnits() {
            List<string> list = new List<string>() { };
            var dirContainer = "/antd/networkd";
            if (Directory.Exists(dirContainer)) {
                string[] dirs = Directory.GetFiles("/antd/networkd");
                foreach (string file in dirs) {
                    string path = Path.Combine("/antd/networkd", file);
                    string text;
                    if (!File.Exists(path)) {
                        text = "Unit file does not exist!";
                    }
                    else {
                        text = File.ReadAllText(path);
                    }
                    list.Add(text);
                }
                return list;
            }
            else {
                return String.Empty;
            }
        }

        public static void CreateCustomUnit(string text, string fname) {
            Directory.CreateDirectory("/antd/networkd");
            string path = Path.Combine("/antd/networkd", fname + ".network");
            if (File.Exists(path)) {
                File.Delete(path);
            }
            using (StreamWriter sw = File.CreateText(path)) {
                sw.Write(text);
            }
        }
    }
}