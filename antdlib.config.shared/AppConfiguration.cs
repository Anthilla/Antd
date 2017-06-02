using antdlib.models;
using Newtonsoft.Json;
using System;
using System.IO;
using anthilla.core;
using Parameter = antdlib.common.Parameter;

namespace antdlib.config.shared {
    public class AppConfiguration {

        private AppConfigurationModel _model;

        private readonly string _file = $"{Parameter.AntdCfg}/app.conf";

        private readonly ApiConsumer _api = new ApiConsumer();

        public AppConfiguration() {
            if(!File.Exists(_file)) {
                _model = new AppConfigurationModel();
                var text = JsonConvert.SerializeObject(_model, Formatting.Indented);
                FileWithAcl.WriteAllText(_file, text, "644", "root", "wheel");
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
            _model = model;
            FileWithAcl.WriteAllText(_file, JsonConvert.SerializeObject(_model, Formatting.Indented), "644", "root", "wheel");
        }

        public AppConfigurationModel UiGet() {
            _model = _api.Get<AppConfigurationModel>($"http://localhost:{_model.AntdPort}/config");
            return _model;
        }

        public void UiSave(AppConfigurationModel model) {
            var savedModel = Get();
            _model = model;
            _api.Post2($"http://localhost:{savedModel.AntdPort}/config", ObjectExtensions.ToDictionary(_model));
        }
    }
}
