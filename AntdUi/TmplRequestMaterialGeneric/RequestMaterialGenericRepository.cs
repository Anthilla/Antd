using System;
using System.Collections.Generic;
using System.IO;
using AntdUi.Database;

namespace AntdUi.TmplRequestMaterialGeneric {
    public class RequestMaterialGenericRepository {

        private readonly string _templateDirectory = $"{Parameter.DirectoryCfg}/request_material_generic";
        private readonly Actions<RequestMaterialGenericModel> _repository;

        public RequestMaterialGenericRepository() {
            Directory.CreateDirectory(_templateDirectory);
            _repository = new Actions<RequestMaterialGenericModel>(_templateDirectory);
        }

        public void Save(RequestMaterialGenericModel model) {
            _repository.Save(model);
        }

        public void Edit(RequestMaterialGenericModel model) {
            _repository.Edit(model);
        }

        public void Delete(Guid guid) {
            _repository.Remove(guid);
        }

        public IEnumerable<RequestMaterialGenericModel> Get() {
            var list = _repository.Get();
            return list;
        }

        public RequestMaterialGenericModel Get(Guid guid) {
            var element = _repository.Get(guid);
            return element;
        }
    }
}
