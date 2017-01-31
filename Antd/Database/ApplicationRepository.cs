using System;
using System.Collections.Generic;
using System.Linq;
using antdlib.common;
using antdlib.views;
using antdlib.views.Repo;

namespace Antd.Database {
    public class ApplicationRepository {

        public class RootObject {
            public string Key { get; set; }
            public List<string> Value { get; set; }
            public string Description { get; set; }
        }

        private const string ViewName = "Application";

        public IEnumerable<ApplicationSchema> GetAll() {
            var result = DatabaseRepository.Query<ApplicationSchema>(Application.Database, ViewName);
            return result;
        }

        public ApplicationSchema GetByGuid(string guid) {
            var result = DatabaseRepository.Query<ApplicationSchema>(Application.Database, ViewName, schema => schema.Id == guid || schema.Guid == guid);
            return result.FirstOrDefault();
        }

        public ApplicationSchema GetByName(string alias) {
            var result = DatabaseRepository.Query<ApplicationSchema>(Application.Database, ViewName, schema => schema.Name == alias);
            return result.FirstOrDefault();
        }

        public bool Create(IDictionary<string, string> dict) {
            var name = dict["Name"];
            var repositoryName = dict["RepositoryName"];
            var exes = dict["Exes"];
            var workingDirectories = dict["WorkingDirectories"];
            var unitPrepare = dict["UnitPrepare"];
            var unitMount = dict["UnitMount"];
            var unitLauncher = dict["UnitLauncher"];
            var obj = new ApplicationModel {
                Name = name,
                RepositoryName = repositoryName,
                Exes = exes.SplitToList(),
                WorkingDirectories = workingDirectories.SplitToList(),
                UnitPrepare = unitPrepare,
                UnitMount = unitMount,
                UnitLauncher = unitLauncher.SplitToList()
            };
            var result = DatabaseRepository.Save(Application.Database, obj, true);
            return result;
        }

        public bool Edit(IDictionary<string, string> dict) {
            var id = dict["Id"];
            var name = dict["Name"];
            var repositoryName = dict["RepositoryName"];
            var exes = dict["Exes"];
            var workingDirectories = dict["WorkingDirectories"];
            var unitPrepare = dict["UnitPrepare"];
            var unitMount = dict["UnitMount"];
            var unitLauncher = dict["UnitLauncher"];
            var objUpdate = new ApplicationModel {
                Id = id.ToGuid(),
                Name = name,
                RepositoryName = repositoryName,
                Exes = exes.SplitToList(),
                WorkingDirectories = workingDirectories.SplitToList(),
                UnitPrepare = unitPrepare,
                UnitMount = unitMount,
                UnitLauncher = unitLauncher.SplitToList()
            };
            var result = DatabaseRepository.Edit(Application.Database, objUpdate, true);
            return result;
        }

        public bool Delete(string guid) {
            var result = DatabaseRepository.Delete<ApplicationModel>(Application.Database, Guid.Parse(guid));
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
