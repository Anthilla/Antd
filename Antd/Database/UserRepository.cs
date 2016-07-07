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

        private const string UserFile = "/etc/shadow";
        private const string UserPasswordFile = "/etc/passwd";

        public void Import() {
            if (!File.Exists(UserFile) || !File.Exists(UserPasswordFile)) {
                return;
            }
            var usersString = Terminal.Execute($"cat {UserFile}");
            var users = usersString.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var user in users) {
                Map(user);
            }
        }

        private void Map(string userString) {
            var userInfo = userString.Split(new[] { ":" }, StringSplitOptions.None).ToArray();
            if (userInfo.Length <= 8) {
                return;
            }
            var passwords = Terminal.Execute($"cat {UserPasswordFile}").SplitToList(Environment.NewLine);
            Create(new Dictionary<string, string> {
                { "FirstName", userInfo[0] },
                { "LastName", userInfo[0] },
                { "Password", $"{userInfo[1]}${passwords.FirstOrDefault(_=>_.Contains(userInfo[0]))}"},
                { "Role", "system-user" },
                { "Email", "" },
                { "CompanyGuid", "" },
                { "Projects", "" },
                { "Usergroups", "" },
                { "Resources", "" },
                { "Users", "" },
                { "Tags", "" }
            });
        }

        private static string[] MapPassword(string passwdString) {
            var passwdInfo = passwdString.Split(new[] { "$" }, StringSplitOptions.None).ToArray();
            //var passwd = new SystemUserPassword {
            //    Type = passwdInfo[0],
            //    Salt = passwdInfo[1],
            //    Result = passwdInfo[2]
            //};
            return passwdInfo;
        }

        public class Shadow {
            public static void Create(string user) {
                Terminal.Execute("useradd " + user);
            }
        }
    }
}
