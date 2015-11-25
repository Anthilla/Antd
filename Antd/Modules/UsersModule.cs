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
using antdlib.Security;
using antdlib.Users;
using Antd.ViewHelpers;
using Nancy;
using Nancy.Security;

namespace Antd.Modules {
    public class UsersModule : CoreModule {
        public UsersModule() {
            this.RequiresAuthentication();

            Get["/users/json"] = x => Response.AsJson(SelectizerMapModel.MapRawUserEntity(UserEntity.Repository.GetAll()));

            Post["/users/refresh/users"] = x => {
                SystemUser.ImportUsersToDatabase();
                return Response.AsJson(true);
            };

            Post["/users/refresh/group"] = x => {
                SystemGroup.ImportGroupsToDatabase();
                return Response.AsJson(true);
            };

            Post["/users/create"] = x => {
                string name = Request.Form.Name;
                SystemUser.CreateUser(name);
                return Response.AsRedirect("/");
            };

            Post["/users/create/group"] = x => {
                string name = Request.Form.Name;
                SystemGroup.CreateGroup(name);
                return Response.AsRedirect("/");
            };

            Post["/users/sysmap/create"] = x => {
                string user = Request.Form.UserAlias;
                string pwd = Request.Form.UserPassword;
                SystemUser.Map.MapUser(user, pwd);
                return Response.AsRedirect("/");
            };

            Get["/users/identity"] = x => {
                dynamic vmod = new ExpandoObject();
                vmod.Users = UserEntity.Repository.GetAll();
                return View["_page-users", vmod];
            };

            Post["/users/identity"] = x => {
                var guid = UserEntity.Repository.GenerateGuid();
                string userIdentity = Request.Form.UserEntity;
                string userPassword = Request.Form.UserPassword;
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
                        Mode = UserEntity.ClaimMode.Antd,
                        Type = UserEntity.ClaimType.UserIdentity,
                        Key = "antd-master-identity",
                        Value = userIdentity
                    },
                    new UserEntity.UserEntityModel.Claim {
                        ClaimGuid = guid,
                        Mode = UserEntity.ClaimMode.Antd,
                        Type = UserEntity.ClaimType.UserIdentity,
                        Key = "antd-master-alias",
                        Value = alias
                    },
                    new UserEntity.UserEntityModel.Claim {
                        ClaimGuid = guid,
                        Mode = UserEntity.ClaimMode.Antd,
                        Type = UserEntity.ClaimType.UserPassword,
                        Key = "antd-master-password",
                        Value = Cryptography.Hash256ToString(userPassword)
                    }
                };
                UserEntity.Repository.Create(guid, userIdentity, alias, claims);
                return Response.AsRedirect("/");
            };

            Post["/users/identity/addclaim"] = x => {
                string userGuid = Request.Form.Userguid;
                string type = Request.Form.Type.Value;
                string mode = Request.Form.Mode.Value;
                string key = Request.Form.Key;
                string val = Request.Form.Value;
                UserEntity.Repository.AddClaim(userGuid, UserEntity.ConvertClaimType(type), UserEntity.ConvertClaimMode(mode), key, val);
                return Response.AsRedirect("/");
            };

            Post["/users/identity/delclaim"] = x => {
                string userGuid = Request.Form.Userguid;
                string guid = Request.Form.Guid;
                UserEntity.Repository.RemoveClaim(userGuid, guid);
                return Response.AsRedirect("/");
            };
        }
    }
}