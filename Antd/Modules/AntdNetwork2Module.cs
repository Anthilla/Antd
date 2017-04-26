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
using Nancy;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

using Random = anthilla.core.Random;

namespace Antd.Modules {
    public class AntdNetwork2Module : NancyModule {

        private readonly Host2Configuration _variablesConfiguration = new Host2Configuration();
        private readonly Network2Configuration _network2Configuration = new Network2Configuration();

        public AntdNetwork2Module() {

            Get["/network2"] = x => {
                var physicalInterfaces = _network2Configuration.InterfacePhysical.ToList();
                var bridgeInterfaces = _network2Configuration.InterfaceBridge.ToList();
                var bondInterfaces = _network2Configuration.InterfaceBond.ToList();
                var virtualInterfaces = _network2Configuration.InterfaceVirtual.ToList();
                foreach(var vif in virtualInterfaces) {
                    if(physicalInterfaces.Any(_ => _ == vif) ||
                    bridgeInterfaces.Any(_ => _ == vif) ||
                    bondInterfaces.Any(_ => _ == vif)) {
                        virtualInterfaces.Remove(vif);
                    }
                }
                var model = new PageNetwork2Model {
                    PhysicalIf = physicalInterfaces,
                    BridgeIf = bridgeInterfaces,
                    BondIf = bondInterfaces,
                    VirtualIf = virtualInterfaces,
                    InterfaceConfigurationList = _network2Configuration.InterfaceConfigurationList,
                    GatewayConfigurationList = _network2Configuration.GatewayConfigurationList,
                    DnsConfigurationList =  _network2Configuration.DnsConfigurationList,
                    Configuration = _network2Configuration.Conf.Interfaces,
                    Variables = _variablesConfiguration.Host
                };
                return JsonConvert.SerializeObject(model);
            };

            Post["/network2/restart"] = x => {
                _network2Configuration.Start();
                return HttpStatusCode.OK;
            };

            Post["/network2/interfaceconfiguration"] = x => {
                string id = Request.Form.Id;
                string type = Request.Form.Type;
                var typedType = type?.ToEnum<NetworkInterfaceType>() ?? NetworkInterfaceType.Null;

                if(typedType == NetworkInterfaceType.Null) {
                    return HttpStatusCode.InternalServerError;
                }

                var index = _network2Configuration.InterfaceConfigurationList.Count(_ => _.Type == typedType);
                string description = Request.Form.Description;

                string verb = Request.Form.Verb;
                var typedVerb = verb?.ToEnum<NetworkRoleVerb>() ?? NetworkRoleVerb.Null;

                if(typedVerb == NetworkRoleVerb.Null) {
                    return HttpStatusCode.InternalServerError;
                }

                var alias = $"{typedVerb.ToString()}{index:D2}";

                string mode = Request.Form.Mode;
                var typedMode = mode?.ToEnum<NetworkInterfaceMode>() ?? NetworkInterfaceMode.Dynamic;
                string status = Request.Form.Status;
                var typedStatus = status?.ToEnum<NetworkInterfaceStatus>() ?? NetworkInterfaceStatus.Down;

                //todo guardare vars interno o esterno
                string ip = Request.Form.Ip;

                string adapter = Request.Form.Adapter;
                var typedAdapter = adapter?.ToEnum<NetworkAdapterType>() ?? NetworkAdapterType.Other;
                if(typedAdapter == NetworkAdapterType.Other) {
                    return HttpStatusCode.InternalServerError;
                }

                string children = Request.Form.Ifs;

                var vars = _variablesConfiguration.Host;

                var hostname = "";
                var subnet = "";
                if(typedType == NetworkInterfaceType.Internal) {
                    hostname = $"{vars.HostName}{typedVerb.ToString()}.{vars.InternalDomainPrimary}";
                    subnet = vars.InternalNetPrimaryBits;
                }
                if(typedType == NetworkInterfaceType.External) {
                    hostname = $"{vars.HostName}{typedVerb.ToString()}.{vars.ExternalDomainPrimary}";
                    subnet = vars.ExternalNetPrimaryBits;
                }

                var broadcast = Cidr.CalcNetwork(ip, subnet).Broadcast.ToString();

                var model = new NetworkInterfaceConfiguration {
                    Id = id ?? Random.ShortGuid(),
                    Type = typedType,
                    Hostname = hostname,
                    Index = index,
                    Description = description,
                    RoleVerb = typedVerb,
                    Alias = alias,
                    Mode = typedMode,
                    Status = typedStatus,
                    Ip = ip,
                    Subnet = subnet,
                    Broadcast = broadcast,
                    Adapter = typedAdapter,
                    ChildrenIf = children == null ? new List<string>() : children.SplitToList()
                };
                _network2Configuration.AddInterfaceConfiguration(model);
                return HttpStatusCode.OK;
            };

            Post["/network2/interfaceconfiguration/del"] = x => {
                string guid = Request.Form.Guid;
                _network2Configuration.RemoveInterfaceSetting(guid);
                return HttpStatusCode.OK;
            };

            Post["/network2/gatewayconfiguration"] = x => {
                string id = Request.Form.Id;
                string route = Request.Form.Route;
                string gatewayAddress = Request.Form.GatewayAddress;
                var model = new NetworkGatewayConfiguration {
                    Id = id ?? Random.ShortGuid(),
                    Route = route,
                    GatewayAddress = gatewayAddress
                };
                _network2Configuration.AddGatewayConfiguration(model);
                return HttpStatusCode.OK;
            };

            Post["/network2/gatewayconfiguration/del"] = x => {
                string guid = Request.Form.Guid;
                _network2Configuration.RemoveGatewayConfiguration(guid);
                return HttpStatusCode.OK;
            };

            Post["/network2/dnsconfiguration"] = x => {
                string id = Request.Form.Id;
                string type = Request.Form.Type;
                var typedType = type?.ToEnum<DnsType>() ?? DnsType.Null;
                if(typedType == DnsType.Null) {
                    return HttpStatusCode.InternalServerError;
                }
                string mode = Request.Form.Mode;
                var typedMode = mode?.ToEnum<DnsMode>() ?? DnsMode.Null;

                if(typedMode == DnsMode.Null) {
                    return HttpStatusCode.InternalServerError;
                }
                string domain = Request.Form.Domain;
                string ip = Request.Form.Ip;
                string auth = Request.Form.Auth;
                var model = new DnsConfiguration {
                    Id = id ?? Random.ShortGuid(),
                    Type = typedType,
                    Mode = typedMode,
                    Domain = domain,
                    Ip = ip,
                    AuthenticationEnabled = auth?.ToBoolean() ?? true
                };
                _network2Configuration.AddDnsConfiguration(model);
                return HttpStatusCode.OK;
            };

            Post["/network2/dnsconfiguration/del"] = x => {
                string guid = Request.Form.Guid;
                _network2Configuration.RemoveDnsConfiguration(guid);
                return HttpStatusCode.OK;
            };

            Post["/network2/dnsconfiguration/active"] = x => {
                string guid = Request.Form.Guid;
                _network2Configuration.SetDnsConfigurationActive(guid);
                return HttpStatusCode.OK;
            };

            Post["/network2/dnsconfiguration/active/del"] = x => {
                string guid = Request.Form.Guid;
                _network2Configuration.SetDnsConfigurationActive(guid);
                return HttpStatusCode.OK;
            };

            Post["/network2/interface"] = x => {
                string dev = Request.Form.Device;
                string conf = Request.Form.Configuration;

                var cc = _network2Configuration.InterfaceConfigurationList.FirstOrDefault(_ => _.Id == conf)?.IsUsed;
                if(cc == true || cc == null) {
                    return HttpStatusCode.InternalServerError;
                }

                string confs = Request.Form.AdditionalConfigurations;
                string gwConf = Request.Form.GatewayConfiguration;

                var cc2 = _network2Configuration.GatewayConfigurationList.FirstOrDefault(_ => _.Id == gwConf)?.IsUsed;

                string txqueuelen = Request.Form.Txqueuelen;
                string mtu = Request.Form.Mtu;
                var model = new NetworkInterface {
                    Device = dev,
                    Configuration = conf,
                    AdditionalConfigurations = confs == null ? new List<string>() : confs.SplitToList(),
                    GatewayConfiguration = cc2 == true || cc2 == null ? "" : gwConf,
                    Txqueuelen = txqueuelen,
                    Mtu = mtu
                };
                _network2Configuration.AddInterfaceSetting(model);
                return HttpStatusCode.OK;
            };

            Post["/network2/interface/del"] = x => {
                string dev = Request.Form.Device;
                _network2Configuration.RemoveInterfaceSetting(dev);
                return HttpStatusCode.OK;
            };
        }
    }
}