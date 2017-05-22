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
    public class AppsManagementModule : NancyModule {

        private readonly ApiConsumer _api = new ApiConsumer();

        public AppsManagementModule() {
            Get["/apps/management"] = x => {
                var model = _api.Get<PageAppsManagementModel>($"http://127.0.0.1:{Application.ServerPort}/apps/management");
                var json = JsonConvert.SerializeObject(model);
                return json;
            };

            Post["/apps/setup"] = x => {
                string appName = Request.Form.AppName;
                var dict = new Dictionary<string, string> {
                    {"AppName", appName}
                };
                return _api.Post($"http://127.0.0.1:{Application.ServerPort}/apps/setup", dict);
            };

            Get["/apps/status/{unit}"] = x => {
                string unit = x.unit;
                var model = _api.Get<string>($"http://127.0.0.1:{Application.ServerPort}/apps/status/" + unit);
                var json = JsonConvert.SerializeObject(model);
                return json;
            };

            Get["/apps/active/{unit}"] = x => {
                string unit = x.unit;
                var model = _api.Get<bool>($"http://127.0.0.1:{Application.ServerPort}/apps/active/" + unit);
                var json = JsonConvert.SerializeObject(model);
                return json;
            };

            Post["/apps/restart"] = x => {
                string name = Request.Form.Name;
                var dict = new Dictionary<string, string> {
                    {"Name", name}
                };
                return _api.Post($"http://127.0.0.1:{Application.ServerPort}/apps/restart", dict);
            };

            Post["/apps/stop"] = x => {
                string name = Request.Form.Name;
                var dict = new Dictionary<string, string> {
                    {"Name", name}
                };
                return _api.Post($"http://127.0.0.1:{Application.ServerPort}/apps/stop", dict);
            };
        }
    }
}