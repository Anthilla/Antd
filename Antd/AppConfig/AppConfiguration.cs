using System;
using System.IO;
using antdlib.common;
using AntdUi.AppConfig;
using Newtonsoft.Json;
using IoDir = System.IO.Directory;

namespace Antd.AppConfig {
    public class AppConfiguration {

        private AppConfigurationModel _model;

        private readonly string _file = $"{Parameter.DirectoryCfg}/app.conf";

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
            _model = model;
            if(File.Exists(_file)) {
                File.Copy(_file, $"{_file}.bck", true);
            }
            File.WriteAllText(_file, JsonConvert.SerializeObject(_model, Formatting.Indented));
        }
    }
}
