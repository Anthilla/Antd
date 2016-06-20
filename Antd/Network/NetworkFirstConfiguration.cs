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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using antdlib;
using antdlib.common;

namespace Antd.Network {
    public class NetworkFirstConfiguration {

        private static readonly string FileName = $"{Parameter.RepoDirs}/{Parameter.NetworkConfig}";

        private static bool CheckNetworkHasConfiguration() {
            return File.Exists(FileName) && FileSystem.ReadFile(FileName).Length > 0;
        }

        public static void Set() {
            if (CheckNetworkHasConfiguration())
                return;
            ConsoleLogger.Log("Network config => no configuration found...");
            ConsoleLogger.Log("Network config => applying a default configuration!");
            SetNetworkInterfaceUp();
        }

        private static IEnumerable<string> DetectAllNetworkInterfaces() {
            var niFlist = new List<string>();
            const int m = 15;
            for (var i = 0; i < m; i++) {
                var r = Terminal.Execute($"ip link set eth{i} up");
                if (r.Length > 0) {
                    break;
                }
                niFlist.Add($"eth{i}");
            }
            return niFlist;
        }

        private static List<string> DetectActiveNetworkInterfaces() {
            var niFlist = DetectAllNetworkInterfaces();
            return (from nif in niFlist let r = Terminal.Execute($"cat /sys/class/net/{nif}/carrier") where r == "1" select nif).ToList();
        }

        private static bool IsIpAvailable(string input) {
            var reply = new Ping().Send(input);
            if (reply != null && reply.Status == IPStatus.DestinationHostUnreachable) { return true; }
            if (reply != null && reply.Status == IPStatus.DestinationNetworkUnreachable) { return true; }
            if (reply != null && reply.Status == IPStatus.DestinationPortUnreachable) { return true; }
            if (reply != null && reply.Status == IPStatus.DestinationUnreachable) { return true; }
            return reply != null && reply.Status == IPStatus.TimedOut;
        }

        private static string AugmentIpValue(string input) {
            var octets = input.Split('.').ToIntArray();
            var octet2 = octets[2];
            var octet3 = octets[3] + 1;
            if (octet3 == 255) {
                octet2 = octets[2] + 1;
                octet3 = octets[3];
            }
            var newOctets = new[] {
                octets[0].ToString(),
                octets[1].ToString(),
                octet2.ToString(),
                octet3.ToString()
            };
            return string.Join(".", newOctets);
        }

        private static string GetFirstAvailableIpFrom(string input) {
            var ip = input;
            while (IsIpAvailable(ip) == false) {
                ip = AugmentIpValue(input);
            }
            return ip;
        }

        private static string PickIp() {
            var ipAddr = GetFirstAvailableIpFrom("169.254.1.1");
            return $"{ipAddr}/16";
        }

        private static readonly List<string> Commands = new List<string>();

        private static void SetNetworkInterfaceUp() {
            if (DetectActiveNetworkInterfaces().Count > 0) {
                var selectedNif = DetectActiveNetworkInterfaces().LastOrDefault();
                ConsoleLogger.Log($"_> Network will be initialized on: {selectedNif}");
                var ip = PickIp();
                ConsoleLogger.Log($"_> Assigning {ip} to {selectedNif}");
                var cmd1 = $"ip addr add {ip} dev {selectedNif}";
                Terminal.Execute(cmd1);
                Commands.Add(cmd1);
                var cmd2 = $"ip route add default via {ip}";
                Terminal.Execute(cmd2);
                Commands.Add(cmd2);
                ShowNetworkInfo(selectedNif, ip);
            }
            else {
                ConsoleLogger.Warn("There's no active network interface,");
                ConsoleLogger.Warn("a direct action is required on the machine!");
            }
        }

        private static void ShowNetworkInfo(string nif, string ip) {
            var n = Environment.NewLine;
            var bluetoothConnectionName = $"{nif}_S{ip.Replace("/", "-")}";
            ConsoleLogger.Log("Showing network configuration with a bluetooth connection ;)");
            ConsoleLogger.Log("bt >> set up all wireless connections");
            Terminal.Execute("rfkill unblock all");
            var btDirs = Directory.EnumerateDirectories("/var/lib/bluetooth").ToArray();
            if (btDirs.Length > 0) {
                ConsoleLogger.Log("bt >> create bt configuration file");
                var btDir = btDirs.FirstOrDefault();
                if (btDir != null) {
                    var dirName = Path.GetFullPath(btDir);
                    var replace = $"{dirName}/settings".Replace("//", "/");
                    if (File.Exists(replace)) {
                        File.Delete(replace);
                    }
                    var fileLines = new[] {
                        "[General]",
                        "Discoverable=true",
                        $"Alias={bluetoothConnectionName}"
                    };
                    FileSystem.WriteFile(replace, string.Join(n, fileLines));
                }
                ConsoleLogger.Log("bt >> restart bluetooth service");
                Terminal.Execute("systemctl restart bluetooth");
                ConsoleLogger.Log("bt >> activate bluetooth connection");
                var btCombo = $"power on{n}discoverable on{n}agent on{n}quit{n}";
                Terminal.Execute($"echo -e \"{btCombo}\" | bluetoothctl");
                ConsoleLogger.Log("bt >> done!");
            }
            else {
                ConsoleLogger.Log("bt >> no bluetooth device found!");
            }
        }
    }
}
