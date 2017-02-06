using System;
using System.IO;
using System.Linq;
using antdlib.common;
using antdlib.models;
using Newtonsoft.Json;
using IoDir = System.IO.Directory;

namespace antdlib.config {
    public class AppsConfiguration {

        private readonly AppsConfigurationModel _serviceModel;

        private readonly string _cfgFile = $"{Parameter.AntdCfgServices}/apps.conf";
        private readonly string _cfgFileBackup = $"{Parameter.AntdCfgServices}/apps.conf.bck";

        public AppsConfiguration() {
            IoDir.CreateDirectory(Parameter.AntdCfgServices);
            if(!File.Exists(_cfgFile)) {
                _serviceModel = new AppsConfigurationModel();
            }
            else {
                try {
                    var text = File.ReadAllText(_cfgFile);
                    var obj = JsonConvert.DeserializeObject<AppsConfigurationModel>(text);
                    _serviceModel = obj;
                }
                catch(Exception) {
                    _serviceModel = new AppsConfigurationModel();
                }

            }
        }

        public void Save(AppsConfigurationModel model) {
            var text = JsonConvert.SerializeObject(model, Formatting.Indented);
            if(File.Exists(_cfgFile)) {
                File.Copy(_cfgFile, _cfgFileBackup, true);
            }
            File.WriteAllText(_cfgFile, text);
            ConsoleLogger.Log("[apps] configuration saved");
        }

        public void Set() {
            Enable();
        }

        public bool IsActive() {
            if(!File.Exists(_cfgFile)) {
                return false;
            }
            return _serviceModel != null && _serviceModel.IsActive;
        }

        public AppsConfigurationModel Get() {
            return _serviceModel;
        }

        public void Enable() {
            _serviceModel.IsActive = true;
            Save(_serviceModel);
            ConsoleLogger.Log("[apps] enabled");
        }

        public void Disable() {
            _serviceModel.IsActive = false;
            Save(_serviceModel);
            ConsoleLogger.Log("[apps] disabled");
        }

        public void AddApp(ApplicationModel model) {
            var zones = _serviceModel.Apps;
            if(zones.Any(_ => _.Name == model.Name)) {
                return;
            }
            zones.Add(model);
            _serviceModel.Apps = zones;
            Save(_serviceModel);
        }

        public void RemoveApp(string guid) {
            var zones = _serviceModel.Apps;
            var model = zones.First(_ => _.Guid == guid);
            if(model == null) {
                return;
            }
            zones.Remove(model);
            _serviceModel.Apps = zones;
            Save(_serviceModel);
        }
    }
}
