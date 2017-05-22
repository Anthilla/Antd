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

using antdlib.models;
using Nancy;
using Newtonsoft.Json;
using System.Collections.Generic;
using anthilla.core;

namespace AntdUi.Modules {
    public class AssetDiscoveryModule : NancyModule {

        private readonly ApiConsumer _api = new ApiConsumer();

        public AssetDiscoveryModule() {
            Get["/discovery"] = x => {
                var model = _api.Get<PageAssetDiscoveryModel>($"http://127.0.0.1:{Application.ServerPort}/discovery");
                var json = JsonConvert.SerializeObject(model);
                return json;
            };

            Post["/asset/handshake/start"] = x => {
                var hostIp = Request.Form.Host;
                var hostPort = Request.Form.Port;
                var dict = new Dictionary<string, string> {
                    { "Host", hostIp },
                    { "Port", hostPort }
                };
                return _api.Post($"http://127.0.0.1:{Application.ServerPort}/asset/handshake/start", dict);
            };

            Post["/asset/handshake"] = x => {
                string apple = Request.Form.ApplePie;
                var dict = new Dictionary<string, string> {
                    { "ApplePie", apple }
                };
                return _api.Post($"http://127.0.0.1:{Application.ServerPort}/asset/handshake", dict);
            };

            Post["/asset/wol"] = x => {
                string mac = Request.Form.MacAddress;
                var dict = new Dictionary<string, string> {
                    { "MacAddress", mac }
                };
                return _api.Post($"http://127.0.0.1:{Application.ServerPort}/asset/wol", dict);
            };

            Get["/asset/nmap/{ip}"] = x => {
                string ip = x.ip;
                var model = _api.Get<List<NmapScanStatus>>($"http://127.0.0.1:{Application.ServerPort}/asset/nmap/" + ip);
                return JsonConvert.SerializeObject(model);
            };
        }
    }
}