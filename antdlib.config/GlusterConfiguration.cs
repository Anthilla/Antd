using antdlib.models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using anthilla.core;
using Parameter = antdlib.common.Parameter;

namespace antdlib.config {
    public static class GlusterConfiguration {

        private static GlusterConfigurationModel ServiceModel => Load();

        private static readonly string CfgFile = $"{Parameter.AntdCfgCluster}/gluster.conf";
        private const string ServiceName = "glusterd.service";

        private static GlusterConfigurationModel Load() {
            if(!File.Exists(CfgFile)) {
                return new GlusterConfigurationModel();
            }
            try {
                var text = File.ReadAllText(CfgFile);
                var obj = JsonConvert.DeserializeObject<GlusterConfigurationModel>(text);
                return obj;
            }
            catch(Exception) {
                return new GlusterConfigurationModel();
            }
        }

        public static void Save(GlusterConfigurationModel model) {
            var text = JsonConvert.SerializeObject(model, Formatting.Indented);
            FileWithAcl.WriteAllText(CfgFile, text, "644", "root", "wheel");
            ConsoleLogger.Log("[gluster] configuration saved");
        }

        public static void Set() {
            Stop();
            Launch();
        }

        public static bool IsActive() {
            return ServiceModel != null && ServiceModel.IsActive;
        }

        public static GlusterConfigurationModel Get() {
            return ServiceModel;
        }

        public static void Enable() {
            Save(ServiceModel);
            ConsoleLogger.Log("[gluster] enabled");
        }

        public static void Disable() {
            ServiceModel.IsActive = false;
            Save(ServiceModel);
            ConsoleLogger.Log("[gluster] disabled");
        }

        public static void Stop() {
            Systemctl.Stop(ServiceName);
            ConsoleLogger.Log("[gluster] stop");
        }

        public static void Start() {
            if(Systemctl.IsEnabled(ServiceName) == false) {
                Systemctl.Enable(ServiceName);
            }
            if(Systemctl.IsActive(ServiceName) == false) {
                Systemctl.Restart(ServiceName);
            }
            ConsoleLogger.Log("[gluster] start");
        }

        public static void Launch() {
            ConsoleLogger.Log("[gluster] launch");
            var config = ServiceModel;
            SetHostnameFile(config);
            Systemctl.Enable(ServiceName);
            Systemctl.Start(ServiceName);
            ConsoleLogger.Log("[gluster] include nodes");
            foreach(var node in config.Nodes) {
                IncludeNode(node.Hostname);
            }
            ConsoleLogger.Log("[gluster] start volumes");
            foreach(var volume in config.Volumes) {
                SetVolume(volume, config.Nodes);
            }
        }

        private static readonly Host2Model Host = Host2Configuration.Host;

        private static void SetHostnameFile(GlusterConfigurationModel conf) {
            var linesToAdd = new List<string>();
            foreach(var node in conf.Nodes) {
                linesToAdd.Add($"{node.Ip} {node.Hostname}.{Host.InternalDomainPrimary} {node.Hostname}");
            }
            const string file = "/etc/hosts";
            var hostsLines = File.ReadAllLines(file).ToList();
            foreach(var line in linesToAdd) {
                if(!hostsLines.Contains(line)) {
                    hostsLines.Add(line);
                }
            }
            FileWithAcl.WriteAllLines(file, hostsLines, "644", "root", "wheel");
        }

        private static void IncludeNode(string node) {
            Bash.Execute($"gluster peer probe {node}", false);
        }

        private static void SetVolume(GlusterVolume volumeInfo, List<GlusterNode> nodes) {
            Directory.CreateDirectory(volumeInfo.MountPoint);
            var replicaNodes = string.Join(" ", nodes.Select(_ => $"{_.Hostname}:{volumeInfo.Brick}")).TrimEnd();
            ConsoleLogger.Log($"[gluster] create {volumeInfo.Name}");
            Bash.Execute($"gluster volume create {volumeInfo.Name} replica {nodes.Count} transport tcp {replicaNodes} force", false);
            Bash.Execute($"gluster volume start {volumeInfo.Name}", false);

            foreach(var node in nodes) {
                ConsoleLogger.Log($"[gluster] mount {volumeInfo.Name} {volumeInfo.MountPoint}");
                ConsoleLogger.Log($"mount -t glusterfs {node.Hostname}:{volumeInfo.Name} {volumeInfo.MountPoint}");
                Bash.Execute($"mount -t glusterfs {node.Hostname}:{volumeInfo.Name} {volumeInfo.MountPoint}", false);
            }
        }
    }
}
