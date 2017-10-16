using System.Collections.Generic;
using anthilla.core;

namespace Antd.cmds {

    public class Firewall {

        private const string nftablesFile = "/cfg/antd/conf/nftables.conf";
        private const string nftFileLocation = "/sbin/nft";
        private const string nftArgs = "-f ";

        /// <summary>
        /// Importa la configurazione di /etc/nftables.conf
        /// </summary>
        /// <returns></returns>
        private static bool Parse() {
            return false;
        }

        /// <summary>
        /// Applica la configurazione current al file /etc/nftables.conf e riavvia il servizio
        /// </summary>
        public static void Apply() {
            var current = Application.CurrentConfiguration.Services.Firewall;
            if(current == null) {
                return;
            }
            if(current.Tables.Length < 1) {
                return;
            }

            #region [    nftables.conf generation    ]
            var lines = new List<string> {
                "flush ruleset;",
                "flush ruleset;",
                "flush ruleset;",
                "flush ruleset;",
                "flush ruleset;"
            };

            for(var t = 0; t < current.Tables.Length; t++) {
                var firewalTable = current.Tables[t];
                lines.Add(CommonString.Append("table ", firewalTable.Family, " ", firewalTable.Name, " {"));
                for(var s = 0; s < firewalTable.Sets.Length; s++) {
                    var set = firewalTable.Sets[s];
                    lines.Add(CommonString.Append("    set ", set.Name, " {"));
                    lines.Add(CommonString.Append("        type ", set.Type));
                    lines.Add(CommonString.Append("        elements = { ", CommonString.Build(set.Elements, ", "), " }"));
                    lines.Add("    }");
                }
                lines.Add("");
                for(var c = 0; c < firewalTable.Chains.Length; c++) {
                    var chain = firewalTable.Chains[c];
                    lines.Add(CommonString.Append("    chain ", chain.Hook, " {"));
                    lines.Add(CommonString.Append("        type ", chain.Type, " hook ", chain.Hook, " priority 0; policy drop;"));

                    for(var r = 0; r < chain.Rules.Length; r++) {
                        var rule = chain.Rules[r];
                        lines.Add(CommonString.Append("        ", rule.Match, " ", rule.MatchArgument, " ", rule.Object, " ", rule.Jump).Replace("  ", " "));
                    }
                    var logPrefix = CommonString.Append("\"a=T", firewalTable.Name, "C", chain.Hook, "\"");
                    lines.Add(CommonString.Append("        log prefix ", logPrefix, " accept"));
                    lines.Add("    }");
                }
                lines.Add("}");
            }

            FileWithAcl.WriteAllLines(nftablesFile, lines, "644", "root", "wheel");
            #endregion
            Start();
        }

        public static void Stop() {
            ConsoleLogger.Log("[firewall] stop");
        }

        public static void Start() {
            var args = CommonString.Append(nftArgs, nftablesFile);
            CommonProcess.Do(nftFileLocation, args);
            ConsoleLogger.Log("[firewall] start");
        }

        public class Verbs {

            public static string[] TableFamily = new string[] {
                "ip",
                "ip6",
                "inet",
                "arp",
                "bridge"
            };

            public static string[] SetType = new string[] {
                "ct_state",
                "iface_index",
                "ipv4_addr",
                "ipv6_addr",
                "icmpv6_type",
                "ether_addr",
                "inet_proto",
                "inet_service",
                "mark"
            };

            public static string[] SetFlag = new string[] {
                "constant",
                "interval",
                "timeout"
            };

            public static string[] ChainType = new string[] {
                "filter",
                "route",
                "nat"
            };

            public static string[] ChainHook = new string[] {
                "prerouting",
                "input",
                "forward",
                "output",
                "postrouting",
                "ingress"
            };

            public static string[] RuleMatch = new string[] {
                "iif",
                "oif",
                "meta",
                "icmp",
                "icmpv6",
                "ip",
                "ip6",
                "tcp",
                "udp",
                "sctp",
                "ct state"
            };

            public static string[] RuleMatchMetaArg = new string[] {
                "oif",
                "iif",
                "oifname",
                "iifname"
            };

            public static string[] RuleMatchIcmpArg = new string[] {
                "type"
            };

            public static string[] RuleMatchIcmpV6Arg = new string[] {
                "type"
            };

            public static string[] RuleMatchIpArg = new string[] {
                "protocol",
                "daddr",
                "saddr"
            };

            public static string[] RuleMatchIpV6Arg = new string[] {
                "daddr",
                "saddr"
            };

            public static string[] RuleMatchTcpArg = new string[] {
                "dport",
                "sport"
            };

            public static string[] RuleMatchUdpArg = new string[] {
                "dport",
                "sport"
            };

            public static string[] RuleMatchSctpArg = new string[] {
                "dport",
                "sport"
            };

            public static string[] RuleMatchCtArg = new string[] {
                "new",
                "established",
                "related",
                "invalid"
            };

            public static string[] RuleJump = new string[] {
                "accept",
                "reject",
                "drop",
                "snat",
                "dnat",
                "log",
                "counter",
                "return",
                "jump",
                "goto"
            };
        }
    }
}