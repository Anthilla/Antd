using System;
using System.Collections.Generic;
using System.Linq;
using antdlib.common;
using antdlib.views;
using antdlib.views.Repo;

namespace Antd.Database {
    public class BootOsParametersLoadRepository {

        public class RootObject {
            public string Key { get; set; }
            public List<string> Value { get; set; }
            public string Description { get; set; }
        }

        private const string ViewName = "BootOsParametersLoad";

        private const string ViewGuid = "2F879053-30AF-4E9F-9F51-D78F534467EB";

        public IEnumerable<BootOsParametersLoadSchema> GetAll() {
            var result = DatabaseRepository.Query<BootOsParametersLoadSchema>(AntdApplication.Database, ViewName);
            return result;
        }

        public BootOsParametersLoadSchema GetByGuid(string guid) {
            var result = DatabaseRepository.Query<BootOsParametersLoadSchema>(AntdApplication.Database, ViewName, schema => schema.Id == guid || schema.Guid == guid);
            return result.FirstOrDefault();
        }

        public bool Create(IDictionary<string, string> dict) {
            var values = dict["Values"];
            var obj = new BootOsParametersLoadModel {
                Values = values.ToObject<Dictionary<string, string>>()
            };
            var result = DatabaseRepository.Save(AntdApplication.Database, obj, true);
            return result;
        }

        public bool Edit(IDictionary<string, string> dict) {
            var id = dict["Id"];
            var values = dict["Values"];
            var objUpdate = new BootOsParametersLoadModel {
                Id = id.ToGuid(),
                Values = values.IsNullOrEmpty() ? null : values.ToObject<Dictionary<string, string>>()
            };
            var result = DatabaseRepository.Edit(AntdApplication.Database, objUpdate, true);
            return result;
        }

        public bool Delete(string guid) {
            var result = DatabaseRepository.Delete<BootOsParametersLoadModel>(AntdApplication.Database, Guid.Parse(guid));
            return result;
        }

        public void DeleteAll() {
            var all = GetAll();
            foreach (var el in all) {
                Delete(el.Id);
            }
        }

        public bool Dump(Dictionary<string, string> values) {
            var tryGet = GetByGuid(ViewGuid);
            if (tryGet != null) {
                Delete(ViewGuid);
            }
            var obj = new BootOsParametersLoadModel {
                Id = Guid.Parse(ViewGuid),
                Guid = ViewGuid,
                Values = values
            };
            var result = DatabaseRepository.Save(AntdApplication.Database, obj, true);
            return result;
        }

        public Dictionary<string, string> Retrieve() {
            var result = DatabaseRepository.Query<BootOsParametersLoadSchema>(AntdApplication.Database, ViewName, schema => schema.Id == ViewGuid || schema.Guid == ViewGuid);
            var obj = result?.FirstOrDefault();
            return obj?.Values.ToObject<Dictionary<string, string>>();
        }
    }
}
