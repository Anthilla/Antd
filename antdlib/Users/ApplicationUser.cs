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

using antdlib.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using antdlib.Common;

namespace antdlib.Users {
    public class ApplicationUser {
        public static IEnumerable<UserModel> GetAll() {
            return DeNSo.Session.New.Get<UserModel>(u => u != null).ToList();
        }

        public static UserModel GetByAlias(string alias) {
            return DeNSo.Session.New.Get<UserModel>(u => u != null && u.Alias == alias).FirstOrDefault();
        }

        public static void Create(string fname, string lname, string passwd, string email) {
            var user = new UserModel {
                _Id = Guid.NewGuid().ToString(),
                Guid = Guid.NewGuid().ToString(),
                FirstName = fname,
                LastName = lname,
                Email = email,
                UserType = UserType.IsApplicationUser,
                Alias = SetUserAlias(fname, lname).ToLower(),
                Password = SetPassword(passwd)
            };
            DeNSo.Session.New.Set(user);
        }

        private static string SetUserAlias(string firstName, string lastName) {
            var first = firstName.Replace(" ", "").Substring(0, 3);
            var last = lastName.Replace(" ", "").Substring(0, 3);
            var stringAlias = last + first;
            var tryAlias = stringAlias + "01";
            var isUser = GetByAlias(tryAlias);
            if (isUser == null) {
                return tryAlias;
            }
            var table = GetAll();
            var existingAlias = (from c in table
                                 where c.Alias.Contains(stringAlias)
                                 orderby c.Alias ascending
                                 select c.Alias).ToArray();
            var lastAlias = existingAlias[existingAlias.Length - 1];
            var newNumber = (Convert.ToInt32(lastAlias.Substring(6, 2)) + 1).ToString("D2");
            return stringAlias + newNumber;
        }

        private static SystemUserPassword SetPassword(string passwdString) {
            var passwd = new SystemUserPassword {
                Type = "",
                Salt = "",
                Result = Cryptography.Hash256(passwdString).ToHex()
            };
            return passwd;
        }
    }
}
