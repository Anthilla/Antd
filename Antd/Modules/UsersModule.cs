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
using System.Dynamic;
using antdlib.common;
using antdlib.common.Tool;
using Antd.Database;
using Antd.Helpers;
using Antd.Users;
using Nancy;
using Nancy.Security;

namespace Antd.Modules {
    public class UsersModule : CoreModule {

        private readonly UserRepository _userRepositoryRepo = new UserRepository();
        private readonly UserClaimRepository _userClaimRepositoryRepo = new UserClaimRepository();
        private readonly SystemGroup _systemGroup = new SystemGroup();
        private readonly SystemUser _systemUser = new SystemUser();

        public UsersModule() {

            this.RequiresAuthentication();

            Post["/users/change/password"] = x => {
                var user = (string)Request.Form.User;
                var password = (string)Request.Form.Password;
                var tryGet = _userRepositoryRepo.GetByAlias(user);
                if (tryGet != null) {
                    var bash = new Bash();
                    var hp = bash.Execute($"mkpasswd -m sha-512 '{password}'", false);
                    _systemUser.SetPassword(user, hp);
                    _userRepositoryRepo.Delete(tryGet.Id);
                    _userRepositoryRepo.FastCreate(user, hp);
                }
                return HttpStatusCode.OK;
            };

            Get["/users/json"] = x => Response.AsJson(SelectizerMapModel.MapRawUserEntity(_userRepositoryRepo.GetAll()));

            Post["/users/refresh/users"] = x => {
                _userRepositoryRepo.Import();
                return HttpStatusCode.OK;
            };

            Post["/users/refresh/group"] = x => {
                _systemGroup.ImportGroupsToDatabase();
                return HttpStatusCode.OK;
            };

            Post["/users/create"] = x => {
                string name = Request.Form.Name;
                UserRepository.Shadow.Create(name);
                return Response.AsRedirect("/");
            };

            Post["/users/create/group"] = x => {
                string name = Request.Form.Name;
                _systemGroup.CreateGroup(name);
                return Response.AsRedirect("/");
            };

            Get["/users/identity"] = x => {
                dynamic vmod = new ExpandoObject();
                vmod.Users = _userRepositoryRepo.GetAll();
                return View["_page-users", vmod];
            };

            Post["/users/identity"] = x => {
                var guid = Guid.NewGuid().ToString();
                string firstName = Request.Form.UserEntity;
                string lastName = Request.Form.UserEntity;
                string password = Request.Form.UserPassword;
                _userRepositoryRepo.Create(new Dictionary<string, string> {
                    { "Guid", guid },
                    { "FirstName", firstName },
                    { "LastName", lastName },
                    { "Password", password },
                    { "Role", "" },
                    { "Email", "" },
                    { "CompanyGuid", "" },
                    { "Projects", "" },
                    { "Usergroups", "" },
                    { "Resources", "" },
                    { "Users", "" },
                    { "Tags", "" }
                });

                _userClaimRepositoryRepo.Create(new Dictionary<string, string> {
                    {"Guid", Guid.NewGuid().ToString() },
                    {"UserGuid", guid},
                    {"Type", "UserIdentity"},
                    {"Mode", "Antd"},
                    {"Label", "antd-master-id"},
                    {"Value", guid},
                });

                _userClaimRepositoryRepo.Create(new Dictionary<string, string> {
                    {"Guid", Guid.NewGuid().ToString() },
                    {"UserGuid", guid},
                    {"Type", "UserIdentity"},
                    {"Mode", "Antd"},
                    {"Label", "antd-master-identity"},
                    {"Value", firstName + " " +  lastName},
                });

                _userClaimRepositoryRepo.Create(new Dictionary<string, string> {
                    {"Guid", Guid.NewGuid().ToString() },
                    {"UserGuid", guid},
                    {"Type", "UserIdentity"},
                    {"Mode", "Antd"},
                    {"Label", "antd-master-password"},
                    {"Value", Encryption.XHash(password)},
                });

                return Response.AsRedirect("/");
            };

            Post["/users/identity/addclaim"] = x => {
                string userGuid = Request.Form.Userguid;
                string type = Request.Form.Type.Value;
                string mode = Request.Form.Mode.Value;
                string key = Request.Form.Key;
                string val = Request.Form.Value;
                _userClaimRepositoryRepo.Create(new Dictionary<string, string> {
                    {"Guid", Guid.NewGuid().ToString() },
                    {"UserGuid", userGuid},
                    {"Type", type},
                    {"Mode", mode},
                    {"Label", key},
                    {"Value", val}
                });
                return Response.AsRedirect("/");
            };

            Post["/users/identity/delclaim"] = x => {
                string guid = Request.Form.Guid;
                _userClaimRepositoryRepo.Delete(guid);
                return Response.AsRedirect("/");
            };
        }
    }
}