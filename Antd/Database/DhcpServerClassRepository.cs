using System;
using System.Collections.Generic;
using System.Linq;
using antdlib.common;
using antdlib.views;
using antdlib.views.Repo;

namespace Antd.Database {
    public class DhcpServerClassRepository {

        private const string ViewName = "DhcpServerClass";

        public IEnumerable<DhcpServerClassSchema> GetAll() {
            var result = DatabaseRepository.Query<DhcpServerClassSchema>(AntdApplication.Database, ViewName);
            return result;
        }

        public DhcpServerClassSchema GetByGuid(string guid) {
            var result = DatabaseRepository.Query<DhcpServerClassSchema>(AntdApplication.Database, ViewName, schema => schema.Id == guid || schema.Guid == guid);
            return result.FirstOrDefault();
        }

        public bool Create(string name, string macVendor) {
            var obj = new DhcpServerClassModel {
                Name = name,
                MacVendor = macVendor
            };
            var result = DatabaseRepository.Save(AntdApplication.Database, obj, true);
            return result;
        }

        public bool Edit(string id, string name, string macVendor) {
            var objUpdate = new DhcpServerClassModel {
                Id = id.ToGuid(),
                Name = name,
                MacVendor = macVendor
            };
            var result = DatabaseRepository.Edit(AntdApplication.Database, objUpdate, true);
            return result;
        }

        public bool Delete(string guid) {
            var result = DatabaseRepository.Delete<DhcpServerClassModel>(AntdApplication.Database, Guid.Parse(guid));
            return result;
        }
    }
}
