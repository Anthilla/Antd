//-------------------------------------------------------------------------------------
//     Copyright (c) 2014, Anthilla S.r.l. (http://www.anthilla.com)
//     All rights reserved.
//
//     Redistribution and use in source and binary forms, with or without
//     modification, are permitted provided that the following conditions are met:
//         * Redistributions of source code must retain the above copyright
//           notice, this list of conditions and the following disclaimer.
//         * Redistributions in binary form must reproduce the above copyright
//           notice, this list of conditions and the following disclaimer in the
//           documentation and/or other materials provided with the distribution.
//         * Neither the name of the Anthilla S.r.l. nor the
//           names of its contributors may be used to endorse or promote products
//           derived from this software without specific prior written permission.
//
//     THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
//     ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
//     WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
//     DISCLAIMED. IN NO EVENT SHALL ANTHILLA S.R.L. BE LIABLE FOR ANY
//     DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
//     (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
//     LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
//     ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
//     (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
//     SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
//
//     20141110
//-------------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Text.RegularExpressions;
using antdlib.models;

namespace antdlib.config.Parsers {
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
            foreach(var match in matches) {
                var file = CaptureGroup(match.ToString(), "allow[\\s]+([\\w\\-]+);");
                list.Add(file);
            }
            return list;
        }

        public static DhcpdConfigurationModel ParseParameters(DhcpdConfigurationModel dhcpdConfigurationModel, string text) {
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
        public static DhcpdConfigurationModel ParseKeySecret(DhcpdConfigurationModel dhcpdConfigurationModel, string text) {
            var regex = new Regex("(key \"[\\w]+\" {[\\s\\d\\w\\-;\"=]+[\\s]+};)");
            var matchedGroups = regex.Match(text).Groups;
            var capturedText = matchedGroups[1].Value;
            dhcpdConfigurationModel.KeyName = CaptureGroup(capturedText, "key \"([\\w]+)\"");
            dhcpdConfigurationModel.KeySecret = CaptureGroup(capturedText, "secret \"([\\s\\S]*)\";");
            return dhcpdConfigurationModel;
        }

        //([^#]host[\s]+[\w\d]+ { hard[\w\d\s:;\-.]+[\s]*})
        public static List<DhcpdReservation> ParseReservation(string text) {
            var list = new List<DhcpdReservation>();
            var regex = new Regex("([^#]host[\\s]+[\\w\\d]+ { hard[\\w\\d\\s:;\\-.]+[\\s]*})");
            var matches = regex.Matches(text);
            foreach(var match in matches) {
                var hostName = CaptureGroup(match.ToString(), "host[\\s]+([\\w\\d]+) {");
                var macAddress = CaptureGroup(match.ToString(), "hardware ethernet[\\s]+([\\w\\d:]+);");
                var ipAddress = CaptureGroup(match.ToString(), "fixed-address[\\s]+([\\d.]+);");
                var reservation = new DhcpdReservation {
                    HostName = hostName,
                    MacAddress = macAddress,
                    IpAddress = ipAddress
                };
                list.Add(reservation);
            }
            return list;
        }

        //(class "[\w\d]+" {[\s\w\-\d(),":=]+;)
        public static List<DhcpdClass> ParseClass(string text) {
            var list = new List<DhcpdClass>();
            var regex = new Regex("(class \"[\\w\\d]+\" {[\\s\\w\\-\\d(),\":=]+;)");
            var matches = regex.Matches(text);
            foreach(var match in matches) {
                var name = CaptureGroup(match.ToString(), "class (\"[\\w\\d]+\") {");
                var vendorMacAddress = CaptureGroup(match.ToString(), "[\\s]*=[\\s]+\"([\\w\\d:]+)\";");
                var reservation = new DhcpdClass {
                    Name = name,
                    VendorMacAddress = vendorMacAddress
                };
                list.Add(reservation);
            }
            return list;
        }

        //(subnet[\s]+[\d.]+[\s]+netmask[\s]+[\d.]+[\s]+{[\s]+option[\s\w\d\-.;{}]+[\s]+pool\b[\s]+{[\s\w";\d.}{]+\-bootp[\s.\d]+;)
        public static List<DhcpdSubnet> ParseSubnet(string text) {
            var list = new List<DhcpdSubnet>();
            var regex = new Regex("(subnet[\\s]+[\\d.]+[\\s]+netmask[\\s]+[\\d.]+[\\s]+{[\\s]+option[\\s\\w\\d\\-.;{}]+[\\s]+pool\\b[\\s]+{[\\s\\w\";\\d.}{]+\\-bootp[\\s.\\d]+;)");
            var matches = regex.Matches(text);
            foreach(var match in matches) {
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
                var subnet = new DhcpdSubnet {
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
        public static List<DhcpdPool> ParsePool(string text) {
            var list = new List<DhcpdPool>();
            var regex = new Regex("(pool[\\s]+{[\\s]+allow[\\s\\w\"\\d;.]+})");
            var matches = regex.Matches(text);
            foreach(var match in matches) {
                var className = CaptureGroup(match.ToString(), "allow members of \"([\\d\\w]+)\";");
                var poolRangeStart = CaptureGroup(match.ToString(), "range ([\\d.]+)");
                var poolRangeEnd = CaptureGroup(match.ToString(), "range [\\d.]+[\\s]+([\\d.]+);");
                var reservation = new DhcpdPool {
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
