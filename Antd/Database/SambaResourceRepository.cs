using System;
using System.Collections.Generic;
using System.Linq;
using antdlib.common;
using antdlib.views;
using antdlib.views.Repo;

namespace Antd.Database {
    public class SambaResourceRepository {

        private const string ViewName = "SambaResource";

        public IEnumerable<SambaResourceSchema> GetAll() {
            var result = DatabaseRepository.Query<SambaResourceSchema>(AntdApplication.Database, ViewName);
            return result;
        }

        public SambaResourceSchema GetByGuid(string guid) {
            var result = DatabaseRepository.Query<SambaResourceSchema>(AntdApplication.Database, ViewName, schema => schema.Id == guid || schema.Guid == guid);
            return result.FirstOrDefault();
        }

        public SambaResourceSchema GetByName(string name) {
            var result = DatabaseRepository.Query<SambaResourceSchema>(AntdApplication.Database, ViewName, schema => schema.Name == name);
            return result.FirstOrDefault();
        }

        public bool Create(string name, string path, string comment = "") {
            var obj = new SambaResourceModel {
                Name = name,
                Path = path,
                Comment = comment
            };
            var result = DatabaseRepository.Save(AntdApplication.Database, obj, true);
            return result;
        }

        public bool Edit(string id, string name, string path, string comment = "") {
            var objUpdate = new SambaResourceModel {
                Id = id.ToGuid(),
                Name = name,
                Path = path,
                Comment = comment
            };
            var result = DatabaseRepository.Edit(AntdApplication.Database, objUpdate, true);
            return result;
        }
   
        public bool Delete(string guid) {
            var result = DatabaseRepository.Delete<SambaResourceModel>(AntdApplication.Database, Guid.Parse(guid));
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
