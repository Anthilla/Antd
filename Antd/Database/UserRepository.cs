using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using antdlib.common;
using antdlib.views;
using antdlib.views.Repo;

namespace Antd.Database {
    public class UserRepository {
        private const string ViewName = "User";

        #region Private Methods
        private static string ShortenUserName(string name) {
            return name.Trim().Replace(" ", "").Substring(0, 3);
        }

        private static string UserAlias(string firstName, string lastName) {
            var result = DatabaseRepository.Query<UserSchema>(AntdApplication.Database, ViewName).Select(_ => _.Alias).ToList();
            var stringAlias = ShortenUserName(firstName) + ShortenUserName(lastName);
            var lastAlias = result.Where(_ => _.Contains(stringAlias)).OrderBy(_ => _).LastOrDefault();
            if (!result.Contains(stringAlias + "01") || lastAlias == null) {
                return stringAlias + "01";
            }
            var newNumber = (Convert.ToInt32(lastAlias.Substring(6, 2)) + 1).ToString("D2");
            return stringAlias + newNumber;
        }
        #endregion

        public IEnumerable<UserSchema> GetAll() {
            var result = DatabaseRepository.Query<UserSchema>(AntdApplication.Database, ViewName);
            return result;
        }

        public UserSchema GetByGuid(string guid) {
            var result = DatabaseRepository.Query<UserSchema>(AntdApplication.Database, ViewName, schema => schema.Id == guid || schema.Guid == guid);
            return result.FirstOrDefault();
        }

        public UserSchema GetByAlias(string alias) {
            var result = DatabaseRepository.Query<UserSchema>(AntdApplication.Database, ViewName, schema => schema.Alias == alias);
            return result.FirstOrDefault();
        }

        public bool Create(IDictionary<string, string> dict) {
            var guid = dict["Guid"];
            var firstName = dict["FirstName"];
            var lastName = dict["LastName"];
            var password = dict["Password"];
            var role = dict["Role"];
            var email = dict["Email"];
            var companyGuid = dict["CompanyGuid"];
            var projectGuids = dict["Projects"];
            var usergroupGuids = dict["Usergroups"];
            var resourceGuids = dict["Resources"];
            var userGuids = dict["Users"];
            var tags = dict["Tags"];
            var userAlias = UserAlias(firstName, lastName);
            var obj = new UserModel {
                Guid = guid,
                FirstName = firstName,
                LastName = lastName,
                Alias = userAlias,
                Password = Encryption.XHash(password),
                Role = role,
                Email = email,
                CompanyGuid = companyGuid,
                TokenGuid = "null",
                ProjectGuids = string.IsNullOrEmpty(projectGuids) ? new List<string>() : projectGuids.SplitToList(),
                UsergroupGuids = string.IsNullOrEmpty(usergroupGuids) ? new List<string>() : usergroupGuids.SplitToList(),
                ResourceGuids = string.IsNullOrEmpty(resourceGuids) ? new List<string>() : resourceGuids.SplitToList(),
                UserGuids = string.IsNullOrEmpty(userGuids) ? new List<string>() : userGuids.SplitToList(),
                IsInsider = true,
                IsEnabled = true,
                Tags = string.IsNullOrEmpty(tags) ? new List<string>() : tags.SplitToList()
            };
            var result = DatabaseRepository.Save(AntdApplication.Database, obj, true);
            return result;
        }

        public bool Edit(IDictionary<string, string> dict) {
            var id = dict["Id"];
            var guid = dict["Guid"];
            var firstName = dict["FirstName"];
            var lastName = dict["LastName"];
            var password = dict["Password"];
            var role = dict["Role"];
            var email = dict["Email"];
            var companyGuid = dict["CompanyGuid"];
            var projectGuids = dict["Projects"];
            var usergroupGuids = dict["Usergroups"];
            var resourceGuids = dict["Resources"];
            var userGuids = dict["Users"];
            var tags = dict["Tags"];
            var tokenGuid = dict["Token"];
            var isInsider = dict["Insider"].ToBoolean();
            var isEnabled = dict["Enabled"].ToBoolean();
            var userAlias = UserAlias(firstName, lastName);
            var objUpdate = new UserModel {
                Id = id.ToGuid(),
                Guid = guid.IsNullOrEmpty() ? null : guid,
                FirstName = firstName.IsNullOrEmpty() ? null : firstName,
                LastName = lastName.IsNullOrEmpty() ? null : lastName,
                Alias = userAlias,
                Password = password.IsNullOrEmpty() ? null : Encryption.XHash(password),
                Role = role.IsNullOrEmpty() ? null : role,
                Email = email.IsNullOrEmpty() ? null : email,
                CompanyGuid = companyGuid.IsNullOrEmpty() ? null : companyGuid,
                TokenGuid = tokenGuid.IsNullOrEmpty() ? null : tokenGuid,
                ProjectGuids = string.IsNullOrEmpty(projectGuids) ? new List<string>() : projectGuids.SplitToList(),
                UsergroupGuids = string.IsNullOrEmpty(usergroupGuids) ? new List<string>() : usergroupGuids.SplitToList(),
                ResourceGuids = string.IsNullOrEmpty(resourceGuids) ? new List<string>() : resourceGuids.SplitToList(),
                UserGuids = string.IsNullOrEmpty(userGuids) ? new List<string>() : userGuids.SplitToList(),
                IsInsider = isInsider.ToString().IsNullOrEmpty() ? null : isInsider,
                IsEnabled = isEnabled.ToString().IsNullOrEmpty() ? null : isEnabled,
                Tags = string.IsNullOrEmpty(tags) ? new List<string>() : tags.SplitToList()
            };
            var result = DatabaseRepository.Edit(AntdApplication.Database, objUpdate, true);
            return result;
        }

        public bool Delete(string guid) {
            var result = DatabaseRepository.Delete<UserModel>(AntdApplication.Database, Guid.Parse(guid));
            return result;
        }

        public void DeleteAll() {
            foreach (var u in GetAll()) {
                Delete(u.Guid);
            }
        }

        private const string EtcPasswd = "/etc/passwd";
        private const string EtcShadow = "/etc/shadow";

        public Dictionary<string, string> Import() {
            DeleteAll();
            var users = File.ReadAllLines(EtcPasswd);
            var passwords = File.ReadAllLines(EtcShadow);
            var sysUsers = new Dictionary<string, string>();
            foreach (var user in users) {
                var u = Map(user, passwords);
                if (u.Key == null)
                    continue;
                if (u.Value != null) {
                    sysUsers.Add(u.Key, u.Value);
                }
            }
            return sysUsers;
        }

        private KeyValuePair<string, string> Map(string userLine, IEnumerable<string> passwords) {
            var userInfo = userLine.Split(new[] { ":" }, StringSplitOptions.None).ToArray();
            if (userInfo.Length <= 2) {
                return new KeyValuePair<string, string>("", "");
            }
            var tryGet = GetByAlias(userInfo[0]);
            if (tryGet != null) {
                return new KeyValuePair<string, string>(tryGet.Alias, tryGet.Password);
            }
            var user = userInfo[0];
            if (user.ToLower().Contains("root")) {
                return new KeyValuePair<string, string>(null, null);
            }
            var passwordLine = passwords.FirstOrDefault(_ => _.Contains(user));
            var passwordInfo = passwordLine?.Split(new[] { ":" }, StringSplitOptions.None).ToArray();
            var password = string.IsNullOrEmpty(passwordInfo?[1]) ? "" : passwordInfo[1];
            if (string.IsNullOrEmpty(password.Trim().RemoveWhiteSpace())) {
                return new KeyValuePair<string, string>(null, null);
            }
            FastCreate(user, password);
            ConsoleLogger.Log($"imported {userInfo[0]} into Database");
            return new KeyValuePair<string, string>(user, password);
        }

        public bool FastCreate(string alias, string password) {
            var obj = new UserModel {
                Guid = Guid.NewGuid().ToString(),
                Alias = alias,
                Password = password,
                Role = "system-user"
            };
            var result = DatabaseRepository.Save(AntdApplication.Database, obj, true);
            return result;
        }

        //private static string[] MapPassword(string passwdString) {
        //    var passwdInfo = passwdString.Split(new[] { "$" }, StringSplitOptions.None).ToArray();
        //    //var passwd = new SystemUserPassword {
        //    //    Type = passwdInfo[0],
        //    //    Salt = passwdInfo[1],
        //    //    Result = passwdInfo[2]
        //    //};
        //    return passwdInfo;
        //}

        public class Shadow {
            public static void Create(string user) {
                Terminal.Execute("useradd " + user);
            }
        }
    }
}
