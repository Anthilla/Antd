using System;
using antdlib.common;
using Newtonsoft.Json;

namespace antdlib.models {
    public class FirewallConfigurationModel {
        public bool IsActive { get; set; }
        public FirewallTable Ipv4FilterTable { get; set; } = new FirewallTable {
            Type = "ip",
            Name = "filter",
            Sets = new[] {
                new FirewallSet { Name="connstatefull", Type = "ct_state", Elements = new [] { "established","related", "new" } },
                new FirewallSet { Name="connstatenew", Type = "ct_state", Elements = new [] { "new" } },
                new FirewallSet { Name="intifs", Type = "iface_index", Elements = new [] { "br1", "lo", "br0" } },
                new FirewallSet { Name="extifs", Type = "iface_index", Elements = new [] { "eth4", "eth3" } },
                new FirewallSet { Name="allifs", Type = "iface_index", Elements = new [] { "br1", "lo", "br0", "eth4", "eth3" } },
                new FirewallSet { Name="iifaddr", Type = "ipv4_addr", Elements = new [] { "127.0.0.1", "10.1.19.1", "10.99.19.1" } },
                new FirewallSet { Name="eifaddr", Type = "ipv4_addr", Elements = new [] { "192.168.111.2", "192.168.222.2" } },
                new FirewallSet { Name="protoset", Type = "inet_proto", Elements = new [] { "esp", "udp", "icmp", "ah" } },
                new FirewallSet { Name="protoset6", Type = "icmpv6_type", Elements = new [] { "nd-neighbor-solicit", "echo-request" }, Flags = "constant"},
                new FirewallSet { Name="tcpportset", Type = "inet_service", Elements = new [] { "22", "25", "53", "80", "88", "137", "138", "139", "389", "636", "123", "443", "445", "465", "555", "587", "953", "993", "1193", "1194", "1195", "1514", "1723", "5222", "5223", "5900", "8000", "8081", "8084", "8085", "8443" } },
                new FirewallSet { Name="udpportset", Type = "inet_service", Elements = new [] { "53", "67", "68", "88", "123", "500", "514", "953", "1193", "1194", "1195", "1701", "4500", "5355" } },
                new FirewallSet { Name="pubsvcset", Type = "inet_service", Elements = new [] { "80", "443", "80852" } },
            },
            Chains = new[] {
                new FirewallChain { Name = "input", Rules = new [] {
                    "type filter hook input priority 0; policy drop;",
                    "ct state @connstatefull accept",
                    "ct state invalid log prefix \"a=I4AccTfilCinRctinvalid \" accept",
                    "ip protocol @protoset accept",
                    "icmp type echo-request accept",
                    "udp dport @udpportset accept",
                    "tcp dport ssh accept",
                    "tcp dport 722 iif @extifs ct state @connstatenew log prefix \"fp=SSH:0 a=I4AccTfilCinRsshnewwan \" accept",
                    "tcp dport @tcpportset accept",
                    "iif @intifs accept",
                    "iif @extifs log prefix \"a=I4AccTfilCinRwan \" accept",
                    "log prefix \"a=I4AccTfilCinRdefNC \" accept"
                }},
                new FirewallChain { Name = "output", Rules = new [] {
                    "type filter hook output priority 0; policy drop;",
                    "ct state @connstatefull accept",
                    "ct state invalid log prefix \"a=I4AccTfilCouRctinvalid \" accept",
                    "ip protocol @protoset accept",
                    "icmp type echo-request accept",
                    "udp dport @udpportset accept",
                    "iif @intifs accept",
                    "iif @intifs oif @allifs accept",
                    "oif @extifs log prefix \"a=I4AccTfilCouRwan \" accept",
                    "log prefix \"a=I4AccTfilCouRdefNC \" accept"
                }},
                new FirewallChain { Name = "forward", Rules = new [] {
                    "type filter hook forward priority 0; policy drop;",
                    "ct state @connstatefull accept",
                    "ct state invalid log prefix \"a=I4AccTfilCfwRctinvalid \" accept",
                    "ip protocol @protoset accept",
                    "icmp type echo-request accept",
                    "udp dport @udpportset accept",
                    "iif @intifs oif @allifs accept",
                    "iif @extifs log prefix \"a=I4AccTfilCfwRwan \" accept",
                    "log prefix \"a=I4AccTfilCfwRdefNC \" accept"
                }}
            }
        };
        public FirewallTable Ipv4NatTable { get; set; } = new FirewallTable {
            Type = "ip",
            Name = "nat",
            Sets = new[] {
                new FirewallSet { Name="intifs", Type = "iface_index", Elements = new [] { "br1", "lo", "br0" } },
                new FirewallSet { Name="extifs01", Type = "iface_index", Elements = new [] { "eth4" } },
                new FirewallSet { Name="extifs02", Type = "iface_index", Elements = new [] { "eth3" } },
                new FirewallSet { Name="extifs", Type = "iface_index", Elements = new [] { "eth4", "eth3" } },
            },
            Chains = new[] {
                new FirewallChain { Name = "prerouting", Rules = new [] {
                    "type nat hook prerouting priority 0; policy accept;",
                    "iif @extifs tcp dport 22 dnat 10.1.3.195:22",
                    "iif @extifs tcp dport 80 dnat 10.1.3.195:80",
                    "iif @extifs tcp dport 443 dnat 10.1.3.195:443",
                    "iif @extifs tcp dport 722 dnat 192.168.111.2:22"
                }},
                new FirewallChain { Name = "postrouting", Rules = new [] {
                    "type nat hook postrouting priority 0; policy accept;",
                    "iif @intifs oif @extif01 snat 192.168.111.2",
                    "iif @intifs oif @extif02 snat 192.168.222.2",
                    "oif @extifs masquerade"
                }}
            }
        };
        public FirewallTable Ipv6FilterTable { get; set; } = new FirewallTable {
            Type = "ip6",
            Name = "filter",
            Sets = new[] {
                new FirewallSet { Name="connstatefull", Type = "ct_state", Elements = new [] { "established","related", "new" } },
                new FirewallSet { Name="connstatenew", Type = "ct_state", Elements = new [] { "new" } },
                new FirewallSet { Name="intifs", Type = "iface_index", Elements = new [] { "br1", "lo", "br0" } },
                new FirewallSet { Name="extifs", Type = "iface_index", Elements = new [] { "eth4", "eth3" } },
                new FirewallSet { Name="allifs", Type = "iface_index", Elements = new [] { "br1", "lo", "br0", "eth4", "eth3" } },
                new FirewallSet { Name="iifaddr", Type = "ipv4_addr", Elements = new [] { "127.0.0.1", "10.1.19.1", "10.99.19.1" } },
                new FirewallSet { Name="eifaddr", Type = "ipv4_addr", Elements = new [] { "192.168.111.2", "192.168.222.2" } },
                new FirewallSet { Name="protoset", Type = "inet_proto", Elements = new [] { "esp", "udp", "icmp", "ah" } },
                new FirewallSet { Name="protoset6", Type = "icmpv6_type", Elements = new [] { "nd-neighbor-solicit", "echo-request" }, Flags = "constant"},
                new FirewallSet { Name="tcpportset", Type = "inet_service", Elements = new [] { "22", "25", "53", "80", "88", "137", "138", "139", "389", "636", "123", "443", "445", "465", "555", "587", "953", "993", "1193", "1194", "1195", "1514", "1723", "5222", "5223", "5900", "8000", "8081", "8084", "8085", "8443" } },
                new FirewallSet { Name="udpportset", Type = "inet_service", Elements = new [] { "53", "67", "68", "88", "123", "500", "514", "953", "1193", "1194", "1195", "1701", "4500", "5355" } },
                new FirewallSet { Name="pubsvcset", Type = "inet_service", Elements = new [] { "80", "443", "80852" } },
            },
            Chains = new[] {
                new FirewallChain { Name = "input", Rules = new [] {
                    "type filter hook input priority 0; policy drop;",
                    "ct state @connstatefull accept",
                    "ct state invalid log prefix \"a=I6AccTfilCinRctinvalid \" accept",
                    "ip6 nexthdr ipv6-icmp accept",
                    "icmpv6 type @protoset6 accept",
                    "icmpv6 type echo-request accept",
                    "udp dport @udpportset accept",
                    "tcp dport ssh accept",
                    "tcp dport 722 iif { eth4, eth3 } ct state @connstatenew log prefix \"fp=SSH:0 a=I6AccTfilCinRsshnewwan \" accept",
                    "tcp dport @tcpportset accept",
                    "iif @intifs accept",
                    "iif @extifs log prefix \"a=I4AccTfilCinRwan \" accept",
                    "log prefix \"a=I6AccTfilCinRdefNC \" accept"
                }},
                new FirewallChain { Name = "output", Rules = new [] {
                    "type filter hook output priority 0; policy drop;",
                    "ct state @connstatefull accept",
                    "ct state invalid log prefix \"a=I6DroTfilCouRctinvalid \" drop",
                    "icmpv6 type echo-request accept",
                    "udp dport @udpportset accept",
                    "iif @intifs accept",
                    "iif @intifs oif @allifs accept",
                    "oif @extifs log prefix \"a=I6AccTfilCouRwan \" accept",
                    "log prefix \"a=I6accTfilCouRdefNC \" accept",
                }},
                new FirewallChain { Name = "forward", Rules = new [] {
                    "type filter hook forward priority 0; policy drop;",
                    "ct state @connstatefull accept",
                    "ct state invalid log prefix \"a=I4AccTfilCfwRctinvalid \" accept",
                    "icmpv6 type echo-request accept",
                    "udp dport @udpportset accept",
                    "iif @intifs oif @allifs accept",
                    "iif @extifs log prefix \"a=I6AccTfilCfwRwan \" accept",
                    "log prefix \"a=I6AccTfilCfwRdefNC \" accept",
                }}
            }
        };
        public FirewallTable Ipv6NatTable { get; set; } = new FirewallTable {
            Type = "ip",
            Name = "filter",
            Sets = new[] {
                new FirewallSet { Name="extifs", Type = "iface_index", Elements = new [] { "eth4", "eth3" } }
            },
            Chains = new[] {
                new FirewallChain { Name = "prerouting", Rules = new [] {
                    "type nat hook prerouting priority 0; policy accept;"
                }},
                new FirewallChain { Name = "postrouting", Rules = new [] {
                    "type nat hook postrouting priority 0; policy accept;",
                    "oif @extifs masquerade"
                }}
            }
        };
    }

    public class FirewallTable {
        public string Type { get; set; }
        public string Name { get; set; }
        public FirewallSet[] Sets { get; set; }
        public FirewallChain[] Chains { get; set; }
    }

    public class FirewallSet {
        public string Name { get; set; }
        public string Type { get; set; }
        public string Flags { get; set; }
        public string[] Elements { get; set; }
        [JsonIgnore]
        public string ElementsString => Elements.JoinToString(", ");
    }

    public class FirewallChain {
        public string Name { get; set; }
        public string[] Rules { get; set; }
        [JsonIgnore]
        public string RulesString => Rules.JoinToString(Environment.NewLine);
    }
}