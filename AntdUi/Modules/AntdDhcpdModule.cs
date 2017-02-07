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

using System.Collections.Generic;
using antdlib.common;
using antdlib.models;
using Nancy;
using Newtonsoft.Json;

namespace AntdUi.Modules {
    public class AntdDhcpdModule : NancyModule {

        private readonly ApiConsumer _api = new ApiConsumer();

        public AntdDhcpdModule() {
            Get["/dhcpd"] = x => {
                var model = _api.Get<PageDhcpdModel>($"http://127.0.0.1:{Application.ServerPort}/dhcpd");
                var json = JsonConvert.SerializeObject(model);
                return json;
            };

            Post["/dhcpd/set"] = x => {
                return _api.Post($"http://127.0.0.1:{Application.ServerPort}/dhcpd/set");
            };

            Post["/dhcpd/restart"] = x => {
                return _api.Post($"http://127.0.0.1:{Application.ServerPort}/dhcpd/restart");
            };

            Post["/dhcpd/stop"] = x => {
                return _api.Post($"http://127.0.0.1:{Application.ServerPort}/dhcpd/stop");
            };

            Post["/dhcpd/enable"] = x => {
                return _api.Post($"http://127.0.0.1:{Application.ServerPort}/dhcpd/enable");
            };

            Post["/dhcpd/disable"] = x => {
                return _api.Post($"http://127.0.0.1:{Application.ServerPort}/dhcpd/disable");
            };

            Post["/dhcpd/options"] = x => {
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
                var dict = new Dictionary<string, string> {
                    { "Allow", allow },
                    { "UpdateStaticLeases", updateStaticLeases },
                    { "UpdateConflictDetection", updateConflictDetection },
                    { "UseHostDeclNames", useHostDeclNames },
                    { "DoForwardUpdates", doForwardUpdates },
                    { "DoReverseUpdates", doReverseUpdates },
                    { "LogFacility", logFacility },
                    { "Option", option },
                    { "ZoneName", zoneName },
                    { "ZonePrimaryAddress", zonePrimaryAddress },
                    { "DdnsUpdateStyle", ddnsUpdateStyle },
                    { "DdnsUpdates", ddnsUpdates },
                    { "DdnsDomainName", ddnsDomainName },
                    { "DdnsRevDomainName", ddnsRevDomainName },
                    { "DefaultLeaseTime", defaultLeaseTime },
                    { "MaxLeaseTime", maxLeaseTime },
                    { "KeyName", keyName },
                    { "KeySecret", keySecret },
                    { "IpFamily", ipFamily },
                    { "IpMask", ipMask },
                    { "OptionRouters", optionRouters },
                    { "NtpServers", ntpServers },
                    { "DoForTimeServerswardUpdates", timeServers },
                    { "DomainNameServers", domainNameServers },
                    { "BroadcastAddress", broadcastAddress },
                    { "SubnetMask", subnetMask }
                };
                return _api.Post($"http://127.0.0.1:{Application.ServerPort}/dhcpd/options", dict);
            };

            Post["/dhcpd/class"] = x => {
                string name = Request.Form.Name;
                string macVendor = Request.Form.MacVendor;
                var dict = new Dictionary<string, string> {
                    { "Name", name },
                    { "MacVendor", macVendor }
                };
                return _api.Post($"http://127.0.0.1:{Application.ServerPort}/dhcpd/class", dict);
            };

            Post["/dhcpd/class/del"] = x => {
                string guid = Request.Form.Guid;
                var dict = new Dictionary<string, string> {
                    { "Guid", guid }
                };
                return _api.Post($"http://127.0.0.1:{Application.ServerPort}/dhcpd/class/del", dict);
            };

            Post["/dhcpd/pool"] = x => {
                string option = Request.Form.Option;
                var dict = new Dictionary<string, string> {
                    { "Option", option }
                };
                return _api.Post($"http://127.0.0.1:{Application.ServerPort}/dhcpd/pool", dict);
            };

            Post["/dhcpd/pool/del"] = x => {
                string guid = Request.Form.Guid;
                var dict = new Dictionary<string, string> {
                    { "Guid", guid }
                };
                return _api.Post($"http://127.0.0.1:{Application.ServerPort}/dhcpd/pool/del", dict);
            };

            Post["/dhcpd/reservation"] = x => {
                string hostName = Request.Form.HostName;
                string macAddress = Request.Form.MacAddress;
                string ipAddress = Request.Form.IpAddress;
                var dict = new Dictionary<string, string> {
                    { "HostName", hostName },
                    { "MacAddress", macAddress },
                    { "IpAddress", ipAddress }
                };
                return _api.Post($"http://127.0.0.1:{Application.ServerPort}/dhcpd/reservation", dict);
            };

            Post["/dhcpd/reservation/del"] = x => {
                string guid = Request.Form.Guid;
                var dict = new Dictionary<string, string> {
                    { "Guid", guid }
                };
                return _api.Post($"http://127.0.0.1:{Application.ServerPort}/dhcpd/reservation/del", dict);
            };
        }
    }
}