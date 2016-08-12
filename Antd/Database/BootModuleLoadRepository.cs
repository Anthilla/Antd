using System;
using System.Collections.Generic;
using System.Linq;
using antdlib.common;
using antdlib.views;
using antdlib.views.Repo;

namespace Antd.Database {
    public class BootModuleLoadRepository {

        public class RootObject {
            public string Key { get; set; }
            public List<string> Value { get; set; }
            public string Description { get; set; }
        }

        private const string ViewName = "BootModuleLoad";

        private const string ViewGuid = "9482D956-E566-4000-9607-8516288AA33D";

        public IEnumerable<BootModuleLoadSchema> GetAll() {
            var result = DatabaseRepository.Query<BootModuleLoadSchema>(AntdApplication.Database, ViewName);
            return result;
        }

        public BootModuleLoadSchema GetByGuid(string guid) {
            var result = DatabaseRepository.Query<BootModuleLoadSchema>(AntdApplication.Database, ViewName, schema => schema.Id == guid || schema.Guid == guid);
            return result.FirstOrDefault();
        }

        public bool Create(IDictionary<string, string> dict) {
            var modules = dict["Modules"];
            var obj = new BootModuleLoadModel {
                Modules = modules.SplitToList()
            };
            var result = DatabaseRepository.Save(AntdApplication.Database, obj, true);
            return result;
        }

        public bool Edit(IDictionary<string, string> dict) {
            var id = dict["Id"];
            var modules = dict["Modules"];
            var objUpdate = new BootModuleLoadModel {
                Id = id.ToGuid(),
                Modules = modules.IsNullOrEmpty() ? null : modules.SplitToList()
            };
            var result = DatabaseRepository.Edit(AntdApplication.Database, objUpdate, true);
            return result;
        }

        public bool Delete(string guid) {
            var result = DatabaseRepository.Delete<BootModuleLoadModel>(AntdApplication.Database, Guid.Parse(guid));
            return result;
        }

        public void DeleteAll() {
            var all = GetAll();
            foreach (var el in all) {
                Delete(el.Id);
            }
        }

        public bool Dump(IEnumerable<string> mods) {
            Delete(ViewGuid);
            var obj = new BootModuleLoadModel {
                Id = Guid.Parse(ViewGuid),
                Guid = ViewGuid,
                Modules = mods
            };
            var result = DatabaseRepository.Save(AntdApplication.Database, obj, true);
            return result;
        }

        public IEnumerable<string> Retrieve() {
            var result = DatabaseRepository.Query<BootModuleLoadSchema>(AntdApplication.Database, ViewName, schema => schema.Id == ViewGuid || schema.Guid == ViewGuid);
            var obj = result?.FirstOrDefault();
            return obj?.Modules.SplitToList();
        }
    }
}
