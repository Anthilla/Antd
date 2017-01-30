using System;
using System.Collections.Generic;
using System.IO;
using AntdUi.Database;

namespace AntdUi.TmplTextFile {
    public class TextFileRepository {

        private readonly string _templateDirectory = $"{Parameter.DirectoryCfg}/text_file";
        private readonly Actions<TextFileModel> _repository;

        public TextFileRepository() {
            Directory.CreateDirectory(_templateDirectory);
            _repository = new Actions<TextFileModel>(_templateDirectory);
        }

        public void Save(TextFileModel model) {
            _repository.Save(model);
        }

        public void Edit(TextFileModel model) {
            _repository.Edit(model);
        }

        public void Delete(Guid guid) {
            _repository.Remove(guid);
        }

        public IEnumerable<TextFileModel> Get() {
            var list = _repository.Get();
            return list;
        }

        public TextFileModel Get(Guid guid) {
            var element = _repository.Get(guid);
            return element;
        }
    }
}
