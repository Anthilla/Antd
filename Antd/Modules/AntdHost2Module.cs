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

using antdlib.config;
using antdlib.models;
using anthilla.commands;
using Nancy;
using Newtonsoft.Json;
using System;
using System.Linq;

namespace Antd.Modules {
    public class AntdHost2Module : NancyModule {

        public AntdHost2Module() {

            Get["/host2"] = x => {
                const StringSplitOptions ssoree = StringSplitOptions.RemoveEmptyEntries;
                var hostnamectl = CommandLauncher.Launch("hostnamectl").ToList();
                var model = new PageHost2Model {
                    Host = Host2Configuration.Host,
                    IconName = hostnamectl.FirstOrDefault(_ => _.Contains("Icon name:"))?.Split(new[] { ":" }, 2, ssoree)[1],
                    MachineId = hostnamectl.FirstOrDefault(_ => _.Contains("Machine ID:"))?.Split(new[] { ":" }, 2, ssoree)[1],
                    BootId = hostnamectl.FirstOrDefault(_ => _.Contains("Boot ID:"))?.Split(new[] { ":" }, 2, ssoree)[1],
                    Virtualization = hostnamectl.FirstOrDefault(_ => _.Contains("Virtualization:"))?.Split(new[] { ":" }, 2, ssoree)[1],
                    Os = hostnamectl.FirstOrDefault(_ => _.Contains("Operating System:"))?.Split(new[] { ":" }, 2, ssoree)[1],
                    Kernel = hostnamectl.FirstOrDefault(_ => _.Contains("Kernel:"))?.Split(new[] { ":" }, 2, ssoree)[1],
                    Architecture = hostnamectl.FirstOrDefault(_ => _.Contains("Architecture:"))?.Split(new[] { ":" }, 2, ssoree)[1]
                };
                return JsonConvert.SerializeObject(model);
            };

            Post["/host2/info"] = x => {
                string hostName = Request.Form.HostName;
                string hostChassis = Request.Form.HostChassis;
                string hostDeployment = Request.Form.HostDeployment;
                string hostLocation = Request.Form.HostLocation;
                string hostAliasPrimary = Request.Form.HostAliasPrimary;
                string internalDomainPrimary = Request.Form.InternalDomainPrimary;
                string externalDomainPrimary = Request.Form.ExternalDomainPrimary;
                string internalHostIpPrimary = Request.Form.InternalHostIpPrimary;
                string externalHostIpPrimary = Request.Form.ExternalHostIpPrimary;
                string internalNetPrimaryBits = Request.Form.InternalNetPrimaryBits;
                string externalNetPrimaryBits = Request.Form.ExternalNetPrimaryBits;
                string resolvNameserver = Request.Form.ResolvNameserver;
                string resolvDomain = Request.Form.ResolvDomain;
                string timezone = Request.Form.Timezone;
                string ntpdateServer = Request.Form.NtpdateServer;
                string cloud = Request.Form.Cloud;
                var old = Host2Configuration.Host;
                var vars = new Host2Model {
                    HostName = hostName ?? old.HostName,
                    HostChassis = hostChassis ?? old.HostChassis,
                    HostDeployment = hostDeployment ?? old.HostDeployment,
                    HostLocation = hostLocation ?? old.HostLocation,
                    HostAliasPrimary = hostAliasPrimary ?? old.HostAliasPrimary,
                    InternalDomainPrimary = internalDomainPrimary ?? old.InternalDomainPrimary,
                    ExternalDomainPrimary = externalDomainPrimary ?? old.ExternalDomainPrimary,
                    InternalHostIpPrimary = internalHostIpPrimary ?? old.InternalHostIpPrimary,
                    ExternalHostIpPrimary = externalHostIpPrimary ?? old.ExternalHostIpPrimary,
                    InternalNetPrimaryBits = internalNetPrimaryBits ?? old.InternalNetPrimaryBits,
                    ExternalNetPrimaryBits = externalNetPrimaryBits ?? old.ExternalNetPrimaryBits,
                    ResolvNameserver = resolvNameserver ?? old.ResolvNameserver,
                    ResolvDomain = resolvDomain ?? old.ResolvDomain,
                    Timezone = timezone ?? old.Timezone,
                    NtpdateServer = ntpdateServer ?? old.NtpdateServer,
                    MachineUid = Machine.MachineIds.Get.MachineUid,
                    Cloud = cloud ?? old.Cloud
                };
                Host2Configuration.Export(vars);
                new Do().HostChanges();
                return HttpStatusCode.OK;
            };
        }
    }
}