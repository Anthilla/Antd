using System;
using System.Collections.Generic;
using System.Linq;
using antdlib.common;
using antdlib.views;
using antdlib.views.Repo;

namespace Antd.Database {
    public class AuthorizedKeysRepository {

        private const string ViewName = "AuthorizedKeys";

        public IEnumerable<AuthorizedKeysSchema> GetAll() {
            var result = DatabaseRepository.Query<AuthorizedKeysSchema>(Application.Database, ViewName);
            return result;
        }

        public AuthorizedKeysSchema GetByGuid(string guid) {
            var result = DatabaseRepository.Query<AuthorizedKeysSchema>(Application.Database, ViewName, schema => schema.Id == guid || schema.Guid == guid);
            return result.FirstOrDefault();
        }

        public bool Create(IDictionary<string, string> dict) {
            var remoteUser = dict["RemoteUser"];
            var user = dict["User"];
            var keyValue = dict["KeyValue"];
            var obj = new AuthorizedKeysModel {
                RemoteUser = remoteUser,
                User = user,
                KeyValue = keyValue
            };
            var result = DatabaseRepository.Save(Application.Database, obj, true);
            return result;
        }

        public bool Edit(IDictionary<string, string> dict) {
            var id = dict["Id"];
            var remoteUser = dict["RemoteUser"];
            var user = dict["User"];
            var keyValue = dict["KeyValue"];
            var objUpdate = new AuthorizedKeysModel {
                Id = id.ToGuid(),
                RemoteUser = remoteUser,
                User = user,
                KeyValue = keyValue
            };
            var result = DatabaseRepository.Edit(Application.Database, objUpdate, true);
            return result;
        }

        public bool Delete(string guid) {
            var result = DatabaseRepository.Delete<AuthorizedKeysModel>(Application.Database, Guid.Parse(guid));
            return result;
        }

        public void DeleteAll() {
            var all = GetAll();
            foreach (var el in all) {
                Delete(el.Id);
            }
        }

        public bool Create2(string remoteUser, string user, string keyValue) {
            var obj = new AuthorizedKeysModel {
                RemoteUser = remoteUser,
                User = user,
                KeyValue = keyValue
            };
            var result = DatabaseRepository.Save(Application.Database, obj, true);
            return result;
        }
    }
}
