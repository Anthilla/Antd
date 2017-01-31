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
using Nancy;
using Nancy.Security;

namespace Antd.Modules {
    public class ServiceVpnModule : NancyModule {

        public ServiceVpnModule() {
            this.RequiresAuthentication();

            Post["/services/vpn/set"] = x => {
                var vpnConfiguration = new VpnConfiguration();
                vpnConfiguration.Set();
                return HttpStatusCode.OK;
            };

            Post["/services/vpn/restart"] = x => {
                var vpnConfiguration = new VpnConfiguration();
                vpnConfiguration.Start();
                return HttpStatusCode.OK;
            };

            Post["/services/vpn/stop"] = x => {
                var vpnConfiguration = new VpnConfiguration();
                vpnConfiguration.Stop();
                return HttpStatusCode.OK;
            };

            Post["/services/vpn/enable"] = x => {
                var dhcpdConfiguration = new VpnConfiguration();
                dhcpdConfiguration.Enable();
                dhcpdConfiguration.Start();
                return HttpStatusCode.OK;
            };

            Post["/services/vpn/disable"] = x => {
                var dhcpdConfiguration = new VpnConfiguration();
                dhcpdConfiguration.Disable();
                dhcpdConfiguration.Stop();
                return HttpStatusCode.OK;
            };

            Post["/services/vpn/options"] = x => {
                string remoteHost = Request.Form.RemoteHost;
                string remoteAddress = Request.Form.RemoteAddress;
                string remoteRange = Request.Form.RemoteRange;
                string localAddress = Request.Form.LocalAddress;
                string localRange = Request.Form.LocalRange;
                var model = new VpnConfigurationModel {
                    RemoteHost = remoteHost,
                    RemotePoint = new VpnPointModel {Address = remoteAddress, Range = remoteRange},
                    LocalPoint = new VpnPointModel {Address = localAddress, Range = localRange }
                };
                var vpnConfiguration = new VpnConfiguration();
                vpnConfiguration.Save(model);
                return Response.AsRedirect("/");
            };
        }
    }
}