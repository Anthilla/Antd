using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using antdlib.config;
using antdlib.models;
using anthilla.commands;
using anthilla.core;
using antdlib.config.shared;

namespace Antd {
    public class Do {
        private static string LocalTemplateDirectory = "/framework/antd/Templates";
        private static readonly Dictionary<string, string> _replacements = new Dictionary<string, string>();

        private static readonly Host2Model _host = Host2Configuration.Host;

        #region [    Main Functions    ]
        public static void AllChanges() {
            ParametersChangesPre();
            NetworkChanges();
            HostChanges();
            ParametersChangesPost();
            ClusterChanges();
        }

        public static void HostChanges() {
            SaveHostname();
            ApplyNtpdate();
            //SaveNtpFile();
            SaveResolvFile();
            SaveNsswitchFile();
            //SaveNetworksFile();
            //SaveHostsFile();

            //SaveDhcpdFile();
            //SaveNamedFile();
            //SaveNftablesFile();
        }

        public static void NetworkChanges() {
            ApplyDefaultNetworkConfiguration();
            ApplyNetworkConfiguration();
        }

        public static void ParametersChanges() {
            ParametersChangesPre();
            ParametersChangesPost();
        }

        public static void ParametersChangesPre() {
            LaunchStart();
            SaveOsParameters();
            ModulesChanges();
            StartService();
            StopService();
        }

        public static void ModulesChanges() {
            SaveModprobes();
            RemoveModules();
            BlacklistMudules();
        }

        public static void ParametersChangesPost() {
            LaunchEnd();
        }

        public static void ClusterChanges() {
            ConsoleLogger.Log("[cluster] applying changes");
            ClusterConfiguration.Prepare();
            var clusterConfig = ClusterConfiguration.GetClusterInfo();
            var publicIp = clusterConfig.VirtualIpAddress;
            if(string.IsNullOrEmpty(publicIp)) {
                ConsoleLogger.Warn("[cluster] public ip not valid");
                return;
            }
            var nodes = ClusterConfiguration.GetNodes();
            if(!nodes.Any()) {
                ConsoleLogger.Warn("[cluster] configuration not valid");
                return;
            }

            SaveKeepalived(publicIp, nodes);
            SaveHaproxy(publicIp, nodes);

            SaveFileSystemSync(clusterConfig, nodes);
        }
        #endregion

        #region [    keepalived    ]
        private static readonly string KeepalivedFileOutput = $"{Parameter.AntdCfgCluster}/keepalived.conf";

        private static void SaveKeepalived(string publicIp, List<NodeModel> nodes) {
            ConsoleLogger.Log("[cluster] init keepalived");
            const string keepalivedService = "keepalived.service";
            if(Systemctl.IsActive(keepalivedService)) {
                ConsoleLogger.Log("[cluster] stop service");
                Systemctl.Stop(keepalivedService);
            }
            ConsoleLogger.Log("[cluster] set configuration file");
            var clusterInfo = ClusterConfiguration.GetClusterInfo();
            var lines = new List<string> {
                "vrrp_script chk_haproxy {",
                "    script \"killall -0 haproxy\"",
                "    interval 2",
                "    weight 2",
                "}",
                "",
                "vrrp_instance RH_INT {",
                $"    interface {clusterInfo.NetworkInterface}",
                "    state MASTER",
                "    virtual_router_id 51",
                $"    priority {clusterInfo.Priority}",
                "    virtual_ipaddress {",
                $"        {clusterInfo.VirtualIpAddress}",
                "    }",
                "    track_script {",
                "        chk_haproxy",
                "    }",
                "}",
            };
            FileWithAcl.WriteAllLines(KeepalivedFileOutput, lines);
            //Systemctl.Enable(keepalivedService);
            //ConsoleLogger.Log("[cluster] keepalived enabled");
            //Systemctl.Restart(keepalivedService);
            //ConsoleLogger.Log("[cluster] keepalived restarted");
            Bash.Execute("killall keepalived");
            Bash.Execute($"keepalived -f {KeepalivedFileOutput}");
        }

        private static readonly string _haproxyFileOutput = $"{Parameter.AntdCfgCluster}/haproxy.conf";

        private static void SaveHaproxy(string publicIp, List<NodeModel> nodes) {
            ConsoleLogger.Log("[cluster] init haproxy");
            CommandLauncher.Launch("haproxy-stop");
            if(File.Exists(_haproxyFileOutput)) {
                File.Copy(_haproxyFileOutput, $"{_haproxyFileOutput}.bck", true);
            }
            ConsoleLogger.Log("[cluster] set haproxy file");
            var clusterInfo = ClusterConfiguration.GetClusterInfo();
            var ports = clusterInfo.PortMapping;
            if(!ports.Any()) {
                return;
            }
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
            var localport = new AppConfiguration().Get().AntdUiPort;
            var localServices = ApiConsumer.Get<List<RssdpServiceModel>>($"http://localhost:{localport}/device/services");
            if(localServices != null) {
                foreach(var portMapping in ports) {
                    portMapping.ServicePort = localServices.FirstOrDefault(_ => _.Name == portMapping.ServiceName)?.Port;
                    if(string.IsNullOrEmpty(portMapping.ServicePort)) {
                        continue;
                    }
                    var frontEndLabel = $"fe_in{portMapping.ServicePort}_out{portMapping.VirtualPort}";
                    var backEndLabel = $"be_in{portMapping.ServicePort}_out{portMapping.VirtualPort}";
                    lines.Add($"frontend {frontEndLabel}");
                    lines.Add("    mode http");
                    lines.Add($"    bind {clusterInfo.VirtualIpAddress}:{portMapping.VirtualPort} transparent");
                    lines.Add("    stats enable");
                    lines.Add("    stats auth admin:Anthilla");
                    lines.Add("    option httpclose");
                    lines.Add("    option forwardfor");
                    lines.Add($"    default_backend {backEndLabel}");
                    lines.Add("");
                    lines.Add($"backend {backEndLabel}");
                    lines.Add("    balance roundrobin");
                    lines.Add("    cookie JSESSIONID prefix");
                    lines.Add("    option httpchk HEAD /check.txt HTTP/1.0");
                    foreach(var node in nodes) {
                        lines.Add($"    server {node.Hostname} {node.PublicIp}:{portMapping.ServicePort} check");
                    }
                    lines.Add("");
                }
            }

            File.WriteAllLines(_haproxyFileOutput, lines);
            CommandLauncher.Launch("haproxy-start", new Dictionary<string, string> { { "$file", _haproxyFileOutput } });
            ConsoleLogger.Log("[cluster] haproxy started");
        }

        private static void SaveFileSystemSync(Cluster.Configuration config, List<NodeModel> nodes) {
            Application.VfsWatcher.Stop();
            Application.VfsWatcher.Start(config, nodes);
        }
        #endregion

        #region [    private functions    ]
        private static string EditLine(string input) {
            var output = input;
            foreach(var r in _replacements) {
                output = output.Replace(r.Key, r.Value);
            }
            return output;
        }

        private static void ImportTemplate(string src, string dest) {
            if(!File.Exists(dest)) {
                File.Copy(src, dest);
            }
        }

        //private  static void ActivateService(string svc) {
        //    if(Systemctl.IsEnabled(svc) == false) {
        //        Systemctl.Enable(svc);
        //    }
        //    if(Systemctl.IsActive(svc) == false) {
        //        Systemctl.Restart(svc);
        //    }
        //}
        #endregion

        #region [    network    ]
        private static void ApplyDefaultNetworkConfiguration() {
            var ifs = Network2Configuration.InterfacePhysical.ToArray();
            foreach(var nif in ifs) {
                CommandLauncher.Launch("ip4-set-mtu", new Dictionary<string, string> { { "$net_if", nif }, { "$mtu", "6000" } });
                CommandLauncher.Launch("ip4-set-txqueuelen", new Dictionary<string, string> { { "$net_if", nif }, { "$txqueuelen", "10000" } });
                CommandLauncher.Launch("ip4-promisc-on", new Dictionary<string, string> { { "$net_if", nif } });
                CommandLauncher.Launch("ip4-enable-if", new Dictionary<string, string> { { "$net_if", nif } });
            }
            var brs = Network2Configuration.InterfaceBridge.ToArray();
            foreach(var nif in brs) {
                CommandLauncher.Launch("ip4-set-mtu", new Dictionary<string, string> { { "$net_if", nif }, { "$mtu", "6000" } });
                CommandLauncher.Launch("ip4-set-txqueuelen", new Dictionary<string, string> { { "$net_if", nif }, { "$txqueuelen", "10000" } });
                CommandLauncher.Launch("ip4-promisc-on", new Dictionary<string, string> { { "$net_if", nif } });
                CommandLauncher.Launch("ip4-enable-if", new Dictionary<string, string> { { "$net_if", nif } });
            }
        }

        private static void ApplyNetworkConfiguration() {
            CreateAggregatedInterface();
            var configurations = Network2Configuration.Conf.Interfaces;
            if(!configurations.Any()) {
                ConsoleLogger.Log("[network] configurations not found: create defaults");
                CreateDefaultNetworkConfiguration();
            }
            var interfaceConfigurations = Network2Configuration.InterfaceConfigurationList;
            var gatewayConfigurations = Network2Configuration.GatewayConfigurationList;
            foreach(var configuration in configurations) {
                var ifConfig = interfaceConfigurations.FirstOrDefault(_ => _.Id == configuration.Configuration);
                if(ifConfig == null) {
                    continue;
                }
                var gwConfig = gatewayConfigurations.FirstOrDefault(_ => _.Id == configuration.GatewayConfiguration);
                SetInterface(configuration, ifConfig, gwConfig);
                foreach(var confGuid in configuration.AdditionalConfigurations) {
                    var conf = interfaceConfigurations.FirstOrDefault(_ => _.Id == confGuid);
                    if(conf == null) {
                        continue;
                    }
                    SetInterface(configuration, conf, null);
                }
            }
            SetAggregatedInterface();
        }

        private static void CreateAggregatedInterface() {
            var confList = Network2Configuration.NetworkAggregatedInterfaceConfigurationList;
            foreach(var conf in confList) {
                if(conf.Parent.StartsWith("br")) {
                    CommandLauncher.Launch("brctl-add", new Dictionary<string, string> { { "$bridge", conf.Parent } });
                }
                else if(conf.Parent.StartsWith("bond")) {
                    CommandLauncher.Launch("bond-set", new Dictionary<string, string> { { "$bond", conf.Parent } });
                }
            }
        }

        private static void SetAggregatedInterface() {
            var confList = Network2Configuration.NetworkAggregatedInterfaceConfigurationList;
            var brs = Network2Configuration.InterfaceBridge;
            foreach(var deviceName in brs) {
                var conf = confList.FirstOrDefault(_ => _.Parent == deviceName);
                if(conf == null) {
                    ConsoleLogger.Log($"[network] no children configured for '{deviceName}'");
                    return;
                }
                foreach(var nif in conf.Children) {
                    ConsoleLogger.Log($"[network] add '{nif}' to bridge '{deviceName}'");
                    CommandLauncher.Launch("brctl-add-if", new Dictionary<string, string> { { "$bridge", deviceName }, { "$net_if", nif } });
                    CommandLauncher.Launch("ip4-flush-configuration", new Dictionary<string, string> { { "$net_if", nif } });
                    CommandLauncher.Launch("ip4-enable-if", new Dictionary<string, string> { { "$net_if", nif } });
                }
            }
            //var bonds = Network2Configuration.InterfaceBond;
            //foreach(var deviceName in brs) {
            //    var conf = confList.FirstOrDefault(_ => _.Parent == deviceName);
            //    if(conf == null) {
            //        ConsoleLogger.Log($"[network] no children configured for '{deviceName}'");
            //        return;
            //    }
            //    foreach(var nif in conf.Children) {
            //        ConsoleLogger.Log($"[network] add '{nif}' to bond '{deviceName}'");
            //        CommandLauncher.Launch("bond-add-if", new Dictionary<string, string> { { "$bond", deviceName }, { "$net_if", nif } });
            //        CommandLauncher.Launch("ip4-flush-configuration", new Dictionary<string, string> { { "$net_if", nif } });
            //        CommandLauncher.Launch("ip4-enable-if", new Dictionary<string, string> { { "$net_if", nif } });
            //    }
            //}
        }

        private static void SetInterface(NetworkInterface configuration, NetworkInterfaceConfiguration interfaceConfiguration, NetworkGatewayConfiguration gatewayConfiguration) {
            if(interfaceConfiguration == null) {
                return;
            }

            var deviceName = configuration.Device;

            var nAt = NetworkAdapterType.Other;
            if(Network2Configuration.InterfacePhysical.Contains(deviceName)) {
                nAt = NetworkAdapterType.Physical;
            }
            else if(Network2Configuration.InterfaceBridge.Contains(deviceName)) {
                nAt = NetworkAdapterType.Bridge;
            }
            else if(Network2Configuration.InterfaceBond.Contains(deviceName)) {
                nAt = NetworkAdapterType.Bond;
            }
            else if(Network2Configuration.InterfaceVirtual.Contains(deviceName)) {
                nAt = NetworkAdapterType.Virtual;
            }

            switch(nAt) {
                case NetworkAdapterType.Physical:
                    break;
                case NetworkAdapterType.Virtual:
                    break;
                case NetworkAdapterType.Bond:
                    //var cconfList = Network2Configuration.NetworkAggregatedInterfaceConfigurationList;
                    //var vconf = cconfList.FirstOrDefault(_ => _.Parent == deviceName);
                    //if(vconf == null) {
                    //    ConsoleLogger.Log($"[network] no children for '{deviceName}'");
                    //    return;
                    //}

                    //foreach(var nif in vconf.Children) {
                    //    CommandLauncher.Launch("bond-add-if", new Dictionary<string, string> { { "$bond", deviceName }, { "$net_if", nif } });
                    //    CommandLauncher.Launch("ip4-flush-configuration", new Dictionary<string, string> { { "$net_if", nif } });
                    //    CommandLauncher.Launch("ip4-enable-if", new Dictionary<string, string> { { "$net_if", nif } });
                    //}
                    break;
                case NetworkAdapterType.Bridge:
                    //    var confList = Network2Configuration.NetworkAggregatedInterfaceConfigurationList;
                    //    var conf = confList.FirstOrDefault(_ => _.Parent == deviceName);
                    //    if(conf == null) {
                    //        ConsoleLogger.Log($"[network] no children for '{deviceName}'");
                    //        return;
                    //    }

                    //    foreach(var nif in conf.Children) {
                    //        ConsoleLogger.Log($"[network] add '{nif}' to bridge '{deviceName}'");
                    //        CommandLauncher.Launch("brctl-add-if", new Dictionary<string, string> { { "$bridge", deviceName }, { "$net_if", nif } });
                    //        CommandLauncher.Launch("ip4-flush-configuration", new Dictionary<string, string> { { "$net_if", nif } });
                    //        CommandLauncher.Launch("ip4-enable-if", new Dictionary<string, string> { { "$net_if", nif } });
                    //    }
                    break;
                case NetworkAdapterType.Other:
                    return;
            }

            CommandLauncher.Launch("ip4-set-mtu", new Dictionary<string, string> { { "$net_if", deviceName }, { "$mtu", "6000" } });
            CommandLauncher.Launch("ip4-set-txqueuelen", new Dictionary<string, string> { { "$net_if", deviceName }, { "$txqueuelen", "10000" } });
            CommandLauncher.Launch("ip4-promisc-on", new Dictionary<string, string> { { "$net_if", deviceName } });

            switch(interfaceConfiguration.Mode) {
                case NetworkInterfaceMode.Null:
                    return;
                case NetworkInterfaceMode.Static:
                    var networkdIsActive = Systemctl.IsActive("systemd-networkd");
                    if(networkdIsActive) {
                        Systemctl.Stop("systemd-networkd");
                    }
                    CommandLauncher.Launch("dhcpcd-killall");
                    CommandLauncher.Launch("ip4-flush-configuration", new Dictionary<string, string> {
                        { "$net_if", deviceName}
                    });
                    CommandLauncher.Launch("ip4-add-addr", new Dictionary<string, string> {
                        { "$net_if", deviceName},
                        { "$address", interfaceConfiguration.Ip },
                        { "$range", interfaceConfiguration.Subnet }
                    });
                    if(networkdIsActive) {
                        Systemctl.Start("systemd-networkd");
                    }
                    break;
                case NetworkInterfaceMode.Dynamic:
                    CommandLauncher.Launch("dhcpcd", new Dictionary<string, string> { { "$net_if", deviceName } });
                    break;
                default:
                    return;
            }

            switch(configuration.Status) {
                case NetworkInterfaceStatus.Down:
                    CommandLauncher.Launch("ip4-disable-if", new Dictionary<string, string> { { "$net_if", deviceName } });
                    return;
                case NetworkInterfaceStatus.Up:
                    CommandLauncher.Launch("ip4-enable-if", new Dictionary<string, string> { { "$net_if", deviceName } });
                    ConsoleLogger.Log($"[network] interface '{deviceName}' configured");
                    break;
                default:
                    CommandLauncher.Launch("ip4-disable-if", new Dictionary<string, string> { { "$net_if", deviceName } });
                    return;
            }

            if(gatewayConfiguration == null) {
                return;
            }
            CommandLauncher.Launch("ip4-add-route", new Dictionary<string, string> { { "$net_if", deviceName }, { "$ip_address", "default" }, { "$gateway", gatewayConfiguration.GatewayAddress } });
        }

        private static void CreateDefaultNetworkConfiguration() {
            Network2Configuration.AddInterfaceConfiguration(Default.InternalPhysicalInterfaceConfiguration());
            Network2Configuration.AddInterfaceConfiguration(Default.ExternalPhysicalInterfaceConfiguration());
            Network2Configuration.AddInterfaceConfiguration(Default.InternalBridgeInterfaceConfiguration());
            Network2Configuration.AddInterfaceConfiguration(Default.ExternalBridgeInterfaceConfiguration());
            Network2Configuration.AddGatewayConfiguration(Default.GatewayConfiguration());
            Network2Configuration.AddDnsConfiguration(Default.PublicDnsConfiguration());
            Network2Configuration.AddDnsConfiguration(Default.PrivateInternalDnsConfiguration());
            Network2Configuration.AddDnsConfiguration(Default.PrivateExternalDnsConfiguration());
            var devs = Network2Configuration.InterfacePhysical.ToList();
            var partIp = Default.InternalPhysicalInterfaceConfiguration().Ip.Split('.').Take(3).ToArray().JoinToString(".");
            var counter = 200;
            var list = new List<NetworkInterface>();
            foreach(var dev in devs) {
                var conf = Default.InternalPhysicalInterfaceConfiguration($"{partIp}.{counter}");
                Network2Configuration.AddInterfaceConfiguration(conf);
                var networkInterface = new NetworkInterface {
                    Device = dev,
                    Configuration = conf.Id,
                    GatewayConfiguration = Default.GatewayConfiguration().Id,
                    AdditionalConfigurations = new List<string>()
                };
                list.Add(networkInterface);
                counter = counter + 1;
            }
            Network2Configuration.SaveInterfaceSetting(list);
            Network2Configuration.SetDnsConfigurationActive(Default.PublicDnsConfiguration().Id);
        }

        //public static void SetupResolvD() {
        //    if(!File.Exists("/etc/systemd/resolved.conf") || string.IsNullOrEmpty(File.ReadAllText("/etc/systemd/resolved.conf"))) {
        //        var lines = new List<string> {
        //            "[Resolve]",
        //            "DNS=10.1.19.1 10.99.19.1",
        //            "FallbackDNS=8.8.8.8 8.8.4.4 2001:4860:4860::8888 2001:4860:4860::8844",
        //            "Domains=antd.local",
        //            "LLMNR=yes"
        //        };
        //        File.WriteAllLines("/etc/systemd/resolved.conf", lines);
        //    }
        //    _launcher.Launch("systemctl-daemonreload");
        //    _launcher.Launch("systemctl-restart", new Dictionary<string, string> { { "$service", "systemd-resolved" } });
        //}
        #endregion

        #region [    hostnamectl    ]
        private static void SaveHostname() {
            CommandLauncher.Launch("set-hostname", new Dictionary<string, string> {
                { "$host_name", _host.HostName }
            });
            CommandLauncher.Launch("set-chassis", new Dictionary<string, string> {
                { "$host_chassis", _host.HostChassis }
            });
            CommandLauncher.Launch("set-deployment", new Dictionary<string, string> {
                { "$host_deployment", _host.HostDeployment }
            });
            CommandLauncher.Launch("set-location", new Dictionary<string, string> {
                { "$host_location", _host.HostLocation }
            });
            File.WriteAllText("/etc/hostname", _host.HostName);
            CommandLauncher.Launch("set-timezone", new Dictionary<string, string> {
                { "$host_timezone", _host.Timezone }
            });
        }
        #endregion

        #region [    ntpdate    ]
        private static void ApplyNtpdate() {
            CommandLauncher.Launch("ntpdate", new Dictionary<string, string> { { "$server", _host.NtpdateServer } });
        }
        #endregion

        #region [    ntp.conf    ]
        private static readonly string _ntpFileInput = $"{LocalTemplateDirectory}/ntp.conf.tmlp";
        private const string NtpFileOutput = "/etc/ntp.conf";

        private static void SaveNtpFile() {
            using(var reader = new StreamReader(_ntpFileInput)) {
                using(TextWriter writer = File.CreateText(NtpFileOutput)) {
                    string line;
                    while((line = reader.ReadLine()) != null) {
                        writer.WriteLine(EditLine(line));
                    }
                }
            }
        }
        #endregion

        #region [    DNS client - resolv.conf    ]
        private const string ResolvFileOutput = "/etc/resolv.conf";

        private static void SaveResolvFile() {
            string[] lines;
            if(string.IsNullOrEmpty(_host.ResolvDomain)) {
                lines = new[] {
                    "nameserver 8.8.8.8",
                    "nameserver 8.8.4.4"
                };
            }
            else {
                lines = new[] {
                    $"nameserver {_host.ResolvNameserver}",
                    $"search {_host.ResolvDomain}",
                    $"domain {_host.ResolvDomain}"
                };
            }
            using(TextWriter writer = File.CreateText(ResolvFileOutput)) {
                foreach(var line in lines) {
                    writer.WriteLine(line);
                }
            }
        }
        #endregion

        #region [    nsswitch.conf    ]
        private static readonly string _nsswitchFileInput = $"{LocalTemplateDirectory}/nsswitch.conf.tmlp";
        private const string NsswitchFileOutput = "/etc/nsswitch.conf";

        private static void SaveNsswitchFile() {
            using(var reader = new StreamReader(_nsswitchFileInput)) {
                using(TextWriter writer = File.CreateText(NsswitchFileOutput)) {
                    string line;
                    while((line = reader.ReadLine()) != null) {
                        writer.WriteLine(EditLine(line));
                    }
                }
            }
        }
        #endregion

        #region [    networks    ]
        private static readonly string _networksFileInput = $"{LocalTemplateDirectory}/networks.tmlp";
        private const string NetworksFileOutput = "/etc/networks";

        private static void SaveNetworksFile() {
            using(var reader = new StreamReader(_networksFileInput)) {
                using(TextWriter writer = File.CreateText(NetworksFileOutput)) {
                    string line;
                    while((line = reader.ReadLine()) != null) {
                        writer.WriteLine(EditLine(line));
                    }
                }
            }
        }
        #endregion

        #region [    hosts    ]
        private static readonly string _hostsFileInput = $"{LocalTemplateDirectory}/hosts.tmlp";
        private const string HostsFileOutput = "/etc/hosts";

        private static void SaveHostsFile() {
            using(var reader = new StreamReader(_hostsFileInput)) {
                using(TextWriter writer = File.CreateText(HostsFileOutput)) {
                    string line;
                    while((line = reader.ReadLine()) != null) {
                        writer.WriteLine(EditLine(line));
                    }
                }
            }
        }
        #endregion

        #region [    dhcpd.conf    ]
        private static void SaveDhcpdFile() {
            Directory.CreateDirectory("/etc/dhcp");
            Directory.CreateDirectory(BindDirectory);
            Directory.CreateDirectory(BindZonesDirectory);
            var newModel = DhcpdConfiguration.Get();
            newModel.ZoneName = _host.InternalDomainPrimary;
            newModel.ZonePrimaryAddress = _host.InternalHostIpPrimary;
            //newModel.SubnetIpFamily = EnumerableExtensions.JoinToString(_host.InternalHostIpPrimary.Split('.').Take(2), ".").TrimEnd('.') + ".0.0";
            //newModel.SubnetIpMask = _host.InternalBroadcastPrimary;
            //newModel.SubnetOptionRouters = _host.InternalHostIpPrimary;
            //newModel.SubnetNtpServers = _host.InternalHostIpPrimary;
            //newModel.SubnetTimeServers = _host.InternalHostIpPrimary;
            //newModel.SubnetDomainNameServers = _host.InternalHostIpPrimary;
            //newModel.SubnetBroadcastAddress = _host.InternalHostIpPrimary;
            //newModel.SubnetMask = _host.InternalBroadcastPrimary;
            newModel.DdnsDomainName = _host.InternalDomainPrimary;
            DhcpdConfiguration.Save(newModel);
            DhcpdConfiguration.Set();
        }
        #endregion

        #region [    named.conf    ]
        private const string BindDirectory = "/etc/bind";
        private const string BindZonesDirectory = "/etc/bind/zones";

        private static void SaveNamedFile() {
            Directory.CreateDirectory(BindDirectory);
            Directory.CreateDirectory(BindZonesDirectory);
            var newModel = BindConfiguration.Get();
            if(!newModel.Forwarders.Contains(_host.InternalHostIpPrimary)) {
                newModel.Forwarders.Add(_host.InternalHostIpPrimary);
            }
            if(!newModel.Forwarders.Contains(_host.ExternalHostIpPrimary)) {
                newModel.Forwarders.Add(_host.ExternalHostIpPrimary);
            }
            newModel.ControlIp = _host.InternalHostIpPrimary;
            //if(!newModel.AclInternalInterfaces.Contains(_host.InternalHostIpPrimary)) {
            //    newModel.AclInternalInterfaces.Add(_host.InternalHostIpPrimary);
            //}
            //if(!newModel.AclExternalInterfaces.Contains(_host.ExternalHostIpPrimary)) {
            //    newModel.AclExternalInterfaces.Add(_host.ExternalHostIpPrimary);
            //}
            //if(!newModel.AclInternalNetworks.Contains(_host.InternalNetPrimary)) {
            //    newModel.AclInternalNetworks.Add(_host.InternalNetPrimary);
            //}
            //if(!newModel.AclExternalNetworks.Contains(_host.ExternalNetPrimary)) {
            //    newModel.AclExternalNetworks.Add(_host.ExternalNetPrimary);
            //}
            var zones = newModel.Zones;
            var internalZoneName = _host.InternalDomainPrimary;
            if(newModel.Zones.FirstOrDefault(_ => _.Name == internalZoneName) == null) {
                var filePath = $"{BindZonesDirectory}/host.{internalZoneName}.db";
                var z = new BindConfigurationZoneModel {
                    Guid = Guid.NewGuid().ToString(),
                    File = filePath,
                    SerialUpdateMethod = "unixtime",
                    AllowUpdate = new List<string> { "loif", "iif", "lonet", "inet", "onet", "key updbindkey" },
                    AllowQuery = new List<string> { "any" },
                    AllowTransfer = new List<string> { "loif", "iif", "lonet", "inet", "onet" }
                };
                zones.Add(z);
            }
            var internalReverseZoneName = _host.InternalArpaPrimary;
            if(newModel.Zones.FirstOrDefault(_ => _.Name == internalReverseZoneName) == null) {
                var filePath = $"{BindZonesDirectory}/rev.{internalReverseZoneName}.db";
                var z = new BindConfigurationZoneModel {
                    Guid = Guid.NewGuid().ToString(),
                    File = filePath,
                    SerialUpdateMethod = "unixtime",
                    AllowUpdate = new List<string> { "loif", "iif", "lonet", "inet", "onet", "key updbindkey" },
                    AllowQuery = new List<string> { "any" },
                    AllowTransfer = new List<string> { "loif", "iif", "lonet", "inet", "onet" }
                };
                zones.Add(z);
            }
            var externalZoneName = _host.ExternalDomainPrimary;
            if(newModel.Zones.FirstOrDefault(_ => _.Name == externalZoneName) == null) {
                var filePath = $"{BindZonesDirectory}/host.{externalZoneName}.db";
                var z = new BindConfigurationZoneModel {
                    Guid = Guid.NewGuid().ToString(),
                    File = filePath,
                    SerialUpdateMethod = "unixtime",
                    AllowUpdate = new List<string> { "loif", "iif", "lonet", "inet", "onet", "key updbindkey" },
                    AllowQuery = new List<string> { "any" },
                    AllowTransfer = new List<string> { "loif", "iif", "lonet", "inet", "onet" }
                };
                zones.Add(z);
            }
            var externalReverseZoneName = _host.ExternalArpaPrimary;
            if(newModel.Zones.FirstOrDefault(_ => _.Name == externalReverseZoneName) == null) {
                var filePath = $"{BindZonesDirectory}/rev.{externalReverseZoneName}.db";
                var z = new BindConfigurationZoneModel {
                    Guid = Guid.NewGuid().ToString(),
                    File = filePath,
                    SerialUpdateMethod = "unixtime",
                    AllowUpdate = new List<string> { "loif", "iif", "lonet", "inet", "onet", "key updbindkey" },
                    AllowQuery = new List<string> { "any" },
                    AllowTransfer = new List<string> { "loif", "iif", "lonet", "inet", "onet" }
                };
                zones.Add(z);
            }
            newModel.Zones = zones;

            var zonesFile = newModel.ZoneFiles;
            if(newModel.ZoneFiles.FirstOrDefault(_ => _.Name == $"{BindZonesDirectory}/host.{internalZoneName}.db") == null) {
                var filePath = $"{BindZonesDirectory}/host.{internalZoneName}.db";
                var z = new BindConfigurationZoneFileModel {
                    Guid = Guid.NewGuid().ToString(),
                    Name = filePath,
                    Configuration = "unixtime"
                };
                zonesFile.Add(z);
                File.WriteAllLines(filePath, BindConfiguration.GetHostZoneText(_host.HostName, _host.InternalDomainPrimary, _host.InternalHostIpPrimary));
            }
            if(newModel.ZoneFiles.FirstOrDefault(_ => _.Name == $"{BindZonesDirectory}/rev.{internalReverseZoneName}.db") == null) {
                var filePath = $"{BindZonesDirectory}/rev.{internalReverseZoneName}.db";
                var z = new BindConfigurationZoneFileModel {
                    Guid = Guid.NewGuid().ToString(),
                    Name = filePath,
                    Configuration = "unixtime"
                };
                zonesFile.Add(z);
                File.WriteAllLines(filePath, BindConfiguration.GetReverseZoneText(_host.HostName, _host.InternalDomainPrimary, _host.InternalArpaPrimary, _host.InternalHostIpPrimary.Split('.').Skip(2).ToArray().JoinToString(".")));
            }
            if(newModel.ZoneFiles.FirstOrDefault(_ => _.Name == $"{BindZonesDirectory}/host.{externalZoneName}.db") == null) {
                var filePath = $"{BindZonesDirectory}/host.{externalZoneName}.db";
                var z = new BindConfigurationZoneFileModel {
                    Guid = Guid.NewGuid().ToString(),
                    Name = filePath,
                    Configuration = "unixtime"
                };
                zonesFile.Add(z);
                File.WriteAllLines(filePath, BindConfiguration.GetHostZoneText(_host.HostName, _host.ExternalDomainPrimary, _host.ExternalHostIpPrimary));
            }
            if(newModel.ZoneFiles.FirstOrDefault(_ => _.Name == $"{BindZonesDirectory}/rev.{externalReverseZoneName}.db") == null) {
                var filePath = $"{BindZonesDirectory}/rev.{externalReverseZoneName}.db";
                var z = new BindConfigurationZoneFileModel {
                    Guid = Guid.NewGuid().ToString(),
                    Name = filePath,
                    Configuration = "unixtime"
                };
                zonesFile.Add(z);
                File.WriteAllLines(filePath, BindConfiguration.GetReverseZoneText(_host.HostName, _host.ExternalDomainPrimary, _host.ExternalArpaPrimary, _host.ExternalHostIpPrimary.Split('.').Skip(2).ToArray().JoinToString(".")));
            }
            newModel.ZoneFiles = zonesFile;
            BindConfiguration.Save(newModel);
            BindConfiguration.Set();
        }
        #endregion

        #region [    rndc.conf    ]
        private static readonly string _rndcFileInput = $"{LocalTemplateDirectory}/rndc.conf.tmpl";
        private const string RndcFileOutput = "/etc/bind/rndc.conf";

        private static void SaveRndcFile() {
            using(var reader = new StreamReader(_rndcFileInput)) {
                using(TextWriter writer = File.CreateText(RndcFileOutput)) {
                    string line;
                    while((line = reader.ReadLine()) != null) {
                        writer.WriteLine(EditLine(line));
                    }
                }
            }
        }
        #endregion

        #region [    bind files    ]
        private static void SaveBindFiles() {
            Directory.CreateDirectory("/etc/bind");
            Directory.CreateDirectory("/etc/bind/zones");
            ImportTemplate($"{LocalTemplateDirectory}/blackhole.zones.tmpl", "/etc/bind/blackhole.zones");
            ImportTemplate($"{LocalTemplateDirectory}/zones_blockeddomain.hosts.tmpl", "/etc/bind/zones/blockeddomain.hosts");
            ImportTemplate($"{LocalTemplateDirectory}/zones_empty.db.tmpl", "/etc/bind/zones/empty.db");
            ImportTemplate($"{LocalTemplateDirectory}/zones_localhost-forward.db.tmpl", "/etc/bind/zones/localhost-forward.db");
            ImportTemplate($"{LocalTemplateDirectory}/zones_localhost-reverse.db.tmpl", "/etc/bind/zones/localhost-reverse.db");
        }

        private static void SaveBindZones() {
            var hostZoneFileInput = $"{LocalTemplateDirectory}/zones_host.domint.local.db.tmpl";
            var hostZoneFileOutput = $"/etc/bind/zones/host.{_host.InternalDomainPrimary}.db";
            using(var reader = new StreamReader(hostZoneFileInput)) {
                using(TextWriter writer = File.CreateText(hostZoneFileOutput)) {
                    string line;
                    while((line = reader.ReadLine()) != null) {
                        writer.WriteLine(EditLine(line));
                    }
                }
            }

            var revZoneFileInput = $"{LocalTemplateDirectory}/zones_rev.10.11.0.0.db.tmpl";
            var revZoneFileOutput = $"/etc/bind/zones/rev.{_host.InternalNetPrimary}.db";
            using(var reader = new StreamReader(revZoneFileInput)) {
                using(TextWriter writer = File.CreateText(revZoneFileOutput)) {
                    string line;
                    while((line = reader.ReadLine()) != null) {
                        writer.WriteLine(EditLine(line));
                    }
                }
            }
        }
        #endregion

        #region [    Nftable    ]
        private static readonly string _nftablesFileInput = $"{LocalTemplateDirectory}/nftables.conf.tmpl";
        private const string NftablesFileOutput = "/etc/nftables.conf";

        private static void SaveNftablesFile() {
            using(var reader = new StreamReader(_nftablesFileInput)) {
                using(TextWriter writer = File.CreateText(NftablesFileOutput)) {
                    string line;
                    while((line = reader.ReadLine()) != null) {
                        writer.WriteLine(EditLine(line));
                    }
                }
            }
            CommandLauncher.Launch("nft-f", new Dictionary<string, string> { { "$file", NftablesFileOutput } });
        }
        #endregion

        #region [    modules    ]
        public static void SaveModprobes() {
            var modules = HostParametersConfiguration.Conf.Modprobes;
            foreach(var mod in modules) {
                ConsoleLogger.Log($"load module: {mod}");
                Bash.Execute($"modprobe {mod}");
                CommandLauncher.Launch("modprobe", new Dictionary<string, string> { { "$package", mod } });
            }
        }

        public static void RemoveModules() {
            var modules = string.Join(" ", HostParametersConfiguration.Conf.Rmmod);
            CommandLauncher.Launch("rmmod", new Dictionary<string, string> { { "$modules", modules } });
        }

        public static void BlacklistMudules() {
            if(!File.Exists("/etc/modprobe.d/blacklist.conf")) { return; }
            File.WriteAllLines("/etc/modprobe.d/blacklist.conf", HostParametersConfiguration.Conf.ModulesBlacklist.Select(_ => $"blacklist {_}"));
        }
        #endregion

        #region [    services    ]
        public static void StartService() {
            var svcs = HostParametersConfiguration.Conf.ServicesStart;
            foreach(var svc in svcs) {
                if(Systemctl.IsEnabled(svc) == false) {
                    Systemctl.Enable(svc);
                }
                if(Systemctl.IsActive(svc) == false) {
                    Systemctl.Start(svc);
                }
            }
        }

        public static void StopService() {
            var svcs = HostParametersConfiguration.Conf.ServicesStop;
            foreach(var svc in svcs) {
                if(Systemctl.IsEnabled(svc)) {
                    Systemctl.Disable(svc);
                }
                if(Systemctl.IsActive(svc)) {
                    Systemctl.Stop(svc);
                }
            }
        }
        #endregion

        #region [    os parameters    ]
        public static void SaveOsParameters() {
            var parameters = HostParametersConfiguration.Conf.OsParameters;
            foreach(var par in parameters) {
                if(!par.Contains(" ")) { continue; }
                var arr = par.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                if(arr.Length != 2) { continue; }
                var file = arr[0];
                if(!File.Exists(file)) { continue; }
                var value = arr[1];
                File.WriteAllText(file, value);
            }
        }
        #endregion

        #region [    commands    ]
        public static void LaunchStart() {
            var controls = HostParametersConfiguration.Conf.StartCommands;
            foreach(var control in controls) {
                Launch(control);
            }
        }

        public static void LaunchEnd() {
            var controls = HostParametersConfiguration.Conf.EndCommands;
            foreach(var control in controls) {
                Launch(control);
            }
        }

        private static void Launch(Control control) {
            if(control?.FirstCommand == null) {
                return;
            }
            var firstCommand = control.FirstCommand;
            if(string.IsNullOrEmpty(control.ControlCommand)) {
                ConsoleLogger.Log($"[setup.conf] {control.FirstCommand}");
                Bash.Execute(firstCommand, false);
                return;
            }
            var controlCommand = control.ControlCommand;
            var controlResult = Bash.Execute(controlCommand);
            var firstCheck = controlResult.Contains(control.Check);
            if(firstCheck) {
                return;
            }
            Bash.Execute(firstCommand, false);
            controlResult = Bash.Execute(controlCommand);
            var secondCheck = controlResult.Contains(control.Check);
            if(secondCheck) {
                return;
            }
            Launch(control);
        }
        #endregion
    }
}
