using System.Collections.Generic;
using anthilla.core;
using System.Linq;
using System.IO;
using System;

namespace Antd.cmds {

    /// <summary>
    /// TODO
    /// - sincronizza file da sincronizzare (in ClusterFs)  ->  DirectoryWatcher
    /// </summary>
    public class Cluster {

        private const string haproxyVarLib = "/var/lib/haproxy";
        private const string ipNonlocalBindSysctlKey = "net.ipv4.ip_nonlocal_bind";
        private const string ipNonlocalBindSysctlFile = "/proc/sys/net/ipv4/ip_nonlocal_bind";
        private const string ipNonlocalBindSysctlValue = "1";
        private const string keepalivedFileOutput = "/cfg/antd/conf/keepalived.conf";
        private const string haproxyFileOutput = "/cfg/antd/conf/haproxy.conf";

        private const string clusterCfgFolder = "/cfg/antd/cluster";

        /// <summary>
        /// Prepara le cartelle e i parametri necessari per avviare i servizi del cluster
        /// </summary>
        private static void Prepare() {
            Directory.CreateDirectory(haproxyVarLib);
            Bash.Execute($"chown haproxy:haproxy {haproxyVarLib}");
            Bash.Execute($"chmod 755 {haproxyVarLib}");
            if(File.Exists(ipNonlocalBindSysctlFile)) {
                File.WriteAllText(ipNonlocalBindSysctlFile, ipNonlocalBindSysctlValue);
            }
            Directory.CreateDirectory(clusterCfgFolder);
            var nodes = Application.CurrentConfiguration.Cluster.Nodes;
            for(var i = 0; i < nodes.Length; i++) {
                var nodeFolder = CommonString.Append(clusterCfgFolder, "/", nodes[i].MachineUid);
                Directory.CreateDirectory(nodeFolder);
            }
        }

        public static bool ApplyNetwork() {
            ConsoleLogger.Log("[cluster] init applying changes");
            var config = Application.CurrentConfiguration.Cluster;
            if(config == null) {
                ConsoleLogger.Error("[cluster] exit: config == null");
                return false;
            }
            var networkConfig = config.SharedNetwork;

            ConsoleLogger.Log("[cluster] applying changes");
            Prepare();
            var publicIp = networkConfig.VirtualIpAddress;
            if(string.IsNullOrEmpty(publicIp)) {
                ConsoleLogger.Warn("[cluster] configuration not valid: publicIp");
                return false;
            }
            var nodesConfig = config.Nodes;
            if(nodesConfig == null) {
                ConsoleLogger.Error("[cluster] exit: nodesConfig == null");
                return false;
            }
            if(!nodesConfig.Any()) {
                ConsoleLogger.Log("[cluster] shared network is disabled");
                return false;
            }
            SaveHaproxy(networkConfig, nodesConfig);
            SaveKeepalived(networkConfig, nodesConfig);
            return true;
        }

        private static void SaveHaproxy(ClusterNetwork networkConfig, ClusterNode[] nodesConfig) {
            if(networkConfig == null) {
                ConsoleLogger.Warn("[cluster] haproxy not configured: missing network parameters");
                return;
            }
            if(!networkConfig.Active) {
                ConsoleLogger.Warn("[cluster] shared network is disabled");
                return;
            }
            var ports = networkConfig.PortMapping;
            if(!ports.Any()) {
                ConsoleLogger.Warn("[cluster] exit: !ports.Any()");
                return;
            }

            ConsoleLogger.Log("[cluster] init haproxy");
            var lines = new List<string> {
                "global",
                "    daemon",
                "    log 127.0.0.1   local0",
                "    log 127.0.0.1   local1 notice",
                "    maxconn 4096",
                "    user haproxy",
                "    group haproxy",
                "",
                "defaults",
                "    log     global",
                "    mode    http",
                "    option  httplog",
                "    option  dontlognull",
                "    retries 3",
                "    option  redispatch",
                "    maxconn 2000",
                "    timeout connect 5000",
                "    timeout client  50000",
                "    timeout server  50000",
                ""
            };
            //var localServices = Application.CurrentConfiguration.Cluster.Nodes.FirstOrDefault(_ => _.MachineUid == Application.CurrentConfiguration.Host.MachineUid.ToString()).Services;
            //if(localServices != null) {
            int errorStateCounter = 0;
            for(var i = 0; i < ports.Length; i++) {
                var port = ports[i];
                //port.ServicePort = localServices.FirstOrDefault(_ => _.Name == port.ServiceName)?.Port.ToString();
                if(string.IsNullOrEmpty(port.ServicePort)) {
                    errorStateCounter++;
                    ConsoleLogger.Warn($"[cluster] haproxy source port is not defined for '{port.ServiceName}'");
                    continue;
                }
                if(string.IsNullOrEmpty(port.VirtualPort)) {
                    ConsoleLogger.Warn($"[cluster] haproxy virtual port is not defined for '{port.ServiceName}'");
                    errorStateCounter++;
                    continue;
                }
                var frontEndLabel = $"fe_in{port.ServicePort}_out{port.VirtualPort}";
                var backEndLabel = $"be_in{port.ServicePort}_out{port.VirtualPort}";
                ConsoleLogger.Log($"[cluster] {port.ServiceName} virtual port: {port.VirtualPort} as {port.ServicePort}");
                lines.Add($"frontend {frontEndLabel}");
                lines.Add("    mode http");
                lines.Add($"    bind {networkConfig.VirtualIpAddress}:{port.VirtualPort} transparent");
                lines.Add("    stats enable");
                lines.Add("    stats auth admin:Anthilla");
                lines.Add("    option httpclose");
                lines.Add("    option forwardfor");
                lines.Add($"    default_backend {backEndLabel}");
                lines.Add("");
                lines.Add($"backend {backEndLabel}");
                lines.Add("    balance roundrobin");
                lines.Add("    cookie JSESSIONID prefix");
                //lines.Add("    option httpchk HEAD /check.txt HTTP/1.0");
                for(var n = 0; n < nodesConfig.Length; n++) {
                    var node = nodesConfig[n];
                    lines.Add($"    server {node.Hostname} {node.PublicIp}:{port.ServicePort} check");
                }
                lines.Add("");
            }
            //}
            //else {
            //    ConsoleLogger.Warn("[cluster] local service does not exist");
            //    return;
            //}

            if(errorStateCounter == ports.Length) {
                ConsoleLogger.Log("[cluster] failed to configure haproxy: port mapping list is empty");
                return;
            }
            File.WriteAllLines(haproxyFileOutput, lines);
            Haproxy.Stop();
            Haproxy.Start(haproxyFileOutput);
            ConsoleLogger.Log("[cluster] haproxy started");
        }

        private static void SaveKeepalived(ClusterNetwork networkConfig, ClusterNode[] nodesConfig) {
            if(networkConfig == null) {
                ConsoleLogger.Log("[cluster] keepalived not configured: missing network parameters");
                return;
            }
            if(!networkConfig.Active) {
                ConsoleLogger.Log("[cluster] shared network is disabled");
                return;
            }
            var ports = networkConfig.PortMapping;
            if(!ports.Any()) {
                ConsoleLogger.Log("[cluster] exit: !ports.Any()");
                return;
            }

            ConsoleLogger.Log("[cluster] init keepalived");
            const string keepalivedService = "keepalived.service";
            if(Systemctl.IsActive(keepalivedService)) {
                ConsoleLogger.Log("[cluster] stop service");
                Systemctl.Stop(keepalivedService);
            }
            ConsoleLogger.Log("[cluster] set configuration file");
            var lines = new string[] {
                "vrrp_script chk_haproxy {",
                "    script \"killall -0 haproxy\"",
                "    interval 30",
                "    weight 2",
                "}",
                "",
                "vrrp_instance RH_INT {",
                $"    interface {networkConfig.NetworkInterface}",
                "    state MASTER",
                "    virtual_router_id 51",
                $"    priority 100",
                "    virtual_ipaddress {",
                $"        {networkConfig.VirtualIpAddress}",
                "    }",
                "    track_script {",
                "        chk_haproxy",
                "    }",
                "}",
            };
            File.WriteAllLines(keepalivedFileOutput, lines);
            Keepalived.Stop();
            Keepalived.Start(keepalivedFileOutput);
        }

        /// <summary>
        /// Controlla la configurazione dei servizi del cluster e applica le modifiche del caso
        /// </summary>
        public static void ApplyServices() {
            var services = Application.CurrentConfiguration.Cluster.SharedService;
            if(services == null) {
                return;
            }
            ConsoleLogger.Log("[cluster] prepare services sync");
            if(Application.LIBVIRT_WATCHER == null) {
                Application.LIBVIRT_WATCHER = new LibvirtWatcher();
                ConsoleLogger.Log("[cluster] prepare virsh sync");
            }
            if(services.Virsh == true) {
                Application.LIBVIRT_WATCHER.Start();
                ConsoleLogger.Log("[cluster] start virsh sync process");
            }
            else {
                Application.LIBVIRT_WATCHER.Stop();
            }
        }

        public static bool ApplyFs() {
            var config = Application.CurrentConfiguration.Cluster;
            if(config == null) {
                ConsoleLogger.Error("[cluster] exit: config == null");
                return false;
            }
            var nodesConfig = config.Nodes;
            if(nodesConfig == null) {
                ConsoleLogger.Error("[cluster] exit: nodesConfig == null");
                return false;
            }
            if(!nodesConfig.Any()) {
                ConsoleLogger.Log("[cluster] shared network is disabled");
                return false;
            }
            var fsConfig = config.SharedFs;
            SaveFileSystemSync(fsConfig, nodesConfig);
            GlusterFs.Set();
            return true;
        }

        private static void SaveFileSystemSync(ClusterFs fsConfig, ClusterNode[] nodesConfig) {
            if(fsConfig == null) {
                ConsoleLogger.Log("[cluster] shared fs is disabled");
                return;
            }
        }
    }
}