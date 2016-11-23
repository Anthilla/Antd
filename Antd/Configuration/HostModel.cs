using System;
using System.Collections.Generic;
using System.Linq;
using antd.commands;
using Newtonsoft.Json;

namespace Antd.Configuration {
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
        };

        /// <summary>
        /// RemoveModules.StoredValues[$modules] is a spaced list of modules
        /// </summary>
        public HostParameter RemoveModules { get; set; } = new HostParameter {
            SetCmd = "rmmod",
            StoredValues = new Dictionary<string, string> {
                { "$modules", "iptable_filter ebtable_filter ip_tables ebtables ip6table_filter eb_tables" }
            }
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
