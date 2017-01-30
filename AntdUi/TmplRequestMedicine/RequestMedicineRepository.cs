using System;
using System.Collections.Generic;
using System.IO;
using AntdUi.Database;

namespace AntdUi.TmplRequestMedicine {
    public class RequestMedicineRepository {

        private readonly string _templateDirectory = $"{Parameter.DirectoryCfg}/request_medicine";
        private readonly Actions<RequestMedicineModel> _repository;

        public RequestMedicineRepository() {
            Directory.CreateDirectory(_templateDirectory);
            _repository = new Actions<RequestMedicineModel>(_templateDirectory);
        }

        public void Save(RequestMedicineModel model) {
            _repository.Save(model);
        }

        public void Edit(RequestMedicineModel model) {
            _repository.Edit(model);
        }

        public void Delete(Guid guid) {
            _repository.Remove(guid);
        }

        public IEnumerable<RequestMedicineModel> Get() {
            var list = _repository.Get();
            return list;
        }

        public RequestMedicineModel Get(Guid guid) {
            var element = _repository.Get(guid);
            return element;
        }
    }
}
