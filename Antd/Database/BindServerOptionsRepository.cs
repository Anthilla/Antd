using System;
using System.Collections.Generic;
using System.Linq;
using antdlib.views;
using antdlib.views.Repo;

namespace Antd.Database {
    public class BindServerOptionsRepository {

        private const string ConfigGuid = "70185A84-6D7E-45F1-B915-493141F6C526";
        private const string ViewName = "BindServerOptions";

        public BindServerOptionsModel Default = new BindServerOptionsModel();

        public IEnumerable<BindServerOptionsSchema> GetAll() {
            var result = DatabaseRepository.Query<BindServerOptionsSchema>(AntdApplication.Database, ViewName);
            return result;
        }

        public BindServerOptionsSchema Get() {
            var result = DatabaseRepository.Query<BindServerOptionsSchema>(AntdApplication.Database, ViewName, schema => schema.Id == ConfigGuid || schema.Guid == ConfigGuid);
            return result.FirstOrDefault();
        }

        public bool IsEnabled() {
            return Get() != null;
        }

        public bool Set(BindServerOptionsModel obj) {
            var tryget = Get();
            if (tryget != null) {
                Delete();
            }
            obj.Id = Guid.Parse(ConfigGuid);
            obj.Guid = ConfigGuid;
            var result = DatabaseRepository.Save(AntdApplication.Database, obj, true);
            return result;
        }

        public bool Delete() {
            var result = DatabaseRepository.Delete<BindServerOptionsModel>(AntdApplication.Database, Guid.Parse(ConfigGuid));
            return result;
        }
    }
}
