using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using antdlib.common;
using antdlib.views;
using antdlib.views.Repo;
using Newtonsoft.Json;

namespace Antd.Database {
    public class ApplicationRepository {

        public class RootObject {
            public string Key { get; set; }
            public List<string> Value { get; set; }
            public string Description { get; set; }
        }

        private const string ViewName = "Application";

        public IEnumerable<ApplicationSchema> GetAll() {
            var result = DatabaseRepository.Query<ApplicationSchema>(AntdApplication.Database, ViewName);
            return result;
        }

        public ApplicationSchema GetByGuid(string guid) {
            var result = DatabaseRepository.Query<ApplicationSchema>(AntdApplication.Database, ViewName, schema => schema.Id == guid || schema.Guid == guid);
            return result.FirstOrDefault();
        }

        public ApplicationSchema GetByName(string alias) {
            var result = DatabaseRepository.Query<ApplicationSchema>(AntdApplication.Database, ViewName, schema => schema.Name == alias);
            return result.FirstOrDefault();
        }

        public bool Create(IDictionary<string, string> dict) {
            var alias = dict["Name"];
            var command = dict["Application"];
            var description = dict["Description"];
            var obj = new ApplicationModel {
                Name = alias,
                Application = command,
                Description = description,
            };
            var result = DatabaseRepository.Save(AntdApplication.Database, obj, true);
            return result;
        }

        public bool Edit(IDictionary<string, string> dict) {
            var id = dict["Id"];
            var alias = dict["Name"];
            var command = dict["Application"];
            var description = dict["Description"];
            var objUpdate = new ApplicationModel {
                Id = id.ToGuid(),
                Name = alias.IsNullOrEmpty() ? null : alias,
                Application = command.IsNullOrEmpty() ? null : command,
                Description = description.IsNullOrEmpty() ? null : description,
            };
            var result = DatabaseRepository.Edit(AntdApplication.Database, objUpdate, true);
            return result;
        }

        public bool Delete(string guid) {
            var result = DatabaseRepository.Delete<ApplicationModel>(AntdApplication.Database, Guid.Parse(guid));
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
