using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using antdlib.common;
using antdlib.views;
using antdlib.views.Repo;

namespace Antd.Database {
    public class SshKeyRepository {

        private const string ViewName = "SshKey";

        public IEnumerable<SshKeySchema> GetAll() {
            var result = DatabaseRepository.Query<SshKeySchema>(AntdApplication.Database, ViewName);
            return result;
        }

        public SshKeySchema GetByGuid(string guid) {
            var result = DatabaseRepository.Query<SshKeySchema>(AntdApplication.Database, ViewName, schema => schema.Id == guid || schema.Guid == guid);
            return result.FirstOrDefault();
        }

        public SshKeySchema GetByValue(string value) {
            var result = DatabaseRepository.Query<SshKeySchema>(AntdApplication.Database, ViewName, schema => schema.Value == value);
            return result.FirstOrDefault();
        }

        public IEnumerable<SshKeySchema> GetByUser(string user) {
            var result = DatabaseRepository.Query<SshKeySchema>(AntdApplication.Database, ViewName, schema => schema.User == user);
            return result;
        }

        public SshKeySchema GetByPublicUser(string user, string value) {
            var result = DatabaseRepository.Query<SshKeySchema>(AntdApplication.Database, ViewName, schema => schema.User == user && schema.Value == value);
            return result.FirstOrDefault();
        }

        private static IEnumerable<string[]> GetAuthorizedKeys() {
            var ak = new List<string[]>();
            var userDirs = Directory.EnumerateDirectories(Parameter.Home);
            foreach (var ud in userDirs) {
                var auk = $"{ud}/.ssh/authorized_keys";
                var list = new List<string>();
                if (File.Exists(auk)) {
                    list.Add(Path.GetDirectoryName(ud));
                    foreach (var line in File.ReadAllLines(auk)) {
                        var split = line.Split(' ').ToList();
                        list.AddRange(split);
                    }
                    ak.Add(list.ToArray());
                }
            }
            return ak;
        }

        public void Import() {
            try {
                var keys = GetAuthorizedKeys();
                foreach (var k in keys) {
                    var tryGet = GetByPublicUser(k[3], k[2]);
                    if (tryGet == null) {
                        var obj = new SshKeyModel {
                            User = k[0],
                            PublicUser = k[3],
                            Type = k[1],
                            Value = k[2]
                        };
                        var ch = DatabaseRepository.Save(AntdApplication.Database, obj, true);
                    }
                }
            }
            catch (Exception ex) {
                ConsoleLogger.Warn(ex);
            }
        }

        public bool Create(IDictionary<string, string> dict) {
            var user = dict["User"];
            var publicUser = dict["PublicUser"];
            var type = dict["Type"];
            var value = dict["Value"];
            var obj = new SshKeyModel {
                User = user,
                PublicUser = publicUser,
                Type = type,
                Value = value
            };
            var result = DatabaseRepository.Save(AntdApplication.Database, obj, true);
            return result;
        }

        public bool Edit(IDictionary<string, string> dict) {
            var id = dict["Id"];
            var user = dict["User"];
            var publicUser = dict["PublicUser"];
            var type = dict["Type"];
            var value = dict["Value"];
            var objUpdate = new SshKeyModel {
                Id = id.ToGuid(),
                User = user.IsNullOrEmpty() ? null : user,
                PublicUser = publicUser.IsNullOrEmpty() ? null : publicUser,
                Type = type.IsNullOrEmpty() ? null : type,
                Value = value.IsNullOrEmpty() ? null : value,
            };
            var result = DatabaseRepository.Edit(AntdApplication.Database, objUpdate, true);
            return result;
        }

        public bool Delete(string guid) {
            var result = DatabaseRepository.Delete<SshKeyModel>(AntdApplication.Database, Guid.Parse(guid));
            return result;
        }

        public void DeleteAll() {
            var all = GetAll();
            foreach (var el in all) {
                Delete(el.Id);
            }
        }
    }
}
