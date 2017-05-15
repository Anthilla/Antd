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

        public static Network2ConfigurationModel Conf => Parse();
        public static List<NetworkInterfaceConfiguration> InterfaceConfigurationList => GetInterfaceConfiguration();
        public static List<NetworkGatewayConfiguration> GatewayConfigurationList => GetGatewayConfiguration();
        public static List<DnsConfiguration> DnsConfigurationList => GetDnsConfiguration();
        public static List<NsUpdateConfiguration> NsUpdateConfigurationList => GetNsUpdateConfiguration();
        public static IEnumerable<string> NetworkInterfaces => GetAll();
        public static IEnumerable<string> InterfacePhysical => GetPhysicalInterfaces();
        public static IEnumerable<string> InterfaceVirtual => GetVirtualInterfaces();
        public static IEnumerable<string> InterfaceBond => GetBondInterfaces();
        public static IEnumerable<string> InterfaceBridge => GetBridgeInterfaces();

        private static readonly string Dir = Parameter.AntdCfgNetwork;
        private static readonly string CfgFile = $"{Parameter.AntdCfgNetwork}/network.conf";

        private const string InterfaceConfigurationExt = ".nif";
        private const string GatewayConfigurationExt = ".gw";
        private const string DnsConfigurationExt = ".dns";
        private const string NsUpdateConfigurationExt = ".nsu";

        #region [    Network conf   ]

        private static Network2ConfigurationModel Parse() {
            var conf = new Network2ConfigurationModel();
            if(!File.Exists(CfgFile)) {
                return conf;
            }
            try {
                var text = File.ReadAllText(CfgFile);
                conf = JsonConvert.DeserializeObject<Network2ConfigurationModel>(text);
            }
            catch(Exception) {
                conf = new Network2ConfigurationModel();
            }
            return conf;
        }

        public static bool Save(Network2ConfigurationModel conf) {
            var text = JsonConvert.SerializeObject(conf, Formatting.Indented);
            try {
                FileWithAcl.WriteAllText(CfgFile, text, "644", "root", "wheel");
            }
            catch(Exception ex) {
                ConsoleLogger.Error($"[network] configuration save error: {ex.Message}");
                return false;
            }
            return true;
        }

        public static void AddInterfaceSetting(NetworkInterface model) {
            var netif = Conf.Interfaces.ToList();
            var check = netif.Where(_ => _.Device == model.Device).ToList();
            if(check.Any()) {
                check.ForEach(_ => RemoveInterfaceSetting(_.Device));
            }
            netif.Add(model);
            Conf.Interfaces = netif;
            Save(Conf);
        }

        public static void RemoveInterfaceSetting(string device) {
            var netif = Conf.Interfaces.ToList();
            var model = netif.First(_ => _.Device == device);
            if(model == null) {
                return;
            }
            netif.Remove(model);
            Conf.Interfaces = netif;
            Save(Conf);
            CommandLauncher.Launch("ip4-flush-configuration", new Dictionary<string, string> { { "$net_if", device } });
        }

        #endregion

        #region [    NetworkInterfaceConfiguration    ]

        private static List<NetworkInterfaceConfiguration> GetInterfaceConfiguration() {
            var list = new List<NetworkInterfaceConfiguration>();
            var files = Directory.EnumerateFiles(Dir, $"*{InterfaceConfigurationExt}");
            var ints = Conf.Interfaces;
            foreach(var file in files) {
                try {
                    var text = File.ReadAllText(file);
                    var conf = JsonConvert.DeserializeObject<NetworkInterfaceConfiguration>(text);
                    var mcContainsConf = ints.Select(_ => _.Configuration).Contains(conf.Id);
                    var scContainsConf =
                        ints.Select(_ => _.AdditionalConfigurations.Where(__ => _.Configuration == conf.Id)).Any();
                    conf.IsUsed = mcContainsConf || scContainsConf;
                    list.Add(conf);
                }
                catch(Exception) {
                    //throw;
                }
            }
            return list;
        }

        public static bool AddInterfaceConfiguration(NetworkInterfaceConfiguration conf) {
            if(string.IsNullOrEmpty(conf.Id)) {
                return false;
            }
            var file = $"{Dir}/{conf.Id}{InterfaceConfigurationExt}";
            var text = JsonConvert.SerializeObject(conf, Formatting.Indented);
            try {
                FileWithAcl.WriteAllText(file, text, "644", "root", "wheel");
            }
            catch(Exception) {
                return false;
            }
            return File.Exists(file);
        }

        public static bool RemoveInterfaceConfiguration(string id) {
            var file = $"{Dir}/{id}{InterfaceConfigurationExt}";
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

        private static List<NetworkGatewayConfiguration> GetGatewayConfiguration() {
            var list = new List<NetworkGatewayConfiguration>();
            var files = Directory.EnumerateFiles(Dir, $"*{GatewayConfigurationExt}");
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

        public static bool AddGatewayConfiguration(NetworkGatewayConfiguration conf) {
            if(string.IsNullOrEmpty(conf.Id)) {
                return false;
            }
            var file = $"{Dir}/{conf.Id}{GatewayConfigurationExt}";
            var text = JsonConvert.SerializeObject(conf, Formatting.Indented);
            try {
                FileWithAcl.WriteAllText(file, text, "644", "root", "wheel");
            }
            catch(Exception) {
                return false;
            }
            return File.Exists(file);
        }

        public static bool RemoveGatewayConfiguration(string id) {
            var file = $"{Dir}/{id}{GatewayConfigurationExt}";
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

        private static List<NsUpdateConfiguration> GetNsUpdateConfiguration() {
            var list = new List<NsUpdateConfiguration>();
            var files = Directory.EnumerateFiles(Dir, $"*{NsUpdateConfigurationExt}");
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

        public static bool AddNsUpdateConfiguration(NsUpdateConfiguration conf) {
            if(string.IsNullOrEmpty(conf.Id)) {
                return false;
            }
            var file = $"{Dir}/{conf.Id}{NsUpdateConfigurationExt}";
            var lines = new List<string>();
            if(!string.IsNullOrEmpty(conf.ServerName)) {
                lines.Add($"server {conf.ServerName} {conf.ServerPort}");
            }
            if(!string.IsNullOrEmpty(conf.LocalAddress)) {
                lines.Add($"local  {conf.LocalAddress} {conf.LocalPort}");
            }
            if(!string.IsNullOrEmpty(conf.ZoneName)) {
                lines.Add($"zone  {conf.ZoneName}");
            }
            if(!string.IsNullOrEmpty(conf.ClassName)) {
                lines.Add($"class  {conf.ClassName}");
            }
            if(!string.IsNullOrEmpty(conf.KeySecret)) {
                lines.Add($"key {conf.KeyName} {conf.KeySecret}");
            }
            if(!string.IsNullOrEmpty(conf.NxDomain)) {
                lines.Add($"prereq nxdomain {conf.NxDomain}");
            }
            if(!string.IsNullOrEmpty(conf.YxDomain)) {
                lines.Add($"prereq yxdomain {conf.YxDomain}");
            }
            if(!string.IsNullOrEmpty(conf.NxRrset)) {
                lines.Add($"prereq nxrrset {conf.NxRrset}");
            }
            if(!string.IsNullOrEmpty(conf.YxRrset)) {
                lines.Add($"prereq yxrrset {conf.YxRrset}");
            }
            if(!string.IsNullOrEmpty(conf.Delete)) {
                lines.Add($"update delete {conf.Delete}");
            }
            if(!string.IsNullOrEmpty(conf.Add)) {
                lines.Add($"update add {conf.Add}");
            }
            lines.Add("show");
            lines.Add("send");
            try {
                FileWithAcl.WriteAllLines(file, lines, "644", "root", "wheel");
            }
            catch(Exception) {
                return false;
            }
            return File.Exists(file);
        }

        public static bool RemoveNsUpdateConfiguration(string id) {
            var file = $"{Dir}/{id}{NsUpdateConfigurationExt}";
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

        private static IEnumerable<string> GetAll() {
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

        #endregion

        #region [    DnsConfiguration    ]
        private static List<DnsConfiguration> GetDnsConfiguration() {
            var list = new List<DnsConfiguration>();
            var files = Directory.EnumerateFiles(Dir, $"*{DnsConfigurationExt}");
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

        public static bool AddDnsConfiguration(DnsConfiguration conf) {
            if(string.IsNullOrEmpty(conf.Id)) {
                return false;
            }
            var file = $"{Dir}/{conf.Id}{DnsConfigurationExt}";
            var text = JsonConvert.SerializeObject(conf, Formatting.Indented);
            try {
                FileWithAcl.WriteAllText(file, text, "644", "root", "wheel");
            }
            catch(Exception) {
                return false;
            }
            return File.Exists(file);
        }

        public static bool RemoveDnsConfiguration(string id) {
            var file = $"{Dir}/{id}{DnsConfigurationExt}";
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

        public static void SetDnsConfigurationActive(string id) {
            Conf.ActiveDnsConfiguration = id;
            Save(Conf);
        }

        public static void RemoveDnsConfigurationActive(string id) {
            Conf.ActiveDnsConfiguration = string.Empty;
            Save(Conf);
        }
        #endregion
    }
}
