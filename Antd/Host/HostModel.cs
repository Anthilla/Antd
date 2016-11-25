using System;
using System.Collections.Generic;
using System.Linq;
using antd.commands;
using Newtonsoft.Json;

namespace Antd.Host {
    /// <summary>
    /// Object that indexes the parameters that are needed to the machine configuration
    /// This model will be Json-serialized and stored in a .conf file on the machine
    /// </summary>
    public class HostModel {

        [JsonIgnore]
        public DateTime DateTime => DateTime.Now;

        [JsonIgnore]
        public string Path => $"{antdlib.common.Parameter.AntdCfg}/host.conf";

        /// <summary>
        /// First configuration steps
        /// For each configurable aspect add one HostParameter to Preparation[]
        /// </summary>
        public HostParameter[] Preparation { get; set; } = {
            new HostParameter { SetCmd = "sysctl-p" },
            new HostParameter { SetCmd = "systemd-machine-id-setup" },
            new HostParameter { SetCmd = "dhclient-killall" },
            new HostParameter { SetCmd = "systemctl-stop", StoredValues = new Dictionary<string, string> { { "$service", "systemd-resolvd.service" } } },
            new HostParameter { SetCmd = "systemctl-stop", StoredValues = new Dictionary<string, string> { { "$service", "systemd-networkd.service" } } },
            new HostParameter { SetCmd = "systemctl-stop", StoredValues = new Dictionary<string, string> { { "$service", "systemd-networkd.socket" } } },
        };

        public HostParameter HostName { get; set; } = new HostParameter {
            SetCmd = "set-hostname",
            GetCmd = "hostnamectl-get-hostname",
            StoredValues = new Dictionary<string, string> {
                { "$host_name", "aosVM01" }
            }
        };

        public HostParameter HostChassis { get; set; } = new HostParameter {
            SetCmd = "set-chassis",
            GetCmd = "hostnamectl-get-chassis",
            StoredValues = new Dictionary<string, string> {
                { "$host_chassis", "vm" }
            }
        };

        public HostParameter HostDeployment { get; set; } = new HostParameter {
            SetCmd = "set-deployment",
            GetCmd = "hostnamectl-get-deployment",
            StoredValues = new Dictionary<string, string> {
                { "$host_deployment", "Development" }
            }
        };

        public HostParameter HostLocation { get; set; } = new HostParameter {
            SetCmd = "set-location",
            GetCmd = "hostnamectl-get-location",
            StoredValues = new Dictionary<string, string> {
                { "$host_location", "My Location" }
            }
        };

        public HostParameter Timezone { get; set; } = new HostParameter {
            SetCmd = "set-timezone",
            GetCmd = "timedatectl-get-timezone",
            StoredValues = new Dictionary<string, string> {
                { "$host_timezone", "Europe/Rome" }
            }
        };

        public HostParameter[] TimeConfiguration { get; set; } = {
            new HostParameter { SetCmd = "ntpdate", StoredValues = new Dictionary<string, string> { { "$server", "ntp1.ien.it" } } },
            new HostParameter { SetCmd = "set-ntpdate" },
            new HostParameter { SetCmd = "sync-clock" },
        };

        public HostParameter[] DnsResolv { get; set; } = {
            new HostParameter {
                SetCmd = "echo-write",
                GetCmd = "cat-etc-resolv",
                StoredValues = new Dictionary<string, string> {
                    { "$value", "nameserver 8.8.8.8" },
                    { "$file", "/etc/resolv.conf" }
                }
            },
            new HostParameter {
                SetCmd = "echo-append",
                GetCmd = "cat-etc-resolv",
                StoredValues = new Dictionary<string, string> {
                    { "$value", "search mylan.local" },
                    { "$file", "/etc/resolv.conf" }
                }
            },
            new HostParameter {
                SetCmd = "echo-append",
                GetCmd = "cat-etc-resolv",
                StoredValues = new Dictionary<string, string> {
                    { "$value", "domain mylan.local" },
                    { "$file", "/etc/resolv.conf" }
                }
            },
        };

        /// <summary>
        /// Each object in Modprobes triggers the "modprobe" command
        /// </summary>
        public HostParameter[] Modprobes { get; set; } = {
            new HostParameter { SetCmd = "modprobe", StoredValues = new Dictionary<string, string> { { "$package", "br_netfilter" } } },
            new HostParameter { SetCmd = "modprobe", StoredValues = new Dictionary<string, string> { { "$package", "tun" } } }
        };

        /// <summary>
        /// RemoveModules.StoredValues[$modules] is a spaced list of modules
        /// </summary>
        public HostParameter RemoveModules { get; set; } = new HostParameter {
            SetCmd = "rmmod",
            StoredValues = new Dictionary<string, string> {
                { "$modules", "iptable_filter ip6table_filter ebtable_filter ebtables iptable_nat ip_tables iptable_mangle" }
            }
        };

        /// <summary>
        /// Each object in Services triggers the "systemctl-restart" command
        /// At Antd boot these services will be restarted
        /// </summary>
        public HostParameter[] Services { get; set; } = {
            new HostParameter { SetCmd = "systemctl-restart", StoredValues = new Dictionary<string, string> { { "$service", "" } } },
        };

        /// <summary>
        /// Write os parameters
        /// Save a $value in a $file
        /// </summary>
        public HostParameter[] OsParameters { get; set; } = {
            new HostParameter { SetCmd = "echo-write", StoredValues = new Dictionary<string, string> { { "$file", "/proc/sys/fs/file-max" }, { "$value", "1024000" } } },
            new HostParameter { SetCmd = "echo-write", StoredValues = new Dictionary<string, string> { { "$file", "/proc/sys/net/bridge/bridge-nf-call-arptables" }, { "$value", "0" } } },
            new HostParameter { SetCmd = "echo-write", StoredValues = new Dictionary<string, string> { { "$file", "/proc/sys/net/bridge/bridge-nf-call-ip6tables" }, { "$value", "0" } } },
            new HostParameter { SetCmd = "echo-write", StoredValues = new Dictionary<string, string> { { "$file", "/proc/sys/net/bridge/bridge-nf-call-iptables" }, { "$value", "0" } } },
            new HostParameter { SetCmd = "echo-write", StoredValues = new Dictionary<string, string> { { "$file", "/proc/sys/net/bridge/bridge-nf-filter-pppoe-tagged" }, { "$value", "0" } } },
            new HostParameter { SetCmd = "echo-write", StoredValues = new Dictionary<string, string> { { "$file", "/proc/sys/net/bridge/bridge-nf-filter-vlan-tagged" }, { "$value", "0" } } },
            new HostParameter { SetCmd = "echo-write", StoredValues = new Dictionary<string, string> { { "$file", "/proc/sys/net/core/netdev_max_backlog" }, { "$value", "300000" } } },
            new HostParameter { SetCmd = "echo-write", StoredValues = new Dictionary<string, string> { { "$file", "/proc/sys/net/core/optmem_max" }, { "$value", "40960" } } },
            new HostParameter { SetCmd = "echo-write", StoredValues = new Dictionary<string, string> { { "$file", "/proc/sys/net/core/rmem_max" }, { "$value", "268435456" } } },
            new HostParameter { SetCmd = "echo-write", StoredValues = new Dictionary<string, string> { { "$file", "/proc/sys/net/core/somaxconn" }, { "$value", "65536" } } },
            new HostParameter { SetCmd = "echo-write", StoredValues = new Dictionary<string, string> { { "$file", "/proc/sys/net/core/wmem_max" }, { "$value", "268435456" } } },
            new HostParameter { SetCmd = "echo-write", StoredValues = new Dictionary<string, string> { { "$file", "/proc/sys/net/ipv4/conf/all/accept_local" }, { "$value", "1" } } },
            new HostParameter { SetCmd = "echo-write", StoredValues = new Dictionary<string, string> { { "$file", "/proc/sys/net/ipv4/conf/all/accept_redirects" }, { "$value", "1" } } },
            new HostParameter { SetCmd = "echo-write", StoredValues = new Dictionary<string, string> { { "$file", "/proc/sys/net/ipv4/conf/all/accept_source_route" }, { "$value", "1" } } },
            new HostParameter { SetCmd = "echo-write", StoredValues = new Dictionary<string, string> { { "$file", "/proc/sys/net/ipv4/conf/all/rp_filter" }, { "$value", "0" } } },
            new HostParameter { SetCmd = "echo-write", StoredValues = new Dictionary<string, string> { { "$file", "/proc/sys/net/ipv4/conf/default/rp_filter" }, { "$value", "0" } } },
            new HostParameter { SetCmd = "echo-write", StoredValues = new Dictionary<string, string> { { "$file", "/proc/sys/net/ipv4/ip_forward" }, { "$value", "1" } } },
            new HostParameter { SetCmd = "echo-write", StoredValues = new Dictionary<string, string> { { "$file", "/proc/sys/net/ipv4/ip_local_port_range" }, { "$value", "1024 65000" } } },
            new HostParameter { SetCmd = "echo-write", StoredValues = new Dictionary<string, string> { { "$file", "/proc/sys/net/ipv4/ip_local_port_range" }, { "$value", "1024 65000" } } },
            new HostParameter { SetCmd = "echo-write", StoredValues = new Dictionary<string, string> { { "$file", "/proc/sys/net/ipv4/ip_no_pmtu_disc" }, { "$value", "1" } } },
            new HostParameter { SetCmd = "echo-write", StoredValues = new Dictionary<string, string> { { "$file", "/proc/sys/net/ipv4/tcp_congestion_control" }, { "$value", "htcp" } } },
            new HostParameter { SetCmd = "echo-write", StoredValues = new Dictionary<string, string> { { "$file", "/proc/sys/net/ipv4/tcp_fin_timeout" }, { "$value", "40" } } },
            new HostParameter { SetCmd = "echo-write", StoredValues = new Dictionary<string, string> { { "$file", "/proc/sys/net/ipv4/tcp_max_syn_backlog" }, { "$value", "3240000" } } },
            new HostParameter { SetCmd = "echo-write", StoredValues = new Dictionary<string, string> { { "$file", "/proc/sys/net/ipv4/tcp_max_tw_buckets" }, { "$value", "1440000" } } },
            new HostParameter { SetCmd = "echo-write", StoredValues = new Dictionary<string, string> { { "$file", "/proc/sys/net/ipv4/tcp_moderate_rcvbuf" }, { "$value", "1" } } },
            new HostParameter { SetCmd = "echo-write", StoredValues = new Dictionary<string, string> { { "$file", "/proc/sys/net/ipv4/tcp_mtu_probing" }, { "$value", "1" } } },
            new HostParameter { SetCmd = "echo-write", StoredValues = new Dictionary<string, string> { { "$file", "/proc/sys/net/ipv4/tcp_rmem" }, { "$value", "4096 87380 134217728" } } },
            new HostParameter { SetCmd = "echo-write", StoredValues = new Dictionary<string, string> { { "$file", "/proc/sys/net/ipv4/tcp_slow_start_after_idle" }, { "$value", "1" } } },
            new HostParameter { SetCmd = "echo-write", StoredValues = new Dictionary<string, string> { { "$file", "/proc/sys/net/ipv4/tcp_slow_start_after_idle" }, { "$value", "1" } } },
            new HostParameter { SetCmd = "echo-write", StoredValues = new Dictionary<string, string> { { "$file", "/proc/sys/net/ipv4/tcp_tw_recycle" }, { "$value", "0" } } },
            new HostParameter { SetCmd = "echo-write", StoredValues = new Dictionary<string, string> { { "$file", "/proc/sys/net/ipv4/tcp_tw_reuse" }, { "$value", "1" } } },
            new HostParameter { SetCmd = "echo-write", StoredValues = new Dictionary<string, string> { { "$file", "/proc/sys/net/ipv4/tcp_window_scaling" }, { "$value", "1" } } },
            new HostParameter { SetCmd = "echo-write", StoredValues = new Dictionary<string, string> { { "$file", "/proc/sys/net/ipv4/tcp_wmem" }, { "$value", "4096 65536 134217728" } } },
            new HostParameter { SetCmd = "echo-write", StoredValues = new Dictionary<string, string> { { "$file", "/proc/sys/net/ipv6/conf/br0/disable_ipv6" }, { "$value", "1" } } },
            new HostParameter { SetCmd = "echo-write", StoredValues = new Dictionary<string, string> { { "$file", "/proc/sys/net/ipv6/conf/eth0/disable_ipv6" }, { "$value", "1" } } },
            new HostParameter { SetCmd = "echo-write", StoredValues = new Dictionary<string, string> { { "$file", "/proc/sys/net/ipv6/conf/wlan0/disable_ipv6" }, { "$value", "1" } } },
            new HostParameter { SetCmd = "echo-write", StoredValues = new Dictionary<string, string> { { "$file", "/proc/sys/vm/swappiness" }, { "$value", "0" } } },
        };

        /// <summary>
        /// These objects will write /etc/networks file
        /// </summary>
        public HostParameter[] EtcNetworks { get; set; } = {
            new HostParameter { SetCmd = "echo-write", StoredValues = new Dictionary<string, string> { { "$file", "/etc/networks" }, { "$value", "# /etc/networks" } } },
            new HostParameter { SetCmd = "echo-append", StoredValues = new Dictionary<string, string> { { "$file", "/etc/networks" }, { "$value", "" } } },
            new HostParameter { SetCmd = "echo-append-rm", StoredValues = new Dictionary<string, string> { { "$file", "/proc/sys/vm/swappiness" }, { "$value", "loopback 127.0.0.0" } } },
            new HostParameter { SetCmd = "echo-append-rm", StoredValues = new Dictionary<string, string> { { "$file", "/proc/sys/vm/swappiness" }, { "$value", "link-local 169.254.0.0" } } }
        };

        /// <summary>
        /// Network and network interfaces configuration
        /// MachineNamedParameter[] Network is an array of each interface
        /// Each interface has a HostParameter[] Configuration that gathers all commands to configure THAT interface
        /// todo pass Network.Name to its Configuration.HostParameter
        /// </summary>
        public HostNamedParameter[] Network { get; set; } = {
            new HostNamedParameter {
                Name = "eth0",
                StoredValues = new Dictionary<string, string> { { "$net_if", "eth0" } },
                Configuration = new [] {
                    new HostParameter(new Dictionary<string, string> { { "$net_if", "eth0" } }) { SetCmd = "anthilla" },
                }
            }
        };

        /// <summary>
        /// Final configuration steps
        /// For each configurable aspect add one HostParameter to Adjustments[]
        /// </summary>
        public HostParameter[] Adjustments { get; set; } = {
            new HostParameter { SetCmd = "anthilla", StoredValues = new Dictionary<string, string> { { "$custom", "rmdir /Data/*" } } },
            new HostParameter { SetCmd = "anthilla", StoredValues = new Dictionary<string, string> { { "$custom", "mkdir -p /Data/Data01" } } },
            new HostParameter { SetCmd = "anthilla", StoredValues = new Dictionary<string, string> { { "$custom", "mount LABEL=Data01 /Data/Data01" } } },
        };
    }

    /// <summary>
    /// This object kinda wraps the HostParameter class, having a "common" Name and StoredValues both passed to the HostParameter[] Configuration
    /// </summary>
    public class HostNamedParameter {
        public string Name { get; set; } = string.Empty;

        public IDictionary<string, string> StoredValues { get; set; } = new Dictionary<string, string>();

        public HostParameter[] Configuration { get; set; } = { };
    }

    /// <summary>
    /// Object that both stores a single element of machine configuration and applies it
    /// </summary>
    public class HostParameter {

        private readonly CommandLauncher _commandLauncher = new CommandLauncher();

        public HostParameter() {
        }

        /// <summary>
        /// Initialize the class HostParameter with known StoredValues
        /// </summary>
        /// <param name="storedValues"></param>
        public HostParameter(Dictionary<string, string> storedValues) {
            StoredValues = storedValues;
        }

        /// <summary>
        /// StoredValues that contain both the label, found in the stored command, and its alternate value.
        /// Where Value.Key == parameter name, ex "$host_name"
        /// Where Value.Value == parameter value, ex "myhost01"
        /// </summary>
        public Dictionary<string, string> StoredValues { get; set; } = new Dictionary<string, string>();

        /// <summary>
        /// This SetCmd belongs to antd.commands.Commands.List keys
        /// ex "set-hostname"
        /// </summary>
        public string SetCmd { get; set; } = string.Empty;

        /// <summary>
        /// This GetCmd belongs to antd.commands.Commands.List keys
        /// ex "hostnamectl-get-hostname"
        /// </summary>
        public string GetCmd { get; set; } = string.Empty;

        /// <summary>
        /// Apply the stored value
        /// </summary>
        public void Apply() {
            if(IsApplied) {
                return;
            }
            _commandLauncher.Launch(SetCmd, StoredValues);
        }

        /// <summary>
        /// Apply and store a new set of values
        /// </summary>
        /// <param name="values">
        ///     Where values.Key == parameter name, ex "$host_name"
        ///     Where values.Value == parameter value, ex "myhost01"
        /// </param>
        public void Apply(Dictionary<string, string> values) {
            if(IsApplied) {
                return;
            }
            StoredValues = values;
            _commandLauncher.Launch(SetCmd, StoredValues);
        }

        /// <summary>
        /// Get the result of the GetCmd and check if it contains the StoredValues
        /// todo what if StoredValues has more than one value?
        /// </summary>
        [JsonIgnore]
        public bool IsApplied {
            get {
                if(string.IsNullOrEmpty(GetCmd)) {
                    return true;
                }
                var result = _commandLauncher.Launch(SetCmd, StoredValues);
                return result.Contains(StoredValues.First().Value);
            }
        }
    }
}
