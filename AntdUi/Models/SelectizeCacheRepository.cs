using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AntdUi.Database;

namespace AntdUi.Models {
    public class SelectizeCacheRepository {

        private readonly string _templateDirectory = $"{antdlib.common.Parameter.AntdCfg}/cache";
        private readonly Actions<SelectizeCacheModel> _repository;

        public SelectizeCacheRepository() {
            Directory.CreateDirectory(_templateDirectory);
            _repository = new Actions<SelectizeCacheModel>(_templateDirectory);
        }

        public void Save(string key, string value) {
            var model = new SelectizeCacheModel {
                Guid = Guid.NewGuid(),
                Key = key,
                Value = value
            };
            var check = _repository.Get(_ => _.Key == key && _.Value == value).FirstOrDefault();
            if(check == null) {
                _repository.Save(model);
            }
        }

        public void Edit(SelectizeCacheModel model) {
            _repository.Edit(model);
        }

        public void Delete(Guid guid) {
            _repository.Remove(guid);
        }

        public IEnumerable<SelectizeCacheModel> Get() {
            var list = _repository.Get();
            return list;
        }

        public IEnumerable<SelectizeCacheModel> Get(string key) {
            var list = _repository.Get().Where(_ => _.Key == key);
            return list;
        }

        public SelectizeCacheModel Get(Guid guid) {
            var element = _repository.Get(guid);
            return element;
        }
    }
}
