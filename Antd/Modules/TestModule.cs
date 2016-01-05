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
using System.Dynamic;
using antdlib.Users;
using Nancy;

namespace Antd.Modules {
    public class TestModule : CoreModule {
        public TestModule() {

            Before += y => null;

            Get["Test page", "/test"] = x => Response.AsText("Hello World!");

            Get["/test/page"] = x => View["page-test"];

            Get["/test/ssh"] = x => {
                antdlib.Ssh.Test.Start("aos003", "root", "root");
                return View["page-test"];
            };

            Get["/test/2"] = x => {
                dynamic vmod = new ExpandoObject();
                vmod.Name = "Rendered with SSVE! ☻";
                var list = new List<string> {"uno", "due", "tre"};
                var list2 = new List<List<string>>();
                list2.Add(list);
                list2.Add(list);
                list2.Add(list);
                vmod.List = list2;
                return View["page-empty", vmod];
            };

            Get["/test/3"] = x => {
                var prova = antdlib.Virsh.Virsh.Monitor.list("--all --title");
                return Response.AsJson(prova);
            };

            Post["/sp/users/identity"] = x => {
                var guid = UserEntity.Repository.GenerateGuid();
                string spUserAlias = Request.Form.UserAlias;
                string spUserFirstName = Request.Form.UserFirstName;
                string spUserLastName = Request.Form.UserLastName;
                string spUserGuid = Request.Form.UserGuid;
                string spUserEmail = Request.Form.UserEmail;
                var userIdentity = spUserFirstName + " " + spUserLastName;
                var alias = UserEntity.Repository.GenerateUserAlias(userIdentity);
                var claims = new List<UserEntity.UserEntityModel.Claim> {
                    new UserEntity.UserEntityModel.Claim {
                        ClaimGuid = guid,
                        Mode = UserEntity.ClaimMode.Antd,
                        Type = UserEntity.ClaimType.UserIdentity,
                        Key = "antd-master-id",
                        Value = guid
                    },
                    new UserEntity.UserEntityModel.Claim {
                        ClaimGuid = guid,
                        Mode = UserEntity.ClaimMode.AnthillaSP,
                        Type = UserEntity.ClaimType.UserIdentity,
                        Key = "anthillasp-alias",
                        Value = spUserAlias
                    },
                    new UserEntity.UserEntityModel.Claim {
                        ClaimGuid = guid,
                        Mode = UserEntity.ClaimMode.AnthillaSP,
                        Type = UserEntity.ClaimType.UserIdentity,
                        Key = "anthillasp-first-name",
                        Value = spUserFirstName
                    },
                    new UserEntity.UserEntityModel.Claim {
                        ClaimGuid = guid,
                        Mode = UserEntity.ClaimMode.AnthillaSP,
                        Type = UserEntity.ClaimType.UserIdentity,
                        Key = "anthillasp-last-name",
                        Value = spUserLastName
                    },
                    new UserEntity.UserEntityModel.Claim {
                        ClaimGuid = guid,
                        Mode = UserEntity.ClaimMode.AnthillaSP,
                        Type = UserEntity.ClaimType.UserIdentity,
                        Key = "anthillasp-guid",
                        Value = spUserGuid
                    },
                    new UserEntity.UserEntityModel.Claim {
                        ClaimGuid = guid,
                        Mode = UserEntity.ClaimMode.AnthillaSP,
                        Type = UserEntity.ClaimType.UserIdentity,
                        Key = "anthillasp-email",
                        Value = spUserEmail
                    }
                };
                string spUserToken = Request.Form.UserToken;
                if (!string.IsNullOrEmpty(spUserToken) && spUserToken != "null") {
                    claims.Add(new UserEntity.UserEntityModel.Claim {
                        ClaimGuid = guid,
                        Mode = UserEntity.ClaimMode.AnthillaSP,
                        Type = UserEntity.ClaimType.UserToken,
                        Key = "anthillasp-token",
                        Value = spUserToken
                    });
                }
                UserEntity.Repository.Create(guid, userIdentity, alias, claims);
                return Response.AsJson(true);
            };
        }
    }
}