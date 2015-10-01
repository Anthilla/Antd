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

using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;

namespace antdlib.Network {
    public class NetworkFirstConfiguration {

        private static string fileName = $"{Folder.Dirs}/{AntdFile.NetworkConfig}";

        private static bool CheckNetworkIsConfigured() {
            if (File.Exists(fileName) && FileSystem.ReadFile(fileName).Length > 0) {
                return true;
            }
            return false;
        }

        public static void Set() {
            if (CheckNetworkIsConfigured() == true) {
                var fileContent = FileSystem.ReadFile(fileName);
                if (fileContent.Length > 0) {
                    var arr = fileContent.Split(new String[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries).ToArray();
                    foreach (var cmd in arr) {
                        NetworkConfigRepository.Create(cmd);
                    }
                }
                ConsoleLogger.Info("Network config => existing configuration found...");
                ConsoleLogger.Info("Network config => applying this configuration!");
                Terminal.Execute($"chmod 777 {fileName}");
                Terminal.Execute($".{fileName}");
                //todo: ShowNetworkInfo("", "");
            }
            else {
                ConsoleLogger.Info("Network config => no configuration found...");
                ConsoleLogger.Info("Network config => applying a default configuration!");
                SetNetworkInterfaceUp();
            }
        }

        private static List<string> DetectAllNetworkInterfaces() {
            var NIFlist = new List<string>() { };
            var m = 15;
            for (int i = 0; i < m; i++) {
                var r = Terminal.Execute($"ip link set eth{i.ToString()} up");
                if (r.Length > 0) {
                    break;
                }
                else {
                    NIFlist.Add($"eth{i.ToString()}");
                }
            }
            return NIFlist;
        }

        private static List<string> DetectActiveNetworkInterfaces() {
            var NIFlist = DetectAllNetworkInterfaces();
            var nlist = new List<string>() { };
            foreach (var nif in NIFlist) {
                var r = Terminal.Execute($"cat /sys/class/net/{nif}/carrier");
                if (r == "1") {
                    nlist.Add(nif);
                }
            }
            return nlist;
        }

        private static bool IsIPAvailable(string input) {
            PingReply reply = new Ping().Send(input);
            if (reply.Status == IPStatus.DestinationHostUnreachable) { return true; }
            else if (reply.Status == IPStatus.DestinationNetworkUnreachable) { return true; }
            else if (reply.Status == IPStatus.DestinationPortUnreachable) { return true; }
            else if (reply.Status == IPStatus.DestinationUnreachable) { return true; }
            else if (reply.Status == IPStatus.TimedOut) { return true; }
            else { return false; }
        }

        private static string AugmentIPValue(string input) {
            var octets = input.Split('.').ToIntArray();
            var octet2 = octets[2];
            var octet3 = octets[3] + 1;
            if (octet3 == 255) {
                octet2 = octets[2] + 1;
                octet3 = octets[3];
            }
            var newOctets = new string[] {
                octets[0].ToString(),
                octets[1].ToString(),
                octet2.ToString(),
                octet3.ToString()
            };
            return String.Join(".", newOctets);
        }

        private static string GetFirstAvailableIPFrom(string input) {
            string ip = input;
            while (IsIPAvailable(ip) == false) {
                ip = AugmentIPValue(input);
            }
            return ip;
        }

        private static string PickIP() {
            var ipAddr = GetFirstAvailableIPFrom("169.254.1.1");
            return $"{ipAddr}/16";
        }

        private static List<string> commands = new List<string>() { };

        private static void WriteConfFile() {
            if (!File.Exists(fileName) && commands.Count > 0) {
                var txt = String.Join(Environment.NewLine, commands.ToArray());
            }
        }

        private static void SetNetworkInterfaceUp() {
            if (DetectActiveNetworkInterfaces().Count > 0) {
                var selectedNIF = DetectActiveNetworkInterfaces().LastOrDefault();
                ConsoleLogger.Info($"_> Network will be initialized on: {selectedNIF}");
                var ip = PickIP();
                ConsoleLogger.Info($"_> Assigning {ip} to {selectedNIF}");
                var cmd1 = $"ip addr add {ip} dev {selectedNIF}";
                Terminal.Execute(cmd1);
                commands.Add(cmd1);
                var cmd2 = $"ip route add default via {ip}";
                Terminal.Execute(cmd2);
                commands.Add(cmd2);
                WriteConfFile();
                ShowNetworkInfo(selectedNIF, ip);
            }
            else {
                ConsoleLogger.Warn("There's no active network interface,");
                ConsoleLogger.Warn("a direct action is required on the machine!");
            }
        }

        private static void ShowNetworkInfo(string nif, string ip) {
            var n = Environment.NewLine;
            var bluetoothConnectionName = $"{nif}_S{ip.Replace("/", "-")}";
            ConsoleLogger.Info("Showing network configuration with a bluetooth connection ;)");
            ConsoleLogger.Info("bt >> set up all wireless connections");
            Terminal.Execute("rfkill unblock all");
            var btDirs = Directory.EnumerateDirectories("/var/lib/bluetooth").ToArray();
            if (btDirs.Length > 0) {
                ConsoleLogger.Info("bt >> create bt configuration file");
                var btDir = btDirs.FirstOrDefault();
                var dirName = Path.GetFullPath(btDir);
                var fileName = $"{dirName}/settings".Replace("//", "/");
                if (File.Exists(fileName)) {
                    File.Delete(fileName);
                }
                var fileLines = new string[] {
                    "[General]",
                    "Discoverable=true",
                    $"Alias={bluetoothConnectionName}",
                };
                FileSystem.WriteFile(fileName, String.Join(n, fileLines));
                ConsoleLogger.Info("bt >> restart bluetooth service");
                Terminal.Execute("systemctl restart bluetooth");
                ConsoleLogger.Info("bt >> activate bluetooth connection");
                var btCombo = $"power on{n}discoverable on{n}agent on{n}quit{n}";
                Terminal.Execute($"echo -e \"{btCombo}\" | bluetoothctl");
                ConsoleLogger.Info("bt >> done!");
            }
            else {
                ConsoleLogger.Info("bt >> no bluetooth device found!");
            }
        }
    }
}
