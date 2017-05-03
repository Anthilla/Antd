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

        private readonly Host2Configuration _host2Configuration = new Host2Configuration();
        private readonly Network2Configuration _network2Configuration = new Network2Configuration();
        private readonly CommandLauncher _commandLauncher = new CommandLauncher();

        private readonly Host2Model Host;
        private readonly DnsConfiguration Dns;
        private readonly bool IsDnsPublic;
        private readonly bool IsDnsDynamic;
        private readonly bool IsDnsExternal;

        /// <summary>
        /// Constructor
        /// </summary>
        public Do() {
            Host = _host2Configuration.Host;
            Dns = _network2Configuration.DnsConfigurationList.FirstOrDefault(_ => _.Id == _network2Configuration.Conf.ActiveDnsConfiguration);

            IsDnsPublic = Dns?.Type == DnsType.Public;
            IsDnsDynamic = Dns?.Mode == DnsMode.Dynamic;
            IsDnsExternal = Dns?.Dest == DnsDestination.External;

            _replacements = new Dictionary<string, string> {
                { "$hostname", Host.HostName },
                { "$internalIp", Host.InternalHostIpPrimary },
                { "$externalIp", Host.ExternalHostIpPrimary },
                { "$internalNet", Host.InternalNetPrimary },
                { "$externalNet", Host.ExternalNetPrimary },
                { "$internalMask", Host.InternalNetMaskPrimary },
                { "$externalMask", Host.ExternalNetMaskPrimary },
                { "$internalNetBits", Host.InternalNetPrimaryBits },
                { "$externalNetBits", Host.ExternalNetPrimaryBits },
                { "$internalDomain", Host.InternalDomainPrimary },
                { "$externalDomain", Host.ExternalDomainPrimary },
                { "$internalBroadcast", Host.InternalBroadcastPrimary },
                { "$externalBroadcast", Host.ExternalBroadcastPrimary },
                { "$internalNetArpa", Host.InternalArpaPrimary },
                { "$externalNetArpa", Host.ExternalArpaPrimary },
                { "$resolvNameserver", Host.ResolvNameserver },
                { "$resolvDomain", Host.ResolvDomain },
                { "$dnsDomain", Dns?.Domain },
                { "$dnsIp", Dns?.Ip },
                { "$secret", Host.Secret },
                { "$internalArpaIpAddress", Host.InternalHostIpPrimary.Split('.').Skip(2).JoinToString(".") }, //se internalIp: 10.11.19.111 -> 19.111
            };

            var interfaces = _network2Configuration.Conf.Interfaces;
            var activeNetworkConfsIds = interfaces.Select(_ => _.Configuration);
            var networkConfs = _network2Configuration.InterfaceConfigurationList;
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
            ConsoleLogger.Log("[setup] apply configured host vars");
            NetworkChanges();
            HostChanges();
        }

        public void HostChanges() {
            SaveHostname();
            SaveNtpFile();
            SaveResolvFile();
            SaveNsswitchFile();
            SaveNetworksFile();
            SaveHostsFile();
            Directory.CreateDirectory("/etc/dhcp");
            SaveDhcpdFile();
            Directory.CreateDirectory("/etc/bind");
            SaveNamedFile();
            SaveRndcFile();
            SaveBindFiles();
            SaveBindZones();
            SaveNftablesFile();
        }

        public void NetworkChanges() {
            ApplyNetworkConfiguration();
        }
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
        #endregion

        #region [    network    ]
        private void ApplyNetworkConfiguration() {
            var configurations = _network2Configuration.Conf.Interfaces;
            var interfaceConfigurations = _network2Configuration.InterfaceConfigurationList;
            var gatewayConfigurations = _network2Configuration.GatewayConfigurationList;
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
                        _commandLauncher.Launch("bond-set", new Dictionary<string, string> { { "$bond", deviceName } });
                        foreach(var nif in ifConfig.ChildrenIf) {
                            _commandLauncher.Launch("bond-add-if", new Dictionary<string, string> { { "$bond", deviceName }, { "$net_if", nif } });
                        }
                        break;
                    case NetworkAdapterType.Bridge:
                        _commandLauncher.Launch("brctl-add", new Dictionary<string, string> { { "$bridge", deviceName } });
                        foreach(var nif in ifConfig.ChildrenIf) {
                            _commandLauncher.Launch("brctl-add-if", new Dictionary<string, string> { { "$bridge", deviceName }, { "$net_if", nif } });
                        }
                        break;
                    case NetworkAdapterType.Other:
                        continue;
                }

                _commandLauncher.Launch("ip4-set-mtu", new Dictionary<string, string> { { "$net_if", deviceName }, { "$mtu", configuration.Mtu } });
                _commandLauncher.Launch("ip4-set-txqueuelen", new Dictionary<string, string> { { "$net_if", deviceName }, { "$txqueuelen", configuration.Txqueuelen } });

                switch(ifConfig.Mode) {
                    case NetworkInterfaceMode.Null:
                        continue;
                    case NetworkInterfaceMode.Static:
                        var networkdIsActive = Systemctl.IsActive("systemd-networkd");
                        if(networkdIsActive) {
                            Systemctl.Stop("systemd-networkd");
                        }
                        _commandLauncher.Launch("dhclient-killall");
                        _commandLauncher.Launch("ip4-flush-configuration", new Dictionary<string, string> {
                            { "$net_if", deviceName }
                        });
                        _commandLauncher.Launch("ip4-add-addr", new Dictionary<string, string> {
                            { "$net_if", deviceName },
                            { "$address", ifConfig.Ip },
                            { "$range", ifConfig.Subnet }
                        });
                        if(networkdIsActive) {
                            Systemctl.Start("systemd-networkd");
                        }
                        break;
                    case NetworkInterfaceMode.Dynamic:
                        _commandLauncher.Launch("dhclient4", new Dictionary<string, string> { { "$net_if", deviceName } });
                        break;
                    default:
                        continue;
                }

                switch(ifConfig.Status) {
                    case NetworkInterfaceStatus.Down:
                        _commandLauncher.Launch("ip4-disable-if", new Dictionary<string, string> { { "$net_if", deviceName } });
                        continue;
                    case NetworkInterfaceStatus.Up:
                        _commandLauncher.Launch("ip4-enable-if", new Dictionary<string, string> { { "$net_if", deviceName } });
                        ConsoleLogger.Log($"[network] interface '{deviceName}' configured");
                        break;
                    default:
                        _commandLauncher.Launch("ip4-disable-if", new Dictionary<string, string> { { "$net_if", deviceName } });
                        continue;
                }

                var gwConfig = gatewayConfigurations.FirstOrDefault(_ => _.Id == configuration.GatewayConfiguration);
                if(gwConfig == null) {
                    continue;
                }

                _commandLauncher.Launch("ip4-add-route", new Dictionary<string, string> { { "$net_if", deviceName }, { "$gateway", gwConfig.GatewayAddress }, { "$ip_address", gwConfig.Route } });
            }
        }
        #endregion

        #region [    hostnamectl    ]
        private void SaveHostname() {
            _commandLauncher.Launch("set-hostname", new Dictionary<string, string> {
                { "$host_name", Host.HostName }
            });
            _commandLauncher.Launch("set-chassis", new Dictionary<string, string> {
                { "$host_chassis", Host.HostChassis }
            });
            _commandLauncher.Launch("set-deployment", new Dictionary<string, string> {
                { "$host_deployment", Host.HostDeployment }
            });
            _commandLauncher.Launch("set-location", new Dictionary<string, string> {
                { "$host_location", Host.HostLocation }
            });
            File.WriteAllText("/etc/hostname", Host.HostName);
            _commandLauncher.Launch("set-timezone", new Dictionary<string, string> {
                { "$host_timezone", Host.Timezone }
            });
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
            if(IsDnsPublic) {
                lines = new[] {
                    "nameserver 8.8.8.8",
                    "nameserver 8.8.4.4"
                };
            }
            else {
                if(IsDnsExternal) {
                    lines = new[] {
                        $"nameserver {Host.ExternalHostIpPrimary}",
                        $"nameserver {Host.ResolvNameserver}",
                        $"search {Host.ResolvDomain}",
                        $"domain {Host.ResolvDomain}"
                    };
                }
                else {
                    lines = new[] {
                        $"nameserver {Host.ResolvNameserver}",
                        $"search {Host.ResolvDomain}",
                        $"domain {Host.ResolvDomain}"
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

        #region [    dhcpd.conf    ]
        private readonly string _dhcpdDynamicExternalFileInput = $"{LocalTemplateDirectory}/dhcpd.dynamic.external.conf.tmlp";
        private readonly string _dhcpdDynamicInternalFileInput = $"{LocalTemplateDirectory}/dhcpd.dynamic.internal.conf.tmlp";
        private readonly string _dhcpdStaticExternalFileInput = $"{LocalTemplateDirectory}/dhcpd.static.external.conf.tmlp";
        private readonly string _dhcpdStaticInternalFileInput = $"{LocalTemplateDirectory}/dhcpd.static.internal.conf.tmlp";
        private const string DhcpdFileOutput = "/etc/dhcp/dhcpd.conf";

        private void SaveDhcpdFile() {
            try {
                var dhcpdFileInput = IsDnsDynamic ?
                    (IsDnsExternal ? _dhcpdDynamicExternalFileInput : _dhcpdDynamicInternalFileInput) :
                    (IsDnsExternal ? _dhcpdStaticExternalFileInput : _dhcpdStaticInternalFileInput);
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
            var hostZoneFileOutput = $"/etc/bind/zones/host.{Host.InternalDomainPrimary}.db";
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
            var revZoneFileOutput = $"/etc/bind/zones/rev.{Host.InternalNetPrimary}.db";
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
            }
            catch(Exception e) {
                Console.WriteLine(e.Message);
            }
        }
        #endregion
    }
}
