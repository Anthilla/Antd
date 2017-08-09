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

using anthilla.core;
using antdlib.models;
using Nancy;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace AntdUi.Modules {
    public class AntdSshdModule : NancyModule {

        public AntdSshdModule() {
            Get["/sshd"] = x => {
                var model = ApiConsumer.Get<PageSshdModel>($"http://127.0.0.1:{Application.ServerPort}/sshd");
                var json = JsonConvert.SerializeObject(model);
                return json;
            };

            Post["/sshd/set"] = x => {
                return ApiConsumer.Post($"http://127.0.0.1:{Application.ServerPort}/sshd/set");
            };

            Post["/sshd/restart"] = x => {
                return ApiConsumer.Post($"http://127.0.0.1:{Application.ServerPort}/sshd/restart");
            };

            Post["/sshd/stop"] = x => {
                return ApiConsumer.Post($"http://127.0.0.1:{Application.ServerPort}/sshd/stop");
            };

            Post["/sshd/enable"] = x => {
                return ApiConsumer.Post($"http://127.0.0.1:{Application.ServerPort}/sshd/enable");
            };

            Post["/sshd/disable"] = x => {
                return ApiConsumer.Post($"http://127.0.0.1:{Application.ServerPort}/sshd/disable");
            };

            Post["/sshd/options"] = x => {
                string port = Request.Form.Port;
                string permitRootLogin = Request.Form.PermitRootLogin;
                string permitTunnel = Request.Form.PermitTunnel;
                string maxAuthTries = Request.Form.MaxAuthTries;
                string maxSessions = Request.Form.MaxSessions;
                string rsaAuthentication = Request.Form.RsaAuthentication;
                string pubkeyAuthentication = Request.Form.PubkeyAuthentication;
                string usePam = Request.Form.UsePam;
                var dict = new Dictionary<string, string> {
                    { "Port", port },
                    { "PermitRootLogin", permitRootLogin },
                    { "PermitTunnel", permitTunnel },
                    { "MaxAuthTries", maxAuthTries },
                    { "MaxSessions", maxSessions },
                    { "RsaAuthentication", rsaAuthentication },
                    { "PubkeyAuthentication", pubkeyAuthentication },
                    { "UsePam", usePam },
                };
                return ApiConsumer.Post($"http://127.0.0.1:{Application.ServerPort}/sshd/options", dict);
            };
        }
    }
}