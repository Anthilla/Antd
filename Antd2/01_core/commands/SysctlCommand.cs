using Antd2.cmds;
using anthilla.core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Antd {
    public class SysctlCommand {
        public static readonly Dictionary<string, Action<string[]>> Options =
            new Dictionary<string, Action<string[]>> {
                { "check", CheckFunc },
                { "set", SetFunc },
                { "write", WriteFunc },
            };

        private static (string Key, string Value)[] RequiredSysctl = new (string Key, string Value)[] {
            ("fs.file-max", "1024000"),
            ("net.bridge.bridge-nf-call-arptables", "0"),
            ("net.bridge.bridge-nf-call-ip6tables", "0"),
            ("net.bridge.bridge-nf-call-iptables", "0"),
            ("net.bridge.bridge-nf-filter-pppoe-tagged", "0"),
            ("net.bridge.bridge-nf-filter-vlan-tagged", "0"),
            ("net.core.netdev_max_backlog", "300000"),
            ("net.core.optmem_max", "40960"),
            ("net.core.rmem_max", "268435456"),
            ("net.core.somaxconn", "65536"),
            ("net.core.wmem_max", "268435456"),
            ("net.ipv4.conf.all.accept_local", "1"),
            ("net.ipv4.conf.all.accept_redirects", "1"),
            ("net.ipv4.conf.all.accept_source_route", "1"),
            ("net.ipv4.conf.all.rp_filter", "0"),
            ("net.ipv4.conf.all.forwarding", "1"),
            ("net.ipv4.conf.default.rp_filter", "0"),
            ("net.ipv4.ip_forward", "1"),
            ("net.ipv4.ip_local_port_range", "1024 65000"),
            ("net.ipv4.ip_no_pmtu_disc", "1"),
            ("net.ipv4.tcp_congestion_control", "htcp"),
            ("net.ipv4.tcp_fin_timeout", "40"),
            ("net.ipv4.tcp_max_syn_backlog", "3240000"),
            ("net.ipv4.tcp_max_tw_buckets", "1440000"),
            ("net.ipv4.tcp_moderate_rcvbuf", "1"),
            ("net.ipv4.tcp_mtu_probing", "1"),
            ("net.ipv4.tcp_rmem", "4096 87380 134217728"),
            ("net.ipv4.tcp_slow_start_after_idle", "1"),
            ("net.ipv4.tcp_tw_recycle", "0"),
            ("net.ipv4.tcp_tw_reuse", "1"),
            ("net.ipv4.tcp_window_scaling", "1"),
            ("net.ipv4.tcp_wmem", "4096 65536 134217728"),
            ("net.ipv6.conf.all.disable_ipv6", "1"),
            ("vm.swappiness", "0"),
        };

        public static void CheckFunc(string[] args) {
            var currentSysctl = Sysctl.Get();
            foreach (var sysctl in RequiredSysctl) {
                var current = currentSysctl.FirstOrDefault(_ => _.Key == sysctl.Key);
                var isConfigured = current.Value == sysctl.Value;
                if (isConfigured) {
                    CheckFunc_PrintInstalled(sysctl.Key);
                }
                else {
                    CheckFunc_PrintNotInstalled(sysctl.Key);
                }
            }
        }

        private static void CheckFunc_PrintInstalled(string param) {
            Console.Write($"  {param}: ");
            Console.ForegroundColor = ConsoleColor.Green;
            ConsoleLogger.Log("configured");
            Console.ForegroundColor = ConsoleColor.White;
        }

        private static void CheckFunc_PrintNotInstalled(string param) {
            Console.Write($"  {param}: ");
            Console.ForegroundColor = ConsoleColor.Red;
            ConsoleLogger.Log("not configured");
            Console.ForegroundColor = ConsoleColor.White;
        }

        public static void SetFunc(string[] args) {
            foreach (var param in RequiredSysctl) {
                ConsoleLogger.Log($"  {param.Key}={param.Value}");
                Sysctl.Set(param.Key, param.Value);
            }
        }

        public static void WriteFunc(string[] args) {
            ConsoleLogger.Log("  Write sysctl file");
            Sysctl.Write(RequiredSysctl);
            ConsoleLogger.Log("  Apply sysctl file");
            Sysctl.Apply();
        }
    }
}