using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using antdlib.common;
using antdlib.config;
using antdlib.models;
using anthilla.commands;

namespace Antd {
    public class Do {
        private const string LocalTemplateDirectory = "/framework/antd/Templates";
        private readonly Dictionary<string, string> _replacements;

        private readonly Host2Model _host;
        private readonly DnsConfiguration _dns;
        private readonly bool _isDnsPublic;
        private readonly bool _isDnsDynamic;
        private readonly bool _isDnsExternal;

        /// <summary>
        /// Constructor
        /// </summary>
        public Do() {
            _host = Host2Configuration.Host;
            _dns = Network2Configuration.DnsConfigurationList.FirstOrDefault(_ => _.Id == Network2Configuration.Conf.ActiveDnsConfiguration);

            _isDnsPublic = _dns?.Type == DnsType.Public;
            _isDnsDynamic = _dns?.Mode == DnsMode.Dynamic;
            _isDnsExternal = _dns?.Dest == DnsDestination.External;

            var clusterInfo = ClusterConfiguration.GetClusterInfo();

            _replacements = new Dictionary<string, string> {
                { "$hostname", _host.HostName },
                { "$internalIp", _host.InternalHostIpPrimary },
                { "$externalIp", _host.ExternalHostIpPrimary },
                { "$internalNet", _host.InternalNetPrimary },
                { "$externalNet", _host.ExternalNetPrimary },
                { "$internalMask", _host.InternalNetMaskPrimary },
                { "$externalMask", _host.ExternalNetMaskPrimary },
                { "$internalNetBits", _host.InternalNetPrimaryBits },
                { "$externalNetBits", _host.ExternalNetPrimaryBits },
                { "$internalDomain", _host.InternalDomainPrimary },
                { "$externalDomain", _host.ExternalDomainPrimary },
                { "$internalBroadcast", _host.InternalBroadcastPrimary },
                { "$externalBroadcast", _host.ExternalBroadcastPrimary },
                { "$internalNetArpa", _host.InternalArpaPrimary },
                { "$externalNetArpa", _host.ExternalArpaPrimary },
                { "$resolvNameserver", _host.ResolvNameserver },
                { "$resolvDomain", _host.ResolvDomain },
                { "$dnsDomain", _dns?.Domain },
                { "$dnsIp", _dns?.Ip },
                { "$secret", _host.Secret },
                { "$virtualIp", clusterInfo.VirtualIpAddress },
                { "$clusterPassword", clusterInfo.Password },
                { "$internalArpaIpAddress", _host.InternalHostIpPrimary.Split('.').Skip(2).JoinToString(".") }, //se internalIp: 10.11.19.111 -> 19.111
            };

            var interfaces = Network2Configuration.Conf.Interfaces;
            var activeNetworkConfsIds = interfaces.Select(_ => _.Configuration);
            var networkConfs = Network2Configuration.InterfaceConfigurationList;
            var activeNetworkConfs = activeNetworkConfsIds.Select(_ => networkConfs.FirstOrDefault(__ => __.Id == _)).ToList();
            var internalActiveNetworkConfs = activeNetworkConfs.Where(_ => _.Type == NetworkInterfaceType.Internal);
            var internalActiveNetworkConfsIds = internalActiveNetworkConfs.Select(_ => _.Id);
            var internalInterfaces = internalActiveNetworkConfsIds.Select(_ => interfaces.FirstOrDefault(__ => __.Configuration == _)).Select(_ => _.Device).ToList().JoinToString(", ");
            _replacements["$internalInterface"] = internalInterfaces;
            var externalActiveNetworkConfs = activeNetworkConfs.Where(_ => _.Type == NetworkInterfaceType.External);
            var externalActiveNetworkConfsIds = externalActiveNetworkConfs.Select(_ => _.Id);
            var externalInterfaces = externalActiveNetworkConfsIds.Select(_ => interfaces.FirstOrDefault(__ => __.Configuration == _)).Select(_ => _.Device).ToList().JoinToString(", ");
            _replacements["$externalInterface"] = externalInterfaces;
            var allInterfaces = interfaces.Select(_ => _.Device).ToList().JoinToString(", ");
            _replacements["$allInterface"] = allInterfaces;
        }

        /// <summary>
        /// Main function
        /// </summary>
        public void AllChanges() {
            ParametersChangesPre();
            NetworkChanges();
            HostChanges();
            ParametersChangesPost();
            ClusterChanges();
        }

        public void HostChanges() {
            SaveHostname();
            ApplyNtpdate();
            SaveNtpFile();
            SaveResolvFile();
            SaveNsswitchFile();
            SaveNetworksFile();
            SaveHostsFile();
            Directory.CreateDirectory("/etc/dhcp");
            SaveDhcpdFile();
            SaveDhcpdService();
            Directory.CreateDirectory("/etc/bind");
            SaveNamedFile();
            SaveRndcFile();
            SaveBindFiles();
            SaveBindZones();
            SaveBindService();
            SaveNftablesFile();
        }

        public void NetworkChanges() {
            ApplyNetworkConfiguration();
        }

        public void ParametersChanges() {
            ParametersChangesPre();
            ParametersChangesPost();
        }

        public void ParametersChangesPre() {
            LaunchStart();
            SaveOsParameters();
            SaveModprobes();
            RemoveModules();
            BlacklistMudules();
            StartService();
            StopService();
        }

        public void ParametersChangesPost() {
            LaunchEnd();
        }

        public void ClusterChanges() {
            ConsoleLogger.Log("[cluster] applying changes");
            var publicIp = ClusterConfiguration.GetClusterInfo().VirtualIpAddress;
            if(string.IsNullOrEmpty(publicIp)) {
                ConsoleLogger.Warn("[cluster] public ip not valid");
                return;
            }
            var nodes = ClusterConfiguration.Get();
            if(!nodes.Any()) {
                ConsoleLogger.Warn("[cluster] configuration not valid");
                return;
            }

            SaveKeepalived(publicIp, nodes);
            SaveHaproxy(publicIp, nodes);
        }

        #region [    keepalived    ]
        private readonly string _keepalivedFileInput = $"{LocalTemplateDirectory}/keepalived.conf.tmpl";
        private const string KeepalivedFileOutput = "/etc/keepalived/keepalived.conf";

        private void SaveKeepalived(string publicIp, List<Cluster.Node> nodes) {
            ConsoleLogger.Log("[cluster] init keepalived");
            const string keepalivedService = "keepalived.service";
            if(Systemctl.IsActive(keepalivedService)) {
                ConsoleLogger.Log("[cluster] stop service");
                Systemctl.Stop(keepalivedService);
            }
            ConsoleLogger.Log("[cluster] set configuration file");
            try {
                using(var reader = new StreamReader(_keepalivedFileInput)) {
                    using(TextWriter writer = File.CreateText(KeepalivedFileOutput)) {
                        string line;
                        while((line = reader.ReadLine()) != null) {
                            writer.WriteLine(EditLine(line));
                        }
                    }
                }
            }
            catch(Exception e) {
                ConsoleLogger.Error("[cluster] error:");
                ConsoleLogger.Error(e.Message);
                return;
            }

            var additionalLines = new List<string> {
                $"virtual_server {publicIp} 80 {{",
                "    delay_loop 6",
                "    lb_algo rr",
                "    lb_kind NAT",
                "    protocol TCP",
                ""
            };
            foreach(var node in nodes) {
                foreach(var svc in node.Services) {
                    additionalLines.Add($"    real_server {node.IpAddress} {svc.Port} {{");
                    additionalLines.Add("        TCP_CHECK {");
                    additionalLines.Add("                connect_timeout 10");
                    additionalLines.Add("        }");
                    additionalLines.Add("    }");
                }
            }
            additionalLines.Add("}");
            additionalLines.Add("");

            File.AppendAllLines(KeepalivedFileOutput, additionalLines);
            //if(Systemctl.IsEnabled(keepalivedService) == false) {
            //    Systemctl.Enable(keepalivedService);
            //    ConsoleLogger.Log("[cluster] keepalived enabled");
            //}
            //if(Systemctl.IsActive(keepalivedService) == false) {
            //    Systemctl.Restart(keepalivedService);
            //    ConsoleLogger.Log("[cluster] keepalived restarted");
            //}
            Systemctl.Enable(keepalivedService);
            ConsoleLogger.Log("[cluster] keepalived enabled");
            Systemctl.Restart(keepalivedService);
            ConsoleLogger.Log("[cluster] keepalived restarted");
            //ConsoleLogger.Log("[cluster] keepalived started");
        }

        private readonly string _haproxyFileOutput = $"{Parameter.AntdCfgCluster}/haproxy.conf";

        private void SaveHaproxy(string publicIp, List<Cluster.Node> nodes) {
            ConsoleLogger.Log("[cluster] init haproxy");
            CommandLauncher.Launch("haproxy-stop");
            if(File.Exists(_haproxyFileOutput)) {
                File.Copy(_haproxyFileOutput, $"{_haproxyFileOutput}.bck", true);
            }
            ConsoleLogger.Log("[cluster] set haproxy file");
            var lines = new List<string> {
                "global",
                "    daemon",
                "    maxconn 256",
                "",
                "defaults",
                "    mode http",
                "    timeout connect 5000ms",
                "    timeout client 50000ms",
                "    timeout server 50000ms",
                "",
                "frontend http-in",
                $"    bind {publicIp}:80",
                "    default_backend servers",
                "",
                "backend servers"
            };
            foreach(var node in nodes) {
                foreach(var svc in node.Services) {
                    lines.Add($"    server {node.Hostname} {node.IpAddress}:{svc.Port} maxconn 32");
                }
            }
            File.WriteAllLines(_haproxyFileOutput, lines);
            CommandLauncher.Launch("haproxy-start", new Dictionary<string, string> { { "$file", _haproxyFileOutput } });
            ConsoleLogger.Log("[cluster] haproxy started");
        }
        #endregion

        #region [    private functions    ]
        private string EditLine(string input) {
            var output = input;
            foreach(var r in _replacements) {
                output = output.Replace(r.Key, r.Value);
            }
            return output;
        }

        private void ImportTemplate(string src, string dest) {
            if(!File.Exists(dest)) {
                File.Copy(src, dest);
            }
        }

        private void ActivateService(string svc) {
            if(Systemctl.IsEnabled(svc) == false) {
                Systemctl.Enable(svc);
            }
            if(Systemctl.IsActive(svc) == false) {
                Systemctl.Restart(svc);
            }
        }
        #endregion

        #region [    network    ]
        private void ApplyNetworkConfiguration() {
            var configurations = Network2Configuration.Conf.Interfaces;
            if(!configurations.Any()) {
                ConsoleLogger.Log("[network] configurations not found: create defaults");
                CreateDefaultNetworkConfiguration();
            }
            var interfaceConfigurations = Network2Configuration.InterfaceConfigurationList;
            var gatewayConfigurations = Network2Configuration.GatewayConfigurationList;
            foreach(var configuration in configurations) {
                var deviceName = configuration.Device;

                var ifConfig = interfaceConfigurations.FirstOrDefault(_ => _.Id == configuration.Configuration);
                if(ifConfig == null) {
                    continue;
                }

                switch(ifConfig.Adapter) {
                    case NetworkAdapterType.Physical:
                        break;
                    case NetworkAdapterType.Virtual:
                        break;
                    case NetworkAdapterType.Bond:
                        CommandLauncher.Launch("bond-set", new Dictionary<string, string> { { "$bond", deviceName } });
                        foreach(var nif in ifConfig.ChildrenIf) {
                            CommandLauncher.Launch("bond-add-if", new Dictionary<string, string> { { "$bond", deviceName }, { "$net_if", nif } });
                            CommandLauncher.Launch("ip4-flush-configuration", new Dictionary<string, string> { { "$net_if", nif } });
                        }
                        break;
                    case NetworkAdapterType.Bridge:
                        CommandLauncher.Launch("brctl-add", new Dictionary<string, string> { { "$bridge", deviceName } });
                        foreach(var nif in ifConfig.ChildrenIf) {
                            CommandLauncher.Launch("brctl-add-if", new Dictionary<string, string> { { "$bridge", deviceName }, { "$net_if", nif } });
                            CommandLauncher.Launch("ip4-flush-configuration", new Dictionary<string, string> { { "$net_if", nif } });
                        }
                        break;
                    case NetworkAdapterType.Other:
                        continue;
                }

                CommandLauncher.Launch("ip4-set-mtu", new Dictionary<string, string> { { "$net_if", deviceName }, { "$mtu", configuration.Mtu } });
                CommandLauncher.Launch("ip4-set-txqueuelen", new Dictionary<string, string> { { "$net_if", deviceName }, { "$txqueuelen", configuration.Txqueuelen } });

                switch(ifConfig.Mode) {
                    case NetworkInterfaceMode.Null:
                        continue;
                    case NetworkInterfaceMode.Static:
                        var networkdIsActive = Systemctl.IsActive("systemd-networkd");
                        if(networkdIsActive) {
                            Systemctl.Stop("systemd-networkd");
                        }
                        CommandLauncher.Launch("dhclient-killall");
                        CommandLauncher.Launch("ip4-flush-configuration", new Dictionary<string, string> {
                            { "$net_if", deviceName }
                        });
                        CommandLauncher.Launch("ip4-add-addr", new Dictionary<string, string> {
                            { "$net_if", deviceName },
                            { "$address", ifConfig.Ip },
                            { "$range", ifConfig.Subnet }
                        });
                        if(networkdIsActive) {
                            Systemctl.Start("systemd-networkd");
                        }
                        break;
                    case NetworkInterfaceMode.Dynamic:
                        CommandLauncher.Launch("dhclient4", new Dictionary<string, string> { { "$net_if", deviceName } });
                        break;
                    default:
                        continue;
                }

                switch(ifConfig.Status) {
                    case NetworkInterfaceStatus.Down:
                        CommandLauncher.Launch("ip4-disable-if", new Dictionary<string, string> { { "$net_if", deviceName } });
                        continue;
                    case NetworkInterfaceStatus.Up:
                        CommandLauncher.Launch("ip4-enable-if", new Dictionary<string, string> { { "$net_if", deviceName } });
                        ConsoleLogger.Log($"[network] interface '{deviceName}' configured");
                        break;
                    default:
                        CommandLauncher.Launch("ip4-disable-if", new Dictionary<string, string> { { "$net_if", deviceName } });
                        continue;
                }

                var gwConfig = gatewayConfigurations.FirstOrDefault(_ => _.Id == configuration.GatewayConfiguration);
                if(gwConfig == null) {
                    continue;
                }

                CommandLauncher.Launch("ip4-add-route", new Dictionary<string, string> { { "$net_if", deviceName }, { "$gateway", gwConfig.GatewayAddress }, { "$ip_address", gwConfig.Route } });
            }
        }

        private void CreateDefaultNetworkConfiguration() {
            Network2Configuration.AddInterfaceConfiguration(Default.InternalPhysicalInterfaceConfiguration());
            Network2Configuration.AddInterfaceConfiguration(Default.ExternalPhysicalInterfaceConfiguration());
            Network2Configuration.AddInterfaceConfiguration(Default.InternalBridgeInterfaceConfiguration());
            Network2Configuration.AddInterfaceConfiguration(Default.ExternalBridgeInterfaceConfiguration());
            Network2Configuration.AddGatewayConfiguration(Default.GatewayConfiguration());
            Network2Configuration.AddDnsConfiguration(Default.PublicDnsConfiguration());
            Network2Configuration.AddDnsConfiguration(Default.PrivateInternalDnsConfiguration());
            Network2Configuration.AddDnsConfiguration(Default.PrivateExternalDnsConfiguration());
            var devs = Network2Configuration.InterfacePhysical.ToList();
            var partIp = Default.InternalPhysicalInterfaceConfiguration().Ip.Split('.').Take(3).JoinToString(".");
            var counter = 200;
            foreach(var dev in devs) {
                var conf = Default.InternalPhysicalInterfaceConfiguration($"{partIp}.{counter}");
                Network2Configuration.AddInterfaceConfiguration(conf);
                var networkInterface = new NetworkInterface {
                    Device = dev,
                    Configuration = conf.Id,
                    GatewayConfiguration = Default.GatewayConfiguration().Id,
                    AdditionalConfigurations = new List<string>(),
                    Mtu = "6000",
                    Txqueuelen = "10000"
                };
                Network2Configuration.AddInterfaceSetting(networkInterface);
                counter = counter + 1;
            }
            Network2Configuration.SetDnsConfigurationActive(Default.PublicDnsConfiguration().Id);
        }

        //public void SetupResolvD() {
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
        private void SaveHostname() {
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
        private void ApplyNtpdate() {
            CommandLauncher.Launch("ntpdate", new Dictionary<string, string> { { "$server", _host.NtpdateServer } });
        }
        #endregion

        #region [    ntp.conf    ]
        private readonly string _ntpFileInput = $"{LocalTemplateDirectory}/ntp.conf.tmlp";
        private const string NtpFileOutput = "/etc/ntp.conf";

        private void SaveNtpFile() {
            try {
                using(var reader = new StreamReader(_ntpFileInput)) {
                    using(TextWriter writer = File.CreateText(NtpFileOutput)) {
                        string line;
                        while((line = reader.ReadLine()) != null) {
                            writer.WriteLine(EditLine(line));
                        }
                    }
                }
            }
            catch(Exception e) {
                Console.WriteLine(e.Message);
            }
        }
        #endregion

        #region [    resolv.conf    ]
        private const string ResolvFileOutput = "/etc/resolv.conf";

        private void SaveResolvFile() {
            string[] lines;
            if(_isDnsPublic) {
                lines = new[] {
                    "nameserver 8.8.8.8",
                    "nameserver 8.8.4.4"
                };
            }
            else {
                if(_isDnsExternal) {
                    lines = new[] {
                        $"nameserver {_host.ExternalHostIpPrimary}",
                        $"nameserver {_host.ResolvNameserver}",
                        $"search {_host.ResolvDomain}",
                        $"domain {_host.ResolvDomain}"
                    };
                }
                else {
                    lines = new[] {
                        $"nameserver {_host.ResolvNameserver}",
                        $"search {_host.ResolvDomain}",
                        $"domain {_host.ResolvDomain}"
                    };
                }
            }
            try {
                using(TextWriter writer = File.CreateText(ResolvFileOutput)) {
                    foreach(var line in lines) {
                        writer.WriteLine(line);
                    }
                }
            }
            catch(Exception e) {
                Console.WriteLine(e.Message);
            }
        }
        #endregion

        #region [    nsswitch.conf    ]
        private readonly string _nsswitchFileInput = $"{LocalTemplateDirectory}/nsswitch.conf.tmlp";
        private const string NsswitchFileOutput = "/etc/nsswitch.conf";

        private void SaveNsswitchFile() {
            try {
                using(var reader = new StreamReader(_nsswitchFileInput)) {
                    using(TextWriter writer = File.CreateText(NsswitchFileOutput)) {
                        string line;
                        while((line = reader.ReadLine()) != null) {
                            writer.WriteLine(EditLine(line));
                        }
                    }
                }
            }
            catch(Exception e) {
                Console.WriteLine(e.Message);
            }
        }
        #endregion

        #region [    networks    ]
        private readonly string _networksFileInput = $"{LocalTemplateDirectory}/networks.tmlp";
        private const string NetworksFileOutput = "/etc/networks";

        private void SaveNetworksFile() {
            try {
                using(var reader = new StreamReader(_networksFileInput)) {
                    using(TextWriter writer = File.CreateText(NetworksFileOutput)) {
                        string line;
                        while((line = reader.ReadLine()) != null) {
                            writer.WriteLine(EditLine(line));
                        }
                    }
                }
            }
            catch(Exception e) {
                Console.WriteLine(e.Message);
            }
        }
        #endregion

        #region [    hosts    ]
        private readonly string _hostsFileInput = $"{LocalTemplateDirectory}/hosts.tmlp";
        private const string HostsFileOutput = "/etc/hosts";

        private void SaveHostsFile() {
            try {
                using(var reader = new StreamReader(_hostsFileInput)) {
                    using(TextWriter writer = File.CreateText(HostsFileOutput)) {
                        string line;
                        while((line = reader.ReadLine()) != null) {
                            writer.WriteLine(EditLine(line));
                        }
                    }
                }
            }
            catch(Exception e) {
                Console.WriteLine(e.Message);
            }
        }
        #endregion

        #region [    dhcpd service    ]
        private void SaveDhcpdService() {
            const string svc = "dhcpd4.service";
            ActivateService(svc);
        }
        #endregion

        #region [    dhcpd.conf    ]
        private readonly string _dhcpdDynamicExternalFileInput = $"{LocalTemplateDirectory}/dhcpd.dynamic.external.conf.tmlp";
        private readonly string _dhcpdDynamicInternalFileInput = $"{LocalTemplateDirectory}/dhcpd.dynamic.internal.conf.tmlp";
        private readonly string _dhcpdStaticExternalFileInput = $"{LocalTemplateDirectory}/dhcpd.static.external.conf.tmlp";
        private readonly string _dhcpdStaticInternalFileInput = $"{LocalTemplateDirectory}/dhcpd.static.internal.conf.tmlp";
        private const string DhcpdFileOutput = "/etc/dhcp/dhcpd.conf";

        private void SaveDhcpdFile() {
            try {
                var dhcpdFileInput = _isDnsDynamic ?
                    (_isDnsExternal ? _dhcpdDynamicExternalFileInput : _dhcpdDynamicInternalFileInput) :
                    (_isDnsExternal ? _dhcpdStaticExternalFileInput : _dhcpdStaticInternalFileInput);
                using(var reader = new StreamReader(dhcpdFileInput)) {
                    using(TextWriter writer = File.CreateText(DhcpdFileOutput)) {
                        string line;
                        while((line = reader.ReadLine()) != null) {
                            writer.WriteLine(EditLine(line));
                        }
                    }
                }
            }
            catch(Exception e) {
                Console.WriteLine(e.Message);
            }
        }
        #endregion

        #region [    bind service    ]
        private void SaveBindService() {
            const string svc = "named.service";
            ActivateService(svc);
            CommandLauncher.Launch("rndc-reconfig");
            CommandLauncher.Launch("rndc-reload");
        }
        #endregion

        #region [    named.conf    ]
        private readonly string _namedFileInput = $"{LocalTemplateDirectory}/named.conf.tmlp";
        private const string NamedFileOutput = "/etc/bind/named.conf";

        private void SaveNamedFile() {
            try {
                using(var reader = new StreamReader(_namedFileInput)) {
                    using(TextWriter writer = File.CreateText(NamedFileOutput)) {
                        string line;
                        while((line = reader.ReadLine()) != null) {
                            writer.WriteLine(EditLine(line));
                        }
                    }
                }
            }
            catch(Exception e) {
                Console.WriteLine(e.Message);
            }
        }
        #endregion

        #region [    rndc.conf    ]
        private readonly string _rndcFileInput = $"{LocalTemplateDirectory}/rndc.conf.tmpl";
        private const string RndcFileOutput = "/etc/bind/rndc.conf";

        private void SaveRndcFile() {
            try {
                using(var reader = new StreamReader(_rndcFileInput)) {
                    using(TextWriter writer = File.CreateText(RndcFileOutput)) {
                        string line;
                        while((line = reader.ReadLine()) != null) {
                            writer.WriteLine(EditLine(line));
                        }
                    }
                }
            }
            catch(Exception e) {
                Console.WriteLine(e.Message);
            }
        }
        #endregion

        #region [    bind files    ]
        private void SaveBindFiles() {
            Directory.CreateDirectory("/etc/bind");
            Directory.CreateDirectory("/etc/bind/zones");
            ImportTemplate($"{LocalTemplateDirectory}/blackhole.zones.tmpl", "/etc/bind/blackhole.zones");
            ImportTemplate($"{LocalTemplateDirectory}/zones_blockeddomain.hosts.tmpl", "/etc/bind/zones/blockeddomain.hosts");
            ImportTemplate($"{LocalTemplateDirectory}/zones_empty.db.tmpl", "/etc/bind/zones/empty.db");
            ImportTemplate($"{LocalTemplateDirectory}/zones_localhost-forward.db.tmpl", "/etc/bind/zones/localhost-forward.db");
            ImportTemplate($"{LocalTemplateDirectory}/zones_localhost-reverse.db.tmpl", "/etc/bind/zones/localhost-reverse.db");
        }

        private void SaveBindZones() {
            var hostZoneFileInput = $"{LocalTemplateDirectory}/zones_host.domint.local.db.tmpl";
            var hostZoneFileOutput = $"/etc/bind/zones/host.{_host.InternalDomainPrimary}.db";
            try {
                using(var reader = new StreamReader(hostZoneFileInput)) {
                    using(TextWriter writer = File.CreateText(hostZoneFileOutput)) {
                        string line;
                        while((line = reader.ReadLine()) != null) {
                            writer.WriteLine(EditLine(line));
                        }
                    }
                }
            }
            catch(Exception e) {
                Console.WriteLine(e.Message);
            }

            var revZoneFileInput = $"{LocalTemplateDirectory}/zones_rev.10.11.0.0.db.tmpl";
            var revZoneFileOutput = $"/etc/bind/zones/rev.{_host.InternalNetPrimary}.db";
            try {
                using(var reader = new StreamReader(revZoneFileInput)) {
                    using(TextWriter writer = File.CreateText(revZoneFileOutput)) {
                        string line;
                        while((line = reader.ReadLine()) != null) {
                            writer.WriteLine(EditLine(line));
                        }
                    }
                }
            }
            catch(Exception e) {
                Console.WriteLine(e.Message);
            }
        }
        #endregion

        #region [    Nftable    ]
        private readonly string _nftablesFileInput = $"{LocalTemplateDirectory}/nftables.conf.tmpl";
        private const string NftablesFileOutput = "/etc/nftables.conf";

        private void SaveNftablesFile() {
            try {
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
            catch(Exception e) {
                Console.WriteLine(e.Message);
            }
        }
        #endregion

        #region [    modules    ]
        private void SaveModprobes() {
            var modules = HostParametersConfiguration.Conf.Modprobes;
            foreach(var mod in modules) {
                CommandLauncher.Launch("modprobe", new Dictionary<string, string> { { "$package", mod } });
            }
        }

        private void RemoveModules() {
            var modules = string.Join(" ", HostParametersConfiguration.Conf.Rmmod);
            CommandLauncher.Launch("rmmod", new Dictionary<string, string> { { "$modules", modules } });
        }

        private void BlacklistMudules() {
            if(!File.Exists("/etc/modprobe.d/blacklist.conf")) { return; }
            File.WriteAllLines("/etc/modprobe.d/blacklist.conf", HostParametersConfiguration.Conf.ModulesBlacklist);
        }
        #endregion

        #region [    services    ]
        private void StartService() {
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

        private void StopService() {
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
        private void SaveOsParameters() {
            var parameters = HostParametersConfiguration.Conf.OsParameters;
            foreach(var par in parameters) {
                if(!par.Contains(" ")) { continue; }
                try {
                    var arr = par.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                    if(arr.Length != 2) { continue; }
                    var file = arr[0];
                    if(!File.Exists(file)) { continue; }
                    var value = arr[1];
                    File.WriteAllText(file, value);
                }
                catch(Exception ex) {
                    ConsoleLogger.Error(ex);
                }
            }
        }
        #endregion

        #region [    commands    ]
        private void LaunchStart() {
            var controls = HostParametersConfiguration.Conf.StartCommands;
            foreach(var control in controls) {
                Launch(control);
            }
        }

        private void LaunchEnd() {
            var controls = HostParametersConfiguration.Conf.EndCommands;
            foreach(var control in controls) {
                Launch(control);
            }
        }

        private void Launch(Control control) {
            if(control?.FirstCommand == null) {
                return;
            }
            try {
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
            catch(NullReferenceException nrex) {
                ConsoleLogger.Warn(nrex.Message + " " + nrex.Source + " c: " + control.FirstCommand);
            }
            catch(Exception ex) {
                ConsoleLogger.Warn(ex.Message + " c: " + control.FirstCommand);
            }
        }
        #endregion
    }
}
