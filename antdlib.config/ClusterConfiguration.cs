using antdlib.models;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using anthilla.core;

namespace antdlib.config {
    public static class ClusterConfiguration {

        private static readonly string CfgFile = $"{Parameter.AntdCfgCluster}/cluster.conf";

        public static void Prepare() {
            var dir = "/var/lib/haproxy";
            Directory.CreateDirectory(dir);
            Bash.Execute($"chown haproxy:haproxy {dir}");
            Bash.Execute($"chmod 755 {dir}");
            //net.ipv4.ip_nonlocal_bind=1
            if(File.Exists("/proc/sys/net/ipv4/ip_nonlocal_bind")) {
                File.WriteAllText("/proc/sys/net/ipv4/ip_nonlocal_bind", "1");
            }
        }

        private static List<NodeModel> Load() {
            if(!File.Exists(CfgFile)) {
                return new List<NodeModel>();
            }
            var text = File.ReadAllText(CfgFile);
            var obj = JsonConvert.DeserializeObject<List<NodeModel>>(text);
            return obj;
        }

        public static void SaveNodes(List<NodeModel> model) {
            Prepare();
            var text = JsonConvert.SerializeObject(model, Formatting.Indented);
            FileWithAcl.WriteAllText(CfgFile, text, "644", "root", "wheel");
            ConsoleLogger.Log("[cluster] configuration saved");
        }

        private static readonly string IpFile = $"{Parameter.AntdCfgCluster}/cluster-info.conf";

        public static void SaveConfiguration(Cluster.Configuration model) {
            Prepare();
            var text = JsonConvert.SerializeObject(model, Formatting.Indented);
            FileWithAcl.WriteAllText(IpFile, text, "644", "root", "wheel");
            ConsoleLogger.Log("[cluster] configuration saved");
        }

        public static Cluster.Configuration GetClusterInfo() {
            if(!File.Exists(IpFile)) {
                return new Cluster.Configuration();
            }
            var text = File.ReadAllText(IpFile);
            var obj = JsonConvert.DeserializeObject<Cluster.Configuration>(text);
            return obj;
        }

        public static List<NodeModel> GetNodes() {
            return Load();
        }

        public static Cluster.DeployConf GetPackagedConfiguration() {
            var conf = GetClusterInfo();
            var nodes = GetNodes();
            var cc = new Cluster.DeployConf {
                Configuration = conf,
                Nodes = nodes
            };
            return cc;
        }
    }
}
