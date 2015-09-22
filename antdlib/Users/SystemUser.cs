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
using System.IO;
using System.Linq;

namespace antdlib.Users {

    public class SystemUser {

        private static string file = "/etc/shadow";

        private static string FILE = Mount.SetFILESPath(file);

        private static string filePwd = "/etc/passwd";

        private static string FILEPWD = Mount.SetFILESPath(filePwd);

        public static void SetReady() {
            //Terminal.Execute($"cp {file} {FILE}");
            ConsoleLogger.Warn($"Copying files:");
            if (!File.Exists(FILE)) {
                ConsoleLogger.Warn($"{file} -> {FILE}:");
                File.Copy(file, FILE, true);
            }
            Mount.File(file);
            //Terminal.Execute($"cp {filePwd} {FILEPWD}");
            if (!File.Exists(FILEPWD)) {
                ConsoleLogger.Warn($"{filePwd} -> {FILEPWD}:");
                File.Copy(filePwd, FILEPWD, true);
            }
            Mount.File(filePwd);
        }

        private static bool CheckIsActive() {
            var mount = MountRepository.Get(file);
            var mountPwd = MountRepository.Get(file);
            return (mount == null && mountPwd == null) ? false : true;
        }

        public static bool IsActive { get { return CheckIsActive(); } }

        public static IEnumerable<UserModel> GetAll() {
            Log.Logger.TraceMethod("Users Management", $"{System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName}.{System.Reflection.MethodBase.GetCurrentMethod().Name}");
            var list = new List<UserModel>() { };
            if (File.Exists(file) && File.Exists(filePwd)) {
                var usersString = FileSystem.ReadFile(file);
                var users = usersString.Split(new String[] { @"\n" }, StringSplitOptions.None).ToArray();
                var passwdUserString = FileSystem.ReadFile(filePwd);
                var passwdUsers = usersString.Split(new String[] { @"\n" }, StringSplitOptions.None).ToArray();
                foreach (var user in users) {
                    var mu = MapUser(user);
                    var pwdstring = passwdUsers.Where(s => s.Contains(mu.Alias)).FirstOrDefault();
                    if (pwdstring.Length > 0) {
                        var mup = AddUserInfoFromPasswd(mu, pwdstring);
                        mu = mup;
                    }
                    list.Add(mu);
                }
            }
            return list;
        }

        private static UserModel MapUser(string userString) {
            Log.Logger.TraceMethod("Users Management", $"{System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName}.{System.Reflection.MethodBase.GetCurrentMethod().Name}");
            var userInfo = userString.Split(new String[] { @":" }, StringSplitOptions.None).ToArray();
            UserModel user = new UserModel() { };
            if (userInfo.Length > 1) {
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
            }
            return user;
        }

        private static UserModel AddUserInfoFromPasswd(UserModel user, string userString) {
            Log.Logger.TraceMethod("Users Management", $"{System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName}.{System.Reflection.MethodBase.GetCurrentMethod().Name}");
            var userPasswdInfo = userString.Split(new String[] { @":" }, StringSplitOptions.None).ToArray();
            if (userPasswdInfo.Length > 0) {
                user.UID = userPasswdInfo[2];
                user.GroupID = userPasswdInfo[3];
                user.Info = userPasswdInfo[4];
                user.HomeDirectory = userPasswdInfo[5];
                user.LoginShell = userPasswdInfo[6];
            }
            return user;
        }

        private static SystemUserPassword MapPassword(string passwdString) {
            Log.Logger.TraceMethod("Users Management", $"{System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName}.{System.Reflection.MethodBase.GetCurrentMethod().Name}");
            var passwdInfo = passwdString.Split(new String[] { @"$" }, StringSplitOptions.None).ToArray();
            ;
            SystemUserPassword passwd = new SystemUserPassword() { };
            if (passwdInfo.Length > 0) {
                passwd.Type = passwdInfo[0];
                passwd.Salt = passwdInfo[1];
                passwd.Result = passwdInfo[2];
            }
            return passwd;
        }

        public static void CreateUser(string user) {
            Log.Logger.TraceMethod("Users Management", $"{System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName}.{System.Reflection.MethodBase.GetCurrentMethod().Name}");
            Terminal.Execute("useradd " + user);
        }
    }
}