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

using System.Linq;
using antdlib.common;
using antdlib.models;
using Antd.Dhcpd;
using Nancy;
using Nancy.Security;

namespace Antd.Modules {
    public class ServiceDhcpdModule : NancyModule {

        public ServiceDhcpdModule() {
            this.RequiresAuthentication();
            Post["/services/dhcpd/set"] = x => {
                var dhcpdConfiguration = new DhcpdConfiguration();
                dhcpdConfiguration.Set();
                return HttpStatusCode.OK;
            };

            Post["/services/dhcpd/restart"] = x => {
                var dhcpdConfiguration = new DhcpdConfiguration();
                dhcpdConfiguration.Start();
                return HttpStatusCode.OK;
            };

            Post["/services/dhcpd/stop"] = x => {
                var dhcpdConfiguration = new DhcpdConfiguration();
                dhcpdConfiguration.Stop();
                return HttpStatusCode.OK;
            };

            Post["/services/dhcpd/enable"] = x => {
                var dhcpdConfiguration = new DhcpdConfiguration();
                dhcpdConfiguration.Enable();
                dhcpdConfiguration.Start();
                return HttpStatusCode.OK;
            };

            Post["/services/dhcpd/disable"] = x => {
                var dhcpdConfiguration = new DhcpdConfiguration();
                dhcpdConfiguration.Disable();
                dhcpdConfiguration.Stop();
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
                string ipFamily = Request.Form.IpFamily;
                string ipMask = Request.Form.IpMask;
                string optionRouters = Request.Form.OptionRouters;
                string ntpServers = Request.Form.NtpServers;
                string timeServers = Request.Form.DoForTimeServerswardUpdates;
                string domainNameServers = Request.Form.DomainNameServers;
                string broadcastAddress = Request.Form.BroadcastAddress;
                string subnetMask = Request.Form.SubnetMask;

                var model = new DhcpdConfigurationModel {
                    Allow = allow.SplitToList().Select(_=>_.Trim()).ToList(),
                    UpdateStaticLeases = updateStaticLeases,
                    UpdateConflictDetection = updateConflictDetection,
                    UseHostDeclNames = useHostDeclNames,
                    DoForwardUpdates = doForwardUpdates,
                    DoReverseUpdates = doReverseUpdates,
                    LogFacility = logFacility,
                    Option = option.SplitToList().Select(_ => _.Trim()).ToList(),
                    ZoneName = zoneName,
                    ZonePrimaryAddress = zonePrimaryAddress,
                    DdnsUpdateStyle = ddnsUpdateStyle,
                    DdnsUpdates = ddnsUpdates,
                    DdnsDomainName = ddnsDomainName,
                    DdnsRevDomainName = ddnsRevDomainName,
                    DefaultLeaseTime = defaultLeaseTime,
                    MaxLeaseTime = maxLeaseTime,
                    KeyName = keyName,
                    KeySecret = keySecret,
                    SubnetIpFamily = ipFamily,
                    SubnetIpMask = ipMask,
                    SubnetOptionRouters = optionRouters,
                    SubnetNtpServers = ntpServers,
                    SubnetTimeServers = timeServers,
                    SubnetDomainNameServers = domainNameServers,
                    SubnetBroadcastAddress = broadcastAddress,
                    SubnetMask = subnetMask,
                };
                var dhcpdConfiguration = new DhcpdConfiguration();
                dhcpdConfiguration.Save(model);
                return Response.AsRedirect("/");
            };

            Post["/services/dhcpd/class"] = x => {
                string name = Request.Form.Name;
                string macVendor = Request.Form.MacVendor;
                var model = new DhcpConfigurationClassModel {
                    Name = name,
                    MacVendor = macVendor
                };
                var dhcpdConfiguration = new DhcpdConfiguration();
                dhcpdConfiguration.AddClass(model);
                return Response.AsRedirect("/");
            };

            Post["/services/dhcpd/class/del"] = x => {
                string guid = Request.Form.Guid;
                var dhcpdConfiguration = new DhcpdConfiguration();
                dhcpdConfiguration.RemoveClass(guid);
                return HttpStatusCode.OK;
            };

            Post["/services/dhcpd/pool"] = x => {
                string option = Request.Form.Option;
                var model = new DhcpConfigurationPoolModel {
                    Options = option.SplitToList().Select(_ => _.Trim()).ToList()
                };
                var dhcpdConfiguration = new DhcpdConfiguration();
                dhcpdConfiguration.AddPool(model);
                return Response.AsRedirect("/");
            };

            Post["/services/dhcpd/pool/del"] = x => {
                string guid = Request.Form.Guid;
                var dhcpdConfiguration = new DhcpdConfiguration();
                dhcpdConfiguration.RemovePool(guid);
                return HttpStatusCode.OK;
            };

            Post["/services/dhcpd/reservation"] = x => {
                string hostName = Request.Form.HostName;
                string macAddress = Request.Form.MacAddress;
                string ipAddress = Request.Form.IpAddress;
                var model = new DhcpConfigurationReservationModel {
                    HostName = hostName,
                    MacAddress = macAddress,
                    IpAddress = ipAddress
                };
                var dhcpdConfiguration = new DhcpdConfiguration();
                dhcpdConfiguration.AddReservation(model);
                return Response.AsRedirect("/");
            };

            Post["/services/dhcpd/reservation/del"] = x => {
                string guid = Request.Form.Guid;
                var dhcpdConfiguration = new DhcpdConfiguration();
                dhcpdConfiguration.RemoveReservation(guid);
                return HttpStatusCode.OK;
            };
        }
    }
}