using System;
using System.Collections.Generic;
using System.Linq;
using antdlib.common;
using antdlib.views;
using antdlib.views.Repo;

namespace Antd.Database {
    public class BindServerZoneFileRepository {

        private const string ViewName = "BindServerZoneFile";

        public IEnumerable<BindServerZoneFileSchema> GetAll() {
            var result = DatabaseRepository.Query<BindServerZoneFileSchema>(AntdApplication.Database, ViewName);
            return result;
        }

        public BindServerZoneFileSchema GetByGuid(string guid) {
            var result = DatabaseRepository.Query<BindServerZoneFileSchema>(AntdApplication.Database, ViewName, schema => schema.Id == guid || schema.Guid == guid);
            return result.FirstOrDefault();
        }

        public bool Create(string name, string config) {
            var obj = new BindServerZoneFileModel {
                Name = name,
                Configuration = config
            };
            var result = DatabaseRepository.Save(AntdApplication.Database, obj, true);
            return result;
        }

        public bool Edit(string id, string name, string config) {
            var objUpdate = new BindServerZoneFileModel {
                Id = id.ToGuid(),
                Name = name,
                Configuration = config
            };
            var result = DatabaseRepository.Edit(AntdApplication.Database, objUpdate, true);
            return result;
        }

        public bool Delete(string guid) {
            var result = DatabaseRepository.Delete<BindServerZoneFileModel>(AntdApplication.Database, Guid.Parse(guid));
            return result;
        }

        public void DeleteAll() {
            var all = GetAll();
            foreach(var el in all) {
                Delete(el.Id);
            }
        }
    }
}
