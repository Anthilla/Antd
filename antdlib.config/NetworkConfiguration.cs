using antdlib.models;
using anthilla.commands;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using anthilla.core;
using Parameter = antdlib.common.Parameter;

namespace antdlib.config {
    /// <summary>
    /// Deprecated. Use Network2Configuration instead.
    /// </summary>
    public static class NetworkConfiguration {

        private static NetworkConfigurationModel ServiceModel => Load();
        private static readonly string CfgFile = $"{Parameter.AntdCfgServices}/network.conf";
        private static readonly string CfgFileBackup = $"{Parameter.AntdCfgServices}/network.conf.bck";

        public static NetworkConfigurationModel Load() {
            if(!File.Exists(CfgFile)) {
                return new NetworkConfigurationModel();
            }
            try {
                var text = File.ReadAllText(CfgFile);
                var obj = JsonConvert.DeserializeObject<NetworkConfigurationModel>(text);
                return obj;
            }
            catch(Exception) {
                return new NetworkConfigurationModel();
            }
        }

        private static IEnumerable<string> _networkInterfaces => GetAllNames();
        public static IEnumerable<string> NetworkInterfaces => GetAllNames();
        public static IEnumerable<string> InterfacePhysical => GetPhysicalInterfaces();
        public static IEnumerable<string> InterfaceVirtual => GetVirtualInterfaces();
        public static IEnumerable<string> InterfaceBond => GetBondInterfaces();
        public static IEnumerable<string> InterfaceBridge => GetBridgeInterfaces();

        public static IEnumerable<NetworkInterfaceConfigurationModel> InterfacePhysicalModel => GetPhysicalInterfacesModel();
        public static IEnumerable<NetworkInterfaceConfigurationModel> InterfaceVirtualModel => GetVirtualInterfacesModel();
        public static IEnumerable<NetworkInterfaceConfigurationModel> InterfaceBondModel => GetBondInterfacesModel();
        public static IEnumerable<NetworkInterfaceConfigurationModel> InterfaceBridgeModel => GetBridgeInterfacesModel();

        public static void Save(NetworkConfigurationModel model) {
            var text = JsonConvert.SerializeObject(model, Formatting.Indented);
            if(File.Exists(CfgFile)) {
                File.Copy(CfgFile, CfgFileBackup, true);
            }
            File.WriteAllText(CfgFile, text);
            ConsoleLogger.Log("[network] configuration saved");
        }

        public static NetworkConfigurationModel Get() {
            return ServiceModel;
        }

        public static void Start() {
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
            var tryStart = CommandLauncher.Launch("dhcpcd", new Dictionary<string, string> { { "$net_if", netIf } }).ToList();
            if(!tryStart.Any()) {
                ConsoleLogger.Log("[network] dhcp client started");
                ConsoleLogger.Log($"[network] {netIf} is configured");
                return;
            }
            var tryGateway =
                CommandLauncher.Launch("ip4-show-routes", new Dictionary<string, string> { { "$net_if", netIf } }).ToList();
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

            var tryDns = CommandLauncher.Launch("cat-etc-resolv").ToList();
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
            var pingDns = CommandLauncher.Launch("ping-c", new Dictionary<string, string> { { "$net_if", dnsAddress } }).ToList();
            if(pingDns.Any(_ => _.ToLower().Contains("unreachable"))) {
                ConsoleLogger.Log("[network] dns is unreachable");
                return;
            }
            ConsoleLogger.Log($"[network] available dns at {dnsAddress}");
        }

        public static void StartFallback() {
            ConsoleLogger.Log("[network] setting up a default configuration");
            const string bridge = "br0";
            if(_networkInterfaces.All(_ => _ != bridge)) {
                CommandLauncher.Launch("brctl-add", new Dictionary<string, string> { { "$bridge", bridge } });
                ConsoleLogger.Log($"[network] {bridge} configured");
            }
            foreach(var phy in InterfacePhysical) {
                CommandLauncher.Launch("brctl-addif", new Dictionary<string, string> { { "$bridge", bridge }, { "$net_if", phy } });
                ConsoleLogger.Log($"[network] {phy} add to {bridge}");
            }
            foreach(var phy in InterfacePhysical) {
                CommandLauncher.Launch("ip4-enable-if", new Dictionary<string, string> { { "$net_if", phy } });
            }
            CommandLauncher.Launch("ip4-enable-if", new Dictionary<string, string> { { "$net_if", bridge } });
            ConsoleLogger.Log("[network] interfaces up");
            const string address = "192.168.1.1";
            const string range = "24";
            CommandLauncher.Launch("ip4-add-addr",
                new Dictionary<string, string> { { "$address", address }, { "$range", range }, { "$net_if", bridge } });
            var tryBridgeAddress =
                CommandLauncher.Launch("ifconfig-if", new Dictionary<string, string> { { "$net_if", bridge } }).ToList();
            if(tryBridgeAddress.FirstOrDefault(_ => _.Contains("inet")) != null) {
                var bridgeAddress = tryBridgeAddress.Print(2, " ");
                ConsoleLogger.Log($"[network] {bridge} is now reachable at {bridgeAddress}");
                return;
            }
            ConsoleLogger.Log($"[network] {bridge} is unreachable");
        }

        public static void ApplyDefaultInterfaceSetting() {
            var interfaces = new List<string>();
            interfaces.AddRange(InterfacePhysical);
            interfaces.AddRange(InterfaceBridge);
            foreach(var netIf in interfaces) {
                CommandLauncher.Launch("ip4-set-mtu",
                    new Dictionary<string, string> { { "$net_if", netIf }, { "$mtu", ServiceModel.DefaultMtu } });
                CommandLauncher.Launch("ip4-set-txqueuelen",
                    new Dictionary<string, string>
                    {
                        {"$net_if", netIf},
                        {"$txqueuelen", ServiceModel.DefaultTxqueuelen}
                    });
            }
        }

        public static bool HasConfiguration() {
            var netif = ServiceModel.Interfaces;
            return netif.Any(_ => _.Status == NetworkInterfaceStatus.Up);
        }

        #region [     Resolv(D)    ]

        public static void SetupResolv() {
            if(!File.Exists("/etc/resolv.conf") || string.IsNullOrEmpty(File.ReadAllText("/etc/resolv.conf"))) {
                var lines = new List<string>
                {
                    "nameserver 8.8.8.8",
                    "search antd.local",
                    "domain antd.local"
                };
                File.WriteAllLines("/etc/resolv.conf", lines);
            }
            CommandLauncher.Launch("link-s",
                new Dictionary<string, string>
                {
                    {"$link", "/run/systemd/resolve/resolv.conf"},
                    {"$file", "/etc/resolv.conf"}
                });
        }

        public static void SetupResolvD() {
            if(!File.Exists("/etc/systemd/resolved.conf") ||
                string.IsNullOrEmpty(File.ReadAllText("/etc/systemd/resolved.conf"))) {
                var lines = new List<string>
                {
                    "[Resolve]",
                    "DNS=10.1.19.1 10.99.19.1",
                    "FallbackDNS=8.8.8.8 8.8.4.4 2001:4860:4860::8888 2001:4860:4860::8844",
                    "Domains=antd.local",
                    "LLMNR=yes"
                };
                File.WriteAllLines("/etc/systemd/resolved.conf", lines);
            }
            CommandLauncher.Launch("systemctl-restart", new Dictionary<string, string> { { "$service", "systemd-resolved" } });
            CommandLauncher.Launch("systemctl-daemonreload");
        }

        #endregion

        #region [    Interface Detection and Mapping  ]

        private static IEnumerable<string> GetAllNames() {
            try {
                var list = Bash.Execute("ls -la /sys/class/net").SplitBash().Where(_ => _.Contains("->"));
                return list.Select(f => f.Print(9, " ")).ToList();
            }
            catch(Exception) {
                return new List<string>();
            }
        }

        private static IEnumerable<string> GetPhysicalInterfaces() {
            var ifList = new List<string>();
            var list = Bash.Execute("ls -la /sys/class/net").SplitBash().Where(_ => _.Contains("->"));
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

        private static IEnumerable<NetworkInterfaceConfigurationModel> GetPhysicalInterfacesModel() {
            var netifs = ServiceModel.Interfaces;
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

        private static IEnumerable<string> GetVirtualInterfaces() {
            var ifList = new List<string>();
            var list = Bash.Execute("ls -la /sys/class/net").SplitBash().Where(_ => _.Contains("->"));
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

        private static IEnumerable<NetworkInterfaceConfigurationModel> GetVirtualInterfacesModel() {
            var netifs = ServiceModel.Interfaces;
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

        private static IEnumerable<string> GetBondInterfaces() {
            var ifList = new List<string>();
            var list = Bash.Execute("ls -la /sys/class/net").SplitBash().Where(_ => _.Contains("->"));
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

        private static IEnumerable<NetworkInterfaceConfigurationModel> GetBondInterfacesModel() {
            var netifs = ServiceModel.Interfaces;
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

        private static IEnumerable<string> GetBridgeInterfaces() {
            var ifList = new List<string>();
            var list = Bash.Execute("ls -la /sys/class/net").SplitBash().Where(_ => _.Contains("->"));
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

        private static IEnumerable<NetworkInterfaceConfigurationModel> GetBridgeInterfacesModel() {
            var netifs = ServiceModel.Interfaces;
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
        public static void AddInterfaceSetting(NetworkInterfaceConfigurationModel model) {
            var netif = ServiceModel.Interfaces;
            var check = netif.Where(_ => _.Interface == model.Interface).ToList();
            if(check.Any()) {
                check.ForEach(_ => RemoveInterfaceSetting(_.Guid));
            }
            netif.Add(model);
            ServiceModel.Interfaces = netif;
            Save(ServiceModel);
        }

        public static void RemoveInterfaceSetting(string guid) {
            var netif = ServiceModel.Interfaces;
            var model = netif.First(_ => _.Guid == guid);
            if(model == null) {
                return;
            }
            netif.Remove(model);
            ServiceModel.Interfaces = netif;
            Save(ServiceModel);
        }

        public static void ApplyInterfaceSetting() {
            var netifs = ServiceModel.Interfaces;
            foreach(var netif in netifs) {
                ApplyInterfaceSetting(netif);
            }
        }

        public static void ApplyInterfaceSetting(NetworkInterfaceConfigurationModel model) {
            var netif = model.Interface;

            switch(model.Type) {
                case NetworkAdapterType.Physical:
                    break;
                case NetworkAdapterType.Virtual:
                    break;
                case NetworkAdapterType.Bond:
                    CommandLauncher.Launch("bond-set", new Dictionary<string, string> { { "$bond", netif } });
                    foreach(var nif in model.InterfaceList) {
                        CommandLauncher.Launch("bond-add-if", new Dictionary<string, string> { { "$bond", netif }, { "$net_if", nif } });
                    }
                    break;
                case NetworkAdapterType.Bridge:
                    CommandLauncher.Launch("brctl-add", new Dictionary<string, string> { { "$bridge", netif } });
                    foreach(var nif in model.InterfaceList) {
                        CommandLauncher.Launch("brctl-add-if", new Dictionary<string, string> { { "$bridge", netif }, { "$net_if", nif } });
                    }
                    break;
                case NetworkAdapterType.Other:
                    break;
            }

            switch(model.Mode) {
                case NetworkInterfaceMode.Null:
                    return;
                case NetworkInterfaceMode.Static:
                    var networkdIsActive = Systemctl.IsActive("systemd-networkd");
                    if(networkdIsActive) {
                        Systemctl.Stop("systemd-networkd");
                    }
                    CommandLauncher.Launch("dhcpcd-killall");
                    CommandLauncher.Launch("ip4-flush-configuration", new Dictionary<string, string> {
                        { "$net_if", netif }
                    });
                    CommandLauncher.Launch("ip4-add-addr", new Dictionary<string, string> {
                        { "$net_if", netif },
                        { "$address", model.StaticAddress },
                        { "$range", model.StaticRange }
                    });
                    if(networkdIsActive) {
                        Systemctl.Start("systemd-networkd");
                    }
                    break;
                case NetworkInterfaceMode.Dynamic:
                    CommandLauncher.Launch("dhcpcd", new Dictionary<string, string> { { "$net_if", netif } });
                    break;
                default:
                    return;
            }
            CommandLauncher.Launch("ip4-set-mtu", new Dictionary<string, string> { { "$net_if", netif }, { "$mtu", model.Mtu } });
            CommandLauncher.Launch("ip4-set-txqueuelen", new Dictionary<string, string> { { "$net_if", netif }, { "$txqueuelen", model.Txqueuelen } });

            if(!string.IsNullOrEmpty(model.Route) && !string.IsNullOrEmpty(model.Gateway)) {
                CommandLauncher.Launch("ip4-add-route", new Dictionary<string, string> { { "$net_if", netif }, { "$gateway", model.Gateway }, { "$ip_address", model.Route } });
            }
            var status = model.Status;
            switch(status) {
                case NetworkInterfaceStatus.Down:
                    CommandLauncher.Launch("ip4-disable-if", new Dictionary<string, string> { { "$net_if", netif } });
                    break;
                case NetworkInterfaceStatus.Up:
                    CommandLauncher.Launch("ip4-enable-if", new Dictionary<string, string> { { "$net_if", netif } });
                    ConsoleLogger.Log($"[network] interface '{model.Interface}' configured");
                    break;
                default:
                    CommandLauncher.Launch("ip4-disable-if", new Dictionary<string, string> { { "$net_if", netif } });
                    break;
            }
        }
        #endregion
    }
}
