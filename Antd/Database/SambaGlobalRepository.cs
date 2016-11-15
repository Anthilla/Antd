using System;
using System.Collections.Generic;
using System.Linq;
using antdlib.views;
using antdlib.views.Repo;

namespace Antd.Database {
    public class SambaGlobalRepository {

        private const string ConfigGuid = "6D6F0C15-4C5C-4D69-9E84-1CCD38FAFC6A";
        private const string ViewName = "SambaGlobal";

        public IEnumerable<SambaGlobalSchema> GetAll() {
            var result = DatabaseRepository.Query<SambaGlobalSchema>(AntdApplication.Database, ViewName);
            return result;
        }

        public SambaGlobalSchema Get() {
            var result = DatabaseRepository.Query<SambaGlobalSchema>(AntdApplication.Database, ViewName, schema => schema.Id == ConfigGuid || schema.Guid == ConfigGuid);
            return result.FirstOrDefault();
        }

        public bool IsEnabled() {
            return Get() != null;
        }

        public bool Set(SambaGlobalModel obj) {
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
            var result = DatabaseRepository.Delete<SambaGlobalModel>(AntdApplication.Database, Guid.Parse(ConfigGuid));
            return result;
        }
    }
}
