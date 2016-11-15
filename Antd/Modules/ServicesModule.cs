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
using antdlib.views;
using Antd.Bind;
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

        private readonly BindServerOptionsRepository _bindServerOptionsRepository = new BindServerOptionsRepository();
        private readonly BindServerZoneRepository _bindServerZoneRepository = new BindServerZoneRepository();

        private readonly BindConfiguration _bindConfiguration = new BindConfiguration();

        public ServicesModule() {
            this.RequiresAuthentication();

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

            #region [    BIND    ]
            Post["/services/bind/enable"] = x => {
                _bindConfiguration.Enable();
                return HttpStatusCode.OK;
            };

            Post["/services/bind/disable"] = x => {
                _bindConfiguration.Disable();
                return HttpStatusCode.OK;
            };

            Post["/services/bind/set"] = x => {
                _bindConfiguration.Set();
                return HttpStatusCode.OK;
            };

            Post["/services/bind/restart"] = x => {
                _bindConfiguration.Enable();
                _bindConfiguration.Restart();
                return HttpStatusCode.OK;
            };

            Post["/services/bind/stop"] = x => {
                _bindConfiguration.Stop();
                return HttpStatusCode.OK;
            };

            Post["/services/bind/reload"] = x => {
                _bindConfiguration.RndcReload();
                return HttpStatusCode.OK;
            };

            Post["/services/bind/reconfig"] = x => {
                _bindConfiguration.RndcReconfig();
                return HttpStatusCode.OK;
            };

            Post["/services/bind/options"] = x => {
                string notify = Request.Form.Notify;
                string maxCacheSize = Request.Form.MaxCacheSize;
                string maxCacheTtl = Request.Form.MaxCacheTtl;
                string maxNcacheTtl = Request.Form.MaxNcacheTtl;
                string forwarders = Request.Form.Forwarders;
                string allowNotify = Request.Form.AllowNotify;
                string allowTransfer = Request.Form.AllowTransfer;
                string recursion = Request.Form.Recursion;
                string transferFormat = Request.Form.TransferFormat;
                string querySourceAddress = Request.Form.QuerySourceAddress;
                string querySourcePort = Request.Form.QuerySourcePort;
                string version = Request.Form.Version;
                string allowQuery = Request.Form.AllowQuery;
                string allowRecursion = Request.Form.AllowRecursion;
                string ixfrFromDifferences = Request.Form.IxfrFromDifferences;
                string listenOnV6 = Request.Form.ListenOnV6;
                string listenOnPort53 = Request.Form.ListenOnPort53;
                string dnssecEnabled = Request.Form.DnssecEnabled;
                string dnssecValidation = Request.Form.DnssecValidation;
                string dnssecLookaside = Request.Form.DnssecLookaside;
                string authNxdomain = Request.Form.AuthNxdomain;
                string keyName = Request.Form.KeyName;
                string keySecret = Request.Form.KeySecret;
                string controlAcl = Request.Form.ControlAcl;
                string controlIp = Request.Form.ControlIp;
                string controlPort = Request.Form.ControlPort;
                string controlAllow = Request.Form.ControlAllow;
                string loggingChannel = Request.Form.LoggingChannel;
                string loggingDaemon = Request.Form.LoggingDaemon;
                string loggingSeverity = Request.Form.LoggingSeverity;
                string loggingPrintCategory = Request.Form.LoggingPrintCategory;
                string loggingPrintSeverity = Request.Form.LoggingPrintSeverity;
                string loggingPrintTime = Request.Form.LoggingPrintTime;
                string trustedKeys = Request.Form.TrustedKey;
                string aclLocalInterfaces = Request.Form.AclLocalInterfaces;
                string aclInternalInterfaces = Request.Form.AclInternalInterfaces;
                string aclExternalInterfaces = Request.Form.AclExternalInterfaces;
                string aclLocalNetworks = Request.Form.AclLocalNetworks;
                string aclInternalNetworks = Request.Form.AclInternalNetworks;
                string aclExternalNetworks = Request.Form.AclExternalNetworks;
                var model = new BindServerOptionsModel {
                    Notify = notify,
                    MaxCacheSize = maxCacheSize,
                    MaxCacheTtl = maxCacheTtl,
                    MaxNcacheTtl = maxNcacheTtl,
                    Forwarders = forwarders.SplitToList(),
                    AllowNotify = allowNotify.SplitToList(),
                    AllowTransfer = allowTransfer.SplitToList(),
                    Recursion = recursion,
                    TransferFormat = transferFormat,
                    QuerySourceAddress = querySourceAddress,
                    QuerySourcePort = querySourcePort,
                    Version = version,
                    AllowQuery = allowQuery.SplitToList(),
                    AllowRecursion = allowRecursion.SplitToList(),
                    IxfrFromDifferences = ixfrFromDifferences,
                    ListenOnV6 = listenOnV6.SplitToList(),
                    ListenOnPort53 = listenOnPort53.SplitToList(),
                    DnssecEnabled = dnssecEnabled,
                    DnssecValidation = dnssecValidation,
                    DnssecLookaside = dnssecLookaside,
                    AuthNxdomain = authNxdomain,
                    KeyName = keyName,
                    KeySecret = keySecret,
                    ControlAcl = controlAcl,
                    ControlIp = controlIp,
                    ControlPort = controlPort,
                    ControlAllow = controlAllow.SplitToList(),
                    LoggingChannel = loggingChannel,
                    LoggingDaemon = loggingDaemon,
                    LoggingSeverity = loggingSeverity,
                    LoggingPrintCategory = loggingPrintCategory,
                    LoggingPrintSeverity = loggingPrintSeverity,
                    LoggingPrintTime = loggingPrintTime,
                    TrustedKeys = trustedKeys,
                    AclLocalInterfaces = aclLocalInterfaces.SplitToList(),
                    AclInternalInterfaces = aclInternalInterfaces.SplitToList(),
                    AclExternalInterfaces = aclExternalInterfaces.SplitToList(),
                    AclLocalNetworks = aclLocalNetworks.SplitToList(),
                    AclInternalNetworks = aclInternalNetworks.SplitToList(),
                    AclExternalNetworks = aclExternalNetworks.SplitToList()
                };
                _bindServerOptionsRepository.Set(model);
                return Response.AsRedirect("/services");
            };

            Post["/services/bind/zone/add"] = x => {
                string name = Request.Form.Name;
                string type = Request.Form.Type;
                string file = Request.Form.File;
                string serialUpdateMethod = Request.Form.NameSerialUpdateMethod;
                string allowUpdate = Request.Form.AllowUpdate;
                string allowQuery = Request.Form.AllowQuery;
                string allowTransfer = Request.Form.AllowTransfer;
                _bindServerZoneRepository.Create(name, type, file, serialUpdateMethod, allowUpdate.SplitToList(), allowQuery.SplitToList(), allowTransfer.SplitToList());
                return Response.AsRedirect("/services");
            };

            Post["/services/bind/zone/del"] = x => {
                string id = Request.Form.Guid;
                _bindServerZoneRepository.Delete(id);
                return HttpStatusCode.OK;
            };
            #endregion [    DHCPD    ]
        }
    }
}