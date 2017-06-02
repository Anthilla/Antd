using antdlib.models;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using anthilla.core;
using IoDir = System.IO.Directory;
using Parameter = antdlib.common.Parameter;

namespace antdlib.config {
    public class TimerConfiguration {

        private readonly TimerConfigurationModel _serviceModel;

        private readonly string _cfgFile = $"{Parameter.AntdCfgServices}/timer.conf";

        public TimerConfiguration() {
            IoDir.CreateDirectory(Parameter.AntdCfgServices);
            if(!File.Exists(_cfgFile)) {
                _serviceModel = new TimerConfigurationModel();
            }
            else {
                try {
                    var text = File.ReadAllText(_cfgFile);
                    var obj = JsonConvert.DeserializeObject<TimerConfigurationModel>(text);
                    _serviceModel = obj;
                }
                catch(Exception) {
                    _serviceModel = new TimerConfigurationModel();
                }

            }
        }

        public void Save(TimerConfigurationModel model) {
            var text = JsonConvert.SerializeObject(model, Formatting.Indented);
            FileWithAcl.WriteAllText(_cfgFile, text, "644", "root", "wheel");
            ConsoleLogger.Log("[timer] configuration saved");
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

        public TimerConfigurationModel Get() {
            return _serviceModel;
        }

        public void Enable() {
            _serviceModel.IsActive = true;
            Save(_serviceModel);
            ConsoleLogger.Log("[timer] enabled");
        }

        public void Disable() {
            _serviceModel.IsActive = false;
            Save(_serviceModel);
            ConsoleLogger.Log("[timer] disabled");
        }

        public void AddTimer(TimerModel model) {
            var zones = _serviceModel.Timers;
            zones.Add(model);
            _serviceModel.Timers = zones;
            Save(_serviceModel);
        }

        public void RemoveTimer(string guid) {
            var zones = _serviceModel.Timers;
            var model = zones.First(_ => _.Guid == guid);
            if(model == null) {
                return;
            }
            zones.Remove(model);
            _serviceModel.Timers = zones;
            Save(_serviceModel);
        }
    }
}
