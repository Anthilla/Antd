using Newtonsoft.Json;
using System.Collections.Generic;
using anthilla.core;
using Kvpbase;
using System.IO;
using antdlib.models;

namespace antdlib.config {
    public class VfsConfiguration {

        public static void SetDefaults() {
            if(!File.Exists(_systemFile)) {
                File.WriteAllText(_systemFile, JsonConvert.SerializeObject(new VfsDefaults.DefaltSystem(), Formatting.Indented));
                File.WriteAllText(_apiKeyFile, JsonConvert.SerializeObject(new List<VfsDefaults.ApiKey>() { new VfsDefaults.ApiKey() }, Formatting.Indented));
                File.WriteAllText(_apiKeyPermissionFile, JsonConvert.SerializeObject(new List<VfsDefaults.ApiKeyPermission>() { new VfsDefaults.ApiKeyPermission() }, Formatting.Indented));
                File.WriteAllText(_userMasterFile, JsonConvert.SerializeObject(new List<VfsDefaults.UserMaster>() { new VfsDefaults.UserMaster() }, Formatting.Indented));
                File.WriteAllText(_topologyFile, JsonConvert.SerializeObject(new VfsDefaults.ListTopology(), Formatting.Indented));
            }
        }

        private static readonly string _systemFile = $"{Parameter.AntdCfgVfs}/system.json";

        public static void SaveSystemConfiguration(Settings model) {
            var text = JsonConvert.SerializeObject(model, Formatting.Indented);
            FileWithAcl.WriteAllText(_systemFile, text, "644", "root", "wheel");
            ConsoleLogger.Log("[vfs] configuration saved");
        }

        public static Settings GetSystemConfiguration() {
            if(!File.Exists(_systemFile)) {
                return new Settings();
            }
            return Settings.FromFile(_systemFile);
        }

        private static readonly string _apiKeyFile = $"{Parameter.AntdCfgVfs}/apiKey.json";

        public static void SaveApiKeyConfiguration(List<ApiKey> model) {
            var text = JsonConvert.SerializeObject(model, Formatting.Indented);
            FileWithAcl.WriteAllText(_apiKeyFile, text, "644", "root", "wheel");
        }

        public static List<ApiKey> GetApiKeyConfiguration() {
            if(!File.Exists(_apiKeyFile)) {
                return new List<ApiKey>();
            }
            return ApiKey.FromFile(_apiKeyFile);
        }

        private static readonly string _apiKeyPermissionFile = $"{Parameter.AntdCfgVfs}/apiKeyPermission.json";

        public static void SaveApiKeyPermissionConfiguration(List<ApiKeyPermission> model) {
            var text = JsonConvert.SerializeObject(model, Formatting.Indented);
            FileWithAcl.WriteAllText(_apiKeyPermissionFile, text, "644", "root", "wheel");
        }

        public static List<ApiKeyPermission> GetApiKeyPermissionConfiguration() {
            if(!File.Exists(_apiKeyPermissionFile)) {
                return new List<ApiKeyPermission>();
            }
            return ApiKeyPermission.FromFile(_apiKeyPermissionFile);
        }

        private static readonly string _userMasterFile = $"{Parameter.AntdCfgVfs}/userMaster.json";

        public static void SaveUserMasterConfiguration(List<UserMaster> model) {
            var text = JsonConvert.SerializeObject(model, Formatting.Indented);
            FileWithAcl.WriteAllText(_userMasterFile, text, "644", "root", "wheel");
        }

        public static List<UserMaster> GetUserMasterConfiguration() {
            if(!File.Exists(_userMasterFile)) {
                return new List<UserMaster>();
            }
            return UserMaster.FromFile(_userMasterFile);
        }

        private static readonly string _topologyFile = $"{Parameter.AntdCfgVfs}/topology.json";

        public static void SaveTopologyConfiguration(Topology model) {
            var text = JsonConvert.SerializeObject(model, Formatting.Indented);
            FileWithAcl.WriteAllText(_topologyFile, text, "644", "root", "wheel");
        }

        public static Topology GetTopologyConfiguration() {
            if(!File.Exists(_topologyFile)) {
                return new Topology();
            }
            return Topology.FromFile(_topologyFile);
        }
    }
}
