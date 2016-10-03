using System.Collections.Generic;
using antdlib.views.Repo;

namespace Antd.Database {
    public class FastObjectRepository {

        public class RootObject {
            public string Key { get; set; }
            public List<string> Value { get; set; }
            public string Description { get; set; }
        }

        public Dictionary<string, object> GetAll() {
            var dict = new Dictionary<string, object>();
            var keys = DatabaseRepository.GetFastObjectKeys(AntdApplication.Database);
            foreach (var k in keys) {
                var obj = DatabaseRepository.GetFastObject(AntdApplication.Database, k);
                dict.Add(k, obj);
            }
            return dict;
        }

        public bool Create(string key, object obj) {
            var result = DatabaseRepository.SetFastObject(AntdApplication.Database, key, obj);
            return result;
        }

        //public bool Edit(IDictionary<string, string> dict) {
        //    var id = dict["Id"];
        //    var alias = dict["Name"];
        //    var command = dict["FastObject"];
        //    var description = dict["Description"];
        //    var objUpdate = new FastObjectModel {
        //        Id = id.ToGuid(),
        //        Name = alias.IsNullOrEmpty() ? null : alias,
        //        FastObject = command.IsNullOrEmpty() ? null : command,
        //        Description = description.IsNullOrEmpty() ? null : description,
        //    };
        //    var result = DatabaseRepository.Edit(AntdApplication.Database, objUpdate, true);
        //    return result;
        //}

        public bool Delete(string key) {
            var result = DatabaseRepository.DeleteFastObject(AntdApplication.Database, key);
            return result;
        }
    }
}
