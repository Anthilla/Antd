using System;
using System.Collections.Generic;
using System.Linq;
using antdlib.views;
using antdlib.views.Repo;

namespace Antd.Database {
    public class DhcpServerSubnetRepository {

        private const string ConfigGuid = "DE3C82F5-4B49-45AF-B72B-5B5558A8F9F1";
        private const string ViewName = "DhcpServerSubnet";

        public IEnumerable<DhcpServerSubnetSchema> GetAll() {
            var result = DatabaseRepository.Query<DhcpServerSubnetSchema>(AntdApplication.Database, ViewName);
            return result;
        }

        public DhcpServerSubnetSchema Get() {
            var result = DatabaseRepository.Query<DhcpServerSubnetSchema>(AntdApplication.Database, ViewName, schema => schema.Id == ConfigGuid || schema.Guid == ConfigGuid);
            return result.FirstOrDefault();
        }

        public bool IsEnabled() {
            return Get() != null;
        }

        public bool Set(DhcpServerSubnetModel obj) {
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
            var result = DatabaseRepository.Delete<DhcpServerSubnetModel>(AntdApplication.Database, Guid.Parse(ConfigGuid));
            return result;
        }
    }
}
