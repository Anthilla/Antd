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

using System.Collections.Generic;
using antdlib.common;
using antdlib.models;
using Nancy;
using Newtonsoft.Json;

namespace AntdUi.Modules {
    public class AssetSyncMachineModule : NancyModule {

        private readonly ApiConsumer _api = new ApiConsumer();

        public AssetSyncMachineModule() {
            Get["/syncmachine"] = x => {
                var model = _api.Get<PageAssetSyncMachineModel>($"http://127.0.0.1:{Application.ServerPort}/syncmachine");
                var json = JsonConvert.SerializeObject(model);
                return json;
            };

            Post["/syncmachine/set"] = x => {
                return _api.Post($"http://127.0.0.1:{Application.ServerPort}/syncmachine/set", null);
            };

            Post["/syncmachine/restart"] = x => {
                return _api.Post($"http://127.0.0.1:{Application.ServerPort}/syncmachine/restart", null);
            };

            Post["/syncmachine/stop"] = x => {
                return _api.Post($"http://127.0.0.1:{Application.ServerPort}/syncmachine/stop", null);
            };

            Post["/syncmachine/enable"] = x => {
                return _api.Post($"http://127.0.0.1:{Application.ServerPort}/syncmachine/enable", null);
            };

            Post["/syncmachine/disable"] = x => {
                return _api.Post($"http://127.0.0.1:{Application.ServerPort}/syncmachine/disable", null);
            };

            Post["/syncmachine/machine"] = x => {
                string machineAddress = Request.Form.MachineAddress;
                var dict = new Dictionary<string, string> {
                    {"MachineAddress", machineAddress}
                };
                return _api.Post($"http://127.0.0.1:{Application.ServerPort}/syncmachine/machine", dict);
            };

            Post["/syncmachine/machine/del"] = x => {
                string guid = Request.Form.Guid;
                var dict = new Dictionary<string, string> {
                    {"Guid", guid}
                };
                return _api.Post($"http://127.0.0.1:{Application.ServerPort}/syncmachine/machine/del", dict);
            };

            Post["Accept Configuration", "/syncmachine/accept"] = x => {
                string file = Request.Form.File;
                string content = Request.Form.Content;
                var dict = new Dictionary<string, string> {
                    {"File", file},
                    {"Content", content}
                };
                return _api.Post($"http://127.0.0.1:{Application.ServerPort}/syncmachine/accept", dict);
            };
        }
    }
}