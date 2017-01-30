using System;
using System.Collections.Generic;
using System.IO;
using AntdUi.Database;

namespace AntdUi.TmplRequestMaterialCleaning {
    public class RequestMaterialCleaningRepository {

        private readonly string _templateDirectory = $"{Parameter.DirectoryCfg}/request_material_cleaning";
        private readonly Actions<RequestMaterialCleaningModel> _repository;

        public RequestMaterialCleaningRepository() {
            Directory.CreateDirectory(_templateDirectory);
            _repository = new Actions<RequestMaterialCleaningModel>(_templateDirectory);
        }

        public void Save(RequestMaterialCleaningModel model) {
            _repository.Save(model);
        }

        public void Edit(RequestMaterialCleaningModel model) {
            _repository.Edit(model);
        }

        public void Delete(Guid guid) {
            _repository.Remove(guid);
        }

        public IEnumerable<RequestMaterialCleaningModel> Get() {
            var list = _repository.Get();
            return list;
        }

        public RequestMaterialCleaningModel Get(Guid guid) {
            var element = _repository.Get(guid);
            return element;
        }
    }
}
