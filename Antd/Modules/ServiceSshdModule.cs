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

using Antd.Ssh;
using Nancy;
using Nancy.Security;

namespace Antd.Modules {
    public class ServiceSshdModule : NancyModule {

        public ServiceSshdModule() {
            this.RequiresAuthentication();

            Post["/services/sshd/set"] = x => {
                var sshdConfiguration = new SshdConfiguration();
                sshdConfiguration.Set();
                return HttpStatusCode.OK;
            };

            Post["/services/sshd/restart"] = x => {
                var sshdConfiguration = new SshdConfiguration();
                sshdConfiguration.Start();
                return HttpStatusCode.OK;
            };

            Post["/services/sshd/stop"] = x => {
                var sshdConfiguration = new SshdConfiguration();
                sshdConfiguration.Stop();
                return HttpStatusCode.OK;
            };

            Post["/services/sshd/enable"] = x => {
                var dhcpdConfiguration = new SshdConfiguration();
                dhcpdConfiguration.Enable();
                dhcpdConfiguration.Start();
                return HttpStatusCode.OK;
            };

            Post["/services/sshd/disable"] = x => {
                var dhcpdConfiguration = new SshdConfiguration();
                dhcpdConfiguration.Disable();
                dhcpdConfiguration.Stop();
                return HttpStatusCode.OK;
            };

            Post["/services/sshd/options"] = x => {
                string port = Request.Form.Port;
                string permitRootLogin = Request.Form.PermitRootLogin;
                string permitTunnel = Request.Form.PermitTunnel;
                string maxAuthTries = Request.Form.MaxAuthTries;
                string maxSessions = Request.Form.MaxSessions;
                string rsaAuthentication = Request.Form.RsaAuthentication;
                string pubkeyAuthentication = Request.Form.PubkeyAuthentication;
                string usePam = Request.Form.UsePam;
                var model = new SshdConfigurationModel {
                    Port = port,
                    PermitRootLogin = permitRootLogin,
                    PermitTunnel = permitTunnel,
                    MaxAuthTries = maxAuthTries,
                    MaxSessions = maxSessions,
                    RsaAuthentication = rsaAuthentication,
                    PubkeyAuthentication = pubkeyAuthentication,
                    UsePam = usePam
                };
                var sshdConfiguration = new SshdConfiguration();
                sshdConfiguration.Save(model);
                return Response.AsRedirect("/");
            };
        }
    }
}