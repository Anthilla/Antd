using antdlib.models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using anthilla.core;
using Parameter = antdlib.common.Parameter;

namespace antdlib.config {
    public class TorConfiguration {

        private static TorConfigurationModel ServiceModel => Load();

        private static readonly string CfgFile = $"{Parameter.AntdCfgServices}/tor.conf";
        private const string ServiceName = "tor.service";
        private const string MainFilePath = "/etc/tor/torrc";
        private const string MainFilePathBackup = "/etc/tor/.torrc";

        private const string LibDir = "/var/lib/tor";
        private static readonly string LibDirMnt = $"{Parameter.RepoDirs}/DIR_lib_tor";

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

        public static void Save(List<TorService> model) {
            var s = new TorConfigurationModel { IsActive = ServiceModel.IsActive, Services = model };
            var text = JsonConvert.SerializeObject(s, Formatting.Indented);
            FileWithAcl.WriteAllText(CfgFile, text, "644", "root", "wheel");
            ConsoleLogger.Log("[tor] configuration saved");
        }

        public static void Set() {
            Stop();
            DirectoryWithAcl.CreateDirectory(LibDirMnt, "755", "root", "root");
            MountManagement.Dir(LibDir);
            #region [    torrc generation    ]
            if(File.Exists(MainFilePath)) {
                if(File.Exists(MainFilePathBackup)) {
                    File.Delete(MainFilePathBackup);
                }
                File.Copy(MainFilePath, MainFilePathBackup);
            }
            var lines = new List<string>();
            foreach(var svc in ServiceModel.Services) {
                if (string.IsNullOrEmpty(svc.Name)
                    || string.IsNullOrEmpty(svc.IpAddress)
                    || string.IsNullOrEmpty(svc.TorPort))
                {
                    continue;
                }
                //HiddenServiceDir /var/lib/tor/hidden_service/
                //HiddenServicePort 80 127.0.0.1:8080
                var dire = $"{LibDirMnt}/{svc.Name}";
                DirectoryWithAcl.CreateDirectory(dire, "755", "root", "root");
                lines.Add($"HiddenServiceDir {dire}");
                lines.Add($"HiddenServicePort {svc.TorPort} {svc.IpAddress}");
            }
            FileWithAcl.WriteAllLines(MainFilePath, lines, "700", "tor", "root");
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
            var s = new TorConfigurationModel { IsActive = true, Services = ServiceModel.Services };
            var text = JsonConvert.SerializeObject(s, Formatting.Indented);
            FileWithAcl.WriteAllText(CfgFile, text, "644", "root", "wheel");
            ConsoleLogger.Log("[tor] enabled");
        }

        public static void Disable() {
            var s = new TorConfigurationModel { IsActive = false, Services = ServiceModel.Services };
            var text = JsonConvert.SerializeObject(s, Formatting.Indented);
            FileWithAcl.WriteAllText(CfgFile, text, "644", "root", "wheel");
            Stop();
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
