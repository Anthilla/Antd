using antdlib.models;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using anthilla.core;
using Parameter = antdlib.common.Parameter;

namespace antdlib.config {
    public class AppsConfiguration {

        private static AppsConfigurationModel ServiceModel => Load();

        private static readonly string CfgFile = $"{Parameter.AntdCfgServices}/apps.conf";
        private static readonly string CfgFileBackup = $"{Parameter.AntdCfgServices}/apps.conf.bck";


        private static AppsConfigurationModel Load() {
            if(!File.Exists(CfgFile)) {
                return new AppsConfigurationModel();

            }
            try {
                var text = File.ReadAllText(CfgFile);
                var obj = JsonConvert.DeserializeObject<AppsConfigurationModel>(text);
                return obj;
            }
            catch(Exception) {
                return new AppsConfigurationModel();
            }
        }

        public static void Save(AppsConfigurationModel model) {
            var text = JsonConvert.SerializeObject(model, Formatting.Indented);
            if(File.Exists(CfgFile)) {
                File.Copy(CfgFile, CfgFileBackup, true);
            }
            FileWithAcl.WriteAllText(CfgFile, text, "644", "root", "wheel");
            ConsoleLogger.Log("[apps] configuration saved");
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

        public static AppsConfigurationModel Get() {
            return ServiceModel;
        }

        public static void Enable() {
            ServiceModel.IsActive = true;
            Save(ServiceModel);
            ConsoleLogger.Log("[apps] enabled");
        }

        public static void Disable() {
            ServiceModel.IsActive = false;
            Save(ServiceModel);
            ConsoleLogger.Log("[apps] disabled");
        }

        public static void AddApp(ApplicationModel model) {
            var zones = ServiceModel.Apps;
            if(zones.Any(_ => _.Name == model.Name)) {
                return;
            }
            zones.Add(model);
            ServiceModel.Apps = zones;
            Save(ServiceModel);
        }

        public static void RemoveApp(string guid) {
            var zones = ServiceModel.Apps;
            var model = zones.First(_ => _.Guid == guid);
            if(model == null) {
                return;
            }
            zones.Remove(model);
            ServiceModel.Apps = zones;
            Save(ServiceModel);
        }
    }
}
