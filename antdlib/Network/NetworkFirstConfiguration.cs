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
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace antdlib.Network {
    public class NetworkFirstConfiguration {

        //private static string fileName = "antd.boot.network.type";

        /// <summary>
        /// todo fix
        /// </summary>
        /// <returns></returns>
        private static bool CheckNetworkIsConfigured() {
            return true;
        }

        public static void SetNetwork() {
            if (CheckNetworkIsConfigured() == true) {
                //configura coi parametri esistenti
            }
            else {
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
                    NIFlist.Add(r);
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

        /// <summary>
        /// todo fix
        /// </summary>
        /// <returns></returns>
        private static string PickIP() {
            return "10.11.19.1/16";
        }

        /// <summary>
        /// todo fix
        /// </summary>
        /// <returns></returns>
        private static void ShowNetworkInfo(string nif, string ip) {

        }

        private static void SetNetworkInterfaceUp() {
            if (DetectActiveNetworkInterfaces().Count > 0) {
                var selectedNIF = DetectActiveNetworkInterfaces().LastOrDefault();
                ConsoleLogger.Info($"_> Network will be initialized on: {selectedNIF}");
                var ip = PickIP();
                ConsoleLogger.Info($"_> Assigning {ip} to {selectedNIF}");
                Terminal.Execute($"ip addr add {ip} dev {selectedNIF}");
                Terminal.Execute($"ip route add default via {selectedNIF}");
                ShowNetworkInfo(selectedNIF, ip);
            }
            else {
                ConsoleLogger.Warn("There's no active network interface,");
                ConsoleLogger.Warn("a direct action is required on the machine!");
            }
        }
    }
}
