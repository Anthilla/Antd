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

using System.Dynamic;
using antdlib.common;
using antdlib.views;
using Antd.Database;
using Antd.DhcpServer;
using Nancy;
using Nancy.Security;

namespace Antd.Modules {
    public class ServicesModule : CoreModule {

        private readonly DhcpServerOptionsRepository _dhcpServerOptionsRepository = new DhcpServerOptionsRepository();
        private readonly DhcpServerSubnetRepository _dhcpServerSubnetRepository = new DhcpServerSubnetRepository();
        private readonly DhcpServerClassRepository _dhcpServerClassRepository = new DhcpServerClassRepository();
        private readonly DhcpServerPoolRepository _dhcpServerPoolRepository = new DhcpServerPoolRepository();
        private readonly DhcpServerReservationRepository _dhcpServerReservationRepository = new DhcpServerReservationRepository();

        private readonly DhcpdConfiguration _dhcpdConfiguration = new DhcpdConfiguration();

        public ServicesModule() {
            this.RequiresAuthentication();

            Get["/services"] = x => {
                dynamic vmod = new ExpandoObject();

                //vmod.SambaIsActive = SambaConfig.IsActive;
                //vmod.SambaStructure = SambaConfig.SimpleStructure;
                //var sambaMapFile = SambaConfig.MapFile.Get();
                //vmod.SambaData = sambaMapFile.Data;
                //vmod.SambaShareData = sambaMapFile.Share;

                //vmod.BindIsActive = BindConfig.IsActive;
                //var bindMapFile = BindConfig.MapFile.Get();
                //vmod.BindOptions = bindMapFile.BindOptions;
                //vmod.BindLogging = bindMapFile.BindLogging;
                //vmod.BindLwres = bindMapFile.BindLwres;
                //vmod.BindStatisticsChannels = bindMapFile.BindStatisticsChannels;
                //vmod.BindTrustedKeys = bindMapFile.BindTrustedKeys;
                //vmod.BindManagedKeys = bindMapFile.BindManagedKeys;
                //vmod.BindZone = bindMapFile.BindZone;
                //vmod.BindAcl = bindMapFile.BindAcl;
                //vmod.BindKey = bindMapFile.BindKey;
                //vmod.BindMasters = bindMapFile.BindMasters;
                //vmod.BindServer = bindMapFile.BindServer;
                //vmod.BindView = bindMapFile.BindView;

                var dhcpdIsActive = _dhcpServerOptionsRepository.Get() != null && _dhcpServerSubnetRepository.Get() != null;
                vmod.DhcpdIsActive = dhcpdIsActive;
                vmod.DhcpdOptions = _dhcpServerOptionsRepository.Get();
                vmod.DhcpdSubnet = _dhcpServerSubnetRepository.Get();
                vmod.DhcpdClass = _dhcpServerClassRepository.GetAll();
                vmod.DhcpdPools = _dhcpServerPoolRepository.GetAll();
                vmod.DhcpdReservation = _dhcpServerReservationRepository.GetAll();

                vmod.CurrentContext = Request.Path;
                return View["_page-services", vmod];
            };

            #region [    DHCPD    ]
            Post["/services/dhcpd/enable"] = x => {
                _dhcpdConfiguration.Enable();
                return HttpStatusCode.OK;
            };

            Post["/services/dhcpd/disable"] = x => {
                _dhcpdConfiguration.Disable();
                return HttpStatusCode.OK;
            };

            Post["/services/dhcpd/set"] = x => {
                _dhcpdConfiguration.Set();
                return HttpStatusCode.OK;
            };

            Post["/services/dhcpd/restart"] = x => {
                _dhcpdConfiguration.Enable();
                _dhcpdConfiguration.Restart();
                return HttpStatusCode.OK;
            };

            Post["/services/dhcpd/stop"] = x => {
                _dhcpdConfiguration.Stop();
                return HttpStatusCode.OK;
            };

            Post["/services/dhcpd/options"] = x => {
                string allow = Request.Form.Allow;
                string updateStaticLeases = Request.Form.UpdateStaticLeases;
                string updateConflictDetection = Request.Form.UpdateConflictDetection;
                string useHostDeclNames = Request.Form.UseHostDeclNames;
                string doForwardUpdates = Request.Form.DoForwardUpdates;
                string doReverseUpdates = Request.Form.DoReverseUpdates;
                string logFacility = Request.Form.LogFacility;
                string option = Request.Form.Option;
                string zoneName = Request.Form.ZoneName;
                string zonePrimaryAddress = Request.Form.ZonePrimaryAddress;
                string ddnsUpdateStyle = Request.Form.DdnsUpdateStyle;
                string ddnsUpdates = Request.Form.DdnsUpdates;
                string ddnsDomainName = Request.Form.DdnsDomainName;
                string ddnsRevDomainName = Request.Form.DdnsRevDomainName;
                string defaultLeaseTime = Request.Form.DefaultLeaseTime;
                string maxLeaseTime = Request.Form.MaxLeaseTime;
                string keyName = Request.Form.KeyName;
                string keySecret = Request.Form.KeySecret;
                var model = new DhcpServerOptionsModel {
                    Allow = allow.SplitToList(),
                    UpdateStaticLeases = updateStaticLeases,
                    UpdateConflictDetection = updateConflictDetection,
                    UseHostDeclNames = useHostDeclNames,
                    DoForwardUpdates = doForwardUpdates,
                    DoReverseUpdates = doReverseUpdates,
                    LogFacility = logFacility,
                    Option = option.SplitToList(),
                    ZoneName = zoneName,
                    ZonePrimaryAddress = zonePrimaryAddress,
                    DdnsUpdateStyle = ddnsUpdateStyle,
                    DdnsUpdates = ddnsUpdates,
                    DdnsDomainName = ddnsDomainName,
                    DdnsRevDomainName = ddnsRevDomainName,
                    DefaultLeaseTime = defaultLeaseTime,
                    MaxLeaseTime = maxLeaseTime,
                    KeyName = keyName,
                    KeySecret = keySecret
                };
                _dhcpServerOptionsRepository.Set(model);
                return Response.AsRedirect("/services");
            };

            Post["/services/dhcpd/subnet"] = x => {
                string ipFamily = Request.Form.IpFamily;
                string ipMask = Request.Form.IpMask;
                string optionRouters = Request.Form.OptionRouters;
                string ntpServers = Request.Form.NtpServers;
                string timeServers = Request.Form.DoForTimeServerswardUpdates;
                string domainNameServers = Request.Form.DomainNameServers;
                string broadcastAddress = Request.Form.BroadcastAddress;
                string subnetMask = Request.Form.SubnetMask;
                string zoneName = Request.Form.ZoneName;
                string zonePrimaryAddress = Request.Form.ZonePrimaryAddress;
                var model = new DhcpServerSubnetModel {
                    IpFamily = ipFamily,
                    IpMask = ipMask,
                    OptionRouters = optionRouters,
                    NtpServers = ntpServers,
                    TimeServers = timeServers,
                    DomainNameServers = domainNameServers,
                    BroadcastAddress = broadcastAddress,
                    SubnetMask = subnetMask,
                    ZoneName = zoneName,
                    ZonePrimaryAddress = zonePrimaryAddress,
                };
                _dhcpServerSubnetRepository.Set(model);
                return Response.AsRedirect("/services");
            };

            Post["/services/dhcpd/class/add"] = x => {
                string name = Request.Form.Name;
                string macVendor = Request.Form.MacVendor;
                _dhcpServerClassRepository.Create(name, macVendor);
                return Response.AsRedirect("/services");
            };

            Post["/services/dhcpd/class/del"] = x => {
                string id = Request.Form.Guid;
                _dhcpServerClassRepository.Delete(id);
                return HttpStatusCode.OK;
            };

            Post["/services/dhcpd/pool/add"] = x => {
                string option = Request.Form.Option;
                _dhcpServerPoolRepository.Create(option.SplitToList());
                return Response.AsRedirect("/services");
            };

            Post["/services/dhcpd/pool/del"] = x => {
                string id = Request.Form.Guid;
                _dhcpServerPoolRepository.Delete(id);
                return HttpStatusCode.OK;
            };

            Post["/services/dhcpd/reservation/add"] = x => {
                string hostName = Request.Form.HostName;
                string macAddress = Request.Form.MacAddress;
                string ipAddress = Request.Form.IpAddress;
                _dhcpServerReservationRepository.Create(hostName, macAddress, ipAddress);
                return Response.AsRedirect("/services");
            };

            Post["/services/dhcpd/reservation/del"] = x => {
                string id = Request.Form.Guid;
                _dhcpServerReservationRepository.Delete(id);
                return HttpStatusCode.OK;
            };

            #endregion [    DHCPD    ]

            //#region SAMBA
            //Post["/services/activate/samba"] = x => {
            //    SambaConfig.SetReady();
            //    SambaConfig.MapFile.Render();
            //    return HttpStatusCode.OK;
            //};

            //Post["/services/refresh/samba"] = x => {
            //    SambaConfig.MapFile.Render();
            //    return HttpStatusCode.OK;
            //};

            //Post["/services/reloadconfig/samba"] = x => {
            //    SambaConfig.ReloadConfig();
            //    return HttpStatusCode.OK;
            //};

            //Post["/services/update/samba"] = x => {
            //    var parameters = this.Bind<List<ServiceSamba>>();
            //    SambaConfig.WriteFile.SaveGlobalConfig(parameters);
            //    Thread.Sleep(1000);
            //    SambaConfig.WriteFile.DumpGlobalConfig();
            //    Thread.Sleep(1000);
            //    SambaConfig.WriteFile.RewriteSmbconf();
            //    return Response.AsRedirect("/");
            //};

            //Post["/services/update/sambashares"] = x => {
            //    var parameters = this.Bind<List<ServiceSamba>>();
            //    string file = Request.Form.ShareFile;
            //    string name = Request.Form.ShareName;
            //    string query = Request.Form.ShareQueryName;
            //    SambaConfig.WriteFile.SaveShareConfig(file, name, query, parameters);
            //    Thread.Sleep(1000);
            //    SambaConfig.WriteFile.DumpShare(name);
            //    Thread.Sleep(1000);
            //    SambaConfig.WriteFile.RewriteSmbconf();
            //    return Response.AsRedirect("/");
            //};

            //Post["/services/samba/addparam"] = x => {
            //    string key = Request.Form.NewParameterKey;
            //    string value = Request.Form.NewParameterValue;
            //    SambaConfig.WriteFile.AddParameterToGlobal(key, value);
            //    Thread.Sleep(1000);
            //    SambaConfig.WriteFile.RewriteSmbconf();
            //    return Response.AsRedirect("/");
            //};

            //Post["/services/samba/addshare"] = x => {
            //    string name = Request.Form.NewShareName;
            //    string directory = Request.Form.NewShareDirectory;
            //    SambaConfig.WriteFile.AddShare(name, directory);
            //    Thread.Sleep(1000);
            //    SambaConfig.WriteFile.RewriteSmbconf();
            //    return Response.AsRedirect("/");
            //};
            //#endregion SAMBA

            //#region BIND
            //Post["/services/activate/bind"] = x => {
            //    BindConfig.SetReady();
            //    BindConfig.MapFile.Render();
            //    return HttpStatusCode.OK;
            //};

            //Post["/services/refresh/bind"] = x => {
            //    BindConfig.MapFile.Render();
            //    return HttpStatusCode.OK;
            //};

            //Post["/services/reloadconfig/bind"] = x => {
            //    BindConfig.ReloadConfig();
            //    return HttpStatusCode.OK;
            //};

            //Post["/services/update/bind/{section}"] = x => {
            //    var section = (string)x.section;
            //    var parameters = this.Bind<List<ServiceBind>>();
            //    if (section == "acl") {
            //        BindConfig.WriteFile.SaveAcls(parameters);
            //    }
            //    else {
            //        BindConfig.WriteFile.SaveGlobalConfig(section, parameters);
            //    }
            //    Thread.Sleep(1000);
            //    BindConfig.WriteFile.DumpGlobalConfig();
            //    return Response.AsRedirect("/");
            //};

            //Post["/services/update/bind/zone/{zone}"] = x => {
            //    var zoneName = (string)x.zone;
            //    var parameters = this.Bind<List<ServiceBind>>();
            //    BindConfig.WriteFile.SaveZoneConfig(zoneName, parameters);
            //    Thread.Sleep(1000);
            //    BindConfig.WriteFile.DumpGlobalConfig();
            //    return Response.AsRedirect("/");
            //};

            //Post["/services/bind/addacl"] = x => {
            //    string k = Request.Form.NewAclKey;
            //    string v = Request.Form.NewAclValue;
            //    BindConfig.MapFile.AddAcl(k, v);
            //    return Response.AsRedirect("/");
            //};

            //Post["/services/bind/addkey"] = x => {
            //    string name = Request.Form.NewKeyName;
            //    BindConfig.MapFile.AddKey(name);
            //    return Response.AsRedirect("/");
            //};

            //Post["/services/bind/addmasters"] = x => {
            //    string name = Request.Form.NewMastersName;
            //    BindConfig.MapFile.AddMasters(name);
            //    return Response.AsRedirect("/");
            //};

            //Post["/services/bind/addserver"] = x => {
            //    string name = Request.Form.NewServerName;
            //    BindConfig.MapFile.AddServer(name);
            //    return Response.AsRedirect("/");
            //};

            //Post["/services/bind/addview"] = x => {
            //    string name = Request.Form.NewViewName;
            //    BindConfig.MapFile.AddView(name);
            //    return Response.AsRedirect("/");
            //};

            //Post["/services/bind/addzone"] = x => {
            //    string name = Request.Form.NewZoneName;
            //    BindConfig.MapFile.AddZone(name);
            //    return Response.AsRedirect("/");
            //};
            //#endregion BIND

            #region SSH
            //Post["/services/activate/ssh"] = x => {
            //    SshConfig.SetReady();
            //    SshConfig.MapFile.Render();
            //    return HttpStatusCode.OK;
            //};

            //Post["/services/refresh/ssh"] = x => {
            //    SshConfig.MapFile.Render();
            //    return HttpStatusCode.OK;
            //};

            //Post["/services/reloadconfig/ssh"] = x => {
            //    SshConfig.ReloadConfig();
            //    return HttpStatusCode.OK;
            //};

            //Post["/services/update/ssh/{section}"] = x => {
            //    var parameters = this.Bind<List<ServiceSsh>>();
            //    var section = (string)x.section;
            //    if (section == "global") {
            //        SshConfig.WriteFile.SaveGlobal(parameters);
            //    }
            //    if (section == "prefix6") {
            //        SshConfig.WriteFile.SavePrefix6(parameters);
            //    }
            //    if (section == "range6") {
            //        SshConfig.WriteFile.SaveRange6(parameters);
            //    }
            //    if (section == "range") {
            //        SshConfig.WriteFile.SaveRange(parameters);
            //    }
            //    else {
            //        SshConfig.WriteFile.SaveConfigFor(section, parameters);
            //    }
            //    Thread.Sleep(1000);
            //    SshConfig.WriteFile.DumpGlobalConfig();
            //    return Response.AsRedirect("/");
            //};

            //Post["/services/ssh/addkey"] = x => {
            //    string name = Request.Form.NewKeyName;
            //    SshConfig.Keys.PropagateKeys(name);
            //    return Response.AsRedirect("/");
            //};
            #endregion SSH
        }
    }
}