using System;
using System.IO;
using antdlib.common;
using antdlib.models;
using Newtonsoft.Json;
using IoDir = System.IO.Directory;

namespace AntdUi.AppConfig {
    public class AppConfiguration {

        private AppConfigurationModel _model;

        private readonly string _file = $"{Parameter.DirectoryCfg}/app.conf";

        private readonly ApiConsumer _api = new ApiConsumer();

        public AppConfiguration() {
            IoDir.CreateDirectory(Parameter.DirectoryCfg);
            if(!File.Exists(_file)) {
                _model = new AppConfigurationModel();
            }
            else {
                try {
                    var text = File.ReadAllText(_file);
                    var obj = JsonConvert.DeserializeObject<AppConfigurationModel>(text);
                    _model = obj;
                }
                catch(Exception) {
                    _model = new AppConfigurationModel();
                }

            }
        }

        public AppConfigurationModel Get() {
            return _model;
        }

        public void Save(AppConfigurationModel model) {
            var savedModel = Get();
            _model = model;
            _api.Post($"http://localhost:{savedModel.AntdPort}/config", _model.ToDictionary());
        }
    }
}
