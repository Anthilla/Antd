using Antd2.models;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Antd.parsing {
    public class DhcpdParser {

        private static string CaptureGroup(string sourceText, string pattern) {
            var regex = new Regex(pattern);
            var matchedGroups = regex.Match(sourceText).Groups;
            var capturedText = matchedGroups[1].Value;
            return capturedText;
        }

        //(allow[\s]+[\w\-]+;)
        public static List<string> ParseAllow(string text) {
            var list = new List<string>();
            var regex = new Regex("(allow[\\s]+[\\w\\-]+;)");
            var matches = regex.Matches(text);
            foreach (var match in matches) {
                var file = CaptureGroup(match.ToString(), "allow[\\s]+([\\w\\-]+);");
                list.Add(file);
            }
            return list;
        }

        public static DhcpdModel ParseParameters(DhcpdModel dhcpdConfigurationModel, string text) {
            dhcpdConfigurationModel.UpdateStaticLeases = CaptureGroup(text, "update-static-leases[\\s]+([\\w\\-]+);");
            dhcpdConfigurationModel.UpdateConflictDetection = CaptureGroup(text, "update-conflict-detection[\\s]+([\\w\\-]+);");
            dhcpdConfigurationModel.UseHostDeclNames = CaptureGroup(text, "use-host-decl-names[\\s]+([\\w\\-]+);");
            dhcpdConfigurationModel.DoForwardUpdates = CaptureGroup(text, "do-forward-updates[\\s]+([\\w\\-]+);");
            dhcpdConfigurationModel.DoReverseUpdates = CaptureGroup(text, "do-reverse-updates[\\s]+([\\w\\-]+);");
            dhcpdConfigurationModel.LogFacility = CaptureGroup(text, "log-facility[\\s]+([\\w\\d\\-]+);");
            dhcpdConfigurationModel.DefaultLeaseTime = CaptureGroup(text, "default-lease-time[\\s]+([\\d]+);");
            dhcpdConfigurationModel.MaxLeaseTime = CaptureGroup(text, "max-lease-time[\\s]+([\\d]+);");
            dhcpdConfigurationModel.OptionRouters = CaptureGroup(text, "option[\\s]+routers[\\s]+([\\w\\d\\-\\s.=\"]+);");
            dhcpdConfigurationModel.OptionLocalProxy = CaptureGroup(text, "option[\\s]+local-proxy-config[\\s]+code[\\s]+([\\w\\d\\-\\s.=\"]+);");
            dhcpdConfigurationModel.OptionDomainName = CaptureGroup(text, "option[\\s]+domain-name[\\s]+([\\w\\d\\-\\s.=\"]+);");
            dhcpdConfigurationModel.ZoneName = CaptureGroup(text, "zone[\\s]+([\\w.\\d]+)[\\s]+{");
            dhcpdConfigurationModel.ZonePrimaryAddress = CaptureGroup(text, "zone[\\s]+[\\w.\\d]+[\\s]+{[\\s]+primary[\\s]+([.\\d]+);");
            dhcpdConfigurationModel.ZonePrimaryKey = CaptureGroup(text, "zone[\\s]+[\\w.\\d]+[\\s]+{[\\s]+primary[\\s]+[.\\d]+;[\\s]+key[\\s]+([\\w\\d]+);");
            dhcpdConfigurationModel.DdnsUpdateStyle = CaptureGroup(text, "ddns-update-style[\\s]+([\\w]+);");
            dhcpdConfigurationModel.DdnsUpdates = CaptureGroup(text, "ddns-updates[\\s]+([\\w]+);");
            dhcpdConfigurationModel.DdnsDomainName = CaptureGroup(text, "ddns-domainname[\\s]+\"([\\w\\d.]+)\";");
            dhcpdConfigurationModel.DdnsRevDomainName = CaptureGroup(text, "ddns-rev-domainname[\\s]+\"([\\w\\d.\\-]+)\";");
            return dhcpdConfigurationModel;
        }

        //(key "[\w]+" {[\s\d\w\-;"=]+[\s]+};)
        public static DhcpdModel ParseKeySecret(DhcpdModel dhcpdConfigurationModel, string text) {
            var regex = new Regex("(key \"[\\w]+\" {[\\s\\d\\w\\-;\"=]+[\\s]+};)");
            var matchedGroups = regex.Match(text).Groups;
            var capturedText = matchedGroups[1].Value;
            dhcpdConfigurationModel.KeyName = CaptureGroup(capturedText, "key \"([\\w]+)\"");
            dhcpdConfigurationModel.KeySecret = CaptureGroup(capturedText, "secret \"([\\s\\S]*)\";");
            return dhcpdConfigurationModel;
        }

        //([^#]host[\s]+[\w\d]+ { hard[\w\d\s:;\-.]+[\s]*})
        public static List<DhcpdReservationModel> ParseReservation(string text) {
            var list = new List<DhcpdReservationModel>();
            var regex = new Regex("([^#]host[\\s]+[\\w\\d]+ { hard[\\w\\d\\s:;\\-.]+[\\s]*})");
            var matches = regex.Matches(text);
            foreach (var match in matches) {
                var hostName = CaptureGroup(match.ToString(), "host[\\s]+([\\w\\d]+) {");
                var macAddress = CaptureGroup(match.ToString(), "hardware ethernet[\\s]+([\\w\\d:]+);");
                var ipAddress = CaptureGroup(match.ToString(), "fixed-address[\\s]+([\\d.]+);");
                var reservation = new DhcpdReservationModel {
                    HostName = hostName,
                    MacAddress = macAddress,
                    IpAddress = ipAddress
                };
                list.Add(reservation);
            }
            return list;
        }

        //(class "[\w\d]+" {[\s\w\-\d(),":=]+;)
        public static List<DhcpdClassModel> ParseClass(string text) {
            var list = new List<DhcpdClassModel>();
            var regex = new Regex("(class \"[\\w\\d]+\" {[\\s\\w\\-\\d(),\":=]+;)");
            var matches = regex.Matches(text);
            foreach (var match in matches) {
                var name = CaptureGroup(match.ToString(), "class (\"[\\w\\d]+\") {");
                var vendorMacAddress = CaptureGroup(match.ToString(), "[\\s]*=[\\s]+\"([\\w\\d:]+)\";");
                var reservation = new DhcpdClassModel {
                    Name = name,
                    VendorMacAddress = vendorMacAddress
                };
                list.Add(reservation);
            }
            return list;
        }

        //(subnet[\s]+[\d.]+[\s]+netmask[\s]+[\d.]+[\s]+{[\s]+option[\s\w\d\-.;{}]+[\s]+pool\b[\s]+{[\s\w";\d.}{]+\-bootp[\s.\d]+;)
        public static List<DhcpdSubnetModel> ParseSubnet(string text) {
            var list = new List<DhcpdSubnetModel>();
            var regex = new Regex("(subnet[\\s]+[\\d.]+[\\s]+netmask[\\s]+[\\d.]+[\\s]+{[\\s]+option[\\s\\w\\d\\-.;{}]+[\\s]+pool\\b[\\s]+{[\\s\\w\";\\d.}{]+\\-bootp[\\s.\\d]+;)");
            var matches = regex.Matches(text);
            foreach (var match in matches) {
                var subnetIpFamily = CaptureGroup(match.ToString(), "subnet[\\s]+([\\d.]+)");
                var subnetIpMask = CaptureGroup(match.ToString(), "subnet[\\s]+[\\d.]+[\\s]+netmask[\\s]+[\\d.]+");
                var subnetOptionRouters = CaptureGroup(match.ToString(), "option routers[\\s]+([\\d-.]+);");
                var subnetNtpServers = CaptureGroup(match.ToString(), "option ntp-servers[\\s]+([\\d.]+);");
                var subnetTimeServers = CaptureGroup(match.ToString(), "option time-servers[\\s]+([\\d.]+);");
                var subnetDomainNameServers = CaptureGroup(match.ToString(), "option domain-name-servers[\\s]+([\\d.]+);");
                var subnetBroadcastAddress = CaptureGroup(match.ToString(), "broadcast-address[\\s]+([\\d.]+);");
                var subnetMask = CaptureGroup(match.ToString(), "option subnet-mask[\\s]+([\\d.]+);");
                var zoneName = CaptureGroup(match.ToString(), "zone[\\s]+([\\w.\\d]+)[\\s]+{");
                var zonePrimaryAddress = CaptureGroup(match.ToString(), "zone[\\s]+[\\w.\\d]+[\\s]+{[\\s]+primary[\\s]+([.\\d]+);");
                var zonePrimaryKey = CaptureGroup(match.ToString(), "zone[\\s]+[\\w.\\d]+[\\s]+{[\\s]+primary[\\s]+[.\\d]+;[\\s]+key[\\s]+([\\w\\d]+);");
                var poolDynamicRangeStart = CaptureGroup(match.ToString(), "range dynamic-bootp ([\\d.]+)");
                var poolDynamicRangeEnd = CaptureGroup(match.ToString(), "range dynamic-bootp [\\d.]+[\\s]+([\\d.]+);");
                var subnet = new DhcpdSubnetModel {
                    SubnetIpFamily = subnetIpFamily,
                    SubnetIpMask = subnetIpMask,
                    SubnetOptionRouters = subnetOptionRouters,
                    SubnetNtpServers = subnetNtpServers,
                    SubnetTimeServers = subnetTimeServers,
                    SubnetDomainNameServers = subnetDomainNameServers,
                    SubnetBroadcastAddress = subnetBroadcastAddress,
                    SubnetMask = subnetMask,
                    ZoneName = zoneName,
                    ZonePrimaryAddress = zonePrimaryAddress,
                    ZonePrimaryKey = zonePrimaryKey,
                    PoolDynamicRangeStart = poolDynamicRangeStart,
                    PoolDynamicRangeEnd = poolDynamicRangeEnd
                };
                list.Add(subnet);
            }
            return list;
        }

        //(pool[\s]+{[\s]+allow[\s\w"\d;.]+})
        public static List<DhcpdPoolModel> ParsePool(string text) {
            var list = new List<DhcpdPoolModel>();
            var regex = new Regex("(pool[\\s]+{[\\s]+allow[\\s\\w\"\\d;.]+})");
            var matches = regex.Matches(text);
            foreach (var match in matches) {
                var className = CaptureGroup(match.ToString(), "allow members of \"([\\d\\w]+)\";");
                var poolRangeStart = CaptureGroup(match.ToString(), "range ([\\d.]+)");
                var poolRangeEnd = CaptureGroup(match.ToString(), "range [\\d.]+[\\s]+([\\d.]+);");
                var reservation = new DhcpdPoolModel {
                    ClassName = className,
                    PoolRangeStart = poolRangeStart,
                    PoolRangeEnd = poolRangeEnd
                };
                list.Add(reservation);
            }
            return list;
        }
    }
}
