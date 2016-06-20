using System;
using System.Collections.Generic;
using System.Linq;
using antdlib.common;
using antdlib.views;
using antdlib.views.Repo;

namespace Antd.Database {
    public class ObjectRepository {
        private const string ViewName = "Object";

        public IEnumerable<ObjectSchema> GetAll() {
            var result = DatabaseRepository.Query<ObjectSchema>(AntdApplication.Database, ViewName);
            return result;
        }

        public ObjectSchema GetByGuid(string guid) {
            var result = DatabaseRepository.Query<ObjectSchema>(AntdApplication.Database, ViewName, schema => schema.Id == guid || schema.Guid == guid);
            return result.FirstOrDefault();
        }

        public bool Create(IDictionary<string, string> dict) {
            var guid = dict["Guid"];
            var label = dict["Label"];
            var value = dict["Value"];
            var obj = new ObjectModel {
                Guid = guid,
                Label = label,
                Value = value
            };
            var result = DatabaseRepository.Save(AntdApplication.Database, obj, true);
            return result;
        }

        public bool Edit(IDictionary<string, string> dict) {
            var id = dict["Id"];
            var guid = dict["Guid"];
            var label = dict["Label"];
            var value = dict["Value"];
            var objUpdate = new ObjectModel {
                Id = id.ToGuid(),
                Guid = guid.IsNullOrEmpty() ? null : guid,
                Label = label.IsNullOrEmpty() ? null : label,
                Value = value.IsNullOrEmpty() ? null : value
            };
            var result = DatabaseRepository.Edit(AntdApplication.Database, objUpdate, true);
            return result;
        }

        public bool Delete(string guid) {
            var result = DatabaseRepository.Delete<ObjectModel>(AntdApplication.Database, Guid.Parse(guid));
            return result;
        }
    }
}
