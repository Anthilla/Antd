using System;
using System.Collections.Generic;
using antdlib.common;

namespace Antd.Users {
    public class SystemUser {
        public class Config {
            public static void ResetPasswordForUser(string user, string password) {
                Terminal.Execute($"usermod -p '{password.Trim()}' {user}");
            }

            public static void ResetPasswordForUserStoredInDb() {
                foreach (var user in Map.GetMappedUsers()) {
                    ResetPasswordForUser(user.Alias, user.Password);
                }
            }
        }

        public class Map {
            private static IEnumerable<SystemUserModel> DefaultMappedUsers() {
                var visorPassword = Terminal.Execute("mkpasswd -m sha-512 Anthilla");
                return new List<SystemUserModel>();
            }

            public static IEnumerable<SystemUserModel> GetMappedUsers() {
                throw new NotImplementedException();
                //userList.AddRange(DefaultMappedUsers());
            }

            public static void MapUser(string userAlias, string password) {
                var model = new SystemUserModel {
                    Guid = Guid.NewGuid().ToString(),
                    Alias = userAlias,
                    Password = Terminal.Execute($"mkpasswd -m sha-512 {password}")
                };
                throw new NotImplementedException();
            }

            //public static void EditMapUser(string guid, string userAlias, string password) {
            //    throw new NotImplementedException();
            //    //model.Alias = userAlias;
            //    //model.Password = Terminal.Execute($"mkpasswd -m sha-512 {password}");
            //}

            //public static void DeleteMapUser(string guid) {
            //    throw new NotImplementedException();
            //}
        }
    }
}