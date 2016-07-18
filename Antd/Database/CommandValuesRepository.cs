using System;
using System.Collections.Generic;
using System.Linq;
using antdlib.common;
using antdlib.views;
using antdlib.views.Repo;

namespace Antd.Database {
    public class CommandValuesRepository {
        private const string ViewName = "CommandValues";

        public IEnumerable<CommandValuesSchema> GetAll() {
            var result = DatabaseRepository.Query<CommandValuesSchema>(AntdApplication.Database, ViewName);
            return result;
        }

        public CommandValuesSchema GetByGuid(string guid) {
            var result = DatabaseRepository.Query<CommandValuesSchema>(AntdApplication.Database, ViewName, schema => schema.Id == guid || schema.Guid == guid);
            return result.FirstOrDefault();
        }

        public CommandValuesSchema GetByName(string name) {
            var result = DatabaseRepository.Query<CommandValuesSchema>(AntdApplication.Database, ViewName, schema => schema.Name == name);
            return result.FirstOrDefault();
        }

        public bool Create(IDictionary<string, string> dict) {
            var name = dict["Name"];
            var value = dict["Value"];
            var obj = new CommandValuesModel {
                Name = name,
                Value = value
            };
            var result = DatabaseRepository.Save(AntdApplication.Database, obj, true);
            return result;
        }

        public bool Edit(IDictionary<string, string> dict) {
            var id = dict["Id"];
            var name = dict["Name"];
            var value = dict["Value"];
            var objUpdate = new CommandValuesModel {
                Id = id.ToGuid(),
                Name = name.IsNullOrEmpty() ? null : name,
                Value = value.IsNullOrEmpty() ? null : value
            };
            var result = DatabaseRepository.Edit(AntdApplication.Database, objUpdate, true);
            return result;
        }

        public bool Delete(string guid) {
            var result = DatabaseRepository.Delete<CommandValuesModel>(AntdApplication.Database, Guid.Parse(guid));
            return result;
        }
    }
}
