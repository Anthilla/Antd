//using System.Collections.Generic;
//using System.IO;
//using anthilla.core;

//namespace Antd.Networkd {

//    public enum NetworkdConfigurationType : byte {
//        Ipv4,
//        Ipv6,
//        All,
//        Static
//    }

//    public class NetworkdConfiguration {

//        private const string NetworkdFolder = "/etc/systemd/network";

//        public void Setup() {
//            if(Systemctl.IsEnabled("systemd-networkd.service") == false) {
//                Systemctl.Enable("systemd-networkd.service");
//            }
//            if(Systemctl.IsActive("systemd-networkd.service") == false) {
//                Systemctl.Start("systemd-networkd.service");
//            }
//            if(Systemctl.IsEnabled("systemd-resolved.service") == false) {
//                Systemctl.Enable("systemd-resolved.service");
//            }
//            if(Systemctl.IsActive("systemd-resolved.service") == false) {
//                Systemctl.Start("systemd-resolved.service");
//            }

//            Bash.Execute("ln -s /run/systemd/resolve/resolv.conf /etc/resolv.conf", false);

//            if(!Directory.Exists(NetworkdFolder)) {
//                Directory.CreateDirectory(NetworkdFolder);
//            }
//        }

//        public void Restart() {
//            Systemctl.Restart("systemd-networkd.service");
//        }

//        /// <summary>
//        /// Dynamic configuration using DHCP
//        /// 
//        /// [Match]
//        /// Name=enp1s0
//        /// 
//        /// [Network]
//        /// DHCP=ipv4
//        /// 
//        /// </summary>
//        /// <param name="networkInterface"></param>
//        /// <param name="type"></param>
//        public void Configure(string networkInterface, NetworkdConfigurationType type) {
//            var file = $"{NetworkdFolder}/{networkInterface}.network";
//            var lines = new List<string> {
//                "[Match]",
//                $"Name={networkInterface}",
//                "",
//                "[Network]",
//                $"DHCP={type.ToString().ToLower()}",
//                ""
//            };
//            FileWithAcl.WriteAllLines(file, lines, "644", "root", "wheel");
//            Restart();
//        }

//        /// <summary>
//        /// Static configuration 
//        /// 
//        /// [Match]
//        /// Name=enp1s0
//        /// 
//        /// [Network]
//        /// Address=10.1.10.9/24
//        /// Gateway=10.1.10.1
//        /// 
//        /// </summary>
//        /// <param name="networkInterface"></param>
//        /// <param name="dns"></param>
//        /// <param name="address"></param>
//        /// <param name="range"></param>
//        /// <param name="gateway"></param>
//        public void Configure(string networkInterface, string dns, string address, string range, string gateway) {
//            var file = $"{NetworkdFolder}/{networkInterface}.network";
//            var lines = new List<string> {
//                "[Match]",
//                $"Name={networkInterface}",
//                "",
//                "[Network]",
//                $"DNS={dns}",
//                $"Address={address}/{range}",
//                $"Gateway={gateway}",
//                ""
//            };
//            FileWithAcl.WriteAllLines(file, lines, "644", "root", "wheel");
//            Restart();
//        }

//        /// <summary>
//        /// Dynamic bridge configuration using DHCP
//        /// </summary>
//        /// <param name="bridgeName"></param>
//        /// <param name="networkInterfaces"></param>
//        /// <param name="type"></param>
//        public void ConfigureBridge(string bridgeName, IEnumerable<string> networkInterfaces, NetworkdConfigurationType type) {
//            var bridgeFile = $"{NetworkdFolder}/{bridgeName}.netdev";
//            var bridgeLines = new List<string> {
//                "[NetDev]",
//                $"Name={bridgeName}",
//                "Kind=bridge",
//                ""
//            };
//            FileWithAcl.WriteAllLines(bridgeFile, bridgeLines, "644", "root", "wheel");
//            Restart();

//            foreach(var networkInterface in networkInterfaces) {
//                var file = $"{NetworkdFolder}/{networkInterface}.network";
//                var lines = new List<string> {
//                    "[Match]",
//                    $"Name={networkInterface}",
//                    "",
//                    "[Network]",
//                    $"Bridge={bridgeName}",
//                    ""
//                };
//                FileWithAcl.WriteAllLines(file, lines, "644", "root", "wheel");
//            }

//            var bridgeNetworkFile = $"{NetworkdFolder}/{bridgeName}.network";
//            var bridgeNetworkLines = new List<string> {
//                "[Match]",
//                $"Name={bridgeName}",
//                "",
//                "[Network]",
//                $"DHCP={type.ToString().ToLower()}",
//                ""
//            };
//            FileWithAcl.WriteAllLines(bridgeNetworkFile, bridgeNetworkLines, "644", "root", "wheel");
//            Restart();
//        }

//        /// <summary>
//        /// Dynamic bridge configuration using DHCP
//        /// </summary>
//        /// <param name="bridgeName"></param>
//        /// <param name="networkInterfaces"></param>
//        /// <param name="dns"></param>
//        /// <param name="address"></param>
//        /// <param name="range"></param>
//        /// <param name="gateway"></param>
//        public void ConfigureBridge(string bridgeName, IEnumerable<string> networkInterfaces, string dns, string address, string range, string gateway) {
//            var bridgeFile = $"{NetworkdFolder}/{bridgeName}.netdev";
//            var bridgeLines = new List<string> {
//                "[NetDev]",
//                $"Name={bridgeName}",
//                "Kind=bridge",
//                ""
//            };
//            if(File.Exists(bridgeFile)) {
//                File.Copy(bridgeFile, $"{bridgeFile}.bck");
//            }
//            FileWithAcl.WriteAllLines(bridgeFile, bridgeLines, "644", "root", "wheel");
//            Restart();

//            foreach(var networkInterface in networkInterfaces) {
//                var file = $"{NetworkdFolder}/{networkInterface}.network";
//                var lines = new List<string> {
//                    "[Match]",
//                    $"Name={networkInterface}",
//                    "",
//                    "[Network]",
//                    $"Bridge={bridgeName}",
//                    ""
//                };
//                FileWithAcl.WriteAllLines(file, lines, "644", "root", "wheel");
//            }

//            var bridgeNetworkFile = $"{NetworkdFolder}/{bridgeName}.network";
//            var bridgeNetworkLines = new List<string> {
//                "[Match]",
//                $"Name={bridgeName}",
//                "",
//                "[Network]",
//                $"DNS={dns}",
//                $"Address={address}/{range}",
//                $"Gateway={gateway}",
//                ""
//            };
//            FileWithAcl.WriteAllLines(bridgeNetworkFile, bridgeNetworkLines, "644", "root", "wheel");
//            Restart();
//        }
//    }
//}
