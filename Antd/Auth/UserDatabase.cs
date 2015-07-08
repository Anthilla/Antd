///-------------------------------------------------------------------------------------
///     Copyright (c) 2014, Anthilla S.r.l. (http://www.anthilla.com)
///     All rights reserved.
///
///     Redistribution and use in source and binary forms, with or without
///     modification, are permitted provided that the following conditions are met:
///         * Redistributions of source code must retain the above copyright
///           notice, this list of conditions and the following disclaimer.
///         * Redistributions in binary form must reproduce the above copyright
///           notice, this list of conditions and the following disclaimer in the
///           documentation and/or other materials provided with the distribution.
///         * Neither the name of the Anthilla S.r.l. nor the
///           names of its contributors may be used to endorse or promote products
///           derived from this software without specific prior written permission.
///
///     THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
///     ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
///     WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
///     DISCLAIMED. IN NO EVENT SHALL ANTHILLA S.R.L. BE LIABLE FOR ANY
///     DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
///     (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
///     LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
///     ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
///     (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
///     SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
///
///     20141110
///-------------------------------------------------------------------------------------

using Antd.Security;
using Antd.Users;
using Nancy;
using Nancy.Authentication.Forms;
using Nancy.Security;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Antd.Auth {

    public class UserIdentity : IUserIdentity {

        public string UserName { get; set; }

        public UserType UserType { get; set; }

        public IEnumerable<string> Claims { get; set; }
    }

    public class UserDatabase : IUserMapper {

        private static HashSet<AuthUser> USERS() {
            HashSet<AuthUser> userList = new HashSet<AuthUser>() { };
            userList.Add(new AuthUser() {
                Name = "master",
                Password = "master",
                UserType = UserType.Master,
                Guid = new Guid("00000000-0000-0000-0000-000000000500")
            });
            var sysUsers = SystemUser.GetAll();
            var appUsers = ApplicationUser.GetAll();
            userList.UnionWith(Map(sysUsers));
            userList.UnionWith(Map(appUsers));
            return userList;
        }

        public IUserIdentity GetUserFromIdentifier(Guid identifier, NancyContext context) {
            var UserRecord = USERS().FirstOrDefault(u => u.Guid == identifier);
            return UserRecord == null
                       ? null
                       : new UserIdentity { UserName = UserRecord.Name };
        }

        public static string GetUserEmail(Guid identifier) {
            var user = USERS().FirstOrDefault(u => u.Guid == identifier);
            return (user == null) ? null : user.Email;
        }

        public static Guid? ValidateUser(string username, string password) {
            var user = USERS().FirstOrDefault(u => u.Name == username);
            if (user == null) {
                return null;
            }
            var type = user.UserType;
            switch (type) {
                case UserType.Master:
                    if (CheckMasterPassword(password) == true) {
                        return user.Guid;
                    }
                    else {
                        return null;
                    }
                case UserType.IsApplicationUser:
                    if (CheckApplicationPassword(password, user.Password) == true) {
                        return user.Guid;
                    }
                    else {
                        return null;
                    }
                case UserType.IsSystemUser:
                    if (CheckSystemPassword(password, user.Password, user.Salt) == true) {
                        return user.Guid;
                    }
                    else {
                        return null;
                    }
                default:
                    return null;
            }
        }

        private static HashSet<AuthUser> Map(IEnumerable<UserModel> users) {
            HashSet<AuthUser> list = new HashSet<AuthUser>() { };
            foreach (UserModel user in users) {
                var au = new AuthUser() {
                    Name = user.Alias,
                    Password = user.Password.Result,
                    Salt = user.Password.Salt,
                    UserType = user.UserType
                };
                au.Guid = Guid.Parse(user.Guid);
                list.Add(au);
            }
            return list;
        }

        private static bool CheckMasterPassword(string input) {
            return (input == "master") ? true : false;
        }

        private static bool CheckApplicationPassword(string input, string passwd) {
            return (Cryptography.Hash256(input).ToHex() == passwd) ? true : false;
        }

        private static bool CheckSystemPassword(string input, string passwd, string salt) {
            return (Cryptography.Hash256Terminal(input, salt) == passwd) ? true : false;
        }
    }
}