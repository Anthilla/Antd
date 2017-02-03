﻿//-------------------------------------------------------------------------------------
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
    public class AntdSchedulerModule : NancyModule {

        private readonly ApiConsumer _api = new ApiConsumer();

        public AntdSchedulerModule() {
            Get["/scheduler"] = x => {
                var model = _api.Get<PageSchedulerModel>($"http://127.0.0.1:{Application.ServerPort}/scheduler");
                var json = JsonConvert.SerializeObject(model);
                return json;
            };

            Post["/scheduler"] = x => {
                var alias = (string)Request.Form.Alias;
                var command = (string)Request.Form.Command;
                var dict = new Dictionary<string, string> {
                    { "Alias", alias },
                    { "Command", command }
                };
                return _api.Post($"http://127.0.0.1:{Application.ServerPort}/scheduler", dict);
            };

            Post["/scheduler/enable"] = x => {
                return _api.Post($"http://127.0.0.1:{Application.ServerPort}/scheduler/enable", null);
            };

            Post["/scheduler/disable"] = x => {
                return _api.Post($"http://127.0.0.1:{Application.ServerPort}/scheduler/disable", null);
            };

            Post["/scheduler/delete"] = x => {
                string guid = Request.Form.Guid;
                var dict = new Dictionary<string, string> {
                    { "Guid", guid }
                };
                return _api.Post($"http://127.0.0.1:{Application.ServerPort}/scheduler/delete", dict);
            };

            Post["/scheduler/edit"] = x => {
                var id = (string)Request.Form.Guid;
                var command = (string)Request.Form.Command;
                var dict = new Dictionary<string, string> {
                    { "Guid", id },
                    { "Command", command }
                };
                return _api.Post($"http://127.0.0.1:{Application.ServerPort}/scheduler/edit", dict);
            };
        }
    }
}