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

using antdlib.common;
using antdlib.config;
using antdlib.models;
using anthilla.commands;
using Nancy;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Antd.Modules {
    public class AntdNetworkModule : NancyModule {

        public AntdNetworkModule() {

            Get["/network"] = x => {
                var physicalInterfaces = NetworkConfiguration.InterfacePhysicalModel.ToList();
                var bridgeInterfaces = NetworkConfiguration.InterfaceBridgeModel.ToList();
                var bondInterfaces = NetworkConfiguration.InterfaceBondModel.ToList();
                var virtualInterfaces = NetworkConfiguration.InterfaceVirtualModel.ToList();
                foreach(var vif in virtualInterfaces) {
                    if(physicalInterfaces.Any(_ => _.Interface == vif.Interface) ||
                    bridgeInterfaces.Any(_ => _.Interface == vif.Interface) ||
                    bondInterfaces.Any(_ => _.Interface == vif.Interface)) {
                        virtualInterfaces.Remove(vif);
                    }
                }
                var model = new PageNetworkModel {
                    NetworkPhysicalIf = physicalInterfaces,
                    NetworkBridgeIf = bridgeInterfaces,
                    NetworkBondIf = bondInterfaces,
                    NetworkVirtualIf = virtualInterfaces,
                    NetworkIfList = NetworkConfiguration.NetworkInterfaces
                };
                return JsonConvert.SerializeObject(model);
            };

            Post["/network/restart"] = x => {
                NetworkConfiguration.Start();
                return HttpStatusCode.OK;
            };

            Post["/network/interface"] = x => {
                string Interface = Request.Form.Interface;
                string mode = Request.Form.Mode;
                string status = Request.Form.Status;
                string staticAddres = Request.Form.StaticAddres;
                string staticRange = Request.Form.StaticRange;
                string txqueuelen = Request.Form.Txqueuelen;
                string mtu = Request.Form.Mtu;
                string route = Request.Form.Route;
                string gateway = Request.Form.Gateway;
                var model = new NetworkInterfaceConfigurationModel {
                    Interface = Interface,
                    Mode = (NetworkInterfaceMode)Enum.Parse(typeof(NetworkInterfaceMode), mode),
                    Status = (NetworkInterfaceStatus)Enum.Parse(typeof(NetworkInterfaceStatus), status),
                    StaticAddress = staticAddres,
                    StaticRange = staticRange,
                    Txqueuelen = txqueuelen,
                    Mtu = mtu,
                    Type = NetworkAdapterType.Physical,
                    Route = route,
                    Gateway = gateway
                };
                NetworkConfiguration.AddInterfaceSetting(model);
                return HttpStatusCode.OK;
            };

            Post["/network/interface/del"] = x => {
                string guid = Request.Form.Guid;
                NetworkConfiguration.RemoveInterfaceSetting(guid);
                return HttpStatusCode.OK;
            };

            Post["/network/interface/bridge"] = x => {
                string Interface = Request.Form.Interface;
                string mode = Request.Form.Mode;
                string status = Request.Form.Status;
                string staticAddres = Request.Form.StaticAddres;
                string staticRange = Request.Form.StaticRange;
                string txqueuelen = Request.Form.Txqueuelen;
                string mtu = Request.Form.Mtu;
                string ifs = Request.Form.InterfaceList;
                string route = Request.Form.Route;
                string gateway = Request.Form.Gateway;
                var ifList = ifs.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries).ToList();
                var launcher = new CommandLauncher();
                launcher.Launch("brctl-add", new Dictionary<string, string> { { "$bridge", Interface } });
                foreach(var nif in ifList) {
                    launcher.Launch("brctl-add-if", new Dictionary<string, string> { { "$bridge", Interface }, { "$net_if", nif } });
                }
                var model = new NetworkInterfaceConfigurationModel {
                    Interface = Interface,
                    Mode = (NetworkInterfaceMode)Enum.Parse(typeof(NetworkInterfaceMode), mode),
                    Status = (NetworkInterfaceStatus)Enum.Parse(typeof(NetworkInterfaceStatus), status),
                    StaticAddress = staticAddres,
                    StaticRange = staticRange,
                    Txqueuelen = txqueuelen,
                    Mtu = mtu,
                    Type = NetworkAdapterType.Bridge,
                    InterfaceList = ifList,
                    Route = route,
                    Gateway = gateway
                };
                NetworkConfiguration.AddInterfaceSetting(model);
                return HttpStatusCode.OK;
            };

            Post["/network/interface/bond"] = x => {
                string Interface = Request.Form.Interface;
                string mode = Request.Form.Mode;
                string status = Request.Form.Status;
                string staticAddres = Request.Form.StaticAddres;
                string staticRange = Request.Form.StaticRange;
                string txqueuelen = Request.Form.Txqueuelen;
                string mtu = Request.Form.Mtu;
                string ifs = Request.Form.InterfaceList;
                string route = Request.Form.Route;
                string gateway = Request.Form.Gateway;
                var ifList = ifs.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries).ToList();
                var launcher = new CommandLauncher();
                launcher.Launch("bond-set", new Dictionary<string, string> { { "$bond", Interface } });
                foreach(var nif in ifList) {
                    launcher.Launch("bond-add-if", new Dictionary<string, string> { { "$bond", Interface }, { "$net_if", nif } });
                }
                var model = new NetworkInterfaceConfigurationModel {
                    Interface = Interface,
                    Mode = (NetworkInterfaceMode)Enum.Parse(typeof(NetworkInterfaceMode), mode),
                    Status = (NetworkInterfaceStatus)Enum.Parse(typeof(NetworkInterfaceStatus), status),
                    StaticAddress = staticAddres,
                    StaticRange = staticRange,
                    Txqueuelen = txqueuelen,
                    Mtu = mtu,
                    Type = NetworkAdapterType.Bond,
                    InterfaceList = ifList,
                    Route = route,
                    Gateway =  gateway
                };
                NetworkConfiguration.AddInterfaceSetting(model);
                return HttpStatusCode.OK;
            };
        }
    }
}