using antdlib.models;
using Newtonsoft.Json;
using System.IO;
using anthilla.core;

namespace antdlib.config.shared {
    public class AppConfiguration {

        private AppConfigurationModel _model;

        private readonly string _file = $"{Parameter.AntdCfg}/app.conf";

        public AppConfiguration() {
            if(!File.Exists(_file)) {
                _model = new AppConfigurationModel();
                var text = JsonConvert.SerializeObject(_model, Formatting.Indented);
                FileWithAcl.WriteAllText(_file, text, "644", "root", "wheel");
            }
            else {
                var text = File.ReadAllText(_file);
                var obj = JsonConvert.DeserializeObject<AppConfigurationModel>(text);
                _model = obj;
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
            _model = ApiConsumer.Get<AppConfigurationModel>($"http://localhost:{_model.AntdPort}/config");
            return _model;
        }

        public void UiSave(AppConfigurationModel model) {
            var savedModel = Get();
            _model = model;
            ApiConsumer.Post2($"http://localhost:{savedModel.AntdPort}/config", ObjectExtensions.ToDictionary(_model));
        }
    }
}
