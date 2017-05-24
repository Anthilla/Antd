using antdlib.common;
using antdlib.models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using antdlib.config.Parsers;
using anthilla.core;
using Parameter = antdlib.common.Parameter;

namespace antdlib.config {
    public static class NginxConfiguration {

        private static NginxConfigurationModel ServiceModel => Load();

        private static readonly string CfgFile = $"{Parameter.AntdCfgServices}/nginx.conf";
        private const string ServiceName = "nginx.service";
        private const string MainFilePath = "/etc/nginx/nginx.conf";
        private const string MainFilePathBackup = "/etc/nginx/.named.conf";

        private static NginxConfigurationModel Load() {
            if(!File.Exists(CfgFile)) {
                return new NginxConfigurationModel();
            }
            try {
                var text = File.ReadAllText(CfgFile);
                var obj = JsonConvert.DeserializeObject<NginxConfigurationModel>(text);
                return obj;
            }
            catch(Exception) {
                return new NginxConfigurationModel();
            }
        }

        public static void Save(NginxConfigurationModel model) {
            var text = JsonConvert.SerializeObject(model, Formatting.Indented);
            FileWithAcl.WriteAllText(CfgFile, text, "644", "root", "wheel");
            ConsoleLogger.Log("[nginx] configuration saved");
        }

        public static NginxConfigurationModel Get() {
            return Load();
        }

        public static void TryImport() {
            if(File.Exists(CfgFile)) {
                return;
            }
            if(!File.Exists(MainFilePath)) {
                return;
            }
            var text = File.ReadAllText(MainFilePath);
            var model = Parse(text);
            Save(model);
            ConsoleLogger.Log("[nginx] import existing configuration");
        }

        private static NginxConfigurationModel Parse(string text) {
            var model = new NginxConfigurationModel { IsActive = false };
            model = NginxParser.ParseOptions(model, text);
            model = NginxParser.ParseEventsOptions(model, text);
            var upstreams = NginxParser.ParseUpstream(text);
            model.Upstreams = upstreams;
            var http = NginxParser.ParseHttpProtocol(text);
            model.Http = http;
            var servers = NginxParser.ParseServer(text);
            model.Servers = servers;
            return model;
        }

        public static void Set() {
            Stop();
            #region [    named.conf generation    ]
            if(File.Exists(MainFilePath)) {
                if(File.Exists(MainFilePathBackup)) {
                    File.Delete(MainFilePathBackup);
                }
                File.Copy(MainFilePath, MainFilePathBackup);
            }
            var lines = new List<string>();

            FileWithAcl.WriteAllLines(MainFilePath, lines, "644", "nginx", "nginx");
            #endregion
            Start();
        }

        public static bool IsActive() {
            if(!File.Exists(CfgFile)) {
                return false;
            }
            return ServiceModel != null && ServiceModel.IsActive;
        }

        public static void Enable() {
            var mo = ServiceModel;
            mo.IsActive = true;
            Save(mo);
            ConsoleLogger.Log("[nginx] enabled");
        }

        public static void Disable() {
            var mo = ServiceModel;
            mo.IsActive = false;
            Save(mo);
            ConsoleLogger.Log("[nginx] disabled");
        }

        public static void Stop() {
            Systemctl.Stop(ServiceName);
            ConsoleLogger.Log("[nginx] stop");
        }

        public static void Start() {
            if(Systemctl.IsEnabled(ServiceName) == false) {
                Systemctl.Enable(ServiceName);
            }
            if(Systemctl.IsActive(ServiceName) == false) {
                Systemctl.Restart(ServiceName);
            }
            ConsoleLogger.Log("[nginx] start");
        }

    }
}
