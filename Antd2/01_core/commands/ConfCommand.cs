using Antd2.Configuration;
using System;
using System.Collections.Generic;

namespace Antd2 {
    public class ConfCommand {
        public static readonly Dictionary<string, Action<string[]>> Options =
            new Dictionary<string, Action<string[]>> {
                { "test", TestFunc },
                { "write", WriteFunc },
                { "read", ReadFunc },
            };

        private const string ConfFile = "/cfg/antd/antd-test.toml";

        private static void TestFunc(string[] args) {
            WriteFunc(args);
            ReadFunc(args);
        }

        private static void TestFunc_PrintOk(string msg) {
            Console.Write(msg);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("  ok");
            Console.ForegroundColor = ConsoleColor.White;
        }

        private static void TestFunc_PrintKo(string msg, string exception) {
            Console.Write(msg);
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("  ko");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(exception);
        }

        private static void WriteFunc(string[] args) {
            try {
                var conf = new MachineConfiguration();
                conf.Boot.ActiveModules = new string[] {
                    "tun",
                    "br_netfilter",
                };
                conf.Boot.InactiveModules = new string[] {
                    "ip_tables",
                };
                conf.Boot.Sysctl = new string[] {
                    "fs.file-max=1024000",
                    "net.bridge.bridge-nf-call-arptables=0",
                    "net.bridge.bridge-nf-call-ip6tables=0",
                    "net.bridge.bridge-nf-call-iptables=0",
                    "net.bridge.bridge-nf-filter-pppoe-tagged=0",
                    "net.bridge.bridge-nf-filter-vlan-tagged=0",
                    "net.core.netdev_max_backlog=300000",
                    "net.core.optmem_max=40960",
                    "net.core.rmem_max=268435456",
                    "net.core.somaxconn=65536",
                    "net.core.wmem_max=268435456",
                    "net.ipv4.conf.all.accept_local=1",
                    "net.ipv4.conf.all.accept_redirects=1",
                    "net.ipv4.conf.all.accept_source_route=1",
                    "net.ipv4.conf.all.rp_filter=0",
                    "net.ipv4.conf.all.forwarding=1",
                    "net.ipv4.conf.default.rp_filter=0",
                    "net.ipv4.ip_forward=1",
                    "net.ipv4.ip_local_port_range=1024 65000",
                    "net.ipv4.ip_no_pmtu_disc=1",
                    "net.ipv4.tcp_congestion_control=htcp",
                    "net.ipv4.tcp_fin_timeout=40",
                    "net.ipv4.tcp_max_syn_backlog=3240000",
                    "net.ipv4.tcp_max_tw_buckets=1440000",
                    "net.ipv4.tcp_moderate_rcvbuf=1",
                    "net.ipv4.tcp_mtu_probing=1",
                    "net.ipv4.tcp_rmem=4096 87380 134217728",
                    "net.ipv4.tcp_slow_start_after_idle=1",
                    "net.ipv4.tcp_tw_recycle=0",
                    "net.ipv4.tcp_tw_reuse=1",
                    "net.ipv4.tcp_window_scaling=1",
                    "net.ipv4.tcp_wmem=4096 65536 134217728",
                    "net.ipv6.conf.all.disable_ipv6=1",
                    "vm.swappiness=0"
                };
                conf.Boot.ActiveServices = new[] {
                    "systemd-journald.service"
                };
                conf.Boot.InactiveServices = new[] {
                    "systemd-networkd.service"
                };

                conf.Time.Timezone = "Europe/Rome";
                conf.Time.EnableNtpSync = true;
                conf.Time.NtpServer = new[] { "ntp1.ien.it" };

                conf.Host.Name = "box01";
                conf.Host.Chassis = "server";
                conf.Host.Deployment = "developement";
                conf.Host.Location = "onEarth";

                conf.Network.Dns.Domain = "domain";
                conf.Network.Dns.Search = "search";
                conf.Network.Dns.Nameserver = new[] {
                    "8.8.8.8",
                    "8.8.4.4",
                };

                conf.Network.Interfaces = new Antd2.Configuration.NetInterface[] {
                    new Antd2.Configuration.NetInterface() { Iface = "eth1", Address ="123.456.78.9/24"}
                };
                conf.Network.Routing = new Antd2.Configuration.NetRoute[] {
                    new Antd2.Configuration.NetRoute() { Gateway = "123.456.78.99", Destination = "default", Device = "eth1" }
                };

                Nett.Toml.WriteFile<MachineConfiguration>(conf, ConfFile);
                TestFunc_PrintOk("  write: ");
            }
            catch (Exception ex) {
                TestFunc_PrintKo("  write: ", ex.ToString());
            }
        }

        private static void ReadFunc(string[] args) {
            try {
                var conf = Nett.Toml.ReadFile<MachineConfiguration>(ConfFile);
                TestFunc_PrintOk("  read");
            }
            catch (Exception ex) {
                TestFunc_PrintKo("  read", ex.ToString());
            }
        }
    }
}