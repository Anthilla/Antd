using System;
using System.Collections.Generic;
using System.Linq;
using antdlib.Common;
using antdlib.MountPoint;
using DeNSo;

namespace antdlib.Users {
    public class SystemUser {
        private const string File = "/etc/shadow";
        private const string FilePwd = "/etc/passwd";
        private static readonly string MntFile = Mount.SetFilesPath(File);
        private static readonly string MntFilePwd = Mount.SetFilesPath(FilePwd);

        public static bool IsActive => CheckIsActive();

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

        public static IEnumerable<UserModel> GetAll() {
            try {
                var list = new List<UserModel>();
                if (!System.IO.File.Exists(File) || !System.IO.File.Exists(FilePwd))
                    return list;
                var usersString = Terminal.Execute($"cat {File}");
                var users =
                    usersString.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries).ToArray();
                var passwdUserString = Terminal.Execute($"cat {FilePwd}");
                var passwdUsers =
                    passwdUserString.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries).ToArray();
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
            var passwdUsers =
                passwdUserString.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries).ToArray();
            foreach (var user in users) {
                var mu = MapUser(user);
                var pwdstring = passwdUsers.FirstOrDefault(s => s.Contains(mu.Alias));
                if (!string.IsNullOrEmpty(pwdstring)) {
                    var mup = AddUserInfoFromPasswd(mu, pwdstring);
                    mu = mup;
                }
                Session.New.Set(mu);
            }
        }

        public static IEnumerable<UserModel> GetAllFromDatabase() {
            var users = Session.New.Get<UserModel>().ToList();
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
            var passwdInfo = passwdString.Split(new[] { "$" }, StringSplitOptions.None).ToArray();
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

        public class Config {
            public static void ResetPasswordForUser(string user, string password) {
                var hashedPassword = Terminal.Execute($"mkpasswd -m sha-512 {password}");
                if (hashedPassword.Length <= 0)
                    return;
                Terminal.Execute($"usermod -p '{hashedPassword.Trim()}' {user}");
            }
        }
    }
}