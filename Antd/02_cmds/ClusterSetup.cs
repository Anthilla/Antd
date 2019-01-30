using Antd.models;
using anthilla.core;
using anthilla.fs.Server;
using Nancy;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Antd.cmds {

    /// <summary>
    /// TODO
    /// - sincronizza file da sincronizzare (in ClusterFs)  ->  DirectoryWatcher
    /// </summary>
    public class ClusterSetup {

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
            if(!networkConfig.Active) {
                return false;
            }
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
                ConsoleLogger.Error("[cluster] keepalived not configured: missing network parameters");
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

            ConsoleLogger.Log("[cluster] init keepalived");
            const string keepalivedService = "keepalived.service";
            if(Systemctl.IsActive(keepalivedService)) {
                ConsoleLogger.Log("[cluster] stop service");
                Systemctl.Stop(keepalivedService);
            }
            ConsoleLogger.Log("[cluster] set configuration file");
            var lines = new string[] {
                "vrrp_script chk_haproxy {",
                "    script \"/usr/bin/killall -0 haproxy\"",
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
                $"        {networkConfig.VirtualIpAddress}/{networkConfig.VirtualIpRange}",
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
            if(!config.SharedFs.Active) {
                return false;
            }
            var nodesConfig = config.Nodes;
            if(nodesConfig == null) {
                ConsoleLogger.Error("[cluster] exit: nodesConfig == null");
                return false;
            }
            if(!nodesConfig.Any()) {
                ConsoleLogger.Warn("[cluster] shared network is disabled");
                return false;
            }
            var fsConfig = config.SharedFs;
            SetStorageServer();
            SaveFileSystemSync();
            //GlusterFs.Set();
            return true;
        }

        private static void SetStorageServer() {
            if(Application.STORAGESERVER == null) {
                var nodeConfig = Application.CurrentConfiguration.Cluster.Nodes.FirstOrDefault(_ => _.MachineUid == Application.CurrentConfiguration.Host.MachineUid.ToString());
                var localIp = nodeConfig == null ? "127.0.0.1" : nodeConfig.PublicIp;
                ConsoleLogger.Warn($"[cluster] init storage server at {localIp}");
                Application.STORAGESERVER = new FileManagerServer("127.0.0.1", Application.STORAGESERVER_PORT);
                while(!Application.STORAGESERVER.IsListening) {
                    Application.STORAGESERVER.Start();
                    System.Threading.Thread.Sleep(1000);
                }
                ConsoleLogger.Warn("[cluster] start storage server");
            }
        }

        private static void SaveFileSystemSync() {
            if(Application.CLUSTER_WATCHER == null) {
                Application.CLUSTER_WATCHER = new ClusterWatcher();
            }
            else {
                Application.CLUSTER_WATCHER.SetWatchers(Application.CurrentConfiguration.Cluster.SharedFs.SyncDirectories.Select(_ => _.Path).ToArray());
            }
        }

        /// <summary>
        /// Deprecato
        /// </summary>
        /// <param name="remoteNode"></param>
        /// <returns></returns>
        public static bool HandshakeBegin(NodeModel[] remoteNode) {
            const string pathToPrivateKey = "/root/.ssh/id_rsa";
            const string pathToPublicKey = "/root/.ssh/id_rsa.pub";
            if(!File.Exists(pathToPublicKey)) {
                var k = Bash.Execute($"ssh-keygen -t rsa -N '' -f {pathToPrivateKey}");
                ConsoleLogger.Log(k);
            }
            var key = File.ReadAllText(pathToPublicKey);
            if(string.IsNullOrEmpty(key)) {
                return false;
            }
            var dict = new Dictionary<string, string> { { "ApplePie", key } };

            //1. controllo la configurazione
            var cluster = Application.CurrentConfiguration.Cluster;
            if(cluster == null) {
                cluster = new Cluster();
                cluster.Label = CommonString.Append("AntdCluster-", cluster.Id.ToString().Substring(0, 8));
            }

            var nodes = cluster.Nodes.ToList();
            for(var i = 0; i < remoteNode.Length; i++) {
                if(remoteNode[i].MachineUid == Application.MACHINE_ID.ToString()) {
                    continue;
                }
                var handshakeResult = ApiConsumer.Post($"{remoteNode[i].ModelUrl}cluster/handshake", dict);
                if(handshakeResult != HttpStatusCode.OK) {
                    return false;
                }
                //ottengo i servizi pubblicati da quel nodo
                var publishedServices = ApiConsumer.Get<ClusterNodeService[]>($"{remoteNode[i].ModelUrl}device/services");
                nodes.Add(new ClusterNode() {
                    MachineUid = remoteNode[i].MachineUid,
                    Hostname = remoteNode[i].Hostname,
                    PublicIp = remoteNode[i].PublicIp,
                    EntryPoint = remoteNode[i].ModelUrl,
                    Services = publishedServices
                });
            }
            //ho fatto gli handshake, quindi il nodo richiesto è pronto per essere integrato nel cluster

            cluster.Active = true;
            if(cluster.Id == Guid.Empty) {
                cluster.Id = Guid.NewGuid();
            }
            cluster.Nodes = nodes.ToArray();

            cluster.SharedNetwork.Active = false;
            var virtualPorts = cluster.SharedNetwork.PortMapping.ToList();
            foreach(var node in nodes) {
                foreach(var svc in node.Services) {
                    var checkPort = virtualPorts.FirstOrDefault(_ => _.ServicePort == svc.Port.ToString());
                    if(checkPort == null) {
                        virtualPorts.Add(new PortMapping() {
                            ServiceName = svc.Name,
                            ServicePort = svc.Port.ToString(),
                            VirtualPort = string.Empty
                        });
                    }
                }
            }
            cluster.SharedNetwork.PortMapping = virtualPorts.ToArray();

            Application.CurrentConfiguration.Cluster = cluster;
            ConfigRepo.Save();
            return true;
        }

        public static bool HandshakeCheck() {
            ConsoleLogger.Log("[handshake] check");
            const string pathToPrivateKey = "/root/.ssh/id_rsa";
            const string pathToPublicKey = "/root/.ssh/id_rsa.pub";
            if(!File.Exists(pathToPublicKey)) {
                ConsoleLogger.Log("[handshake] generate ssh key");
                var k = Bash.Execute($"ssh-keygen -t rsa -N '' -f {pathToPrivateKey}");
                ConsoleLogger.Log(k);
            }
            var key = File.ReadAllText(pathToPublicKey);
            if(string.IsNullOrEmpty(key)) {
                return false;
            }
            var dict = new Dictionary<string, string> { { "ApplePie", key } };

            //1. controllo la configurazione
            var cluster = Application.CurrentConfiguration.Cluster;
            if(cluster == null) {
                cluster = new Cluster();
                cluster.Label = CommonString.Append("AntdCluster-", cluster.Id.ToString().Substring(0, 8));
            }

            var nodes = cluster.Nodes.ToList();
            foreach(var node in Application.CurrentConfiguration.Cluster.Nodes) {
                if(CommonString.AreEquals(Application.CurrentConfiguration.Host.MachineUid.ToString(), node.MachineUid)) {
                    continue;
                }
                ConsoleLogger.Log($"[handshake] send to {node.Hostname}");

                var handshakeResult = ApiConsumer.Post($"{node.EntryPoint}cluster/handshake", dict);
                if(handshakeResult != HttpStatusCode.OK) {
                    ConsoleLogger.Log($"[handshake] error handshaking with {node.Hostname}");
                    return false;
                }
                //ottengo i servizi pubblicati da quel nodo
                ConsoleLogger.Log($"[handshake] get service details {node.Hostname}");
                var nodoDaModificare = nodes.FirstOrDefault(_ => _.MachineUid == node.MachineUid);
                if(nodoDaModificare != null) {
                    var publishedServices = ApiConsumer.Get<ClusterNodeService[]>($"{node.EntryPoint}device/services");
                    nodoDaModificare.Services = publishedServices;
                }
            }
            //ho fatto gli handshake, quindi il nodo richiesto è pronto per essere integrato nel cluster

            ConsoleLogger.Log("[handshake] adjust configuration");
            cluster.Active = true;
            if(cluster.Id == Guid.Empty) {
                cluster.Id = Guid.NewGuid();
            }
            cluster.Nodes = nodes.ToArray();

            cluster.SharedNetwork.Active = false;
            var virtualPorts = cluster.SharedNetwork.PortMapping.ToList();
            foreach(var node in nodes) {
                foreach(var svc in node.Services) {
                    var checkPort = virtualPorts.FirstOrDefault(_ => _.ServicePort == svc.Port.ToString());
                    if(checkPort == null) {
                        virtualPorts.Add(new PortMapping() {
                            ServiceName = svc.Name,
                            ServicePort = svc.Port.ToString(),
                            VirtualPort = string.Empty
                        });
                    }
                }
            }
            cluster.SharedNetwork.PortMapping = virtualPorts.ToArray();

            Application.CurrentConfiguration.Cluster = cluster;
            ConfigRepo.Save();
            return true;
        }

        public static bool Handshake(string apple) {
            ConsoleLogger.Log("[handshake] received handshake");
            //tipo      divisore    chiave  divisore    utente  divisore    host
            //ssh-rsa   [ ]         xxx     [ ]         root    [@]         localhost
            var info = apple.Split(new[] { ' ' }, 3, StringSplitOptions.RemoveEmptyEntries);
            var key = CommonString.Append(info[0], " ", info[1]);
            var keys = Application.CurrentConfiguration.Services.Ssh.AuthorizedKey.ToList();
            if(!keys.Any(_ => _.Key == key)) {
                ConsoleLogger.Log("[handshake] register new key");
                var userInfo = info[2].Split(new[] { '@' }, StringSplitOptions.RemoveEmptyEntries);
                var model = new AuthorizedKey {
                    User = userInfo[0],
                    Host = userInfo[1],
                    Key = key
                };
                keys.Add(model);
            }
            ConsoleLogger.Log("[handshake] save and apply key configuration");
            Application.CurrentConfiguration.Services.Ssh.AuthorizedKey = keys.ToArray();
            ConfigRepo.Save();
            Directory.CreateDirectory("/root/.ssh");
            const string authorizedKeysPath = "/root/.ssh/authorized_keys";
            if(File.Exists(authorizedKeysPath)) {
                var f = File.ReadAllText(authorizedKeysPath);
                if(!f.Contains(apple)) {
                    File.AppendAllLines(authorizedKeysPath, new List<string> { apple });
                }
            }
            else {
                File.WriteAllLines(authorizedKeysPath, new List<string> { apple });
            }
            return true;
        }
    }
}