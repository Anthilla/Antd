using antdlib.common;
using antdlib.models;
using Newtonsoft.Json;
using System;
using System.IO;

namespace antdlib.config {
    public class TorConfiguration {

        private static TorConfigurationModel ServiceModel => Load();

        private static readonly string CfgFile = $"{Parameter.AntdCfgServices}/tor.conf";
        private static readonly string CfgFileBackup = $"{Parameter.AntdCfgServices}/tor.conf.bck";
        private const string ServiceName = "tor.service";
        private const string MainFilePath = "/etc/tor/torrc";
        private const string MainFilePathBackup = "/etc/tor/.torrc";

        private static TorConfigurationModel Load() {
            if(!File.Exists(CfgFile)) {
                return new TorConfigurationModel();

            }
            try {
                var text = File.ReadAllText(CfgFile);
                var obj = JsonConvert.DeserializeObject<TorConfigurationModel>(text);
                return obj;
            }
            catch(Exception) {
                return new TorConfigurationModel();
            }
        }

        public static void Save(TorConfigurationModel model) {
            var text = JsonConvert.SerializeObject(model, Formatting.Indented);
            if(File.Exists(CfgFile)) {
                File.Copy(CfgFile, CfgFileBackup, true);
            }
            File.WriteAllText(CfgFile, text);
            ConsoleLogger.Log("[tor] configuration saved");
        }

        public static void Set() {
            Enable();
            Stop();
            #region [    torrc generation    ]
            if(File.Exists(MainFilePath)) {
                if(File.Exists(MainFilePathBackup)) {
                    File.Delete(MainFilePathBackup);
                }
                File.Copy(MainFilePath, MainFilePathBackup);
            }
            #endregion
            Start();
        }

        public static bool IsActive() {
            if(!File.Exists(CfgFile)) {
                return false;
            }
            return ServiceModel != null && ServiceModel.IsActive;
        }

        public static TorConfigurationModel Get() {
            return ServiceModel;
        }

        public static void Enable() {
            ServiceModel.IsActive = true;
            Save(ServiceModel);
            ConsoleLogger.Log("[tor] enabled");
        }

        public static void Disable() {
            ServiceModel.IsActive = false;
            Save(ServiceModel);
            ConsoleLogger.Log("[tor] disabled");
        }

        public static void Stop() {
            Systemctl.Stop(ServiceName);
            ConsoleLogger.Log("[tor] stop");
        }

        public static void Start() {
            if(Systemctl.IsEnabled(ServiceName) == false) {
                Systemctl.Enable(ServiceName);
            }
            if(Systemctl.IsActive(ServiceName) == false) {
                Systemctl.Restart(ServiceName);
            }
            ConsoleLogger.Log("[tor] start");
        }
    }
}
