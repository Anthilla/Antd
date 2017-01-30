using System;
using System.Collections.Generic;
using System.IO;
using AntdUi.Database;

namespace AntdUi.TmplMaiRecap {
    public class MaiRecapRepository {

        private readonly string _templateDirectory = $"{Parameter.DirectoryCfg}/mai_recap";
        private readonly Actions<MaiRecapModel> _repository;

        public MaiRecapRepository() {
            Directory.CreateDirectory(_templateDirectory);
            _repository = new Actions<MaiRecapModel>(_templateDirectory);
        }

        public void Save(MaiRecapModel model) {
            _repository.Save(model);
        }

        public void Edit(MaiRecapModel model) {
            _repository.Edit(model);
        }

        public void Delete(Guid guid) {
            _repository.Remove(guid);
        }

        public IEnumerable<MaiRecapModel> Get() {
            var list = _repository.Get();
            return list;
        }

        public MaiRecapModel Get(Guid guid) {
            var element = _repository.Get(guid);
            return element;
        }
    }
}
