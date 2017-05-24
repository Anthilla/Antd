using antdlib.common;
using antdlib.models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using anthilla.core;
using Parameter = antdlib.common.Parameter;

namespace antdlib.config {
    public static class ClusterConfiguration {

        private static readonly string CfgFile = $"{Parameter.AntdCfgCluster}/cluster.conf";

        private static List<Cluster.Node> Load() {
            if(!File.Exists(CfgFile)) {
                return new List<Cluster.Node>();
            }
            try {
                var text = File.ReadAllText(CfgFile);
                var obj = JsonConvert.DeserializeObject<List<Cluster.Node>>(text);
                return obj;
            }
            catch(Exception) {
                return new List<Cluster.Node>();
            }
        }

        public static void Save(List<Cluster.Node> model) {
            var text = JsonConvert.SerializeObject(model, Formatting.Indented);
            FileWithAcl.WriteAllText(CfgFile, text, "644", "root", "wheel");
            ConsoleLogger.Log("[cluster] configuration saved");
        }

        private static readonly string IpFile = $"{Parameter.AntdCfgCluster}/cluster-info.conf";
        //private static readonly string IpFileBackup = $"{Parameter.AntdCfgCluster}/cluster-info.conf.bck";

        public static void SaveClusterInfo(Cluster.Configuration model) {
            var text = JsonConvert.SerializeObject(model, Formatting.Indented);
            FileWithAcl.WriteAllText(IpFile, text, "644", "root", "wheel");
            ConsoleLogger.Log("[cluster] configuration saved");
        }

        public static Cluster.Configuration GetClusterInfo() {
            if(!File.Exists(IpFile)) {
                return new Cluster.Configuration();
            }
            try {
                var text = File.ReadAllText(IpFile);
                var obj = JsonConvert.DeserializeObject<Cluster.Configuration>(text);
                return obj;
            }
            catch(Exception) {
                return new Cluster.Configuration();
            }
        }

        public static List<Cluster.Node> Get() {
            return Load();
        }
    }
}
