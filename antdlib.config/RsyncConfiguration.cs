using antdlib.common;
using antdlib.models;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;

namespace antdlib.config {
    public static class RsyncConfiguration {

        private static RsyncConfigurationModel _serviceModel => Load();

        private static readonly string _cfgFile = $"{Parameter.AntdCfgServices}/rsync.conf";
        private static readonly string _cfgFileBackup = $"{Parameter.AntdCfgServices}/rsync.conf.bck";

        public static RsyncConfigurationModel Load() {
            if(!File.Exists(_cfgFile)) {
                return new RsyncConfigurationModel();
            }
            try {
                var text = File.ReadAllText(_cfgFile);
                var obj = JsonConvert.DeserializeObject<RsyncConfigurationModel>(text);
                return obj;
            }
            catch(Exception) {
                return new RsyncConfigurationModel();
            }
        }

        public static void Save(RsyncConfigurationModel model) {
            var text = JsonConvert.SerializeObject(model, Formatting.Indented);
            FileWithAcl.WriteAllText(_cfgFile, text, "644", "root", "wheel");
            ConsoleLogger.Log("[rsync] configuration saved");
        }

        public static void Set() {
            Enable();
        }

        public static bool IsActive() {
            if(!File.Exists(_cfgFile)) {
                return false;
            }
            return _serviceModel != null && _serviceModel.IsActive;
        }

        public static RsyncConfigurationModel Get() {
            return _serviceModel;
        }

        public static void Enable() {
            _serviceModel.IsActive = true;
            Save(_serviceModel);
            ConsoleLogger.Log("[rsync] enabled");
        }

        public static void Disable() {
            _serviceModel.IsActive = false;
            Save(_serviceModel);
            ConsoleLogger.Log("[rsync] disabled");
        }

        public static void AddDirectory(RsyncObjectModel model) {
            var dirs = _serviceModel.Directories;
            dirs.Add(model);
            _serviceModel.Directories = dirs;
            Save(_serviceModel);
            DirectoryWatcherRsync.Start(dirs);
        }

        public static void RemoveDirectory(string guid) {
            var dirs = _serviceModel.Directories;
            var model = dirs.First(_ => _.Guid == guid);
            if(model == null) {
                return;
            }
            dirs.Remove(model);
            _serviceModel.Directories = dirs;
            Save(_serviceModel);
        }
    }
}
