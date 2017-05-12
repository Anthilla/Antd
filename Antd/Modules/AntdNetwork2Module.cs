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
using antdlib.common;
using antdlib.config;
using antdlib.models;
using Nancy;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using anthilla.commands;
using Random = anthilla.core.Random;

namespace Antd.Modules {
    public class AntdNetwork2Module : NancyModule {

        public AntdNetwork2Module() {

            Get["/network2"] = x => {
                var physicalInterfaces = Network2Configuration.InterfacePhysical.ToList();
                var bridgeInterfaces = Network2Configuration.InterfaceBridge.ToList();
                var bondInterfaces = Network2Configuration.InterfaceBond.ToList();
                var virtualInterfaces = Network2Configuration.InterfaceVirtual.ToList();
                foreach(var vif in virtualInterfaces) {
                    if(physicalInterfaces.Any(_ => _ == vif) ||
                        bridgeInterfaces.Any(_ => _ == vif) ||
                        bondInterfaces.Any(_ => _ == vif)) {
                        virtualInterfaces.Remove(vif);
                    }
                }
                var allifs = new List<string>();
                allifs.AddRange(physicalInterfaces);
                allifs.AddRange(bridgeInterfaces);
                allifs.AddRange(bondInterfaces);
                var model = new PageNetwork2Model {
                    PhysicalIf = physicalInterfaces,
                    BridgeIf = bridgeInterfaces,
                    BondIf = bondInterfaces,
                    VirtualIf = virtualInterfaces,
                    AllIfs = allifs, //new List <string> { "dev1", "dev2", "dev3" },
                    InterfaceConfigurationList = Network2Configuration.InterfaceConfigurationList,
                    GatewayConfigurationList = Network2Configuration.GatewayConfigurationList,
                    DnsConfigurationList = Network2Configuration.DnsConfigurationList,
                    Configuration = Network2Configuration.Conf.Interfaces,
                    Variables = Host2Configuration.Host
                };
                return JsonConvert.SerializeObject(model);
            };

            Post["/network2/restart"] = x => {
                new Do().NetworkChanges();
                return HttpStatusCode.OK;
            };

            Post["/network2/interfaceconfiguration"] = x => {
                string id = Request.Form.Id;
                string type = Request.Form.Type;
                var typedType = type?.ToEnum<NetworkInterfaceType>() ?? NetworkInterfaceType.Null;

                if(typedType == NetworkInterfaceType.Null) {
                    return HttpStatusCode.InternalServerError;
                }

                var index = Network2Configuration.InterfaceConfigurationList.Count(_ => _.Type == typedType);
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

                var vars = Host2Configuration.Host;

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

                var broadcast = "";
                try {
                    broadcast = Cidr.CalcNetwork(ip, subnet).Broadcast.ToString();
                }
                catch(Exception ex) {
                    ConsoleLogger.Error(ex.Message);
                }

                var model = new NetworkInterfaceConfiguration {
                    Id = string.IsNullOrEmpty(id) ? Random.ShortGuid() : id,
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
                Network2Configuration.AddInterfaceConfiguration(model);
                return HttpStatusCode.OK;
            };

            Post["/network2/interfaceconfiguration/del"] = x => {
                string guid = Request.Form.Guid;
                Network2Configuration.RemoveInterfaceConfiguration(guid);
                return HttpStatusCode.OK;
            };

            Post["/network2/gatewayconfiguration"] = x => {
                string id = Request.Form.Id;
                string route = Request.Form.Route;
                string gatewayAddress = Request.Form.GatewayAddress;
                var model = new NetworkGatewayConfiguration {
                    Id = string.IsNullOrEmpty(id) ? Random.ShortGuid() : id,
                    Route = route,
                    GatewayAddress = gatewayAddress
                };
                Network2Configuration.AddGatewayConfiguration(model);
                return HttpStatusCode.OK;
            };

            Post["/network2/gatewayconfiguration/del"] = x => {
                string guid = Request.Form.Guid;
                Network2Configuration.RemoveGatewayConfiguration(guid);
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
                string dest = Request.Form.Destination;
                string domain = Request.Form.Domain;
                string ip = Request.Form.Ip;
                string auth = Request.Form.Auth;
                var model = new DnsConfiguration {
                    Id = string.IsNullOrEmpty(id) ? Random.ShortGuid() : id,
                    Type = typedType,
                    Mode = typedMode,
                    Dest = dest?.ToEnum<DnsDestination>() ?? DnsDestination.Internal,
                    Domain = domain,
                    Ip = ip,
                    AuthenticationEnabled = auth?.ToBoolean() ?? true
                };
                Network2Configuration.AddDnsConfiguration(model);
                return HttpStatusCode.OK;
            };

            Post["/network2/dnsconfiguration/del"] = x => {
                string guid = Request.Form.Guid;
                Network2Configuration.RemoveDnsConfiguration(guid);
                return HttpStatusCode.OK;
            };

            Post["/network2/dnsconfiguration/active"] = x => {
                string guid = Request.Form.Guid;
                Network2Configuration.SetDnsConfigurationActive(guid);
                return HttpStatusCode.OK;
            };

            Post["/network2/dnsconfiguration/active/del"] = x => {
                string guid = Request.Form.Guid;
                Network2Configuration.SetDnsConfigurationActive(guid);
                return HttpStatusCode.OK;
            };

            Post["/network2/nsupdateconfiguration"] = x => {
                string id = Request.Form.Id;
                string serverName = Request.Form.ServerName;
                string serverPort = Request.Form.ServerPort;
                string localAddress = Request.Form.LocalAddress;
                string localPort = Request.Form.LocalPort;
                string zoneName = Request.Form.ZoneName;
                string className = Request.Form.ClassName;
                string nxDomain = Request.Form.NxDomain;
                string yxDomain = Request.Form.YxDomain;
                string nxRrset = Request.Form.NxRrset;
                string yxRrset = Request.Form.YxRrset;
                string delete = Request.Form.Delete;
                string add = Request.Form.Add;
                var model = new NsUpdateConfiguration {
                    Id = string.IsNullOrEmpty(id) ? Random.ShortGuid() : id,
                    ServerName = serverName,
                    ServerPort = serverPort,
                    LocalAddress = localAddress,
                    LocalPort = localPort,
                    ZoneName = zoneName,
                    ClassName = className,
                    NxDomain = nxDomain,
                    YxDomain = yxDomain,
                    NxRrset = nxRrset,
                    YxRrset = yxRrset,
                    Delete = delete,
                    Add = add
                };
                Network2Configuration.AddNsUpdateConfiguration(model);
                return HttpStatusCode.OK;
            };

            Post["/network2/nsupdateconfiguration/del"] = x => {
                string guid = Request.Form.Guid;
                Network2Configuration.RemoveNsUpdateConfiguration(guid);
                return HttpStatusCode.OK;
            };

            Post["/network2/interface"] = x => {
                string dev = Request.Form.Device;
                string conf = Request.Form.Configuration;

                //var cc = Network2Configuration.InterfaceConfigurationList.FirstOrDefault(_ => _.Id == conf)?.IsUsed;
                //if(cc == true || cc == null) {
                //    return HttpStatusCode.InternalServerError;
                //}

                string confs = Request.Form.AdditionalConfigurations;
                string gwConf = Request.Form.GatewayConfiguration;

                var cc2 = Network2Configuration.GatewayConfigurationList.FirstOrDefault(_ => _.Id == gwConf)?.IsUsed;

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
                Network2Configuration.AddInterfaceSetting(model);

                new Do().NetworkChanges();

                return HttpStatusCode.OK;
            };

            Post["/network2/interface/del"] = x => {
                string dev = Request.Form.Device;
                Network2Configuration.RemoveInterfaceSetting(dev);

                new Do().NetworkChanges();

                return HttpStatusCode.OK;
            };

            Post["/network2/add/bond"] = x => {
                string name = Request.Form.Name;
                try {
                    CommandLauncher.Launch("bond-set", new Dictionary<string, string> { { "$bond", name } });
                    ConsoleLogger.Log($"created bond {name}");
                }
                catch(Exception ex) {
                    ConsoleLogger.Error(ex.Message);
                }
                return HttpStatusCode.OK;
            };

            Post["/network2/add/bridge"] = x => {
                string name = Request.Form.Name;
                try {
                    CommandLauncher.Launch("brctl-add", new Dictionary<string, string> { { "$bridge", name } });
                    ConsoleLogger.Log($"created bridge {name}");
                }
                catch(Exception ex) {
                    ConsoleLogger.Error(ex.Message);
                }
                return HttpStatusCode.OK;
            };
        }
    }
}