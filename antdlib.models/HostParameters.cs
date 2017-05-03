using System.Collections.Generic;

namespace antdlib.models {

    public class HostParameters {

        #region [    modules    ]
        public List<string> Modprobes { get; set; } = new List<string> { "br_netfilter", "tun" };
        public List<string> Rmmod { get; set; } = new List<string> { "iptable_filter", "ip6table_filter", "ebtable_filter", "ebtables", "iptable_nat", "ip_tables", "iptable_mangle" };
        public List<string> ModulesBlacklist { get; set; } = new List<string> { "iptable_filter", "ip6table_filter" };
        #endregion

        #region [    services    ]
        public List<string> ServicesStart { get; set; } = new List<string>();
        public List<string> ServicesStop { get; set; } = new List<string>();
        #endregion

        #region [    parameters    ]
        public List<string> OsParameters { get; set; } = new List<string> {
            "/proc/sys/fs/file-max 1024000",
            "/proc/sys/net/bridge/bridge-nf-call-arptables 0",
            "/proc/sys/net/bridge/bridge-nf-call-ip6tables 0",
            "/proc/sys/net/bridge/bridge-nf-call-iptables 0",
            "/proc/sys/net/bridge/bridge-nf-filter-pppoe-tagged 0",
            "/proc/sys/net/bridge/bridge-nf-filter-vlan-tagged 0",
            "/proc/sys/net/core/netdev_max_backlog 300000",
            "/proc/sys/net/core/optmem_max 40960",
            "/proc/sys/net/core/rmem_max 268435456",
            "/proc/sys/net/core/somaxconn 65536",
            "/proc/sys/net/core/wmem_max 268435456",
            "/proc/sys/net/ipv4/conf/all/accept_local 1",
            "/proc/sys/net/ipv4/conf/all/accept_redirects 1",
            "/proc/sys/net/ipv4/conf/all/accept_source_route 1",
            "/proc/sys/net/ipv4/conf/all/rp_filter 0",
            "/proc/sys/net/ipv4/conf/all/forwarding 1",
            "/proc/sys/net/ipv4/conf/default/rp_filter 0",
            "/proc/sys/net/ipv4/ip_forward 1",
            "/proc/sys/net/ipv4/ip_local_port_range 1024 65000",
            "/proc/sys/net/ipv4/ip_no_pmtu_disc 1",
            "/proc/sys/net/ipv4/tcp_congestion_control htcp",
            "/proc/sys/net/ipv4/tcp_fin_timeout 40",
            "/proc/sys/net/ipv4/tcp_max_syn_backlog 3240000",
            "/proc/sys/net/ipv4/tcp_max_tw_buckets 1440000",
            "/proc/sys/net/ipv4/tcp_moderate_rcvbuf 1",
            "/proc/sys/net/ipv4/tcp_mtu_probing 1",
            "/proc/sys/net/ipv4/tcp_rmem 4096 87380 134217728",
            "/proc/sys/net/ipv4/tcp_slow_start_after_idle 1",
            "/proc/sys/net/ipv4/tcp_tw_recycle 0",
            "/proc/sys/net/ipv4/tcp_tw_reuse 1",
            "/proc/sys/net/ipv4/tcp_window_scaling 1",
            "/proc/sys/net/ipv4/tcp_wmem 4096 65536 134217728",
            "/proc/sys/net/ipv6/conf/br0/disable_ipv6 1",
            "/proc/sys/net/ipv6/conf/eth0/disable_ipv6 1",
            "/proc/sys/net/ipv6/conf/wlan0/disable_ipv6 1",
            "/proc/sys/vm/swappiness 0",
        };
        #endregion

        #region [    commands    ]
        public List<Control> StartCommands { get; set; } = new List<Control> {
            new Control { Index = 0, FirstCommand = "sysctl-p", ControlCommand = "", Check = "" },
            new Control { Index = 1, FirstCommand = "systemd-machine-id-setup", ControlCommand = "", Check = "" },
            new Control { Index = 2, FirstCommand = "dhclient-killall", ControlCommand = "", Check = "" },
            new Control { Index = 3, FirstCommand = "systemctl stop systemd-resolvd.service", ControlCommand = "", Check = "" },
            new Control { Index = 4, FirstCommand = "systemctl stop systemd-networkd.service", ControlCommand = "", Check = "" },
            new Control { Index = 5, FirstCommand = "systemctl stop systemd-networkd.socket", ControlCommand = "", Check = "" }
        };
        public List<Control> EndCommands { get; set; } = new List<Control> {
            new Control { Index = 0, FirstCommand = "rmdir /Data/*", ControlCommand = "", Check = "" },
            new Control { Index = 1, FirstCommand = "mkdir -p /Data/Data01", ControlCommand = "", Check = "" },
            new Control { Index = 2, FirstCommand = "mount LABEL=Data01 /Data/Data01", ControlCommand = "", Check = "" },
        };
        #endregion
    }
}
