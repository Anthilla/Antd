using System;
using System.Collections.Generic;
using System.Linq;
using antdlib.common;
using antdlib.views;
using antdlib.views.Repo;

namespace Antd.Database {
    public class CustomTableRepository {
        private const string ViewName = "CustomTable";

        public IEnumerable<CustomTableSchema> GetAll() {
            var result = DatabaseRepository.Query<CustomTableSchema>(AntdApplication.Database, ViewName);
            return result;
        }

        public CustomTableSchema GetByGuid(string guid) {
            var result = DatabaseRepository.Query<CustomTableSchema>(AntdApplication.Database, ViewName, schema => schema.Id == guid || schema.Guid == guid);
            return result.FirstOrDefault();
        }

        public bool Create(IDictionary<string, string> dict) {
            var alias = dict["Alias"];
            var context = dict["Context"];
            var type = dict["Type"];
            var content = dict["Content"];
            var obj = new CustomTableModel {
                Alias = alias,
                Context = context,
                Type = (CustomTableType)Enum.Parse(typeof(CustomTableType), type),
                Content = content.SplitToList()
            };
            var result = DatabaseRepository.Save(AntdApplication.Database, obj, true);
            return result;
        }

        public bool Edit(IDictionary<string, string> dict) {
            var id = dict["Id"];
            var alias = dict["Alias"];
            var context = dict["Context"];
            var type = dict["Type"];
            var content = dict["Content"];
            var objUpdate = new CustomTableModel {
                Id = id.ToGuid(),
                Alias = alias.IsNullOrEmpty() ? null : alias,
                Context = context.IsNullOrEmpty() ? null : context,
                Type = type.IsNullOrEmpty() ? CustomTableType.None : (CustomTableType)Enum.Parse(typeof(CustomTableType), type),
                Content = content.IsNullOrEmpty() ? new List<string>() : content.SplitToList(),
            };
            var result = DatabaseRepository.Edit(AntdApplication.Database, objUpdate, true);
            return result;
        }

        public bool Delete(string guid) {
            var result = DatabaseRepository.Delete<CustomTableModel>(AntdApplication.Database, Guid.Parse(guid));
            return result;
        }
    }
}
