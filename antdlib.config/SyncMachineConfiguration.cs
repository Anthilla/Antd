using antdlib.common;
using antdlib.models;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;

namespace antdlib.config {
    public static class SyncMachineConfiguration {

        private static SyncMachineConfigurationModel _serviceModel => Load();

        private static readonly string _cfgFile = $"{Parameter.AntdCfgServices}/syncmachine.conf";
        private static readonly string _cfgFileBackup = $"{Parameter.AntdCfgServices}/syncmachine.conf.bck";
        private static readonly string[] _syncPaths = { Parameter.AntdCfg };
        private static DirectoryWatcher _directoryWatcher;

        private static SyncMachineConfigurationModel Load() {
            if(!File.Exists(_cfgFile)) {
                return new SyncMachineConfigurationModel();
            }
            try {
                var text = File.ReadAllText(_cfgFile);
                var obj = JsonConvert.DeserializeObject<SyncMachineConfigurationModel>(text);
                return obj;
            }
            catch(Exception) {
                return new SyncMachineConfigurationModel();
            }
        }

        public static void Save(SyncMachineConfigurationModel model) {
            var text = JsonConvert.SerializeObject(model, Formatting.Indented);
            if(File.Exists(_cfgFile)) {
                File.Copy(_cfgFile, _cfgFileBackup, true);
            }
            File.WriteAllText(_cfgFile, text);
            ConsoleLogger.Log("[syncmachine] configuration saved");
        }

        public static void Set() {
            Enable();
            Stop();
            Start();
        }

        public static bool IsActive() {
            if(!File.Exists(_cfgFile)) {
                return false;
            }
            return _serviceModel != null && _serviceModel.IsActive;
        }

        public static SyncMachineConfigurationModel Get() {
            return _serviceModel;
        }

        public static void Enable() {
            _serviceModel.IsActive = true;
            Save(_serviceModel);
            ConsoleLogger.Log("[syncmachine] enabled");
        }

        public static void Disable() {
            _serviceModel.IsActive = false;
            Save(_serviceModel);
            ConsoleLogger.Log("[syncmachine] disabled");
        }

        public static void Stop() {
            _directoryWatcher?.Stop();
            ConsoleLogger.Log("[syncmachine] stop");
        }

        public static void Start() {
            _directoryWatcher = new DirectoryWatcher(_syncPaths);
            _directoryWatcher.StartWatching();
            ConsoleLogger.Log("[syncmachine] start");
        }

        public static void AddResource(SyncMachineModel model) {
            var resources = _serviceModel.Machines;
            if(resources.Any(_ => _.MachineAddress == model.MachineAddress)) {
                return;
            }
            resources.Add(model);
            _serviceModel.Machines = resources;
            Save(_serviceModel);
        }

        public static void RemoveResource(string guid) {
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
