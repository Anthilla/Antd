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

using System.Dynamic;
using antdlib.CCTable;
using antdlib.Users;
using Nancy;
//using Nancy.Security;

namespace Antd.Modules {
    public class UsersModule : NancyModule {
        public UsersModule()
            : base("/users") {
            //this.RequiresAuthentication();

            Get["/"] = x => {
                dynamic vmod = new ExpandoObject();
                vmod.UserEntities = UserEntity.Repository.GetAll();
                vmod.SystemUsers = SystemUser.GetAllFromDatabase();
                vmod.SystemGroups = SystemGroup.GetAllFromDatabase();
                vmod.CurrentContext = Request.Path;
                vmod.CCTable = CCTableRepository.GetAllByContext(Request.Path);
                vmod.Count = CCTableRepository.GetAllByContext(Request.Path).ToArray().Length;
                return View["_page-users", vmod];
            };

            Post["/refresh/users"] = x => {
                SystemUser.ImportUsersToDatabase();
                return Response.AsJson(true);
            };

            Post["/refresh/group"] = x => {
                SystemGroup.ImportGroupsToDatabase();
                return Response.AsJson(true);
            };

            Post["/create"] = x => {
                string name = Request.Form.Name;
                SystemUser.CreateUser(name);
                return Response.AsRedirect("/users");
            };

            Post["/create/group"] = x => {
                string name = Request.Form.Name;
                SystemGroup.CreateGroup(name);
                return Response.AsRedirect("/users");
            };

            Post["/sysmap/create"] = x => {
                string user = Request.Form.UserAlias;
                string pwd = Request.Form.UserPassword;
                SystemUser.Map.MapUser(user, pwd);
                return Response.AsRedirect("/users");
            };

            Get["/identity"] = x => {
                dynamic vmod = new ExpandoObject();
                vmod.Users = UserEntity.Repository.GetAll();
                return View["_page-users", vmod];
            };

            Post["/identity"] = x => {
                var guid = UserEntity.Repository.GenerateGuid();
                string userIdentity = Request.Form.UserEntity;
                string userPassword = Request.Form.UserPassword;
                UserEntity.Repository.Create(guid, userIdentity);
                UserEntity.Repository.AddClaim(guid, UserEntity.ClaimType.UserIdentity, "antd-master-id", guid);
                UserEntity.Repository.AddClaim(guid, UserEntity.ClaimType.UserIdentity, "antd-master-identity", userIdentity);
                UserEntity.Repository.AddClaim(guid, UserEntity.ClaimType.UserPassword, "antd-master-password", userPassword);
                return Response.AsRedirect("/users");
            };

            Post["/identity/addclaim"] = x => {
                string userGuid = Request.Form.Userguid;
                string type = Request.Form.Type.Value;
                string key = Request.Form.Key;
                string val = Request.Form.Value;
                UserEntity.Repository.AddClaim(userGuid, UserEntity.ConvertClaimType(type), key, val);
                return Response.AsRedirect("/users");
            };

            Post["/identity/delclaim"] = x => {
                string userGuid = Request.Form.Userguid;
                string guid = Request.Form.Guid;
                UserEntity.Repository.RemoveClaim(userGuid, guid);
                return Response.AsRedirect("/users");
            };
        }
    }
}