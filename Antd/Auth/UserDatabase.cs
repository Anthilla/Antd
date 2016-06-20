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
using antdlib.views;
using Nancy;
using Nancy.Authentication.Forms;
using Nancy.Security;

namespace Antd.Auth {

    public class UserIdentity : IUserIdentity {
        public Guid UserGuid { get; set; }
        public string UserName { get; set; }
        public IEnumerable<string> Claims { get; set; }
    }

    public class UserDatabase : IUserMapper {

        private static IEnumerable<UserIdentity> Users() {
            var userList = new List<UserIdentity> {
                new UserIdentity {
                    UserGuid = Guid.Parse("00000000-0000-0000-0000-000000000500"),
                    UserName = "master",
                    Claims = new List<string> { Encryption.XHash("master"), "00000000-0000-0000-0000-000000000500" }
                }
            };
            var appUsers = new Database.UserRepository().GetAll().Select(MapUser);
            var userEntityModels = userList.Concat(appUsers);
            return userEntityModels;
        }

        private static UserIdentity MapUser(UserSchema user) {
            return new UserIdentity { UserGuid = user.Guid.ToGuid(), UserName = user.Alias, Claims = new List<string> { user.Password, user.Guid } };
        }

        public IUserIdentity GetUserFromIdentifier(Guid identifier, NancyContext context) {
            var userRecord = Users().FirstOrDefault(u => u.UserGuid == identifier);
            return userRecord == null
                       ? null
                       : new UserIdentity { UserName = userRecord.UserName };
        }

        public static Guid? ValidateUser(string userIdentity, string password) {
            //if (userIdentity == "master" && password == "master") {
            //    return Guid.Parse("00000000-0000-0000-0000-000000000500");
            //}
            var hash = Encryption.XHash(password);
            var user = Users().FirstOrDefault(_=>_.UserName == userIdentity && _.Claims.Contains(hash));
            return user?.UserGuid;
        }
    }
}