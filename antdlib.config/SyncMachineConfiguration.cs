using System;
using System.IO;
using System.Linq;
using antdlib.common;
using antdlib.models;
using Newtonsoft.Json;
using IoDir = System.IO.Directory;

namespace antdlib.config {
    public class SyncMachineConfiguration {

        private readonly SyncMachineConfigurationModel _serviceModel;

        private readonly string _cfgFile = $"{Parameter.AntdCfgServices}/syncmachine.conf";
        private readonly string _cfgFileBackup = $"{Parameter.AntdCfgServices}/syncmachine.conf.bck";
        private readonly string[] _syncPaths = { Parameter.AntdCfg };
        private DirectoryWatcher _directoryWatcher;

        public SyncMachineConfiguration() {
            IoDir.CreateDirectory(Parameter.AntdCfgServices);
            if(!File.Exists(_cfgFile)) {
                _serviceModel = new SyncMachineConfigurationModel();
            }
            else {
                try {
                    var text = File.ReadAllText(_cfgFile);
                    var obj = JsonConvert.DeserializeObject<SyncMachineConfigurationModel>(text);
                    _serviceModel = obj;
                }
                catch(Exception) {
                    _serviceModel = new SyncMachineConfigurationModel();
                }
            }
        }

        public void Save(SyncMachineConfigurationModel model) {
            var text = JsonConvert.SerializeObject(model, Formatting.Indented);
            if(File.Exists(_cfgFile)) {
                File.Copy(_cfgFile, _cfgFileBackup, true);
            }
            File.WriteAllText(_cfgFile, text);
            ConsoleLogger.Log("[syncmachine] configuration saved");
        }

        public void Set() {
            Enable();
            Stop();
            Start();
        }

        public bool IsActive() {
            if(!File.Exists(_cfgFile)) {
                return false;
            }
            return _serviceModel != null && _serviceModel.IsActive;
        }

        public SyncMachineConfigurationModel Get() {
            return _serviceModel;
        }

        public void Enable() {
            _serviceModel.IsActive = true;
            Save(_serviceModel);
            ConsoleLogger.Log("[syncmachine] enabled");
        }

        public void Disable() {
            _serviceModel.IsActive = false;
            Save(_serviceModel);
            ConsoleLogger.Log("[syncmachine] disabled");
        }

        public void Stop() {
            _directoryWatcher?.Stop();
            ConsoleLogger.Log("[syncmachine] stop");
        }

        public void Start() {
            _directoryWatcher = new DirectoryWatcher(_syncPaths);
            _directoryWatcher.StartWatching();
            ConsoleLogger.Log("[syncmachine] start");
        }

        public void AddResource(SyncMachineModel model) {
            var resources = _serviceModel.Machines;
            if(resources.Any(_ => _.MachineAddress == model.MachineAddress)) {
                return;
            }
            resources.Add(model);
            _serviceModel.Machines = resources;
            Save(_serviceModel);
        }

        public void RemoveResource(string guid) {
            var resources = _serviceModel.Machines;
            var model = resources.First(_ => _.Guid == guid);
            if(model == null) {
                return;
            }
            resources.Remove(model);
            _serviceModel.Machines = resources;
            Save(_serviceModel);
        }
    }
}
