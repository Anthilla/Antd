using System.Collections.Generic;
using antdlib.common;
using antdlib.models;
using Nancy;
using Newtonsoft.Json;

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

namespace AntdUi.Modules {
    public class WizardModule : NancyModule {

        private readonly ApiConsumer _api = new ApiConsumer();

        public WizardModule() {
            Get["/wizard/data"] = x => {
                var model = _api.Get<PageWizardModel>($"http://127.0.0.1:{Application.ServerPort}/wizard/data");
                var json = JsonConvert.SerializeObject(model);
                return json;
            };

            Post["/wizard"] = x => {
                string password = Request.Form.Password;
                string hostname = Request.Form.Hostname;
                string location = Request.Form.Location;
                string chassis = Request.Form.Chassis;
                string deployment = Request.Form.Deployment;
                string timezone = Request.Form.Timezone;
                string ntpServer = Request.Form.NtpServer;
                string domainInt = Request.Form.DomainInt;
                string domainExt = Request.Form.DomainExt;
                string hosts = Request.Form.Hosts;
                string networks = Request.Form.Networks;
                string resolv = Request.Form.Resolv;
                string nsswitch = Request.Form.Nsswitch;
                string Interface = Request.Form.Interface;
                string txqueuelen = Request.Form.Txqueuelen;
                string mtu = Request.Form.Mtu;
                string mode = Request.Form.Mode;
                string staticAddress = Request.Form.StaticAddress;
                string staticRange = Request.Form.StaticRange;
                var dict = new Dictionary<string, string> {
                    { "Password", password},
                    { "Hostname", hostname },
                    { "Location", location },
                    { "Chassis", chassis },
                    { "Deployment", deployment },
                    { "Timezone", timezone },
                    { "NtpServer", ntpServer },
                    { "DomainInt", domainInt },
                    { "DomainExt", domainExt },
                    { "Hosts", hosts },
                    { "Networks", networks },
                    { "Resolv", resolv },
                    { "Nsswitch", nsswitch },
                    { "Interface", Interface },
                    { "Txqueuelen", txqueuelen },
                    { "Mtu", mtu },
                    { "Mode", mode },
                    { "StaticAddress", staticAddress },
                    { "StaticRange", staticRange }
                };
                _api.Post($"http://127.0.0.1:{Application.ServerPort}/wizard", dict);
                return Response.AsRedirect("/logout");
            };
        }
    }
}