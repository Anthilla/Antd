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
using System.Linq;

namespace Antd.Modules {
    public class AntdNameServiceModule : NancyModule {

        public AntdNameServiceModule() {
            Get["/nameservice"] = x => {
                var launcher = new CommandLauncher();
                var hostConfiguration = new HostConfiguration();
                var hosts = launcher.Launch("cat-etc-hosts").ToArray();
                var networks = launcher.Launch("cat-etc-networks").ToArray();
                var resolv = launcher.Launch("cat-etc-resolv").ToArray();
                var nsswitch = launcher.Launch("cat-etc-nsswitch").ToArray();
                var model = new PageNameServiceModel {
                    Hostname = launcher.Launch("cat-etc-hostname").JoinToString("<br />"),
                    DomainInt = hostConfiguration.Host.InternalDomain,
                    DomainExt = hostConfiguration.Host.ExternalDomain,
                    Hosts = hosts.JoinToString("<br />"),
                    Networks = networks.JoinToString("<br />"),
                    Resolv = resolv.JoinToString("<br />"),
                    Nsswitch = nsswitch.JoinToString("<br />")
                };
                return JsonConvert.SerializeObject(model);
            };

            Post["/nameservice/hosts"] = x => {
                string hosts = Request.Form.Hosts;
                var hostConfiguration = new HostConfiguration();
                hostConfiguration.SetNsHosts(hosts.Contains("\n")
                    ? hosts.SplitToList("\n").ToArray()
                    : hosts.SplitToList(Environment.NewLine).ToArray());
                hostConfiguration.ApplyNsHosts();
                return HttpStatusCode.OK;
            };

            Post["/nameservice/networks"] = x => {
                string networks = Request.Form.Networks;
                var hostConfiguration = new HostConfiguration();
                hostConfiguration.SetNsNetworks(networks.Contains("\n")
                  ? networks.SplitToList("\n").ToArray()
                  : networks.SplitToList(Environment.NewLine).ToArray());
                hostConfiguration.ApplyNsNetworks();
                return HttpStatusCode.OK;
            };

            Post["/nameservice/resolv"] = x => {
                string resolv = Request.Form.Resolv;
                var hostConfiguration = new HostConfiguration();
                hostConfiguration.SetNsResolv(resolv.Contains("\n")
                  ? resolv.SplitToList("\n").ToArray()
                  : resolv.SplitToList(Environment.NewLine).ToArray());
                hostConfiguration.ApplyNsResolv();
                return HttpStatusCode.OK;
            };

            Post["/nameservice/switch"] = x => {
                string @switch = Request.Form.Switch;
                var hostConfiguration = new HostConfiguration();
                hostConfiguration.SetNsSwitch(@switch.Contains("\n")
                  ? @switch.SplitToList("\n").ToArray()
                  : @switch.SplitToList(Environment.NewLine).ToArray());
                hostConfiguration.ApplyNsSwitch();
                return HttpStatusCode.OK;
            };

            Post["/host/int/domain"] = x => {
                string domain = Request.Form.Domain;
                var hostConfiguration = new HostConfiguration();
                hostConfiguration.SetInternalDomain(domain);
                return HttpStatusCode.OK;
            };

            Post["/host/ext/domain"] = x => {
                string domain = Request.Form.Domain;
                var hostConfiguration = new HostConfiguration();
                hostConfiguration.SetExtenalDomain(domain);
                return HttpStatusCode.OK;
            };
        }
    }
}