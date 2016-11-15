using System;
using System.Collections.Generic;
using System.Linq;
using antdlib.common;
using antdlib.views;
using antdlib.views.Repo;

namespace Antd.Database {
    public class BindServerZoneRepository {

        private const string ViewName = "BindServerZone";

        public IEnumerable<BindServerZoneSchema> GetAll() {
            var result = DatabaseRepository.Query<BindServerZoneSchema>(AntdApplication.Database, ViewName);
            return result;
        }

        public BindServerZoneSchema GetByGuid(string guid) {
            var result = DatabaseRepository.Query<BindServerZoneSchema>(AntdApplication.Database, ViewName, schema => schema.Id == guid || schema.Guid == guid);
            return result.FirstOrDefault();
        }

        public BindServerZoneSchema GetByName(string name) {
            var result = DatabaseRepository.Query<BindServerZoneSchema>(AntdApplication.Database, ViewName, schema => schema.Name == name);
            return result.FirstOrDefault();
        }

        public bool Create(string name, string type, string file) {
            var obj = new BindServerZoneModel {
                Name = name,
                Type = type,
                File = file
            };
            var result = DatabaseRepository.Save(AntdApplication.Database, obj, true);
            return result;
        }

        public bool Create(string name, string type, string file, string sum, IEnumerable<string> allupd, IEnumerable<string> allqry, IEnumerable<string> alltrn) {
            var obj = new BindServerZoneModel {
                Name = name,
                Type = type,
                File = file,
                SerialUpdateMethod = sum,
                AllowUpdate = allupd.ToList(),
                AllowQuery = allqry.ToList(),
                AllowTransfer = alltrn.ToList()
            };
            var result = DatabaseRepository.Save(AntdApplication.Database, obj, true);
            return result;
        }

        public bool Edit(string id, string name, string type, string file) {
            var objUpdate = new BindServerZoneModel {
                Id = id.ToGuid(),
                Name = name,
                Type = type,
                File = file
            };
            var result = DatabaseRepository.Edit(AntdApplication.Database, objUpdate, true);
            return result;
        }

        public bool Edit(string id, string name, string type, string file, string sum, IEnumerable<string> allupd, IEnumerable<string> allqry, IEnumerable<string> alltrn) {
            var objUpdate = new BindServerZoneModel {
                Id = id.ToGuid(),
                Name = name,
                Type = type,
                File = file,
                SerialUpdateMethod = sum,
                AllowUpdate = allupd.ToList(),
                AllowQuery = allqry.ToList(),
                AllowTransfer = alltrn.ToList()
            };
            var result = DatabaseRepository.Edit(AntdApplication.Database, objUpdate, true);
            return result;
        }

        public bool Delete(string guid) {
            var result = DatabaseRepository.Delete<BindServerZoneModel>(AntdApplication.Database, Guid.Parse(guid));
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
