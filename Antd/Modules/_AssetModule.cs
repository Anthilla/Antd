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
using System.Dynamic;
using System.Linq;
using antd.commands;
using antdlib.common;
using antdlib.common.Tool;
using Antd.Asset;
using Antd.Ssh;
using Antd.SyncMachine;
using Nancy;
using Nancy.Security;

namespace Antd.Modules {
    public class AssetModule : NancyModule {

        public class AvahiServiceViewModel {
            public string HostName { get; set; }
            public string Ip { get; set; }
            public string Port { get; set; }
            public string MacAddress { get; set; }
            public bool IsKnown { get; set; }
        }

        public class NmapScanStatus {
            public string Protocol { get; set; } = "";
            public string Status { get; set; } = "";
            public string Type { get; set; } = "";
        }

        public AssetModule() {
            this.RequiresAuthentication();

            #region [    Home    ]
            Get["/asset"] = x => {
                dynamic vmod = new ExpandoObject();
                return View["antd/page-asset", vmod];
            };
            #endregion

            #region [    Partials    ]
            Get["/part/asset/discovery"] = x => {
                try {
                    dynamic viewModel = new ExpandoObject();
                    var avahiBrowse = new AvahiBrowse();
                    avahiBrowse.DiscoverService("antd");
                    var localServices = avahiBrowse.Locals;
                    var launcher = new CommandLauncher();
                    var list = new List<AvahiServiceViewModel>();
                    var kh = new SshKnownHosts();
                    foreach(var ls in localServices) {
                        var arr = ls.Split(new[] { ":" }, StringSplitOptions.RemoveEmptyEntries);
                        var mo = new AssetModule.AvahiServiceViewModel {
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
                    //var hostnamectl = launcher.Launch("hostnamectl").ToList();
                    //var ssoree = StringSplitOptions.RemoveEmptyEntries;
                    //var myHostName = hostnamectl?.First(_ => _.Contains("Transient hostname:")).Split(new[] { ":" }, 2, ssoree)[1];
                    viewModel.AntdAvahiServices = list/*.Where(_ => !_.HostName.ToLower().Contains(myHostName.ToLower())).OrderBy(_ => _.HostName)*/;
                    return View["antd/part/page-asset-discovery", viewModel];
                }
                catch(Exception ex) {
                    ConsoleLogger.Error($"{Request.Url} request failed: {ex.Message}");
                    ConsoleLogger.Error(ex);
                    return View["antd/part/page-error"];
                }
            };

            Get["/part/asset/scan"] = x => {
                try {
                    dynamic viewModel = new ExpandoObject();
                    var settings = new NetscanSetting();
                    var set = settings.Get();
                    var values = set.Values.Where(_ => !string.IsNullOrEmpty(_.Label));
                    viewModel.Values = values.ToDictionary(k => k.Label, v => set.Subnet + v.Number + ".0");
                    return View["antd/part/page-asset-scan", viewModel];
                }
                catch(Exception ex) {
                    ConsoleLogger.Error($"{Request.Url} request failed: {ex.Message}");
                    ConsoleLogger.Error(ex);
                    return View["antd/part/page-error"];
                }
            };

            Get["/part/asset/sync"] = x => {
                try {
                    dynamic viewModel = new ExpandoObject();
                    var settings = new SyncMachineConfiguration();
                    var set = settings.Get();
                    var syncedMachines = set.Machines.Any() ? set.Machines : new List<SyncMachineModel>();
                    viewModel.SyncedMachines = syncedMachines.OrderBy(_ => _.MachineAddress);
                    return View["antd/part/page-asset-syncmachine", viewModel];
                }
                catch(Exception ex) {
                    ConsoleLogger.Error($"{Request.Url} request failed: {ex.Message}");
                    ConsoleLogger.Error(ex);
                    return View["antd/part/page-error"];
                }
            };

            Get["/part/asset/setting"] = x => {
                try {
                    dynamic viewModel = new ExpandoObject();
                    var settings = new NetscanSetting();
                    var set = settings.Get();
                    viewModel.SettingsSubnet = set.Subnet;
                    viewModel.SettingsSubnetLabel = set.SubnetLabel;
                    viewModel.Settings = set.Values;
                    return View["antd/part/page-asset-setting", viewModel];
                }
                catch(Exception ex) {
                    ConsoleLogger.Error($"{Request.Url} request failed: {ex.Message}");
                    ConsoleLogger.Error(ex);
                    return View["antd/part/page-error"];
                }
            };
            #endregion

            #region [    Actions    ]
            Post["/netscan/setsubnet"] = x => {
                string subnet = Request.Form.Subnet;
                string label = Request.Form.Label;
                if(string.IsNullOrEmpty(subnet) || string.IsNullOrEmpty(label)) {
                    return HttpStatusCode.BadRequest;
                }
                var settings = new NetscanSetting();
                settings.SetSubnet(subnet, label);
                settings.SaveEtcNetworks();
                return HttpStatusCode.OK;
            };

            Post["/netscan/setlabel"] = x => {
                string letter = Request.Form.Letter;
                string number = Request.Form.Number;
                string label = Request.Form.Label;
                if(string.IsNullOrEmpty(letter) || string.IsNullOrEmpty(number) || string.IsNullOrEmpty(label)) {
                    return HttpStatusCode.BadRequest;
                }
                var settings = new NetscanSetting();
                settings.SetLabel(letter, number, label);
                return HttpStatusCode.OK;
            };

            Get["/asset/nmap/{ip}"] = x => {
                string ip = x.ip;
                if(string.IsNullOrEmpty(ip)) {
                    return HttpStatusCode.BadRequest;
                }
                var launcher = new CommandLauncher();
                var result = launcher.Launch("nmap-ip-fast", new Dictionary<string, string> { { "$ip", ip } }).Where(_ => !_.Contains("MAC Address")).Skip(5).Reverse().Skip(1).Reverse();
                var list = new List<NmapScanStatus>();
                foreach(var r in result) {
                    var a = r.SplitToList(" ").ToArray();
                    var mo = new NmapScanStatus {
                        Protocol = a[0],
                        Status = a[1],
                        Type = a[2]
                    };
                    list.Add(mo);
                }
                return Response.AsJson(list.OrderBy(_ => _.Protocol));
            };

            Get["/asset/wol/{mac}"] = x => {
                string mac = x.mac;
                if(string.IsNullOrEmpty(mac)) {
                    return HttpStatusCode.BadRequest;
                }
                var launcher = new CommandLauncher();
                launcher.Launch("wol", new Dictionary<string, string> { { "$mac", mac } });
                return Response.AsJson(true);
            };

            Get["/asset/scan/{subnet}"] = x => {
                string subnet = x.subnet;
                if(string.IsNullOrEmpty(subnet)) {
                    return HttpStatusCode.BadRequest;
                }
                var launcher = new CommandLauncher();
                var result = launcher.Launch("nmap-ip-sp", new Dictionary<string, string> { { "$subnet", subnet + "/24" } }).Skip(1).Reverse().Skip(1).Reverse();
                return Response.AsJson(result.OrderBy(_ => _));
            };
            #endregion

            #region [    Hooks    ]
            After += ctx => {
                if(ctx.Response.ContentType == "text/html") {
                    ctx.Response.ContentType = "text/html; charset=utf-8";
                }
            };
            #endregion
        }
    }
}