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

using antdlib.MountPoint;
using System;
using System.Collections.Generic;
using System.Linq;
using antdlib.Common;

namespace antdlib.Users {
    public class SystemUser {
        public class Config {
            private static string UnixHashPassword(string password, string salt) {
                return Terminal.Execute($"mkpasswd -m sha-512 {password} -s \"{salt}\"");
            }

            public static void ResetPasswordForUser(string user, string password) {
                var salt = Guid.NewGuid().ToString();
                var hashedPassword = UnixHashPassword(password, salt);
                if (hashedPassword.Length <= 0)
                    return;
                Terminal.Execute($"usermod -p {hashedPassword} LOGIN(?)");
            }
        }

        private const string File = "/etc/shadow";
        private static readonly string MntFile = Mount.SetFilesPath(File);
        private const string FilePwd = "/etc/passwd";
        private static readonly string MntFilePwd = Mount.SetFilesPath(FilePwd);

        public static void SetReady() {
            if (!System.IO.File.Exists(MntFile)) {
                System.IO.File.Copy(File, MntFile, true);
            }
            else if (System.IO.File.Exists(MntFile) && FileSystem.IsNewerThan(File, MntFile)) {
                System.IO.File.Delete(MntFile);
                System.IO.File.Copy(File, MntFile, true);
            }
            Mount.File(File);
            if (!System.IO.File.Exists(MntFilePwd)) {
                System.IO.File.Copy(FilePwd, MntFilePwd, true);
            }
            else if (System.IO.File.Exists(MntFilePwd) && FileSystem.IsNewerThan(FilePwd, MntFilePwd)) {
                System.IO.File.Delete(MntFilePwd);
                System.IO.File.Copy(FilePwd, MntFilePwd, true);
            }
            Mount.File(FilePwd);
        }

        private static bool CheckIsActive() {
            var mount = MountRepository.Get(File);
            var mountPwd = MountRepository.Get(File);
            return (mount != null || mountPwd != null);
        }

        public static bool IsActive => CheckIsActive();

        public static IEnumerable<UserModel> GetAll() {
            try {
                var list = new List<UserModel>();
                if (!System.IO.File.Exists(File) || !System.IO.File.Exists(FilePwd))
                    return list;
                var usersString = Terminal.Execute($"cat {File}");
                var users = usersString.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries).ToArray();
                var passwdUserString = Terminal.Execute($"cat {FilePwd}");
                var passwdUsers = passwdUserString.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries).ToArray();
                foreach (var user in users) {
                    var mu = MapUser(user);
                    var pwdstring = passwdUsers.FirstOrDefault(s => s.Contains(mu.Alias));
                    if (!string.IsNullOrEmpty(pwdstring)) {
                        var mup = AddUserInfoFromPasswd(mu, pwdstring);
                        mu = mup;
                    }
                    list.Add(mu);
                }
                return list;
            }
            catch (Exception) {
                ConsoleLogger.Warn("There's something wrong while getting system users...");
                return new List<UserModel>();
            }
        }

        public static void ImportUsersToDatabase() {
            if (!System.IO.File.Exists(File) || !System.IO.File.Exists(FilePwd))
                return;
            var usersString = Terminal.Execute($"cat {File}");
            var users = usersString.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries).ToArray();
            var passwdUserString = Terminal.Execute($"cat {FilePwd}");
            var passwdUsers = passwdUserString.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries).ToArray();
            foreach (var user in users) {
                var mu = MapUser(user);
                var pwdstring = passwdUsers.FirstOrDefault(s => s.Contains(mu.Alias));
                if (!string.IsNullOrEmpty(pwdstring)) {
                    var mup = AddUserInfoFromPasswd(mu, pwdstring);
                    mu = mup;
                }
                DeNSo.Session.New.Set(mu);
            }
        }

        public static IEnumerable<UserModel> GetAllFromDatabase() {
            var users = DeNSo.Session.New.Get<UserModel>().ToList();
            if (users.Count < 1) {
                ImportUsersToDatabase();
            }
            return users;
        }

        private static UserModel MapUser(string userString) {
            var userInfo = userString.Split(new[] { ":" }, StringSplitOptions.None).ToArray();
            var user = new UserModel();
            if (userInfo.Length <= 8)
                return user;
            user.Guid = Guid.NewGuid().ToString();
            user.Alias = userInfo[0];
            user.Email = null;
            user.Password = MapPassword(userInfo[1]);
            user.LastChanged = userInfo[2];
            user.MinimumNumberOfDays = userInfo[3];
            user.MaximumNumberOfDays = userInfo[4];
            user.Warn = userInfo[5];
            user.Inactive = userInfo[6];
            user.Expire = userInfo[7];
            user.UserType = UserType.IsSystemUser;
            return user;
        }

        private static UserModel AddUserInfoFromPasswd(UserModel user, string userString) {
            var userPasswdInfo = userString.Split(new[] { ":" }, StringSplitOptions.None).ToArray();
            if (userPasswdInfo.Length <= 6)
                return user;
            user.Uid = userPasswdInfo[2];
            user.GroupId = userPasswdInfo[3];
            user.Info = userPasswdInfo[4];
            user.HomeDirectory = userPasswdInfo[5];
            user.LoginShell = userPasswdInfo[6];
            return user;
        }

        private static SystemUserPassword MapPassword(string passwdString) {
            var passwdInfo = passwdString.Split(new[] { @"$" }, StringSplitOptions.None).ToArray();
            var passwd = new SystemUserPassword();
            if (passwdInfo.Length <= 2)
                return passwd;
            passwd.Type = passwdInfo[0];
            passwd.Salt = passwdInfo[1];
            passwd.Result = passwdInfo[2];
            return passwd;
        }

        public static void CreateUser(string user) {
            Terminal.Execute("useradd " + user);
        }
    }
}