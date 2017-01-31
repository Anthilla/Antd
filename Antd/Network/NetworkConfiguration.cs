using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using antd.commands;
using antdlib.common;
using antdlib.models;
using Newtonsoft.Json;

namespace Antd.Network {
    public class NetworkConfiguration {

        private NetworkConfigurationModel _serviceModel;
        private readonly string _cfgFile = $"{Parameter.AntdCfgServices}/network.conf";
        private readonly string _cfgFileBackup = $"{Parameter.AntdCfgServices}/network.conf.bck";
        private readonly CommandLauncher _launcher = new CommandLauncher();

        public NetworkConfiguration() {
            _networkInterfaces = GetAllNames();
            InterfacePhysical = GetPhysicalInterfaces();
            InterfaceVirtual = GetVirtualInterfaces();
            InterfaceBond = GetBondInterfaces();
            InterfaceBridge = GetBridgeInterfaces();
            InterfacePhysicalModel = GetPhysicalInterfacesModel();
            InterfaceVirtualModel = GetVirtualInterfacesModel();
            InterfaceBondModel = GetBondInterfacesModel();
            InterfaceBridgeModel = GetBridgeInterfacesModel();
            Directory.CreateDirectory(Parameter.AntdCfgServices);
            if(!File.Exists(_cfgFile)) {
                _serviceModel = new NetworkConfigurationModel();
            }
            else {
                try {
                    var text = File.ReadAllText(_cfgFile);
                    var obj = JsonConvert.DeserializeObject<NetworkConfigurationModel>(text);
                    _serviceModel = obj;
                }
                catch(Exception) {
                    _serviceModel = new NetworkConfigurationModel();
                }
            }
        }

        private readonly IEnumerable<string> _networkInterfaces;
        public IEnumerable<string> InterfacePhysical;
        public IEnumerable<string> InterfaceVirtual;
        public IEnumerable<string> InterfaceBond;
        public IEnumerable<string> InterfaceBridge;

        public IEnumerable<NetworkInterfaceConfigurationModel> InterfacePhysicalModel;
        public IEnumerable<NetworkInterfaceConfigurationModel> InterfaceVirtualModel;
        public IEnumerable<NetworkInterfaceConfigurationModel> InterfaceBondModel;
        public IEnumerable<NetworkInterfaceConfigurationModel> InterfaceBridgeModel;

        private NetworkConfigurationModel LoadModel() {
            if(!File.Exists(_cfgFile)) {
                return new NetworkConfigurationModel();
            }
            try {
                return JsonConvert.DeserializeObject<NetworkConfigurationModel>(File.ReadAllText(_cfgFile));
            }
            catch(Exception) {
                return new NetworkConfigurationModel();
            }
        }

        public void Save(NetworkConfigurationModel model) {
            var text = JsonConvert.SerializeObject(model, Formatting.Indented);
            if(File.Exists(_cfgFile)) {
                File.Copy(_cfgFile, _cfgFileBackup, true);
            }
            File.WriteAllText(_cfgFile, text);
            ConsoleLogger.Log("[network] configuration saved");
        }

        public NetworkConfigurationModel Get() {
            return _serviceModel;
        }

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
            if(_networkInterfaces.All(_ => _ != bridge)) {
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

        public void ApplyDefaultInterfaceSetting() {
            _serviceModel = LoadModel();
            var interfaces = new List<string>();
            interfaces.AddRange(InterfacePhysical);
            interfaces.AddRange(InterfaceBridge);
            var launcher = new CommandLauncher();
            foreach(var netIf in interfaces) {
                launcher.Launch("ip4-set-mtu", new Dictionary<string, string> { { "$net_if", netIf }, { "$mtu", _serviceModel.DefaultMtu } });
                launcher.Launch("ip4-set-txqueuelen", new Dictionary<string, string> { { "$net_if", netIf }, { "$txqueuelen", _serviceModel.DefaultTxqueuelen } });
            }
        }

        public bool HasConfiguration() {
            _serviceModel = LoadModel();
            var netif = _serviceModel.Interfaces;
            return netif.Any(_ => _.Status == NetworkInterfaceStatus.Up);
        }

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
            if(!File.Exists("/etc/resolv.conf") || string.IsNullOrEmpty(File.ReadAllText("/etc/resolv.conf"))) {
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

        #region [    Interface Detection and Mapping  ]
        private static IEnumerable<string> GetAllNames() {
            if(!Parameter.IsUnix) {
                return new List<string>();
            }
            var bash = new Bash();
            var list = bash.Execute("ls -la /sys/class/net").SplitBash().Where(_ => _.Contains("->"));
            return list.Select(f => f.Print(9, " ")).ToList();
        }

        private IEnumerable<string> GetPhysicalInterfaces() {
            var ifList = new List<string>();
            var bash = new Bash();
            var list = bash.Execute("ls -la /sys/class/net").SplitBash().Where(_ => _.Contains("->"));
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

        private IEnumerable<NetworkInterfaceConfigurationModel> GetPhysicalInterfacesModel() {
            _serviceModel = LoadModel();
            var netifs = _serviceModel.Interfaces;
            var list = new List<NetworkInterfaceConfigurationModel>();
            foreach(var pi in InterfacePhysical) {
                if(netifs.Any(_ => _.Interface == pi)) {
                    list.Add(netifs.FirstOrDefault(_ => _.Interface == pi));
                }
                else {
                    list.Add(new NetworkInterfaceConfigurationModel {
                        Interface = pi
                    });
                }
            }
            return list;
        }

        private IEnumerable<string> GetVirtualInterfaces() {
            var ifList = new List<string>();
            var bash = new Bash();
            var list = bash.Execute("ls -la /sys/class/net").SplitBash().Where(_ => _.Contains("->"));
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

        private IEnumerable<NetworkInterfaceConfigurationModel> GetVirtualInterfacesModel() {
            _serviceModel = LoadModel();
            var netifs = _serviceModel.Interfaces;
            var list = new List<NetworkInterfaceConfigurationModel>();
            foreach(var pi in InterfaceVirtual) {
                if(netifs.Any(_ => _.Interface == pi)) {
                    list.Add(netifs.FirstOrDefault(_ => _.Interface == pi));
                }
                else {
                    list.Add(new NetworkInterfaceConfigurationModel {
                        Interface = pi
                    });
                }
            }
            return list;
        }

        private IEnumerable<string> GetBondInterfaces() {
            var ifList = new List<string>();
            var bash = new Bash();
            var list = bash.Execute("ls -la /sys/class/net").SplitBash().Where(_ => _.Contains("->"));
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

        private IEnumerable<NetworkInterfaceConfigurationModel> GetBondInterfacesModel() {
            _serviceModel = LoadModel();
            var netifs = _serviceModel.Interfaces;
            var list = new List<NetworkInterfaceConfigurationModel>();
            foreach(var pi in InterfaceBond) {
                if(netifs.Any(_ => _.Interface == pi)) {
                    list.Add(netifs.FirstOrDefault(_ => _.Interface == pi));
                }
                else {
                    list.Add(new NetworkInterfaceConfigurationModel {
                        Interface = pi
                    });
                }
            }
            return list;
        }

        private IEnumerable<string> GetBridgeInterfaces() {
            var ifList = new List<string>();
            var bash = new Bash();
            var list = bash.Execute("ls -la /sys/class/net").SplitBash().Where(_ => _.Contains("->"));
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

        private IEnumerable<NetworkInterfaceConfigurationModel> GetBridgeInterfacesModel() {
            _serviceModel = LoadModel();
            var netifs = _serviceModel.Interfaces;
            var list = new List<NetworkInterfaceConfigurationModel>();
            foreach(var pi in InterfaceBridge) {
                if(netifs.Any(_ => _.Interface == pi)) {
                    list.Add(netifs.FirstOrDefault(_ => _.Interface == pi));
                }
                else {
                    list.Add(new NetworkInterfaceConfigurationModel {
                        Interface = pi
                    });
                }
            }
            return list;
        }
        #endregion

        #region [    Network Interface    ]
        public void AddInterfaceSetting(NetworkInterfaceConfigurationModel model) {
            _serviceModel = LoadModel();
            var netif = _serviceModel.Interfaces;
            var check = netif.Where(_ => _.Interface == model.Interface).ToList();
            if(check.Any()) {
                check.ForEach(_ => RemoveInterfaceSetting(_.Guid));
            }
            netif.Add(model);
            _serviceModel.Interfaces = netif;
            Save(_serviceModel);
        }

        public void RemoveInterfaceSetting(string guid) {
            _serviceModel = LoadModel();
            var netif = _serviceModel.Interfaces;
            var model = netif.First(_ => _.Guid == guid);
            if(model == null) {
                return;
            }
            netif.Remove(model);
            _serviceModel.Interfaces = netif;
            Save(_serviceModel);
        }

        public void ApplyInterfaceSetting() {
            _serviceModel = LoadModel();
            var netifs = _serviceModel.Interfaces;
            foreach(var netif in netifs) {
                ApplyInterfaceSetting(netif);
            }
        }

        public void ApplyInterfaceSetting(NetworkInterfaceConfigurationModel model) {
            var launcher = new CommandLauncher();
            var netif = model.Interface;
            var mode = model.Mode;
            switch(mode) {
                case NetworkInterfaceMode.Null:
                    return;
                case NetworkInterfaceMode.Static:
                    var networkdIsActive = Systemctl.IsActive("systemd-networkd");
                    if(networkdIsActive) {
                        Systemctl.Stop("systemd-networkd");
                    }
                    launcher.Launch("dhclient-killall");
                    launcher.Launch("ip4-flush-configuration", new Dictionary<string, string> {
                        { "$net_if", netif }
                    });
                    launcher.Launch("ip4-add-addr", new Dictionary<string, string> {
                        { "$net_if", netif },
                        { "$address", model.StaticAddress },
                        { "$range", model.StaticRange }
                    });
                    if(networkdIsActive) {
                        Systemctl.Start("systemd-networkd");
                    }
                    break;
                case NetworkInterfaceMode.Dynamic:
                    launcher.Launch("dhclient4", new Dictionary<string, string> { { "$net_if", netif } });
                    break;
                default:
                    return;
            }
            launcher.Launch("ip4-set-mtu", new Dictionary<string, string> { { "$net_if", netif }, { "$mtu", model.Mtu } });
            launcher.Launch("ip4-set-txqueuelen", new Dictionary<string, string> { { "$net_if", netif }, { "$txqueuelen", model.Txqueuelen } });
            var status = model.Status;
            switch(status) {
                case NetworkInterfaceStatus.Down:
                    launcher.Launch("ip4-disable-if", new Dictionary<string, string> { { "$net_if", netif } });
                    break;
                case NetworkInterfaceStatus.Up:
                    launcher.Launch("ip4-enable-if", new Dictionary<string, string> { { "$net_if", netif } });
                    ConsoleLogger.Log($"[network] interface '{model.Interface}' configured");
                    break;
                default:
                    launcher.Launch("ip4-disable-if", new Dictionary<string, string> { { "$net_if", netif } });
                    break;
            }
        }
        #endregion
    }
}
