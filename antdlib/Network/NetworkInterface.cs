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
using antdlib.Log;

namespace antdlib.Network {

    public enum NetworkInterfaceType {
        Physical = 1,
        Virtual = 2,
        Bond = 3,
        Bridge = 4,
        Other = 99
    }

    public class NetworkInterfaceModel {
        public string _Id { get; set; }
        public string InterfaceName { get; set; }
        public NetworkInterfaceType InterfaceType { get; set; }
        public IEnumerable<NetworkConfig.CommandListModel> PostCommands => NetworkConfig.CommandList.CommandTypePost();
        public IEnumerable<NetworkConfig.CommandListModel> GetCommands => NetworkConfig.CommandList.CommandTypeGet();
        public IEnumerable<NetworkConfig.CommandListModel> BridgePostCommands => NetworkConfig.CommandList.BridgeCommandTypePost();
        public IEnumerable<NetworkConfig.CommandListModel> BridgeGetCommands => NetworkConfig.CommandList.BridgeCommandTypeGet();
    }

    public class NetworkInterface {
        public static IEnumerable<NetworkInterfaceModel> GetAll() => DeNSo.Session.New.Get<NetworkInterfaceModel>();

        private static void FlushDb() => DeNSo.Session.New.DeleteAll(GetAll());

        public static void ImportNetworkInterface() {
            FlushDb();
            if (!AssemblyInfo.IsUnix)
                return;
            var dirs = Directory.GetDirectories("/sys/class/net");
            var physicalIf = from dir in dirs
                             let f = Terminal.Terminal.Execute($"file {dir}")
                             where !f.Contains("virtual") && !f.Contains("fake")
                             select Path.GetFileName(dir);
            foreach (var p in physicalIf) {
                var phMod = new NetworkInterfaceModel {
                    _Id = Guid.NewGuid().ToString(),
                    InterfaceType = NetworkInterfaceType.Physical,
                    InterfaceName = p
                };
                DeNSo.Session.New.Set(phMod);
            }
            var virtualIf = (from dir in dirs
                             let f = Terminal.Terminal.Execute($"file {dir}")
                             where f.Contains("virtual") || f.Contains("fake")
                             select Path.GetFileName(dir)).Where(_ => !_.Contains("bond"));
            foreach (var v in virtualIf) {
                var phMod = new NetworkInterfaceModel {
                    _Id = Guid.NewGuid().ToString(),
                    InterfaceType = NetworkInterfaceType.Virtual,
                    InterfaceName = v
                };
                DeNSo.Session.New.Set(phMod);
            }
            var bondIf = (from dir in dirs
                          let f = Terminal.Terminal.Execute($"file {dir}")
                          where f.Contains("virtual") || f.Contains("fake")
                          select Path.GetFileName(dir)).Where(_ => _.Contains("bond"));
            foreach (var b in bondIf) {
                var phMod = new NetworkInterfaceModel {
                    _Id = Guid.NewGuid().ToString(),
                    InterfaceType = NetworkInterfaceType.Bond,
                    InterfaceName = b
                };
                DeNSo.Session.New.Set(phMod);
            }
            var bridgeIf = Terminal.Terminal.Execute("brctl show").Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries).Skip(1).ToList();
            var brList = new List<string>();
            foreach (var bbrr in bridgeIf) {
                ConsoleLogger.Point(bbrr);
                var brAttr = bbrr.Replace("\t", " ").Replace("/t", " ").Replace("  ", " ").Split(' ')[0];
                brList.Add(brAttr.Trim());
            }
            foreach (var br in brList) {
                ConsoleLogger.Point(br);
                var phMod = new NetworkInterfaceModel {
                    _Id = Guid.NewGuid().ToString(),
                    InterfaceType = NetworkInterfaceType.Bridge,
                    InterfaceName = br
                };
                DeNSo.Session.New.Set(phMod);
            }
        }

        public static IEnumerable<NetworkInterfaceModel> Physical => GetAll().Where(_ => _.InterfaceType == NetworkInterfaceType.Physical).OrderBy(_ => _.InterfaceName);
        public static IEnumerable<NetworkInterfaceModel> Virtual => GetAll().Where(_ => _.InterfaceType == NetworkInterfaceType.Virtual).OrderBy(_ => _.InterfaceName);
        public static IEnumerable<NetworkInterfaceModel> Bond => GetAll().Where(_ => _.InterfaceType == NetworkInterfaceType.Bond).OrderBy(_ => _.InterfaceName);
        public static IEnumerable<NetworkInterfaceModel> Bridge => GetAll().Where(_ => _.InterfaceType == NetworkInterfaceType.Bridge).OrderBy(_ => _.InterfaceName);
    }
}
