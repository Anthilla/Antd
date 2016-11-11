using System.Collections.Generic;
using System.Linq;
using antd.commands;

namespace Antd.Configuration {

    //public class Production {
    //    //The other members, properties etc...
    //    private Meter m;

    //    private Production(Meter m) {
    //        this.m = m;
    //    }
    //}

    //public class Meter {
    //    private int _powerRating = 0;
    //    private Production _production;

    //    public Meter() {
    //        _production = new Production(this);
    //    }
    //}

    /// <summary>
    /// Object that indexes the parameters that are needed to the machine configuration
    /// This model will be Json-serialized and stored in a .conf file on the machine
    /// </summary>
    public class MachineModel {

        /// <summary>
        /// First configuration steps
        /// For each configurable aspect add one MachineParameter to Preparation[]
        /// </summary>
        public MachineParameter[] Preparation { get; set; } = {
            new MachineParameter { SetCmd = "sysctl-p" },
            new MachineParameter { SetCmd = "systemd-machine-id-setup" },
            new MachineParameter { SetCmd = "dhclient-killall" },
            new MachineParameter { SetCmd = "systemctl-stop", StoredValues = new Dictionary<string, string> { { "$service", "systemd-resolvd.service" } } },
            new MachineParameter { SetCmd = "systemctl-stop", StoredValues = new Dictionary<string, string> { { "$service", "systemd-networkd.service" } } },
            new MachineParameter { SetCmd = "systemctl-stop", StoredValues = new Dictionary<string, string> { { "$service", "systemd-networkd.socket" } } },
        };

        public MachineParameter HostName { get; set; } = new MachineParameter {
            SetCmd = "set-hostname",
            GetCmd = "hostnamectl-get-hostname",
            StoredValues = new Dictionary<string, string> {
                { "$host_name", "aosVM01" }
            }
        };

        public MachineParameter HostChassis { get; set; } = new MachineParameter {
            SetCmd = "set-chassis",
            GetCmd = "hostnamectl-get-chassis",
            StoredValues = new Dictionary<string, string> {
                { "$host_chassis", "vm" }
            }
        };

        public MachineParameter HostDeployment { get; set; } = new MachineParameter {
            SetCmd = "set-deployment",
            GetCmd = "hostnamectl-get-deployment",
            StoredValues = new Dictionary<string, string> {
                { "$host_deployment", "Development" }
            }
        };

        public MachineParameter HostLocation { get; set; } = new MachineParameter {
            SetCmd = "set-location",
            GetCmd = "hostnamectl-get-location",
            StoredValues = new Dictionary<string, string> {
                { "$host_location", "My Location" }
            }
        };

        public MachineParameter Timezone { get; set; } = new MachineParameter {
            SetCmd = "set-timezone",
            GetCmd = "timedatectl-get-timezone",
            StoredValues = new Dictionary<string, string> {
                { "$host_timezone", "Europe/Rome" }
            }
        };

        public MachineParameter[] TimeConfiguration { get; set; } = {
            new MachineParameter { SetCmd = "ntpdate", StoredValues = new Dictionary<string, string> { { "$server", "ntp1.ien.it" } } },
            new MachineParameter { SetCmd = "set-ntpdate" },
            new MachineParameter { SetCmd = "sync-clock" },
        };

        public MachineParameter[] DnsResolv { get; set; } = {
            new MachineParameter {
                SetCmd = "echo-write",
                GetCmd = "cat-etc-resolv",
                StoredValues = new Dictionary<string, string> {
                    { "$value", "nameserver 8.8.8.8" },
                    { "$file", "/etc/resolv.conf" }
                }
            },
            new MachineParameter {
                SetCmd = "echo-append",
                GetCmd = "cat-etc-resolv",
                StoredValues = new Dictionary<string, string> {
                    { "$value", "search mylan.local" },
                    { "$file", "/etc/resolv.conf" }
                }
            },
            new MachineParameter {
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
        public MachineParameter[] Modprobes { get; set; } = {
            new MachineParameter { SetCmd = "modprobe", StoredValues = new Dictionary<string, string> { { "$package", "br_netfilter" } } },
        };

        /// <summary>
        /// RemoveModules.StoredValues[$modules] is a spaced list of modules
        /// </summary>
        public MachineParameter RemoveModules { get; set; } = new MachineParameter {
            SetCmd = "rmmod",
            StoredValues = new Dictionary<string, string> {
                { "$modules", "iptable_filter ebtable_filter ip_tables ebtables ip6table_filter eb_tables" }
            }
        };

        /// <summary>
        /// Network and network interfaces configuration
        /// MachineNamedParameter[] Network is an array of each interface
        /// Each interface has a MachineParameter[] Configuration that gathers all commands to configure THAT interface
        /// todo pass Network.Name to its Configuration.MachineParameter
        /// </summary>
        public MachineNamedParameter[] Network { get; set; } = {
            new MachineNamedParameter {
                Name = "eth0",
                StoredValues = new Dictionary<string, string> { { "$net_if", "eth0" } },
                Configuration = new [] {
                    new MachineParameter(new Dictionary<string, string> { { "$net_if", "eth0" } }) { SetCmd = "anthilla" },
                }
            }
        };

        public MachineParameter[] Firewall { get; set; } = {
            //new MachineParameter { SetCmd = "anthilla", StoredValues = new Dictionary<string, string> { { "$custom", "nft -f /mnt/cdrom/DIRS/FILE_etc_nftables.conf" } } },
        };

        /// <summary>
        /// Final configuration steps
        /// For each configurable aspect add one MachineParameter to Adjustments[]
        /// </summary>
        public MachineParameter[] Adjustments { get; set; } = {
            new MachineParameter { SetCmd = "anthilla", StoredValues = new Dictionary<string, string> { { "$custom", "rmdir /Data/*" } } },
            new MachineParameter { SetCmd = "anthilla", StoredValues = new Dictionary<string, string> { { "$custom", "mkdir -p /Data/Data01" } } },
            new MachineParameter { SetCmd = "anthilla", StoredValues = new Dictionary<string, string> { { "$custom", "mount LABEL=Data01 /Data/Data01" } } },
        };
    }

    /// <summary>
    /// This object kinda wraps the MachineParameter class, having a "common" Name and StoredValues both passed to the MachineParameter[] Configuration
    /// </summary>
    public class MachineNamedParameter {
        public string Name { get; set; } = string.Empty;

        public IDictionary<string, string> StoredValues { get; set; } = new Dictionary<string, string>();

        public MachineParameter[] Configuration { get; set; } = { };
    }

    /// <summary>
    /// Object that both stores a single element of machine configuration and applies it
    /// </summary>
    public class MachineParameter {

        private readonly CommandLauncher _commandLauncher = new CommandLauncher();

        public MachineParameter() {
        }

        /// <summary>
        /// Initialize the class MachineParameter with known StoredValues
        /// </summary>
        /// <param name="storedValues"></param>
        public MachineParameter(Dictionary<string, string> storedValues) {
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
