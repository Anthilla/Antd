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
using System.Collections.Generic;
using System.Linq;
using antdlib.common;
using antdlib.config;
using antdlib.models;
using Nancy;
using Newtonsoft.Json;

namespace AntdUi.Modules {
    public class AntdAclModule : NancyModule {

        private readonly ApiConsumer _api = new ApiConsumer();

        public AntdAclModule() {
            Get["/acl"] = x => {
                var model = _api.Get<AclPersistentSettingModel>($"http://127.0.0.1:{Application.ServerPort}/acl");
                var json = JsonConvert.SerializeObject(model);
                return json;
            };

            Post["/acl/set"] = x => {
                return _api.Post($"http://127.0.0.1:{Application.ServerPort}/acl/set", null);
            };

            Post["/acl/restart"] = x => {
                return _api.Post($"http://127.0.0.1:{Application.ServerPort}/acl/restart", null);
            };

            Post["/acl/stop"] = x => {
                return _api.Post($"http://127.0.0.1:{Application.ServerPort}/acl/stop", null);
            };

            Post["/acl/enable"] = x => {
                return _api.Post($"http://127.0.0.1:{Application.ServerPort}/acl/enable", null);
            };

            Post["/acl/disable"] = x => {
                return _api.Post($"http://127.0.0.1:{Application.ServerPort}/acl/disable", null);
            };

            Post["/acl/add"] = x => {
                string dir = Request.Form.Path;
                var dict = new Dictionary<string, string> {
                    { "Path", dir }
                };
                return _api.Post($"http://127.0.0.1:{Application.ServerPort}/acl/add", dict);
            };

            Post["/acl/create"] = x => {
                string guid = Request.Form.Guid;
                string textall = Request.Form.Acl;
                var dict = new Dictionary<string, string> {
                    { "Guid", guid },
                    { "Acl", textall }
                };
                return _api.Post($"http://127.0.0.1:{Application.ServerPort}/acl/apply", dict);
            };

            Get["/acl/get/{guid}"] = x => {
                string guid = x.guid;
                var model = _api.Get<string[]>($"http://127.0.0.1:{Application.ServerPort}/acl/get/" + guid);
                var json = JsonConvert.SerializeObject(model);
                return json;
            };

            Post["/acl/del"] = x => {
                string guid = Request.Form.Guid;
                var dict = new Dictionary<string, string> {
                    { "Guid", guid }
                };
                return _api.Post($"http://127.0.0.1:{Application.ServerPort}/acl/del", dict);
            };

            Post["/acl/apply"] = x => {
                string guid = Request.Form.Guid;
                var dict = new Dictionary<string, string> {
                    { "Guid", guid }
                };
                return _api.Post($"http://127.0.0.1:{Application.ServerPort}/acl/apply", dict);
            };

            #region [    Script    ]
            Post["/acl/apply/script"] = x => {
                string user = Request.Form.User;
                var dict = new Dictionary<string, string> {
                    { "User", user }
                };
                return _api.Post($"http://127.0.0.1:{Application.ServerPort}/acl/apply/script", dict);
            };
            #endregion
        }
    }
}