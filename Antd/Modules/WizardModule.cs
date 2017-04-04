using System;
using System.Linq;
using antdlib.common;
using antdlib.config;
using antdlib.models;
using anthilla.commands;
using Antd.Users;
using Nancy;
using Newtonsoft.Json;
using antdlib.config.shared;

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

namespace Antd.Modules {
    public class WizardModule : NancyModule {

        public WizardModule() {
            Get["/wizard/data"] = x => {
                var bash = new Bash();
                var timezones = bash.Execute("timedatectl list-timezones --no-pager").SplitBash();
                var networkConfiguration = new NetworkConfiguration();
                var launcher = new CommandLauncher();
                var hostConfiguration = new HostConfiguration();
                var hosts = launcher.Launch("cat-etc-hosts").ToArray();
                var networks = launcher.Launch("cat-etc-networks").ToArray();
                var resolv = launcher.Launch("cat-etc-resolv").ToArray();
                var nsswitch = launcher.Launch("cat-etc-nsswitch").ToArray();
                var hostnamectl = launcher.Launch("hostnamectl").ToList();
                const StringSplitOptions ssoree = StringSplitOptions.RemoveEmptyEntries;
                var model = new PageWizardModel {
                    Timezones = timezones,
                    NetworkInterfaceList = networkConfiguration.InterfacePhysical,
                    DomainInt = hostConfiguration.Host.InternalDomain,
                    DomainExt = hostConfiguration.Host.ExternalDomain,
                    Hosts = hosts.JoinToString(Environment.NewLine),
                    Networks = networks.JoinToString(Environment.NewLine),
                    Resolv = resolv.JoinToString(Environment.NewLine),
                    Nsswitch = nsswitch.JoinToString(Environment.NewLine),
                    StaticHostname = hostnamectl.First(_ => _.Contains("Static hostname:")).Split(new[] { ":" }, 2, ssoree)[1],
                    Chassis = hostnamectl.First(_ => _.Contains("Chassis:")).Split(new[] { ":" }, 2, ssoree)[1],
                    Deployment = hostnamectl.First(_ => _.Contains("Deployment:")).Split(new[] { ":" }, 2, ssoree)[1],
                    Location = hostnamectl.First(_ => _.Contains("Location:")).Split(new[] { ":" }, 2, ssoree)[1],
                };
                return JsonConvert.SerializeObject(model);
            };

            Post["/wizard"] = x => {
                ConsoleLogger.Log("[wizard] start basic configuration");
                string password = Request.Form.Password;
                var masterManager = new ManageMaster();
                masterManager.ChangePassword(password);
                ConsoleLogger.Log("[wizard] changed master password");
                string hostname = Request.Form.Hostname;
                string location = Request.Form.Location;
                string chassis = Request.Form.Chassis;
                string deployment = Request.Form.Deployment;
                ConsoleLogger.Log($"[wizard] host info:\t{hostname}");
                ConsoleLogger.Log($"[wizard] host info:\t{location}");
                ConsoleLogger.Log($"[wizard] host info:\t{chassis}");
                ConsoleLogger.Log($"[wizard] host info:\t{deployment}");
                var hostConfiguration = new HostConfiguration();
                hostConfiguration.SetHostInfoName(hostname);
                hostConfiguration.SetHostInfoChassis(chassis);
                hostConfiguration.SetHostInfoDeployment(deployment);
                hostConfiguration.SetHostInfoLocation(location);
                hostConfiguration.ApplyHostInfo();
                string timezone = Request.Form.Timezone;
                hostConfiguration.SetTimezone(timezone);
                hostConfiguration.ApplyTimezone();
                string ntpServer = Request.Form.NtpServer;
                hostConfiguration.SetNtpdate(ntpServer);
                hostConfiguration.ApplyNtpdate();
                string domainInt = Request.Form.DomainInt;
                string domainExt = Request.Form.DomainExt;
                string hosts = Request.Form.Hosts;
                string networks = Request.Form.Networks;
                string resolv = Request.Form.Resolv;
                string nsswitch = Request.Form.Nsswitch;
                hostConfiguration.SetNsHosts(hosts.Contains("\n")
                    ? hosts.SplitToList("\n").ToArray()
                    : hosts.SplitToList(Environment.NewLine).ToArray());
                hostConfiguration.ApplyNsHosts();
                hostConfiguration.SetNsNetworks(networks.Contains("\n")
                    ? networks.SplitToList("\n").ToArray()
                    : networks.SplitToList(Environment.NewLine).ToArray());
                hostConfiguration.ApplyNsNetworks();
                hostConfiguration.SetNsResolv(resolv.Contains("\n")
                  ? resolv.SplitToList("\n").ToArray()
                  : resolv.SplitToList(Environment.NewLine).ToArray());
                hostConfiguration.ApplyNsResolv();
                hostConfiguration.SetNsSwitch(nsswitch.Contains("\n")
                  ? nsswitch.SplitToList("\n").ToArray()
                  : nsswitch.SplitToList(Environment.NewLine).ToArray());
                hostConfiguration.ApplyNsSwitch();
                hostConfiguration.SetInternalDomain(domainInt);
                hostConfiguration.SetExtenalDomain(domainExt);
                ConsoleLogger.Log("[wizard] name services configured");
                string Interface = Request.Form.Interface;
                string txqueuelen = Request.Form.Txqueuelen;
                string mtu = Request.Form.Mtu;
                string mode = Request.Form.Mode;
                string staticAddress = Request.Form.StaticAddress;
                string staticRange = Request.Form.StaticRange;
                var networkConfiguration = new NetworkConfiguration();
                var model = new NetworkInterfaceConfigurationModel {
                    Interface = Interface,
                    Mode = (NetworkInterfaceMode)Enum.Parse(typeof(NetworkInterfaceMode), mode),
                    Status = NetworkInterfaceStatus.Up,
                    StaticAddress = staticAddress,
                    StaticRange = staticRange,
                    Txqueuelen = txqueuelen,
                    Mtu = mtu,
                    Type = NetworkInterfaceType.Physical
                };
                networkConfiguration.AddInterfaceSetting(model);
                networkConfiguration.ApplyInterfaceSetting(model);
                ConsoleLogger.Log($"[wizard] network configured at {Interface}");
                hostConfiguration.SetHostAsConfigured();
                ConsoleLogger.Log("[wizard] configuration complete");
                return HttpStatusCode.OK;
            };
        }
    }
}