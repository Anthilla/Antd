using System;
using System.Collections.Generic;
using System.Linq;
using antdlib.common;
using antdlib.views;
using antdlib.views.Repo;

namespace Antd.Database {
    public class NftRepository {

        public class RootObject {
            public string Key { get; set; }
            public List<string> Value { get; set; }
            public string Description { get; set; }
        }

        private const string ViewName = "Nft";
        private const string ViewGuid = "D1CA7DE0-AAA2-4A2D-BAB5-DCC0CA1EBD29";

        public IEnumerable<NftSchema> GetAll() {
            var result = DatabaseRepository.Query<NftSchema>(AntdApplication.Database, ViewName);
            return result;
        }

        public NftSchema GetByGuid(string guid) {
            var result = DatabaseRepository.Query<NftSchema>(AntdApplication.Database, ViewName, schema => schema.Id == guid || schema.Guid == guid);
            return result.FirstOrDefault();
        }

        public bool Create(IDictionary<string, string> dict) {
            var configuration = dict["Configuration"];
            var obj = new NftModel {
                Id = ViewGuid.ToGuid(),
                Guid = ViewGuid,
                Configuration = configuration
            };
            var result = DatabaseRepository.Save(AntdApplication.Database, obj, true);
            return result;
        }

        public bool Edit(IDictionary<string, string> dict) {
            var configuration = dict["Configuration"];
            var objUpdate = new NftModel {
                Id = ViewGuid.ToGuid(),
                Configuration = configuration.IsNullOrEmpty() ? null : configuration
            };
            var result = DatabaseRepository.Edit(AntdApplication.Database, objUpdate, true);
            return result;
        }

        public bool Delete(string guid) {
            var result = DatabaseRepository.Delete<NftModel>(AntdApplication.Database, Guid.Parse(guid));
            return result;
        }

        public void DeleteAll() {
            var all = GetAll();
            foreach (var el in all) {
                Delete(el.Id);
            }
        }

        public bool Set(string config) {
            var obj = new NftModel {
                Id = ViewGuid.ToGuid(),
                Guid = ViewGuid,
                Configuration = config
            };
            var result = DatabaseRepository.Save(AntdApplication.Database, obj, true);
            return result;
        }

        public NftSchema Get() {
            var result = GetByGuid(ViewGuid);
            return result;
        }
    }
}
