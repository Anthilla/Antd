using antdlib.models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using anthilla.core;
using Parameter = antdlib.common.Parameter;
using System.Net.NetworkInformation;

namespace antdlib.config {
    public class Network2Configuration {

        public static Network2ConfigurationModel Conf => Parse();
        public static List<NetworkInterfaceConfiguration> InterfaceConfigurationList => GetInterfaceConfiguration();
        public static List<NetworkGatewayConfiguration> GatewayConfigurationList => GetGatewayConfiguration();
        public static List<NetworkRouteConfiguration> RouteConfigurationList => GetRouteConfiguration();
        public static List<DnsConfiguration> DnsConfigurationList => GetDnsConfiguration();
        public static List<NsUpdateConfiguration> NsUpdateConfigurationList => GetNsUpdateConfiguration();
        public static List<NetworkHardwareConfiguration> NetworkHardwareConfigurationList => GetNetworkHardwareConfiguration();
        public static List<NetworkAggregatedInterfaceConfiguration> NetworkAggregatedInterfaceConfigurationList => GetAggregatedInterfaceConfiguration();
        public static string[] NetworkInterfaces => GetAll();
        public static List<string> InterfacePhysical => SortNetworkInterfaces()[0];
        public static List<string> InterfaceBridge => SortNetworkInterfaces()[1];
        public static List<string> InterfaceBond => SortNetworkInterfaces()[2];
        public static List<string> InterfaceVirtual => SortNetworkInterfaces()[3];

        private static readonly string Dir = Parameter.AntdCfgNetwork;
        private static readonly string CfgFile = $"{Parameter.AntdCfgNetwork}/network.conf";

        private static readonly string InterfaceDir = $"{Dir}/addr";
        private const string InterfaceConfigurationExt = ".nif";
        private static readonly string GatewayDir = $"{Dir}/gateway";
        private const string GatewayConfigurationExt = ".gw";
        private static readonly string RouteDir = $"{Dir}/route";
        private const string RouteConfigurationExt = ".rt";
        private static readonly string DnsDir = $"{Dir}/dns";
        private const string DnsConfigurationExt = ".dns";
        private static readonly string NsUpdateDir = $"{Dir}/nsu";
        private const string NsUpdateConfigurationExt = ".nsu";
        private static readonly string NetworkHardwareDir = $"{Dir}/hw";
        private const string NetworkHardwareConfigurationExt = ".nhc";
        private static readonly string NetworkAggregatedInterfaceDir = $"{Dir}/aggregate";
        private const string NetworkAggregatedInterfaceConfigurationExt = ".lag";

        public static void CreateWorkingDirectories() {
            Directory.CreateDirectory(Dir);
            Directory.CreateDirectory(InterfaceDir);
            Directory.CreateDirectory(GatewayDir);
            Directory.CreateDirectory(RouteDir);
            Directory.CreateDirectory(DnsDir);
            Directory.CreateDirectory(NsUpdateDir);
            Directory.CreateDirectory(NetworkHardwareDir);
            Directory.CreateDirectory(NetworkAggregatedInterfaceDir);
        }

        public static void SetWorkingDirectories() {
            foreach(var file in Directory.EnumerateFiles(Dir, $"*{InterfaceConfigurationExt}*")) {
                MoveFile(file, InterfaceDir);
            }

            foreach(var file in Directory.EnumerateFiles(Dir, $"*{GatewayConfigurationExt}*")) {
                MoveFile(file, GatewayDir);
            }

            foreach(var file in Directory.EnumerateFiles(Dir, $"*{RouteConfigurationExt}*")) {
                MoveFile(file, RouteDir);
            }

            foreach(var file in Directory.EnumerateFiles(Dir, $"*{NsUpdateConfigurationExt}*")) {
                MoveFile(file, NsUpdateDir);
            }

            foreach(var file in Directory.EnumerateFiles(Dir, $"*{DnsConfigurationExt}*")) {
                MoveFile(file, DnsDir);
            }

            foreach(var file in Directory.EnumerateFiles(Dir, $"*{NetworkHardwareConfigurationExt}*")) {
                MoveFile(file, NetworkHardwareDir);
            }

            foreach(var file in Directory.EnumerateFiles(Dir, $"*{NetworkAggregatedInterfaceConfigurationExt}*")) {
                MoveFile(file, NetworkAggregatedInterfaceDir);
            }
        }

        private static void MoveFile(string src, string destinationDir) {
            var fname = Path.GetFileName(src);
            if(string.IsNullOrEmpty(fname)) { return; }
            var dst = Path.Combine(destinationDir, fname);
            File.Move(src, dst);
        }

        #region [    Network conf   ]
        private static Network2ConfigurationModel Parse() {
            var conf = new Network2ConfigurationModel();
            if(!File.Exists(CfgFile)) {
                return conf;
            }
            var text = File.ReadAllText(CfgFile);
            conf = JsonConvert.DeserializeObject<Network2ConfigurationModel>(text);
            return conf;
        }

        public static bool Save(Network2ConfigurationModel conf) {
            var text = JsonConvert.SerializeObject(conf, Formatting.Indented);
            FileWithAcl.WriteAllText(CfgFile, text, "644", "root", "wheel");
            return true;
        }

        public static void SaveInterfaceSetting(List<models.NetworkInterface> model) {
            var n = new Network2ConfigurationModel {
                Interfaces = model
            };
            Json.Save(n, CfgFile);
            ConsoleLogger.Error("[network] configuration saved");
        }
        #endregion

        #region [    NetworkInterfaceConfiguration    ]
        private static List<NetworkInterfaceConfiguration> GetInterfaceConfiguration() {
            var list = new List<NetworkInterfaceConfiguration>();
            var files = Directory.EnumerateFiles(InterfaceDir, $"*{InterfaceConfigurationExt}");
            var ints = Conf.Interfaces;
            foreach(var file in files) {
                var text = File.ReadAllText(file);
                var conf = JsonConvert.DeserializeObject<NetworkInterfaceConfiguration>(text);
                var mcContainsConf = ints.Select(_ => _.Configuration).Contains(conf.Id);
                var scContainsConf =
                    ints.Select(_ => _.AdditionalConfigurations.Where(__ => _.Configuration == conf.Id)).Any();
                conf.IsUsed = mcContainsConf || scContainsConf;
                list.Add(conf);
            }
            return list;
        }

        public static bool AddInterfaceConfiguration(NetworkInterfaceConfiguration conf) {
            if(string.IsNullOrEmpty(conf.Id)) {
                return false;
            }
            var file = $"{InterfaceDir}/{conf.Id}{InterfaceConfigurationExt}";
            var text = JsonConvert.SerializeObject(conf, Formatting.Indented);
            FileWithAcl.WriteAllText(file, text, "644", "root", "wheel");
            return File.Exists(file);
        }

        public static bool RemoveInterfaceConfiguration(string id) {
            var file = $"{InterfaceDir}/{id}{InterfaceConfigurationExt}";
            if(!File.Exists(file)) {
                return false;
            }
            File.Delete(file);
            return !File.Exists(file);
        }
        #endregion

        #region [    NetworkGatewayConfiguration    ]
        private static List<NetworkGatewayConfiguration> GetGatewayConfiguration() {
            var list = new List<NetworkGatewayConfiguration>();
            var files = Directory.EnumerateFiles(GatewayDir, $"*{GatewayConfigurationExt}");
            foreach(var file in files) {
                var text = File.ReadAllText(file);
                var conf = JsonConvert.DeserializeObject<NetworkGatewayConfiguration>(text);
                list.Add(conf);
            }
            return list;
        }

        public static bool AddGatewayConfiguration(NetworkGatewayConfiguration conf) {
            if(string.IsNullOrEmpty(conf.Id)) {
                return false;
            }
            var file = $"{GatewayDir}/{conf.Id}{GatewayConfigurationExt}";
            var text = JsonConvert.SerializeObject(conf, Formatting.Indented);
            FileWithAcl.WriteAllText(file, text, "644", "root", "wheel");
            return File.Exists(file);
        }

        public static bool RemoveGatewayConfiguration(string id) {
            var file = $"{GatewayDir}/{id}{GatewayConfigurationExt}";
            if(!File.Exists(file)) {
                return false;
            }
            File.Delete(file);
            return !File.Exists(file);
        }
        #endregion

        #region [    NetworkRouteConfiguration    ]
        private static List<NetworkRouteConfiguration> GetRouteConfiguration() {
            var list = new List<NetworkRouteConfiguration>();
            var files = Directory.EnumerateFiles(RouteDir, $"*{RouteConfigurationExt}");
            foreach(var file in files) {
                var text = File.ReadAllText(file);
                var conf = JsonConvert.DeserializeObject<NetworkRouteConfiguration>(text);
                list.Add(conf);
            }
            return list;
        }

        public static bool AddRouteConfiguration(NetworkRouteConfiguration conf) {
            if(string.IsNullOrEmpty(conf.Id)) {
                return false;
            }
            var file = $"{RouteDir}/{conf.Id}{RouteConfigurationExt}";
            var text = JsonConvert.SerializeObject(conf, Formatting.Indented);
            FileWithAcl.WriteAllText(file, text, "644", "root", "wheel");
            return File.Exists(file);
        }

        public static bool RemoveRouteConfiguration(string id) {
            var file = $"{RouteDir}/{id}{RouteConfigurationExt}";
            if(!File.Exists(file)) {
                return false;
            }
            File.Delete(file);
            return !File.Exists(file);
        }
        #endregion

        #region [    NsUpdateConfiguration    ]
        private static List<NsUpdateConfiguration> GetNsUpdateConfiguration() {
            var list = new List<NsUpdateConfiguration>();
            var files = Directory.EnumerateFiles(NsUpdateDir, $"*{NsUpdateConfigurationExt}");
            foreach(var file in files) {
                var text = File.ReadAllText(file);
                var conf = JsonConvert.DeserializeObject<NsUpdateConfiguration>(text);
                list.Add(conf);
            }
            return list;
        }

        public static bool AddNsUpdateConfiguration(NsUpdateConfiguration conf) {
            if(string.IsNullOrEmpty(conf.Id)) {
                return false;
            }
            var file = $"{NsUpdateDir}/{conf.Id}{NsUpdateConfigurationExt}";
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
            FileWithAcl.WriteAllLines(file, lines, "644", "root", "wheel");
            return File.Exists(file);
        }

        public static bool RemoveNsUpdateConfiguration(string id) {
            var file = $"{NsUpdateDir}/{id}{NsUpdateConfigurationExt}";
            if(!File.Exists(file)) {
                return false;
            }
            File.Delete(file);
            return !File.Exists(file);
        }

        #endregion

        #region [    Network Devices Mapping    ]

        private static string[] GetAll() {
            var ifs = System.Net.NetworkInformation.NetworkInterface.GetAllNetworkInterfaces();
            var names = new string[ifs.Length];
            for(var i = 0; i < ifs.Length; i++) {
                names[i] = ifs[i].Id;
            }
            return names;
        }

        private static List<string>[] SortNetworkInterfaces() {
            var sorted = new List<string>[4];
            sorted[0] = new List<string>(); //0 phy
            sorted[1] = new List<string>(); //1 br
            sorted[2] = new List<string>(); //2 bnd
            sorted[3] = new List<string>(); //3 virt
            var adapters = System.Net.NetworkInformation.NetworkInterface.GetAllNetworkInterfaces();
            for(var i = 0; i < adapters.Length; i++) {
                var adapter = adapters[i];
                if(adapter.NetworkInterfaceType != System.Net.NetworkInformation.NetworkInterfaceType.Ethernet) {
                    //3
                    sorted[3].Add(adapter.Id);
                }
                else {
                    if(adapter.Id.StartsWith("dummy")) {
                        sorted[3].Add(adapter.Id);
                    }
                    else if(adapter.Id.StartsWith("ifb")) {
                        sorted[3].Add(adapter.Id);
                    }
                    else if(adapter.Id.StartsWith("gretap")) {
                        sorted[3].Add(adapter.Id);
                    }
                    else if(adapter.Id.StartsWith("gretun")) {
                        sorted[3].Add(adapter.Id);
                    }
                    else if(adapter.Id.StartsWith("bond")) {
                        //2
                        sorted[2].Add(adapter.Id);
                    }
                    else if(adapter.Id.StartsWith("br")) {
                        //1
                        sorted[1].Add(adapter.Id);
                    }
                    else {
                        //0
                        sorted[0].Add(adapter.Id);
                    }
                }
            }
            return sorted;
        }

        //private static IEnumerable<string> GetPhysicalInterfaces() {
        //    var ifList = new List<string>();
        //    var list = Bash.Execute("ls -la /sys/class/net").Split().Where(_ => _.Contains("->"));
        //    foreach(var f in list) {
        //        if(f.Contains("bond")) { }
        //        else if(f.Contains("br")) { }
        //        else if(f.Contains("virtual/net") || f.Contains("platform")) { }
        //        else if(!f.Contains("virtual/net")) {
        //            var name = f.Print(9, " ");
        //            ifList.Add(name.Trim());
        //        }
        //    }
        //    return ifList;
        //}

        //private static IEnumerable<string> GetVirtualInterfaces() {
        //    var ifList = new List<string>();
        //    var list = Bash.Execute("ls -la /sys/class/net").Split().Where(_ => _.Contains("->"));
        //    foreach(var f in list) {
        //        if(f.Contains("bond")) { }
        //        else if(f.Contains("br")) { }
        //        else if(f.Contains("virtual/net") || f.Contains("platform")) {
        //            var name = f.Print(9, " ");
        //            ifList.Add(name.Trim());
        //        }
        //        else if(!f.Contains("virtual/net")) { }
        //    }
        //    return ifList;
        //}

        //private static IEnumerable<string> GetBondInterfaces() {
        //    var ifList = new List<string>();
        //    var list = Bash.Execute("ls -la /sys/class/net").Split().Where(_ => _.Contains("->"));
        //    foreach(var f in list) {
        //        if(f.Contains("bond")) {
        //            var name = f.Print(9, " ");
        //            ifList.Add(name.Trim());
        //        }
        //        else if(f.Contains("br")) { }
        //        else if(f.Contains("virtual/net") || f.Contains("platform")) { }
        //        else if(!f.Contains("virtual/net")) { }
        //    }
        //    return ifList;
        //}

        //private static IEnumerable<string> GetBridgeInterfaces() {
        //    var ifList = new List<string>();
        //    var list = Bash.Execute("ls -la /sys/class/net").Split().Where(_ => _.Contains("->"));
        //    foreach(var f in list) {
        //        if(f.Contains("bond")) { }
        //        else if(f.Contains("br")) {
        //            var name = f.Print(9, " ");
        //            ifList.Add(name.Trim());
        //        }
        //        else if(f.Contains("virtual/net") || f.Contains("platform")) { }
        //        else if(!f.Contains("virtual/net")) { }
        //    }
        //    return ifList;
        //}

        #endregion

        #region [    DnsConfiguration    ]
        private static List<DnsConfiguration> GetDnsConfiguration() {
            var list = new List<DnsConfiguration>();
            var files = Directory.EnumerateFiles(DnsDir, $"*{DnsConfigurationExt}");
            foreach(var file in files) {
                var text = File.ReadAllText(file);
                var conf = JsonConvert.DeserializeObject<DnsConfiguration>(text);
                list.Add(conf);
            }
            return list;
        }

        public static bool AddDnsConfiguration(DnsConfiguration conf) {
            if(string.IsNullOrEmpty(conf.Id)) {
                return false;
            }
            var file = $"{DnsDir}/{conf.Id}{DnsConfigurationExt}";
            var text = JsonConvert.SerializeObject(conf, Formatting.Indented);
            FileWithAcl.WriteAllText(file, text, "644", "root", "wheel");
            return File.Exists(file);
        }

        public static bool RemoveDnsConfiguration(string id) {
            var file = $"{DnsDir}/{id}{DnsConfigurationExt}";
            if(!File.Exists(file)) {
                return false;
            }
            File.Delete(file);
            return !File.Exists(file);
        }

        public static void SetDnsConfigurationActive(string id) {
            //Conf.ActiveDnsConfiguration = id;
            //Save(Conf);
        }

        public static void RemoveDnsConfigurationActive(string id) {
            //Conf.ActiveDnsConfiguration = string.Empty;
            //Save(Conf);
        }
        #endregion

        #region [    NetworkHardwareConfiguration    ]
        private static List<NetworkHardwareConfiguration> GetNetworkHardwareConfiguration() {
            var list = new List<NetworkHardwareConfiguration>();
            var files = Directory.EnumerateFiles(NetworkHardwareDir, $"*{NetworkHardwareConfigurationExt}");
            foreach(var file in files) {
                var text = File.ReadAllText(file);
                var conf = JsonConvert.DeserializeObject<NetworkHardwareConfiguration>(text);
                list.Add(conf);
            }
            return list;
        }

        public static bool AddNetworkHardwareConfiguration(NetworkHardwareConfiguration conf) {
            if(string.IsNullOrEmpty(conf.Id)) {
                return false;
            }
            var file = $"{NetworkHardwareDir}/{conf.Id}{NetworkHardwareConfigurationExt}";
            var text = JsonConvert.SerializeObject(conf, Formatting.Indented);
            FileWithAcl.WriteAllText(file, text, "644", "root", "wheel");
            return File.Exists(file);
        }

        public static bool RemoveNetworkHardwareConfiguration(string id) {
            var file = $"{NetworkHardwareDir}/{id}{NetworkHardwareConfigurationExt}";
            if(!File.Exists(file)) {
                return false;
            }
            File.Delete(file);
            return !File.Exists(file);
        }

        #endregion

        #region [    NetworkAggregatedInterfaceConfiguration    ]
        private static List<NetworkAggregatedInterfaceConfiguration> GetAggregatedInterfaceConfiguration() {
            var list = new List<NetworkAggregatedInterfaceConfiguration>();
            var files = Directory.EnumerateFiles(NetworkAggregatedInterfaceDir, $"*{NetworkAggregatedInterfaceConfigurationExt}");
            foreach(var file in files) {
                var conf = Json.Read<NetworkAggregatedInterfaceConfiguration>(file);
                list.Add(conf);
            }
            return list;
        }

        public static bool AddAggregatedInterfaceConfiguration(NetworkAggregatedInterfaceConfiguration conf) {
            if(string.IsNullOrEmpty(conf.Id)) {
                return false;
            }
            var file = $"{NetworkAggregatedInterfaceDir}/{conf.Id}{NetworkAggregatedInterfaceConfigurationExt}";
            Json.Save(conf, file);
            return File.Exists(file);
        }

        public static bool RemoveAggregatedInterfaceConfiguration(string id) {
            var file = $"{NetworkAggregatedInterfaceDir}/{id}{NetworkAggregatedInterfaceConfigurationExt}";
            if(!File.Exists(file)) {
                return false;
            }
            File.Delete(file);
            return !File.Exists(file);
        }
        #endregion
    }
}
