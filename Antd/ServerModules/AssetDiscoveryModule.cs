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
using System.Linq;
using antd.commands;
using antdlib.common;
using antdlib.config;
using antdlib.models;
using Antd.Asset;
using Nancy;
using Newtonsoft.Json;

namespace Antd.ServerModules {
    public class AssetDiscoveryModule : NancyModule {

        public AssetDiscoveryModule() {
            Get["/discovery"] = x => {
                var avahiBrowse = new AvahiBrowse();
                avahiBrowse.DiscoverService("antd");
                var localServices = avahiBrowse.Locals;
                var launcher = new CommandLauncher();
                var list = new List<AvahiServiceViewModel>();
                var kh = new SshKnownHosts();
                foreach(var ls in localServices) {
                    var arr = ls.Split(new[] { ":" }, StringSplitOptions.RemoveEmptyEntries);
                    var mo = new AvahiServiceViewModel {
                        HostName = arr[0].Trim(),
                        Ip = arr[1].Trim(),
                        Port = arr[2].Trim(),
                        MacAddress = ""
                    };
                    launcher.Launch("ping-c", new Dictionary<string, string> { { "$ip", arr[1].Trim() } });
                    var result = launcher.Launch("arp", new Dictionary<string, string> { { "$ip", arr[1].Trim() } }).ToList();
                    if(result.Any()) {
                        var mac = result.LastOrDefault().Print(3, " ");
                        mo.MacAddress = mac;
                    }
                    mo.IsKnown = kh.Hosts.Contains(arr[1].Trim());
                    list.Add(mo);
                }
                var model = new PageAssetDiscoveryModel {
                    AntdAvahiServices = list
                };
                return JsonConvert.SerializeObject(model);
            };
        }
    }
}