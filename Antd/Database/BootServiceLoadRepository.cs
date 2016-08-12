using System;
using System.Collections.Generic;
using System.Linq;
using antdlib.common;
using antdlib.views;
using antdlib.views.Repo;

namespace Antd.Database {
    public class BootServiceLoadRepository {

        public class RootObject {
            public string Key { get; set; }
            public List<string> Value { get; set; }
            public string Description { get; set; }
        }

        private const string ViewName = "BootServiceLoad";

        private const string ViewGuid = "2062175F-3A83-4D60-98CE-296C7D17D759";

        public IEnumerable<BootServiceLoadSchema> GetAll() {
            var result = DatabaseRepository.Query<BootServiceLoadSchema>(AntdApplication.Database, ViewName);
            return result;
        }

        public BootServiceLoadSchema GetByGuid(string guid) {
            var result = DatabaseRepository.Query<BootServiceLoadSchema>(AntdApplication.Database, ViewName, schema => schema.Id == guid || schema.Guid == guid);
            return result.FirstOrDefault();
        }

        public bool Create(IDictionary<string, string> dict) {
            var modules = dict["Services"];
            var obj = new BootServiceLoadModel {
                Services = modules.SplitToList()
            };
            var result = DatabaseRepository.Save(AntdApplication.Database, obj, true);
            return result;
        }

        public bool Edit(IDictionary<string, string> dict) {
            var id = dict["Id"];
            var modules = dict["Services"];
            var objUpdate = new BootServiceLoadModel {
                Id = id.ToGuid(),
                Services = modules.IsNullOrEmpty() ? null : modules.SplitToList()
            };
            var result = DatabaseRepository.Edit(AntdApplication.Database, objUpdate, true);
            return result;
        }

        public bool Delete(string guid) {
            var result = DatabaseRepository.Delete<BootServiceLoadModel>(AntdApplication.Database, Guid.Parse(guid));
            return result;
        }

        public void DeleteAll() {
            var all = GetAll();
            foreach (var el in all) {
                Delete(el.Id);
            }
        }

        public bool Dump(IEnumerable<string> mods) {
            var tryGet = GetByGuid(ViewGuid);
            if (tryGet != null) {
                Delete(ViewGuid);
            }
            var obj = new BootServiceLoadModel {
                Id = Guid.Parse(ViewGuid),
                Guid = ViewGuid,
                Services = mods
            };
            var result = DatabaseRepository.Save(AntdApplication.Database, obj, true);
            return result;
        }

        public IEnumerable<string> Retrieve() {
            var result = DatabaseRepository.Query<BootServiceLoadSchema>(AntdApplication.Database, ViewName, schema => schema.Id == ViewGuid || schema.Guid == ViewGuid);
            var obj = result?.FirstOrDefault();
            return obj?.Services.SplitToList();
        }
    }
}
