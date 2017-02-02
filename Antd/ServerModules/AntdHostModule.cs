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
using System.Linq;
using antd.commands;
using antdlib.config;
using antdlib.models;
using Nancy;
using Newtonsoft.Json;

namespace Antd.ServerModules {
    public class AntdHostModule : NancyModule {

        public AntdHostModule() {

            Get["/host/info"] = x => {
                const StringSplitOptions ssoree = StringSplitOptions.RemoveEmptyEntries;
                var launcher = new CommandLauncher();
                var hostnamectl = launcher.Launch("hostnamectl").ToList();
                var model = new PageHostModel {
                    StaticHostname =
                        hostnamectl.First(_ => _.Contains("Static hostname:")).Split(new[] { ":" }, 2, ssoree)[1],
                    IconName = hostnamectl.First(_ => _.Contains("Icon name:")).Split(new[] { ":" }, 2, ssoree)[1],
                    Chassis = hostnamectl.First(_ => _.Contains("Chassis:")).Split(new[] { ":" }, 2, ssoree)[1],
                    Deployment = hostnamectl.First(_ => _.Contains("Deployment:")).Split(new[] { ":" }, 2, ssoree)[1],
                    Location = hostnamectl.First(_ => _.Contains("Location:")).Split(new[] { ":" }, 2, ssoree)[1],
                    MachineId = hostnamectl.First(_ => _.Contains("Machine ID:")).Split(new[] { ":" }, 2, ssoree)[1],
                    BootId = hostnamectl.First(_ => _.Contains("Boot ID:")).Split(new[] { ":" }, 2, ssoree)[1],
                    Virtualization =
                        hostnamectl.First(_ => _.Contains("Virtualization:")).Split(new[] { ":" }, 2, ssoree)[1],
                    Os = hostnamectl.First(_ => _.Contains("Operating System:")).Split(new[] { ":" }, 2, ssoree)[1],
                    Kernel = hostnamectl.First(_ => _.Contains("Kernel:")).Split(new[] { ":" }, 2, ssoree)[1],
                    Architecture = hostnamectl.First(_ => _.Contains("Architecture:")).Split(new[] { ":" }, 2, ssoree)[1]
                };
                return JsonConvert.SerializeObject(model);
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
        }
    }
}