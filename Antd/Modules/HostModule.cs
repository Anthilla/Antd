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
using Nancy;
using Nancy.Security;
using Newtonsoft.Json;

namespace Antd.Modules {

    public class HostModule : NancyModule {
        public HostModule() {
            this.RequiresAuthentication();

            Get["/host"] = x => {
                var hostconfiguration = new HostConfiguration();
                return JsonConvert.SerializeObject(hostconfiguration.Host);
            };

            Post["/host/info/name"] = x => {
                string name = Request.Form.Name;
                if(string.IsNullOrEmpty(name)) {
                    return HttpStatusCode.BadRequest;
                }
                var hostconfiguration = new HostConfiguration();
                hostconfiguration.SetHostInfoName(name);
                hostconfiguration.ApplyHostInfo();
                return HttpStatusCode.OK;
            };

            Post["/host/info/chassis"] = x => {
                string chassis = Request.Form.Chassis;
                if(string.IsNullOrEmpty(chassis)) {
                    return HttpStatusCode.BadRequest;
                }
                var hostconfiguration = new HostConfiguration();
                hostconfiguration.SetHostInfoChassis(chassis);
                hostconfiguration.ApplyHostInfo();
                return HttpStatusCode.OK;
            };

            Post["/host/info/deployment"] = x => {
                string deployment = Request.Form.Deployment;
                if(string.IsNullOrEmpty(deployment)) {
                    return HttpStatusCode.BadRequest;
                }
                var hostconfiguration = new HostConfiguration();
                hostconfiguration.SetHostInfoDeployment(deployment);
                hostconfiguration.ApplyHostInfo();
                return HttpStatusCode.OK;
            };

            Post["/host/info/location"] = x => {
                string location = Request.Form.Location;
                if(string.IsNullOrEmpty(location)) {
                    return HttpStatusCode.BadRequest;
                }
                var hostconfiguration = new HostConfiguration();
                hostconfiguration.SetHostInfoLocation(location);
                hostconfiguration.ApplyHostInfo();
                return HttpStatusCode.OK;
            };

            Post["/host/info"] = x => {
                string name = Request.Form.Name;
                string chassis = Request.Form.Chassis;
                string deployment = Request.Form.Deployment;
                string location = Request.Form.Location;
                if(string.IsNullOrEmpty(name) || string.IsNullOrEmpty(chassis) || string.IsNullOrEmpty(deployment) || string.IsNullOrEmpty(location)) {
                    return HttpStatusCode.BadRequest;
                }
                var hostconfiguration = new HostConfiguration();
                hostconfiguration.SetHostInfo(name, chassis, deployment, location);
                hostconfiguration.ApplyHostInfo();
                return HttpStatusCode.OK;
            };

            Post["/host/timezone"] = x => {
                string timezone = Request.Form.Timezone;
                if(string.IsNullOrEmpty(timezone)) {
                    return HttpStatusCode.BadRequest;
                }
                var hostconfiguration = new HostConfiguration();
                hostconfiguration.SetTimezone(timezone);
                hostconfiguration.ApplyTimezone();
                return HttpStatusCode.OK;
            };

            Post["/host/synctime"] = x => {
                var hostconfiguration = new HostConfiguration();
                hostconfiguration.SyncClock();
                return HttpStatusCode.OK;
            };

            Post["/host/ntpdate"] = x => {
                string ntpdate = Request.Form.Ntpdate;
                if(string.IsNullOrEmpty(ntpdate)) {
                    return HttpStatusCode.BadRequest;
                }
                var hostconfiguration = new HostConfiguration();
                hostconfiguration.SetNtpdate(ntpdate);
                hostconfiguration.ApplyNtpdate();
                return HttpStatusCode.OK;
            };

            Post["/host/ntpd"] = x => {
                string ntpd = Request.Form.Ntpd;
                if(string.IsNullOrEmpty(ntpd)) {
                    return HttpStatusCode.BadRequest;
                }
                var hostConfiguration = new HostConfiguration();
                hostConfiguration.SetNtpd(ntpd.Contains("\n")
                  ? ntpd.SplitToList("\n").ToArray()
                  : ntpd.SplitToList(Environment.NewLine).ToArray());
                hostConfiguration.ApplyNtpd();
                return HttpStatusCode.OK;
            };

            Post["/host/ns/hosts"] = x => {
                string hosts = Request.Form.Hosts;
                if(string.IsNullOrEmpty(hosts)) {
                    return HttpStatusCode.BadRequest;
                }
                var hostConfiguration = new HostConfiguration();
                hostConfiguration.SetNsHosts(hosts.Contains("\n")
                    ? hosts.SplitToList("\n").ToArray()
                    : hosts.SplitToList(Environment.NewLine).ToArray());
                hostConfiguration.ApplyNsHosts();
                return HttpStatusCode.OK;
            };

            Post["/host/ns/networks"] = x => {
                string networks = Request.Form.Networks;
                if(string.IsNullOrEmpty(networks)) {
                    return HttpStatusCode.BadRequest;
                }
                var hostConfiguration = new HostConfiguration();
                hostConfiguration.SetNsNetworks(networks.Contains("\n")
                  ? networks.SplitToList("\n").ToArray()
                  : networks.SplitToList(Environment.NewLine).ToArray());
                hostConfiguration.ApplyNsNetworks();
                return HttpStatusCode.OK;
            };

            Post["/host/ns/resolv"] = x => {
                string resolv = Request.Form.Resolv;
                if(string.IsNullOrEmpty(resolv)) {
                    return HttpStatusCode.BadRequest;
                }
                var hostConfiguration = new HostConfiguration();
                hostConfiguration.SetNsResolv(resolv.Contains("\n")
                  ? resolv.SplitToList("\n").ToArray()
                  : resolv.SplitToList(Environment.NewLine).ToArray());
                hostConfiguration.ApplyNsResolv();
                return HttpStatusCode.OK;
            };

            Post["/host/ns/switch"] = x => {
                string @switch = Request.Form.Switch;
                if(string.IsNullOrEmpty(@switch)) {
                    return HttpStatusCode.BadRequest;
                }
                var hostConfiguration = new HostConfiguration();
                hostConfiguration.SetNsSwitch(@switch.Contains("\n")
                  ? @switch.SplitToList("\n").ToArray()
                  : @switch.SplitToList(Environment.NewLine).ToArray());
                hostConfiguration.ApplyNsSwitch();
                return HttpStatusCode.OK;
            };

            Post["/host/int/domain"] = x => {
                string domain = Request.Form.Domain;
                if(string.IsNullOrEmpty(domain)) {
                    return HttpStatusCode.BadRequest;
                }
                var hostConfiguration = new HostConfiguration();
                hostConfiguration.SetInternalDomain(domain);
                return HttpStatusCode.OK;
            };

            Post["/host/ext/domain"] = x => {
                string domain = Request.Form.Domain;
                if(string.IsNullOrEmpty(domain)) {
                    return HttpStatusCode.BadRequest;
                }
                var hostConfiguration = new HostConfiguration();
                hostConfiguration.SetExtenalDomain(domain);
                return HttpStatusCode.OK;
            };
        }
    }
}