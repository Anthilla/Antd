using antdlib.models;
using Newtonsoft.Json;
using System.IO;
using System.Linq;
using anthilla.core;

namespace antdlib.config {
    public static class RsyncConfiguration {

        private static RsyncConfigurationModel ServiceModel => Load();

        private static readonly string CfgFile = $"{Parameter.AntdCfgServices}/rsync.conf";

        public static RsyncConfigurationModel Load() {
            if(!File.Exists(CfgFile)) {
                return new RsyncConfigurationModel();
            }
            var text = File.ReadAllText(CfgFile);
            var obj = JsonConvert.DeserializeObject<RsyncConfigurationModel>(text);
            return obj;
        }

        public static void Save(RsyncConfigurationModel model) {
            var text = JsonConvert.SerializeObject(model, Formatting.Indented);
            FileWithAcl.WriteAllText(CfgFile, text, "644", "root", "wheel");
            ConsoleLogger.Log("[rsync] configuration saved");
        }

        public static void Set() {
            Enable();
        }

        public static bool IsActive() {
            if(!File.Exists(CfgFile)) {
                return false;
            }
            return ServiceModel != null && ServiceModel.IsActive;
        }

        public static RsyncConfigurationModel Get() {
            return ServiceModel;
        }

        public static void Enable() {
            ServiceModel.IsActive = true;
            Save(ServiceModel);
            ConsoleLogger.Log("[rsync] enabled");
        }

        public static void Disable() {
            ServiceModel.IsActive = false;
            Save(ServiceModel);
            ConsoleLogger.Log("[rsync] disabled");
        }

        public static void AddDirectory(RsyncObjectModel model) {
            var dirs = ServiceModel.Directories;
            dirs.Add(model);
            ServiceModel.Directories = dirs;
            Save(ServiceModel);
            DirectoryWatcherRsync.Start(dirs);
        }

        public static void RemoveDirectory(string guid) {
            var dirs = ServiceModel.Directories;
            var model = dirs.First(_ => _.Guid == guid);
            if(model == null) {
                return;
            }
            dirs.Remove(model);
            ServiceModel.Directories = dirs;
            Save(ServiceModel);
        }
    }
}
