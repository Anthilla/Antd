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
    public class AssetClusterModule : NancyModule {

        private readonly ApiConsumer _api = new ApiConsumer();

        public AssetClusterModule() {
            Get["/cluster"] = x => {
                var model = _api.Get<PageAssetClusterModel>($"http://127.0.0.1:{Application.ServerPort}/cluster");
                var json = JsonConvert.SerializeObject(model);
                return json;
            };

            Post["/cluster/set"] = x => {
                return _api.Post($"http://127.0.0.1:{Application.ServerPort}/cluster/set");
            };

            Post["/cluster/restart"] = x => {
                return _api.Post($"http://127.0.0.1:{Application.ServerPort}/cluster/restart");
            };

            Post["/cluster/stop"] = x => {
                return _api.Post($"http://127.0.0.1:{Application.ServerPort}/cluster/stop");
            };

            Post["/cluster/enable"] = x => {
                return _api.Post($"http://127.0.0.1:{Application.ServerPort}/cluster/enable");
            };

            Post["/cluster/disable"] = x => {
                return _api.Post($"http://127.0.0.1:{Application.ServerPort}/cluster/disable");
            };

            Post["/cluster/save"] = x => {
                string config = Request.Form.Config;
                string ip = Request.Form.Ip;
                var dict = new Dictionary<string, string> {
                    {"Config", config},
                    {"Ip", ip}
                };
                return _api.Post($"http://127.0.0.1:{Application.ServerPort}/cluster/save", dict);
            };

            Post["Accept Configuration", "/cluster/accept"] = x => {
                string file = Request.Form.File;
                string content = Request.Form.Content;
                var dict = new Dictionary<string, string> {
                    {"File", file},
                    {"Content", content}
                };
                return _api.Post($"http://127.0.0.1:{Application.ServerPort}/cluster/accept", dict);
            };
        }
    }
}