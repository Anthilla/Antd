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
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using anthilla.core;

namespace Antd.Modules {
    public class AntdAclModule : NancyModule {

        public AntdAclModule() {
            Get["/acl"] = x => {
                var aclConfig = AclConfiguration.Get() ?? new AclConfigurationModel();
                var aclIsActive = aclConfig.IsActive;
                var list = new List<AclPersistentSettingModel>();
                foreach(var aaa in aclConfig.Settings) {
                    aaa.AclText = System.IO.File.ReadAllLines(aaa.Acl).JoinToString(Environment.NewLine);
                    list.Add(aaa);
                }
                var model = new PageAclModel {
                    AclIsActive = aclIsActive,
                    Acl = list.OrderBy(_ => _.Path)
                };
                return JsonConvert.SerializeObject(model);
            };

            Post["/acl/set"] = x => {
                AclConfiguration.Set();
                return HttpStatusCode.OK;
            };

            Post["/acl/restart"] = x => {
                AclConfiguration.Start();
                return HttpStatusCode.OK;
            };

            Post["/acl/stop"] = x => {
                AclConfiguration.Stop();
                return HttpStatusCode.OK;
            };

            Post["/acl/enable"] = x => {
                AclConfiguration.Enable();
                AclConfiguration.Start();
                return HttpStatusCode.OK;
            };

            Post["/acl/disable"] = x => {
                AclConfiguration.Disable();
                AclConfiguration.Stop();
                return HttpStatusCode.OK;
            };

            Post["/acl/add"] = x => {
                string dir = Request.Form.Path;
                if(string.IsNullOrEmpty(dir)) {
                    return HttpStatusCode.OK;
                }
                AclConfiguration.AddAcl(dir);
                return HttpStatusCode.OK;
            };

            Post["/acl/create"] = x => {
                string guid = Request.Form.Guid;
                string textall = Request.Form.Acl;
                if(string.IsNullOrEmpty(guid) || string.IsNullOrEmpty(textall)) {
                    return HttpStatusCode.OK;
                }
                AclConfiguration.SetAcl(guid, textall.SplitToList(Environment.NewLine).ToArray());
                return HttpStatusCode.OK;
            };

            Get["/acl/get/{guid}"] = x => {
                string guid = x.guid;
                if(string.IsNullOrEmpty(guid)) {
                    return HttpStatusCode.OK;
                }
                var result = AclConfiguration.GetAcl(guid);
                return JsonConvert.SerializeObject(result);
            };

            Post["/acl/del"] = x => {
                string guid = Request.Form.Guid;
                if(string.IsNullOrEmpty(guid)) {
                    return HttpStatusCode.BadRequest;
                }
                AclConfiguration.RemoveAcl(guid);
                return HttpStatusCode.OK;
            };

            Post["/acl/apply"] = x => {
                string guid = Request.Form.Guid;
                if(string.IsNullOrEmpty(guid)) {
                    return HttpStatusCode.BadRequest;
                }
                var r = AclConfiguration.ApplyAcl(guid);
                return Response.AsText(r);
            };

            #region [    Script    ]
            Post["/acl/apply/script"] = x => {
                string user = Request.Form.User;
                if(string.IsNullOrEmpty(user)) {
                    return HttpStatusCode.BadRequest;
                }
                AclConfiguration.ApplyAclScript(user);
                return HttpStatusCode.OK;
            };
            #endregion
        }
    }
}