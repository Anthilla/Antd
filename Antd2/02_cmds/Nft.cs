using System.Collections.Generic;

namespace Antd2.cmds {

    /// <summary>
    /// https://manpages.debian.org/testing/nftables/nft.8.en.html
    /// https://www.mankier.com/8
    /// </summary>
    public class Nft {

        private const string nftCommand = "nft";
        private const string nftablesFile = "/etc/nftables.conf";

        public static IEnumerable<string> RulesetList(string family = "") {
            return Bash.Execute($"{nftCommand} -a list ruleset {family}");
        }
        public static IEnumerable<string> RulesetFlush(string family = "") {
            return Bash.Execute($"{nftCommand} flush ruleset {family}");
        }

        public static IEnumerable<string> TableList() {
            return Bash.Execute($"{nftCommand} -a list tables");
        }
        public static IEnumerable<string> TableList(string family, string table) {
            return Bash.Execute($"{nftCommand} -a list table {family} {table}");
        }
        public static IEnumerable<string> TableCreate(string family, string table) {
            return Bash.Execute($"{nftCommand} create table {family} {table}");
        }
        public static IEnumerable<string> TableFlush(string family, string table) {
            return Bash.Execute($"{nftCommand} flush table {family} {table}");
        }
        public static IEnumerable<string> TableDelete(string family, string table) {
            return Bash.Execute($"{nftCommand} delete table {family} {table}");
        }
        public static IEnumerable<string> TableDisable(string family, string table) {
            return Bash.Execute($"{nftCommand} add table {family} {table} {{ flags dormant; }}");
        }
        public static IEnumerable<string> TableEnable(string family, string table) {
            return Bash.Execute($"{nftCommand} add table {family} {table}");
        }

        public static IEnumerable<string> ChainList() {
            return Bash.Execute($"{nftCommand} -a list chains");
        }
        public static IEnumerable<string> ChainList(string family, string table, string chain) {
            return Bash.Execute($"{nftCommand} -a list chain {family} {table} {chain}");
        }
        public static IEnumerable<string> ChainCreate(string family, string table, string chain) {
            return Bash.Execute($"{nftCommand} create chain {family} {table} {chain}");
        }
        //[{ type type hook hook [device device] priority priority ; [policy policy ;] }]
        public static IEnumerable<string> ChainCreate(string family, string table, string chain,
            string type, string hook, string device, string priority, string policy) {
            return Bash.Execute($"{nftCommand} create chain {family} {table} {chain} {{ type {type}; hook {hook}; device {device}; priority {priority}; policy {policy}; }}");
        }
        public static IEnumerable<string> ChainRename(string family, string table, string chain, string newName) {
            return Bash.Execute($"{nftCommand} rename  chain {family} {table} {chain} {newName}");
        }
        public static IEnumerable<string> ChainFlush(string family, string table, string chain) {
            return Bash.Execute($"{nftCommand} flush chain {family} {table} {chain}");
        }
        public static IEnumerable<string> ChainDelete(string family, string table, string chain) {
            return Bash.Execute($"{nftCommand} delete chain {family} {table} {chain}");
        }

        public static IEnumerable<string> RuleAdd(string family, string table, string chain, string statement) {
            return Bash.Execute($"{nftCommand} add rule {family} {table} {chain} {statement}");
        }
        public static IEnumerable<string> RuleDelete(string family, string table, string chain, string handle) {
            return Bash.Execute($"{nftCommand} delete rule {family} {table} {chain} handle {handle}");
        }

        public static void SaveConfiguration() {
            Bash.Do($"{nftCommand} -a list ruleset > {nftablesFile}");
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