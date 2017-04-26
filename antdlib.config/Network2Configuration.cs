using antdlib.common;
using antdlib.models;
using anthilla.commands;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace antdlib.config {
    public class Network2Configuration {

        public Network2ConfigurationModel Conf => Parse();
        public List<NetworkInterfaceConfiguration> InterfaceConfigurationList => GetInterfaceConfiguration();
        public List<NetworkGatewayConfiguration> GatewayConfigurationList => GetGatewayConfiguration();
        public List<DnsConfiguration> DnsConfigurationList => GetDnsConfiguration();
        public IEnumerable<string> NetworkInterfaces => GetAll();
        public IEnumerable<string> InterfacePhysical => GetPhysicalInterfaces();
        public IEnumerable<string> InterfaceVirtual => GetVirtualInterfaces();
        public IEnumerable<string> InterfaceBond => GetBondInterfaces();
        public IEnumerable<string> InterfaceBridge => GetBridgeInterfaces();

        private readonly string _dir = Parameter.AntdCfgNetwork;
        private readonly string _cfgFile = $"{Parameter.AntdCfgNetwork}/network.conf";
        private readonly string _cfgFileBackup = $"{Parameter.AntdCfgNetwork}/network.conf.bck";

        private const string InterfaceConfigurationExt = ".nif";
        private const string GatewayConfigurationExt = ".gw";
        private const string DnsConfigurationExt = ".dns";

        private readonly CommandLauncher _launcher = new CommandLauncher();
        private readonly Bash _bash = new Bash();

        public Network2Configuration() {
            Directory.CreateDirectory(_dir);
        }

        #region [    Network conf   ]
        private Network2ConfigurationModel Parse() {
            var conf = new Network2ConfigurationModel();
            if(!File.Exists(_cfgFile)) {
                return conf;
            }
            try {
                var text = File.ReadAllText(_cfgFile);
                conf = JsonConvert.DeserializeObject<Network2ConfigurationModel>(text);
            }
            catch(Exception) {
                conf = new Network2ConfigurationModel();
            }
            return conf;
        }

        public bool Save(Network2ConfigurationModel conf) {
            var text = JsonConvert.SerializeObject(conf, Formatting.Indented);
            if(File.Exists(_cfgFile)) {
                File.Copy(_cfgFile, _cfgFileBackup, true);
            }
            try {
                File.WriteAllText(_cfgFile, text);
                ConsoleLogger.Log("[network] configuration saved");
            }
            catch(Exception) {
                ConsoleLogger.Error("[network] configuration save error");
                return false;
            }
            return true;
        }

        public void AddInterfaceSetting(NetworkInterface model) {
            var netif = Conf.Interfaces;
            var check = netif.Where(_ => _.Device == model.Device).ToList();
            if(check.Any()) {
                check.ForEach(_ => RemoveInterfaceSetting(_.Device));
            }
            netif.Add(model);
            Conf.Interfaces = netif;
            Save(Conf);
        }

        public void RemoveInterfaceSetting(string device) {
            var netif = Conf.Interfaces;
            var model = netif.First(_ => _.Device == device);
            if(model == null) {
                return;
            }
            netif.Remove(model);
            Conf.Interfaces = netif;
            Save(Conf);
        }
        #endregion

        #region [    Network conf apply    ]
        public void ApplyInterfaceSetting() {
            var netifs = Conf.Interfaces;
            foreach(var netif in netifs) {
                ApplyInterfaceSetting(netif);
            }
        }

        private void ApplyInterfaceSetting(NetworkInterface model) {
            var device = model.Device;

            var interfaceConf = InterfaceConfigurationList.FirstOrDefault(_ => _.Id == model.Configuration);
            if(interfaceConf == null) {
                return;
            }

            var adapterType = interfaceConf.Adapter;
            var mode = interfaceConf.Mode;
            var staticAddress = interfaceConf.Ip;
            var staticRange = interfaceConf.Subnet;
            var status = interfaceConf.Status;

            switch(adapterType) {
                case NetworkAdapterType.Physical:
                    break;
                case NetworkAdapterType.Virtual:
                    break;
                case NetworkAdapterType.Bond:
                    _launcher.Launch("bond-set", new Dictionary<string, string> { { "$bond", device } });
                    foreach(var nif in interfaceConf.ChildrenIf) {
                        _launcher.Launch("bond-add-if", new Dictionary<string, string> { { "$bond", device }, { "$net_if", nif } });
                    }
                    break;
                case NetworkAdapterType.Bridge:
                    _launcher.Launch("brctl-add", new Dictionary<string, string> { { "$bridge", device } });
                    foreach(var nif in interfaceConf.ChildrenIf) {
                        _launcher.Launch("brctl-add-if", new Dictionary<string, string> { { "$bridge", device }, { "$net_if", nif } });
                    }
                    break;
                case NetworkAdapterType.Other:
                    break;
            }

            switch(mode) {
                case NetworkInterfaceMode.Null:
                    return;
                case NetworkInterfaceMode.Static:
                    var networkdIsActive = Systemctl.IsActive("systemd-networkd");
                    if(networkdIsActive) {
                        Systemctl.Stop("systemd-networkd");
                    }
                    _launcher.Launch("dhclient-killall");
                    _launcher.Launch("ip4-flush-configuration", new Dictionary<string, string> {
                        { "$net_if", device }
                    });
                    _launcher.Launch("ip4-add-addr", new Dictionary<string, string> {
                        { "$net_if", device },
                        { "$address", staticAddress },
                        { "$range", staticRange }
                    });
                    if(networkdIsActive) {
                        Systemctl.Start("systemd-networkd");
                    }
                    break;
                case NetworkInterfaceMode.Dynamic:
                    _launcher.Launch("dhclient4", new Dictionary<string, string> { { "$net_if", device } });
                    break;
                default:
                    return;
            }
            _launcher.Launch("ip4-set-mtu", new Dictionary<string, string> { { "$net_if", device }, { "$mtu", model.Mtu } });
            _launcher.Launch("ip4-set-txqueuelen", new Dictionary<string, string> { { "$net_if", device }, { "$txqueuelen", model.Txqueuelen } });


            var gatewayConf = GatewayConfigurationList.FirstOrDefault(_ => _.Id == model.GatewayConfiguration);
            if(gatewayConf != null) {
                _launcher.Launch("ip4-add-route", new Dictionary<string, string> { { "$net_if", device }, { "$gateway", gatewayConf.GatewayAddress }, { "$ip_address", gatewayConf.Route } });
            }

            switch(status) {
                case NetworkInterfaceStatus.Down:
                    _launcher.Launch("ip4-disable-if", new Dictionary<string, string> { { "$net_if", device } });
                    break;
                case NetworkInterfaceStatus.Up:
                    _launcher.Launch("ip4-enable-if", new Dictionary<string, string> { { "$net_if", device } });
                    ConsoleLogger.Log($"[network] dev '{device}' configured");
                    break;
                default:
                    return;
            }
        }
        #endregion

        #region [    NetworkInterfaceConfiguration    ]
        private List<NetworkInterfaceConfiguration> GetInterfaceConfiguration() {
            var list = new List<NetworkInterfaceConfiguration>();
            var files = Directory.EnumerateFiles(_dir, $"*{InterfaceConfigurationExt}");
            var ints = Conf.Interfaces;
            foreach(var file in files) {
                try {
                    var text = File.ReadAllText(file);
                    var conf = JsonConvert.DeserializeObject<NetworkInterfaceConfiguration>(text);
                    var mcContainsConf = ints.Select(_ => _.Configuration).Contains(conf.Id);
                    var scContainsConf = ints.Select(_ => _.AdditionalConfigurations.Where(__ => _.Configuration == conf.Id)).Any();
                    conf.IsUsed = mcContainsConf || scContainsConf;
                    list.Add(conf);
                }
                catch(Exception) {
                    //throw;
                }
            }
            return list;
        }

        public bool AddInterfaceConfiguration(NetworkInterfaceConfiguration conf) {
            if(string.IsNullOrEmpty(conf.Id)) {
                return false;
            }
            var file = $"{_dir}/{conf.Id}{InterfaceConfigurationExt}";
            var text = JsonConvert.SerializeObject(conf, Formatting.Indented);
            try {
                File.WriteAllText(file, text);
            }
            catch(Exception) {
                return false;
            }
            return File.Exists(file);
        }

        public bool RemoveInterfaceConfiguration(string id) {
            var file = $"{_dir}/{id}{InterfaceConfigurationExt}";
            if(!File.Exists(file)) {
                return false;
            }
            try {
                File.Delete(file);
            }
            catch(Exception) {
                return false;
            }
            return !File.Exists(file);
        }
        #endregion

        #region [    NetworkGatewayConfiguration    ]
        private List<NetworkGatewayConfiguration> GetGatewayConfiguration() {
            var list = new List<NetworkGatewayConfiguration>();
            var files = Directory.EnumerateFiles(_dir, $"*{GatewayConfigurationExt}");
            var ints = Conf.Interfaces;
            foreach(var file in files) {
                try {
                    var text = File.ReadAllText(file);
                    var conf = JsonConvert.DeserializeObject<NetworkGatewayConfiguration>(text);
                    var mcContainsConf = ints.Select(_ => _.GatewayConfiguration).Contains(conf.Id);
                    conf.IsUsed = mcContainsConf;
                    list.Add(conf);
                }
                catch(Exception) {
                    //throw;
                }
            }
            return list;
        }

        public bool AddGatewayConfiguration(NetworkGatewayConfiguration conf) {
            if(string.IsNullOrEmpty(conf.Id)) {
                return false;
            }
            var file = $"{_dir}/{conf.Id}{GatewayConfigurationExt}";
            var text = JsonConvert.SerializeObject(conf, Formatting.Indented);
            try {
                File.WriteAllText(file, text);
            }
            catch(Exception) {
                return false;
            }
            return File.Exists(file);
        }

        public bool RemoveGatewayConfiguration(string id) {
            var file = $"{_dir}/{id}{GatewayConfigurationExt}";
            if(!File.Exists(file)) {
                return false;
            }
            try {
                File.Delete(file);
            }
            catch(Exception) {
                return false;
            }
            return !File.Exists(file);
        }
        #endregion

        #region [    Network Devices Mapping    ]
        private IEnumerable<string> GetAll() {
            if(!Parameter.IsUnix) {
                return new List<string>();
            }
            var list = _bash.Execute("ls -la /sys/class/net").SplitBash().Where(_ => _.Contains("->"));
            return list.Select(f => f.Print(9, " ")).ToList();
        }

        private IEnumerable<string> GetPhysicalInterfaces() {
            var ifList = new List<string>();
            var list = _bash.Execute("ls -la /sys/class/net").SplitBash().Where(_ => _.Contains("->"));
            foreach(var f in list) {
                if(f.Contains("bond")) { }
                else if(f.Contains("br")) { }
                else if(f.Contains("virtual/net") || f.Contains("platform")) { }
                else if(!f.Contains("virtual/net")) {
                    var name = f.Print(9, " ");
                    ifList.Add(name.Trim());
                }
            }
            return ifList;
        }

        private IEnumerable<string> GetVirtualInterfaces() {
            var ifList = new List<string>();
            var list = _bash.Execute("ls -la /sys/class/net").SplitBash().Where(_ => _.Contains("->"));
            foreach(var f in list) {
                if(f.Contains("bond")) { }
                else if(f.Contains("br")) { }
                else if(f.Contains("virtual/net") || f.Contains("platform")) {
                    var name = f.Print(9, " ");
                    ifList.Add(name.Trim());
                }
                else if(!f.Contains("virtual/net")) { }
            }
            return ifList;
        }

        private IEnumerable<string> GetBondInterfaces() {
            var ifList = new List<string>();
            var list = _bash.Execute("ls -la /sys/class/net").SplitBash().Where(_ => _.Contains("->"));
            foreach(var f in list) {
                if(f.Contains("bond")) {
                    var name = f.Print(9, " ");
                    ifList.Add(name.Trim());
                }
                else if(f.Contains("br")) { }
                else if(f.Contains("virtual/net") || f.Contains("platform")) { }
                else if(!f.Contains("virtual/net")) { }
            }
            return ifList;
        }

        private IEnumerable<string> GetBridgeInterfaces() {
            var ifList = new List<string>();
            var list = _bash.Execute("ls -la /sys/class/net").SplitBash().Where(_ => _.Contains("->"));
            foreach(var f in list) {
                if(f.Contains("bond")) { }
                else if(f.Contains("br")) {
                    var name = f.Print(9, " ");
                    ifList.Add(name.Trim());
                }
                else if(f.Contains("virtual/net") || f.Contains("platform")) { }
                else if(!f.Contains("virtual/net")) { }
            }
            return ifList;
        }

        #endregion

        #region [    Defaults    ]
        //todo check procedure
        public void Start() {
            if(HasConfiguration()) {
                ConsoleLogger.Log("[network] applying saved settings");
                ApplyInterfaceSetting();
                return;
            }
            if(!InterfacePhysical.Any()) {
                ConsoleLogger.Log("[network] no interface to set");
                StartFallback();
                return;
            }
            var netIf = InterfacePhysical.FirstOrDefault();
            var tryStart = _launcher.Launch("dhclient4", new Dictionary<string, string> { { "$net_if", netIf } }).ToList();
            if(!tryStart.Any()) {
                ConsoleLogger.Log("[network] dhclient started");
                ConsoleLogger.Log($"[network] {netIf} is configured");
                return;
            }
            var tryGateway = _launcher.Launch("ip4-show-routes", new Dictionary<string, string> { { "$net_if", netIf } }).ToList();
            if(!tryGateway.Any()) {
                ConsoleLogger.Log("[network] no gateway available");
                StartFallback();
                return;
            }
            var gateway = tryGateway.FirstOrDefault(_ => _.Contains("default"));
            if(string.IsNullOrEmpty(gateway)) {
                ConsoleLogger.Log("[network] no gateway available");
                StartFallback();
                return;
            }
            var gatewayAddress = gateway.Replace("default", "").Trim();
            ConsoleLogger.Log($"[network] available gateway at {gatewayAddress}");

            var tryDns = _launcher.Launch("cat-etc-resolv").ToList();
            if(!tryGateway.Any()) {
                ConsoleLogger.Log("[network] no dns configured");
                SetupResolv();
            }
            var dns = tryDns.FirstOrDefault(_ => _.Contains("nameserver"));
            if(string.IsNullOrEmpty(dns)) {
                ConsoleLogger.Log("[network] no dns configured");
                SetupResolv();
            }
            var dnsAddress = gateway.Replace("nameserver", "").Trim();
            ConsoleLogger.Log($"[network] configured dns is {dnsAddress}");
            var pingDns = _launcher.Launch("ping-c", new Dictionary<string, string> { { "$net_if", dnsAddress } }).ToList();
            if(pingDns.Any(_ => _.ToLower().Contains("unreachable"))) {
                ConsoleLogger.Log("[network] dns is unreachable");
                return;
            }
            ConsoleLogger.Log($"[network] available dns at {dnsAddress}");
        }

        public void StartFallback() {
            ConsoleLogger.Log("[network] setting up a default configuration");
            const string bridge = "br0";
            if(InterfacePhysical.All(_ => _ != bridge)) {
                _launcher.Launch("brctl-add", new Dictionary<string, string> { { "$bridge", bridge } });
                ConsoleLogger.Log($"[network] {bridge} configured");
            }
            foreach(var phy in InterfacePhysical) {
                _launcher.Launch("brctl-addif", new Dictionary<string, string> { { "$bridge", bridge }, { "$net_if", phy } });
                ConsoleLogger.Log($"[network] {phy} add to {bridge}");
            }
            foreach(var phy in InterfacePhysical) {
                _launcher.Launch("ip4-enable-if", new Dictionary<string, string> { { "$net_if", phy } });
            }
            _launcher.Launch("ip4-enable-if", new Dictionary<string, string> { { "$net_if", bridge } });
            ConsoleLogger.Log("[network] interfaces up");
            const string address = "192.168.1.1";
            const string range = "24";
            _launcher.Launch("ip4-add-addr", new Dictionary<string, string> { { "$address", address }, { "$range", range }, { "$net_if", bridge } });
            var tryBridgeAddress = _launcher.Launch("ifconfig-if", new Dictionary<string, string> { { "$net_if", bridge } }).ToList();
            if(tryBridgeAddress.FirstOrDefault(_ => _.Contains("inet")) != null) {
                var bridgeAddress = tryBridgeAddress.Print(2, " ");
                ConsoleLogger.Log($"[network] {bridge} is now reachable at {bridgeAddress}");
                return;
            }
            ConsoleLogger.Log($"[network] {bridge} is unreachable");
        }

        public bool HasConfiguration() {
            var netif = Conf.Interfaces;
            var config = InterfaceConfigurationList.Any(_ => netif.Select(__ => __.Configuration).Contains(_.Id));
            return config;
        }
        #endregion

        #region [     Resolv(D)    ]
        public void SetupResolv() {
            if(!File.Exists("/etc/resolv.conf") || string.IsNullOrEmpty(File.ReadAllText("/etc/resolv.conf"))) {
                var lines = new List<string> {
                    "nameserver 8.8.8.8",
                    "search antd.local",
                    "domain antd.local"
                };
                File.WriteAllLines("/etc/resolv.conf", lines);
            }
            _launcher.Launch("link-s", new Dictionary<string, string> { { "$link", "/run/systemd/resolve/resolv.conf" }, { "$file", "/etc/resolv.conf" } });
        }

        public void SetupResolvD() {
            if(!File.Exists("/etc/systemd/resolved.conf") || string.IsNullOrEmpty(File.ReadAllText("/etc/systemd/resolved.conf"))) {
                var lines = new List<string> {
                    "[Resolve]",
                    "DNS=10.1.19.1 10.99.19.1",
                    "FallbackDNS=8.8.8.8 8.8.4.4 2001:4860:4860::8888 2001:4860:4860::8844",
                    "Domains=antd.local",
                    "LLMNR=yes"
                };
                File.WriteAllLines("/etc/systemd/resolved.conf", lines);
            }
            _launcher.Launch("systemctl-restart", new Dictionary<string, string> { { "$service", "systemd-resolved" } });
            _launcher.Launch("systemctl-daemonreload");
        }
        #endregion

        #region [    DnsConfiguration    ]
        private List<DnsConfiguration> GetDnsConfiguration() {
            var list = new List<DnsConfiguration>();
            var files = Directory.EnumerateFiles(_dir, $"*{DnsConfigurationExt}");
            foreach(var file in files) {
                try {
                    var text = File.ReadAllText(file);
                    var conf = JsonConvert.DeserializeObject<DnsConfiguration>(text);
                    list.Add(conf);
                }
                catch(Exception) {
                    //throw;
                }
            }
            return list;
        }

        public bool AddDnsConfiguration(DnsConfiguration conf) {
            if(string.IsNullOrEmpty(conf.Id)) {
                return false;
            }
            var file = $"{_dir}/{conf.Id}{DnsConfigurationExt}";
            var text = JsonConvert.SerializeObject(conf, Formatting.Indented);
            try {
                File.WriteAllText(file, text);
            }
            catch(Exception) {
                return false;
            }
            return File.Exists(file);
        }

        public bool RemoveDnsConfiguration(string id) {
            var file = $"{_dir}/{id}{DnsConfigurationExt}";
            if(!File.Exists(file)) {
                return false;
            }
            try {
                File.Delete(file);
            }
            catch(Exception) {
                return false;
            }
            return !File.Exists(file);
        }

        public void SetDnsConfigurationActive(string id) {
            Conf.ActiveDnsConfiguration = id;
            Save(Conf);
        }

        public void RemoveDnsConfigurationActive(string id) {
            Conf.ActiveDnsConfiguration = string.Empty;
            Save(Conf);
        }
        #endregion
    }
}
