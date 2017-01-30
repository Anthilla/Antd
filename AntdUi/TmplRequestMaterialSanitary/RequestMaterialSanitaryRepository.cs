using System;
using System.Collections.Generic;
using System.IO;
using AntdUi.Database;

namespace AntdUi.TmplRequestMaterialSanitary {
    public class RequestMaterialSanitaryRepository {

        private readonly string _templateDirectory = $"{Parameter.DirectoryCfg}/request_material_sanitary";
        private readonly Actions<RequestMaterialSanitaryModel> _repository;

        public RequestMaterialSanitaryRepository() {
            Directory.CreateDirectory(_templateDirectory);
            _repository = new Actions<RequestMaterialSanitaryModel>(_templateDirectory);
        }

        public void Save(RequestMaterialSanitaryModel model) {
            _repository.Save(model);
        }

        public void Edit(RequestMaterialSanitaryModel model) {
            _repository.Edit(model);
        }

        public void Delete(Guid guid) {
            _repository.Remove(guid);
        }

        public IEnumerable<RequestMaterialSanitaryModel> Get() {
            var list = _repository.Get();
            return list;
        }

        public RequestMaterialSanitaryModel Get(Guid guid) {
            var element = _repository.Get(guid);
            return element;
        }
    }
}
