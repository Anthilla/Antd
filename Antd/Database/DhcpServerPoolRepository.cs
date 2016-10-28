using System;
using System.Collections.Generic;
using System.Linq;
using antdlib.common;
using antdlib.views;
using antdlib.views.Repo;

namespace Antd.Database {
    public class DhcpServerPoolRepository {

        private const string ViewName = "DhcpServerPool";

        public IEnumerable<DhcpServerPoolSchema> GetAll() {
            var result = DatabaseRepository.Query<DhcpServerPoolSchema>(AntdApplication.Database, ViewName);
            return result;
        }

        public DhcpServerPoolSchema GetByGuid(string guid) {
            var result = DatabaseRepository.Query<DhcpServerPoolSchema>(AntdApplication.Database, ViewName, schema => schema.Id == guid || schema.Guid == guid);
            return result.FirstOrDefault();
        }

        public bool Create(List<string> options) {
            var obj = new DhcpServerPoolModel {
                Options = options
            };
            var result = DatabaseRepository.Save(AntdApplication.Database, obj, true);
            return result;
        }

        public bool Edit(string id, List<string> options) {
            var objUpdate = new DhcpServerPoolModel {
                Id = id.ToGuid(),
                Options = options
            };
            var result = DatabaseRepository.Edit(AntdApplication.Database, objUpdate, true);
            return result;
        }

        public bool Delete(string guid) {
            var result = DatabaseRepository.Delete<DhcpServerPoolModel>(AntdApplication.Database, Guid.Parse(guid));
            return result;
        }

        public void DeleteAll() {
            var all = GetAll();
            foreach (var el in all) {
                Delete(el.Id);
            }
        }
    }
}
