using System;
using System.Collections.Generic;
using System.Linq;
using antdlib.common;
using antdlib.views;
using antdlib.views.Repo;

namespace Antd.Database {
    public class BindConfigRepository {

        public class RootObject {
            public string Key { get; set; }
            public List<string> Value { get; set; }
            public string Description { get; set; }
        }

        private const string ViewName = "BindConfig";

        public IEnumerable<BindConfigSchema> GetAll() {
            var result = DatabaseRepository.Query<BindConfigSchema>(AntdApplication.Database, ViewName);
            return result;
        }

        public BindConfigSchema GetByGuid(string guid) {
            var result = DatabaseRepository.Query<BindConfigSchema>(AntdApplication.Database, ViewName, schema => schema.Id == guid || schema.Guid == guid);
            return result.FirstOrDefault();
        }

        public bool Create(IDictionary<string, string> dict) {
            var config = dict["Config"];
            var obj = new BindConfigModel {
                Config = config
            };
            var result = DatabaseRepository.Save(AntdApplication.Database, obj, true);
            return result;
        }

        public bool Edit(IDictionary<string, string> dict) {
            var id = dict["Id"];
            var config = dict["Config"];
            var objUpdate = new BindConfigModel {
                Id = id.ToGuid(),
                Config = config.IsNullOrEmpty() ? null : config
            };
            var result = DatabaseRepository.Edit(AntdApplication.Database, objUpdate, true);
            return result;
        }

        public bool Delete(string guid) {
            var result = DatabaseRepository.Delete<BindConfigModel>(AntdApplication.Database, Guid.Parse(guid));
            return result;
        }

        public void DeleteAll() {
            var all = GetAll();
            foreach (var el in all) {
                Delete(el.Id);
            }
        }

        public bool Dump(string id, string config) {
            Delete(id);
            var obj = new BindConfigModel {
                Id = Guid.Parse(id),
                Guid = id,
                Config = config
            };
            var result = DatabaseRepository.Save(AntdApplication.Database, obj, true);
            return result;
        }
    }
}
