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

        public Network2ConfigurationModel Conf;
        public List<NetworkInterfaceConfiguration> InterfaceConfigurationList => GetInterfaceConfiguration();
        public List<NetworkGatewayConfiguration> GatewayConfigurationList => GetGatewayConfiguration();
        public List<DnsConfiguration> DnsConfigurationList => GetDnsConfiguration();
        public List<NsUpdateConfiguration> NsUpdateConfigurationList => GetNsUpdateConfiguration();
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
        private const string NsUpdateConfigurationExt = ".nsu";

        private readonly CommandLauncher _launcher = new CommandLauncher();
        private readonly Bash _bash = new Bash();

        public Network2Configuration() {
            Directory.CreateDirectory(_dir);
            Conf = Parse();
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
            }
            catch(Exception ex) {
                ConsoleLogger.Error($"[network] configuration save error: {ex.Message}");
                return false;
            }
            return true;
        }

        public void AddInterfaceSetting(NetworkInterface model) {
            var netif = Conf.Interfaces.ToList();
            var check = netif.Where(_ => _.Device == model.Device).ToList();
            if(check.Any()) {
                check.ForEach(_ => RemoveInterfaceSetting(_.Device));
            }
            netif.Add(model);
            Conf.Interfaces = netif;
            Save(Conf);
        }

        public void RemoveInterfaceSetting(string device) {
            var netif = Conf.Interfaces.ToList();
            var model = netif.First(_ => _.Device == device);
            if(model == null) {
                return;
            }
            netif.Remove(model);
            Conf.Interfaces = netif;
            Save(Conf);
            _launcher.Launch("ip4-flush-configuration", new Dictionary<string, string> { { "$net_if", device } });
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

        #region [    NsUpdateConfiguration    ]
        private List<NsUpdateConfiguration> GetNsUpdateConfiguration() {
            var list = new List<NsUpdateConfiguration>();
            var files = Directory.EnumerateFiles(_dir, $"*{NsUpdateConfigurationExt}");
            foreach(var file in files) {
                try {
                    var text = File.ReadAllText(file);
                    var conf = JsonConvert.DeserializeObject<NsUpdateConfiguration>(text);
                    list.Add(conf);
                }
                catch(Exception) {
                    //throw;
                }
            }
            return list;
        }

        public bool AddNsUpdateConfiguration(NsUpdateConfiguration conf) {
            if(string.IsNullOrEmpty(conf.Id)) {
                return false;
            }
            var file = $"{_dir}/{conf.Id}{NsUpdateConfigurationExt}";
            var lines = new List<string>();
            if(!string.IsNullOrEmpty(conf.ServerName)) { lines.Add($"server {conf.ServerName} {conf.ServerPort}"); }
            if(!string.IsNullOrEmpty(conf.LocalAddress)) { lines.Add($"local  {conf.LocalAddress} {conf.LocalPort}"); }
            if(!string.IsNullOrEmpty(conf.ZoneName)) { lines.Add($"zone  {conf.ZoneName}"); }
            if(!string.IsNullOrEmpty(conf.ClassName)) { lines.Add($"class  {conf.ClassName}"); }
            if(!string.IsNullOrEmpty(conf.KeySecret)) { lines.Add($"key {conf.KeyName} {conf.KeySecret}"); }
            if(!string.IsNullOrEmpty(conf.NxDomain)) { lines.Add($"prereq nxdomain {conf.NxDomain}"); }
            if(!string.IsNullOrEmpty(conf.YxDomain)) { lines.Add($"prereq yxdomain {conf.YxDomain}"); }
            if(!string.IsNullOrEmpty(conf.NxRrset)) { lines.Add($"prereq nxrrset {conf.NxRrset}"); }
            if(!string.IsNullOrEmpty(conf.YxRrset)) { lines.Add($"prereq yxrrset {conf.YxRrset}"); }
            if(!string.IsNullOrEmpty(conf.Delete)) { lines.Add($"update delete {conf.Delete}"); }
            if(!string.IsNullOrEmpty(conf.Add)) { lines.Add($"update add {conf.Add}"); }
            lines.Add("show");
            lines.Add("send");
            try {
                File.WriteAllLines(file, lines);
            }
            catch(Exception) {
                return false;
            }
            return File.Exists(file);
        }

        public bool RemoveNsUpdateConfiguration(string id) {
            var file = $"{_dir}/{id}{NsUpdateConfigurationExt}";
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
