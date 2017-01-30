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
using antdlib.common;
using Antd.Acl;
using Nancy;
using Nancy.Security;
using Newtonsoft.Json;

namespace Antd.Modules {
    public class ServiceAclModule : NancyModule {

        public ServiceAclModule() {
            this.RequiresAuthentication();

            Post["/services/acl/set"] = x => {
                var aclConfiguration = new AclConfiguration();
                aclConfiguration.Set();
                return HttpStatusCode.OK;
            };

            Post["/services/acl/restart"] = x => {
                var aclConfiguration = new AclConfiguration();
                aclConfiguration.Start();
                return HttpStatusCode.OK;
            };

            Post["/services/acl/stop"] = x => {
                var aclConfiguration = new AclConfiguration();
                aclConfiguration.Stop();
                return HttpStatusCode.OK;
            };

            Post["/services/acl/enable"] = x => {
                var dhcpdConfiguration = new AclConfiguration();
                dhcpdConfiguration.Enable();
                dhcpdConfiguration.Start();
                return HttpStatusCode.OK;
            };

            Post["/services/acl/disable"] = x => {
                var dhcpdConfiguration = new AclConfiguration();
                dhcpdConfiguration.Disable();
                dhcpdConfiguration.Stop();
                return HttpStatusCode.OK;
            };

            Post["/services/acl/add"] = x => {
                string dir = Request.Form.Path;
                if(string.IsNullOrEmpty(dir)) {
                    return Response.AsRedirect("/");
                }
                var aclConfiguration = new AclConfiguration();
                aclConfiguration.AddAcl(dir);
                return Response.AsRedirect("/");
            };

            Post["/services/acl/set"] = x => {
                string guid = Request.Form.Guid;
                string textall = Request.Form.Acl;
                if(string.IsNullOrEmpty(guid) || string.IsNullOrEmpty(textall)) {
                    return Response.AsRedirect("/");
                }
                var aclConfiguration = new AclConfiguration();
                aclConfiguration.SetAcl(guid, textall.SplitToList(Environment.NewLine).ToArray());
                return Response.AsRedirect("/");
            };

            Get["/services/acl/get/{guid}"] = x => {
                string guid = x.guid;
                if(string.IsNullOrEmpty(guid)) {
                    return Response.AsRedirect("/");
                }
                var aclConfiguration = new AclConfiguration();
                var result = aclConfiguration.GetAcl(guid);
                return JsonConvert.SerializeObject(result);
            };

            Post["/services/acl/del"] = x => {
                string guid = Request.Form.Guid;
                if(string.IsNullOrEmpty(guid)) {
                    return HttpStatusCode.BadRequest;
                }
                var aclConfiguration = new AclConfiguration();
                aclConfiguration.RemoveAcl(guid);
                return HttpStatusCode.OK;
            };

            Post["/services/acl/apply"] = x => {
                string guid = Request.Form.Guid;
                if(string.IsNullOrEmpty(guid)) {
                    return HttpStatusCode.BadRequest;
                }
                var aclConfiguration = new AclConfiguration();
                var r = aclConfiguration.ApplyAcl(guid);
                return Response.AsText(r);
            };

            #region [    Script    ]
            Post["/services/acl/apply/script"] = x => {
                string user = Request.Form.User;
                if(string.IsNullOrEmpty(user)) {
                    return HttpStatusCode.BadRequest;
                }
                var aclConfiguration = new AclConfiguration();
                aclConfiguration.ApplyAclScript(user);
                return Response.AsRedirect("/");
            };
            #endregion
        }
    }
}