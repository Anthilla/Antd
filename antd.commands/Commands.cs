using System.Collections.Generic;
using System.Linq;
using antdlib.common;
using antdlib.common.Tool;

namespace antd.commands {


    public class Commands {

        public static string RootFrameworkAntdShellScripts => "/framework/antd/ShellScript";
        private static readonly Bash BashTool = new Bash();
        private static readonly Read ReadTool = new Read();
        private static readonly Write WriteTool = new Write();

        public static Dictionary<string, object> List => GetDict();

        private static Dictionary<string, object> GetDict() {
            var dict = new Dictionary<string, object>();

            #region [    Command - Test    ]
            dict["test-sub-string"] = new Command {
                InputType = typeof(string),
                OutputType = typeof(string),
                Arguments = new[] { "$obj" },
                Function = (x, y) => {
                    var list = new List<string> {
                        $"element 1: {x.First()}",
                        $"element 2: {x.First()}",
                        $"element 3: {x.First()}",
                    };
                    return list;
                }
            };

            dict["test-sub-list"] = new Command {
                InputType = typeof(string),
                OutputType = typeof(string),
                Arguments = new[] { "$obj", "prova $obj", "$value is another value" },
                Function = (x, y) => {
                    var list = new List<string> {
                        $"element 1: {x.First()}",
                        $"element 2: {x.ToArray()[1]}",
                        $"element 3: {x.Last()}",
                        $"element combo: {x.JoinToString(", ")}"
                    };
                    return list;
                }
            };
            #endregion

            #region [    Command - Misc    ]
            dict["anthilla"] = new Command {
                InputType = typeof(string),
                OutputType = typeof(string),
                Arguments = new[] { "$custom" },
                Function = (x, y) => BashTool.Execute(x).SplitBash()
            };
            dict["modprobe"] = new Command {
                InputType = typeof(string),
                OutputType = typeof(string),
                Arguments = new[] { "modprobe $package" },
                Function = (x, y) => BashTool.Execute(x).SplitBash()
            };
            dict["mono-antdsh-update"] = new Command {
                InputType = typeof(string),
                OutputType = typeof(string),
                Arguments = new[] { "mono /framework/antdsh/antdsh.exe update $context" },
                Function = (x, y) => BashTool.Execute(x).SplitBash()
            };
            dict["mono-antdsh-update-check"] = new Command {
                InputType = typeof(string),
                OutputType = typeof(string),
                Arguments = new[] { "mono /framework/antdsh/antdsh.exe update check" },
                Function = (x, y) => BashTool.Execute(x).SplitBash()
            };
            dict["rmmod"] = new Command {
                InputType = typeof(string),
                OutputType = typeof(string),
                Arguments = new[] { "rmmod $modules" },
                Function = (x, y) => BashTool.Execute(x).SplitBash()
            };
            dict["set-vlan"] = new Command {
                InputType = typeof(IEnumerable<string>),
                OutputType = typeof(string),
                Arguments = new[] {
                    "ip link set $vlan_interface_name down",
                    "ip link del $vlan_interface_name",
                    "ip link add name $vlan_interface_name link $interface_name type vlan id $vlan_id",
                    "ip link set $vlan_interface_name txqueuelen 10000",
                    "ip link set $vlan_interface_name up"
                },
                Function = (x, y) => BashTool.Execute(x).SplitBash()
            };
            dict["sysctl-p"] = new Command {
                InputType = typeof(string),
                OutputType = typeof(string),
                Arguments = new[] { "sysctl -p" },
                Function = (x, y) => BashTool.Execute(x).SplitBash()
            };
            dict["systemd-machine-id-setup"] = new Command {
                InputType = typeof(string),
                OutputType = typeof(string),
                Arguments = new[] { "systemd-machine-id-setup" },
                Function = (x, y) => BashTool.Execute(x).SplitBash()
            };
            #endregion

            #region [    Command - Bond    ]
            dict["bond-add-if"] = new Command {
                InputType = typeof(string),
                OutputType = typeof(string),
                Arguments = new[] { "ifenslave $bond $net_if" },
                Function = (x, y) => BashTool.Execute(x, false).SplitBash()
            };
            dict["bond-del-if"] = new Command {
                InputType = typeof(string),
                OutputType = typeof(string),
                Arguments = new[] { "ifenslave -d $bond $net_if" },
                Function = (x, y) => BashTool.Execute(x, false).SplitBash()
            };
            dict["bond-set"] = new Command {
                InputType = typeof(IEnumerable<string>),
                OutputType = typeof(string),
                Arguments = new[] {
                    "ip link set $bond down",
                    "ip link del $bond",
                    "ip link add name $bond type bond",
                    "ip link set $bondme txqueuelen 10000",
                    "ip link set $bond down",
                    "echo 4 > /sys/class/net/$bond/bonding/mode",
                    "echo 1 > /sys/class/net/$bond/bonding/lacp_rate",
                    "echo 1 > /sys/class/net/$bond/lacp_rate",
                    "echo 100 > /sys/class/net/$bond/bonding/miimon",
                    "ip link set $bond up"
                },
                Function = (x, y) => BashTool.Execute(x, false).SplitBash()
            };
            dict["set-bond"] = new Command {
                InputType = typeof(IEnumerable<string>),
                OutputType = typeof(string),
                Arguments = new[] {
                    "ip link set $bond_name down",
                    "ip link del $bond_name",
                    "ip link add name $bond_name type bond",
                    "ip link set $bond_name txqueuelen 10000",
                    "ip link set $bond_name down",
                    "echo 4 > /sys/class/net/$bond_name/bonding/mode",
                    "echo 1 > /sys/class/net/$bond_name/bonding/lacp_rate",
                    "echo 1 > /sys/class/net/$bond_name/lacp_rate",
                    "echo 100 > /sys/class/net/$bond_name/bonding/miimon",
                    "ip link set $bond_name up"
                },
                Function = (x, y) => BashTool.Execute(x, false).SplitBash()
            };
            #endregion

            #region [    Command - Brctl    ]
            dict["brctl-add"] = new Command {
                InputType = typeof(string),
                OutputType = typeof(string),
                Arguments = new[] { "brctl addbr $bridge" },
                Function = (x, y) => BashTool.Execute(x, false).SplitBash()
            };
            dict["brctl-add-if"] = new Command {
                InputType = typeof(string),
                OutputType = typeof(string),
                Arguments = new[] { "brctl addif $bridge $net_if" },
                Function = (x, y) => BashTool.Execute(x, false).SplitBash()
            };
            dict["brctl-del"] = new Command {
                InputType = typeof(string),
                OutputType = typeof(string),
                Arguments = new[] { "brctl delbr $bridge" },
                Function = (x, y) => BashTool.Execute(x, false).SplitBash()
            };
            dict["brctl-del-if"] = new Command {
                InputType = typeof(string),
                OutputType = typeof(string),
                Arguments = new[] { "brctl delif $bridge $net_if" },
                Function = (x, y) => BashTool.Execute(x, false).SplitBash()
            };
            dict["brctl-set-pathcost"] = new Command {
                InputType = typeof(string),
                OutputType = typeof(string),
                Arguments = new[] { "brctl setpathcost $bridge $path $cost set path cost" },
                Function = (x, y) => BashTool.Execute(x, false).SplitBash()
            };
            dict["brctl-set-portprio"] = new Command {
                InputType = typeof(string),
                OutputType = typeof(string),
                Arguments = new[] { "brctl setportprio $bridge $port $priority set port priority" },
                Function = (x, y) => BashTool.Execute(x, false).SplitBash()
            };
            dict["brctl-show-br"] = new Command {
                InputType = typeof(string),
                OutputType = typeof(IEnumerable<string>),
                Arguments = new[] { "brctl show $bridge" },
                Grep = "$bridge",
                Function = (x, y) => BashTool.Execute(x).Grep(y)
            };
            dict["brctl-show-brid"] = new Command {
                InputType = typeof(string),
                OutputType = typeof(IEnumerable<string>),
                Arguments = new[] { "brctl show $bridge" },
                Grep = "$bridge",
                Function = (x, y) => BashTool.Execute(x).Grep(y).Print(2, " ")
            };
            dict["brctl-show-brif"] = new Command {
                InputType = typeof(string),
                OutputType = typeof(IEnumerable<string>),
                Arguments = new[] { "brctl show $bridge" },
                Grep = "$bridge",
                Function = (x, y) => BashTool.Execute(x).Grep(y).Print(4, " ")
            };
            dict["brctl-show-brstpstatus"] = new Command {
                InputType = typeof(string),
                OutputType = typeof(IEnumerable<string>),
                Arguments = new[] { "brctl show $bridge" },
                Grep = "$bridge",
                Function = (x, y) => BashTool.Execute(x).Grep(y).Print(3, " ")
            };
            dict["brctl-show-macs"] = new Command {
                InputType = typeof(string),
                OutputType = typeof(IEnumerable<string>),
                Arguments = new[] { "brctl showmacs $bridge" },
                Function = (x, y) => BashTool.Execute(x).Grep(y)
            };
            dict["brctl-show-stp"] = new Command {
                InputType = typeof(string),
                OutputType = typeof(IEnumerable<string>),
                Arguments = new[] { "brctl showstp $bridge" },
                Function = (x, y) => BashTool.Execute(x).Grep(y)
            };
            dict["brctl-stp-off"] = new Command {
                InputType = typeof(string),
                OutputType = typeof(IEnumerable<string>),
                Arguments = new[] { "brctl stp $bridge off" },
                Function = (x, y) => BashTool.Execute(x).Grep(y)
            };
            dict["brctl-stp-on"] = new Command {
                InputType = typeof(string),
                OutputType = typeof(IEnumerable<string>),
                Arguments = new[] { "brctl stp $bridge on" },
                Function = (x, y) => BashTool.Execute(x).Grep(y)
            };
            #endregion

            #region [    Command - Cat    ]
            dict["cat"] = new Command {
                InputType = typeof(string),
                OutputType = typeof(string),
                Arguments = new[] { "$file" },
                Function = (x, y) => ReadTool.FileLines(x.First())
            };
            dict["cat-etc-gentoorel"] = new Command {
                InputType = typeof(string),
                OutputType = typeof(string),
                Arguments = new[] { "/etc/gentoo-release" },
                Function = (x, y) => ReadTool.FileLines(x.First())
            };
            dict["cat-etc-hostname"] = new Command {
                InputType = typeof(string),
                OutputType = typeof(string),
                Arguments = new[] { "/etc/hostname" },
                Function = (x, y) => ReadTool.FileLines(x.First())
            };
            dict["cat-etc-hosts"] = new Command {
                InputType = typeof(string),
                OutputType = typeof(string),
                Arguments = new[] { "/etc/hosts" },
                Function = (x, y) => ReadTool.FileLines(x.First())
            };
            dict["cat-etc-lsbrel"] = new Command {
                InputType = typeof(string),
                OutputType = typeof(string),
                Arguments = new[] { "/etc/lsb-release" },
                Function = (x, y) => ReadTool.FileLines(x.First())
            };
            dict["cat-etc-osrel"] = new Command {
                InputType = typeof(string),
                OutputType = typeof(string),
                Arguments = new[] { "/etc/os-release" },
                Function = (x, y) => ReadTool.FileLines(x.First())
            };
            dict["cat-etc-resolv"] = new Command {
                InputType = typeof(string),
                OutputType = typeof(string),
                Arguments = new[] { "/etc/resolv.conf" },
                Function = (x, y) => ReadTool.FileLines(x.First())
            };
            dict["cat-etc-nsswitch"] = new Command {
                InputType = typeof(string),
                OutputType = typeof(string),
                Arguments = new[] { "/etc/nsswitch.conf" },
                Function = (x, y) => ReadTool.FileLines(x.First()).GrepIgnore("#")
            };
            dict["cat-etc-networks"] = new Command {
                InputType = typeof(string),
                OutputType = typeof(string),
                Arguments = new[] { "/etc/networks" },
                Function = (x, y) => ReadTool.FileLines(x.First()).GrepIgnore("#")
            };
            #endregion

            #region [    Command - Dhclient    ]
            dict["dhclient-killall"] = new Command {
                InputType = typeof(string),
                OutputType = typeof(string),
                Arguments = new[] { "killall dhclient" },
                Function = (x, y) => BashTool.Execute(x).SplitBash()
            };
            dict["dhclient4"] = new Command {
                InputType = typeof(string),
                OutputType = typeof(string),
                Arguments = new[] { "dhclient $net_if" },
                Function = (x, y) => BashTool.Execute(x).SplitBash()
            };
            dict["dhclient6"] = new Command {
                InputType = typeof(string),
                OutputType = typeof(string),
                Arguments = new[] { "dhclient -6 $net_if" },
                Function = (x, y) => BashTool.Execute(x).SplitBash()
            };
            #endregion

            #region [    Command - Echo (write/append)    ]
            dict["echo-write"] = new Command {
                InputType = typeof(string),
                OutputType = typeof(string),
                Arguments = new[] { "$file" },
                Grep = "$value",
                Function = (x, y) => WriteTool.WriteFile(x.First(), y)
            };
            dict["echo-append"] = new Command {
                InputType = typeof(string),
                OutputType = typeof(string),
                Arguments = new[] { "$file" },
                Grep = "$value",
                Function = (x, y) => WriteTool.AppendFile(x.First(), y)
            };
            dict["echo-append-rm"] = new Command {
                InputType = typeof(string),
                OutputType = typeof(string),
                Arguments = new[] { "$file" },
                Grep = "$value",
                Function = (x, y) => {
                    if(!System.IO.File.ReadAllText(y).Contains(x.First())) {
                        WriteTool.AppendFile(x.First(), y);
                    }
                    return new List<string>();
                }
            };
            #endregion

            #region [    Command - Fdisk    ]
            dict["fdisk-print"] = new Command {
                InputType = typeof(string),
                OutputType = typeof(string),
                Arguments = new[] { "echo -e \"p\" | fdisk $disk_device" },
                Function = (x, y) => BashTool.Execute(x).SplitBash()
            };
            dict["fdisk-set-partition"] = new Command {
                InputType = typeof(IEnumerable<string>),
                OutputType = typeof(string),
                Arguments = new[] {
                    "echo -e \"n\\n $part_number\\n $part_first_sector\\n $part_size\\n w\\n\" | fdisk $disk_device",
                    "parted $disk_device $part_number 1 $name",
                    "parted $disk_device align-check opt $part_number"
                },
                Function = (x, y) => BashTool.Execute(x).SplitBash()
            };
            #endregion

            #region [    Command - Hostnamectl    ]
            dict["hostnamectl"] = new Command {
                InputType = typeof(string),
                OutputType = typeof(IEnumerable<string>),
                Arguments = new[] { "hostnamectl" },
                Function = (x, y) => BashTool.Execute(x).SplitBash()
            };
            dict["hostnamectl-get-arch"] = new Command {
                InputType = typeof(string),
                OutputType = typeof(string),
                Arguments = new[] { "hostnamectl" },
                Grep = "Architecture: ",
                Function = (x, y) => BashTool.Execute(x).Grep(y).First().Print(2, ':').SplitBash()
            };
            dict["hostnamectl-get-bootid"] = new Command {
                InputType = typeof(string),
                OutputType = typeof(string),
                Arguments = new[] { "hostnamectl" },
                Grep = "Boot ID: ",
                Function = (x, y) => BashTool.Execute(x).Grep(y).First().Print(2, ':').SplitBash()
            };
            dict["hostnamectl-get-chassis"] = new Command {
                InputType = typeof(string),
                OutputType = typeof(string),
                Arguments = new[] { "hostnamectl" },
                Grep = "Chassis: ",
                Function = (x, y) => BashTool.Execute(x).Grep(y).First().Print(2, ':').SplitBash()
            };
            dict["hostnamectl-get-deployment"] = new Command {
                InputType = typeof(string),
                OutputType = typeof(string),
                Arguments = new[] { "hostnamectl" },
                Grep = "Deployment: ",
                Function = (x, y) => BashTool.Execute(x).Grep(y).First().Print(2, ':').SplitBash()
            };
            dict["hostnamectl-get-hostname"] = new Command {
                InputType = typeof(string),
                OutputType = typeof(string),
                Arguments = new[] { "hostnamectl" },
                Grep = "Transient hostname: ",
                Function = (x, y) => BashTool.Execute(x).Grep(y).First().Print(2, ':').SplitBash()
            };
            dict["hostnamectl-get-iconname"] = new Command {
                InputType = typeof(string),
                OutputType = typeof(string),
                Arguments = new[] { "hostnamectl" },
                Grep = "Icon name: ",
                Function = (x, y) => BashTool.Execute(x).Grep(y).First().Print(2, ':').SplitBash()
            };
            dict["hostnamectl-get-kernel"] = new Command {
                InputType = typeof(string),
                OutputType = typeof(string),
                Arguments = new[] { "hostnamectl" },
                Grep = "Kernel: ",
                Function = (x, y) => BashTool.Execute(x).Grep(y).First().Print(2, ':').SplitBash()
            };
            dict["hostnamectl-get-location"] = new Command {
                InputType = typeof(string),
                OutputType = typeof(string),
                Arguments = new[] { "hostnamectl" },
                Grep = "Location: ",
                Function = (x, y) => BashTool.Execute(x).Grep(y).First().Print(2, ':').SplitBash()
            };
            dict["hostnamectl-get-machineid"] = new Command {
                InputType = typeof(string),
                OutputType = typeof(string),
                Arguments = new[] { "hostnamectl" },
                Grep = "Machine ID: ",
                Function = (x, y) => BashTool.Execute(x).Grep(y).First().Print(2, ':').SplitBash()
            };
            dict["hostnamectl-get-os"] = new Command {
                InputType = typeof(string),
                OutputType = typeof(string),
                Arguments = new[] { "hostnamectl" },
                Grep = "Operating System: ",
                Function = (x, y) => BashTool.Execute(x).Grep(y).First().Print(2, ':').SplitBash()
            };
            dict["hostnamectl-get-virtualization"] = new Command {
                InputType = typeof(string),
                OutputType = typeof(string),
                Arguments = new[] { "hostnamectl" },
                Grep = "Virtualization: ",
                Function = (x, y) => BashTool.Execute(x).Grep(y).First().Print(2, ':').SplitBash()
            };
            dict["set-chassis"] = new Command {
                InputType = typeof(string),
                OutputType = typeof(string),
                Arguments = new[] { "hostnamectl set-chassis $host_chassis" },
                Function = (x, y) => BashTool.Execute(x).SplitBash()
            };
            dict["set-deployment"] = new Command {
                InputType = typeof(string),
                OutputType = typeof(string),
                Arguments = new[] { "hostnamectl set-deployment $host_deployment" },
                Function = (x, y) => BashTool.Execute(x).SplitBash()
            };
            dict["set-hostname"] = new Command {
                InputType = typeof(string),
                OutputType = typeof(string),
                Arguments = new[] { "hostnamectl set-hostname $host_name" },
                Function = (x, y) => BashTool.Execute(x).SplitBash()
            };
            dict["set-location"] = new Command {
                InputType = typeof(string),
                OutputType = typeof(string),
                Arguments = new[] { "hostnamectl set-location \"$host_location\"" },
                Function = (x, y) => BashTool.Execute(x).SplitBash()
            };
            #endregion

            #region [    Command - Network    ]
            dict["nmap-ip"] = new Command {
                InputType = typeof(string),
                OutputType = typeof(IEnumerable<string>),
                Arguments = new[] { "nmap $ip" },
                Function = (x, y) => BashTool.Execute(x).SplitBash()
            };
            dict["nmap-snmp-interfaces"] = new Command {
                InputType = typeof(string),
                OutputType = typeof(IEnumerable<string>),
                Arguments = new[] { "nmap -sU -p 161 -T4 -d -v -n -Pn --script snmp-interfaces $ip" },
                Function = (x, y) => BashTool.Execute(x).SplitBash()
            };
            dict["ping-c"] = new Command {
                InputType = typeof(string),
                OutputType = typeof(IEnumerable<string>),
                Arguments = new[] { "ping -c3 -w10 $ip" },
                Function = (x, y) => BashTool.Execute(x, false).SplitBash()
            };
            dict["arp"] = new Command {
                InputType = typeof(string),
                OutputType = typeof(IEnumerable<string>),
                Arguments = new[] { "arp $ip" },
                Function = (x, y) => BashTool.Execute(x).SplitBash()
            };
            dict["ifconfig"] = new Command {
                InputType = typeof(string),
                OutputType = typeof(IEnumerable<string>),
                Arguments = new[] { "ifconfig" },
                Function = (x, y) => BashTool.Execute(x).SplitBash()
            };
            dict["ifconfig-if"] = new Command {
                InputType = typeof(string),
                OutputType = typeof(IEnumerable<string>),
                Arguments = new[] { "ifconfig $if" },
                Function = (x, y) => BashTool.Execute(x).SplitBash()
            };
            dict["net-carrier"] = new Command {
                InputType = typeof(string),
                OutputType = typeof(string),
                Arguments = new[] { "/sys/class/net/$if/carrier" },
                Function = (x, y) => ReadTool.FileLines(x.First())
            };
            #endregion

            #region [    Command - Ipv4    ]
            dict["set-network-interface"] = new Command {
                InputType = typeof(IEnumerable<string>),
                OutputType = typeof(string),
                Arguments = new[] {
                    "ip link set dev $interface_name down",
                    "ip link set dev $interface_name up",
                    "ip link set $interface_name txqueuelen 10000",
                    "ip link set dev $interface_name up"
                },
                Function = (x, y) => BashTool.Execute(x).SplitBash()
            };
            dict["ip4-add-addr"] = new Command {
                InputType = typeof(string),
                OutputType = typeof(string),
                Arguments = new[] { "ip addr add $address/$range dev $net_if" },
                Function = (x, y) => BashTool.Execute(x).SplitBash()
            };
            dict["ip4-add-addr-broadcast"] = new Command {
                InputType = typeof(string),
                OutputType = typeof(string),
                Arguments = new[] { "ip addr add $address/$range broadcast $broadcast dev $net_if" },
                Function = (x, y) => BashTool.Execute(x).SplitBash()
            };
            dict["ip4-add-multipath-route"] = new Command {
                InputType = typeof(string),
                OutputType = typeof(string),
                Arguments = new[] { "ip route add default scope global nexthop dev $net1 nexthop dev $net2" },
                Function = (x, y) => BashTool.Execute(x).SplitBash()
            };
            dict["ip4-add-nat"] = new Command {
                InputType = typeof(string),
                OutputType = typeof(string),
                Arguments = new[] { "ip route add nat $ip_address via $ip_via_address" },
                Function = (x, y) => BashTool.Execute(x).SplitBash()
            };
            dict["ip4-add-route"] = new Command {
                InputType = typeof(string),
                OutputType = typeof(string),
                Arguments = new[] { "ip route add $ip_address via $gateway dev $net_if" },
                Function = (x, y) => BashTool.Execute(x).SplitBash()
            };
            dict["ip4-add-tunnel-point-to-point"] = new Command {
                InputType = typeof(string),
                OutputType = typeof(string),
                Arguments = new[] { "ip tunnel add $net_if mode sit ttl $ttl remote $tunnel local $local_address" },
                Function = (x, y) => BashTool.Execute(x).SplitBash()
            };
            dict["ip4-del-addr"] = new Command {
                InputType = typeof(string),
                OutputType = typeof(string),
                Arguments = new[] { "ip addr del $address/$range dev $net_if" },
                Function = (x, y) => BashTool.Execute(x).SplitBash()
            };
            dict["ip4-del-addr-broadcast"] = new Command {
                InputType = typeof(string),
                OutputType = typeof(string),
                Arguments = new[] { "ip addr del $address/$range broadcast $broadcast dev $net_if" },
                Function = (x, y) => BashTool.Execute(x).SplitBash()
            };
            dict["ip4-del-route"] = new Command {
                InputType = typeof(string),
                OutputType = typeof(string),
                Arguments = new[] { "ip route del $ip_address via $gateway dev $net_if" },
                Function = (x, y) => BashTool.Execute(x).SplitBash()
            };
            dict["ip4-del-tunnel-point-to-point"] = new Command {
                InputType = typeof(string),
                OutputType = typeof(string),
                Arguments = new[] { "ip tunnel del $net_if" },
                Function = (x, y) => BashTool.Execute(x).SplitBash()
            };
            dict["ip4-disable-if"] = new Command {
                InputType = typeof(string),
                OutputType = typeof(string),
                Arguments = new[] { "ip link set $net_if down" },
                Function = (x, y) => BashTool.Execute(x).SplitBash()
            };
            dict["ip4-enable-if"] = new Command {
                InputType = typeof(string),
                OutputType = typeof(string),
                Arguments = new[] { "ip link set $net_if up" },
                Function = (x, y) => BashTool.Execute(x).SplitBash()
            };
            dict["ip4-flush-configuration"] = new Command {
                InputType = typeof(string),
                OutputType = typeof(string),
                Arguments = new[] { "ip addr flush dev $net_if" },
                Function = (x, y) => BashTool.Execute(x).SplitBash()
            };
            dict["ip4-get-if-addr"] = new Command {
                InputType = typeof(string),
                OutputType = typeof(string),
                Arguments = new[] { "ip addr show $net_if" },
                Function = (x, y) => BashTool.Execute(x).Grep("inet .").First().Print(2, " ").SplitBash()
            };
            dict["ip4-get-if-brd"] = new Command {
                InputType = typeof(string),
                OutputType = typeof(string),
                Arguments = new[] { "ip addr show $net_if" },
                Function = (x, y) => BashTool.Execute(x).Grep("inet .").First().Print(4, " ").SplitBash()
            };
            dict["ip4-get-if-macaddress"] = new Command {
                InputType = typeof(string),
                OutputType = typeof(string),
                Arguments = new[] { "/sys/class/net/$net_if/address" },
                Function = (x, y) => ReadTool.FileLines(x.First())
            };
            dict["ip4-get-if-mtu"] = new Command {
                InputType = typeof(string),
                OutputType = typeof(string),
                Arguments = new[] { "/sys/class/net/$net_if/mtu" },
                Function = (x, y) => ReadTool.FileLines(x.First())
            };
            dict["ip4-if-isdown"] = new Command {
                InputType = typeof(string),
                OutputType = typeof(string),
                Arguments = new[] { "ip addr show $net_if" },
                Function = (x, y) => BashTool.Execute(x).Grep("state DOWN").First().SplitBash()
            };
            dict["ip4-if-isup"] = new Command {
                InputType = typeof(string),
                OutputType = typeof(string),
                Arguments = new[] { "ip addr show $net_if" },
                Function = (x, y) => BashTool.Execute(x).Grep("state UP").First().SplitBash()
            };
            dict["ip4-set-macaddress"] = new Command {
                InputType = typeof(IEnumerable<string>),
                OutputType = typeof(string),
                Arguments = new[] {
                    "ip link set $net_if down",
                    "ip link set dev $net_if address $address",
                    "ip link set $net_if up"
                },
                Function = (x, y) => BashTool.Execute(x).SplitBash()
            };
            dict["ip4-set-mtu"] = new Command {
                InputType = typeof(string),
                OutputType = typeof(string),
                Arguments = new[] { "ip link set dev $net_if mtu $mtu" },
                Function = (x, y) => BashTool.Execute(x).SplitBash()
            };
            dict["ip4-show-if-addr"] = new Command {
                InputType = typeof(string),
                OutputType = typeof(string),
                Arguments = new[] { "ip addr show $net_if" },
                Function = (x, y) => BashTool.Execute(x).SplitBash()
            };
            dict["ip4-show-if-link"] = new Command {
                InputType = typeof(string),
                OutputType = typeof(string),
                Arguments = new[] { "ip -s link ls $net_if" },
                Function = (x, y) => BashTool.Execute(x).SplitBash()
            };
            dict["ip4-show-routes"] = new Command {
                InputType = typeof(string),
                OutputType = typeof(string),
                Arguments = new[] { "ip route show $net_if" },
                Function = (x, y) => BashTool.Execute(x).SplitBash()
            };
            dict["ip4-show-tunnels"] = new Command {
                InputType = typeof(string),
                OutputType = typeof(string),
                Arguments = new[] { "ip tunnel show $net_if" },
                Function = (x, y) => BashTool.Execute(x).SplitBash()
            };
            dict["ip4-show-updown"] = new Command {
                InputType = typeof(string),
                OutputType = typeof(string),
                Arguments = new[] { "ip link show $net_if" },
                //todo fare multigrep  | grep -ho \' UP \\| DOWN \'
                Function = (x, y) => BashTool.Execute(x).Grep("UP").First().SplitBash()
            };
            #endregion

            #region [    Command - Ipv6    ]
            dict["ip6-add-addr"] = new Command {
                InputType = typeof(string),
                OutputType = typeof(string),
                Arguments = new[] { "ip -6 addr add $address/$range dev $net_if" },
                Function = (x, y) => BashTool.Execute(x).SplitBash()
            };
            dict["ip6-add-neigh"] = new Command {
                InputType = typeof(string),
                OutputType = typeof(string),
                Arguments = new[] { "ip -6 neigh add $ip_address lladdr $ip_lay_address dev $net_if" },
                Function = (x, y) => BashTool.Execute(x).SplitBash()
            };
            dict["ip6-add-new-address"] = new Command {
                InputType = typeof(string),
                OutputType = typeof(string),
                Arguments = new[] { "ip -6 addr add $ip_address dev $net_if" },
                Function = (x, y) => BashTool.Execute(x).SplitBash()
            };
            dict["ip6-add-route"] = new Command {
                InputType = typeof(string),
                OutputType = typeof(string),
                Arguments = new[] { "ip -6 route add $ip_address via $gateway" },
                Function = (x, y) => BashTool.Execute(x).SplitBash()
            };
            dict["ip6-add-route-dev"] = new Command {
                InputType = typeof(string),
                OutputType = typeof(string),
                Arguments = new[] { "ip -6 addr del $address/$range dev $net_if" },
                Function = (x, y) => BashTool.Execute(x).SplitBash()
            };
            dict["ip6-del-addr"] = new Command {
                InputType = typeof(string),
                OutputType = typeof(string),
                Arguments = new[] { "ip -6 addr del $address/$range dev $net_if" },
                Function = (x, y) => BashTool.Execute(x).SplitBash()
            };
            dict["ip6-del-address"] = new Command {
                InputType = typeof(string),
                OutputType = typeof(string),
                Arguments = new[] { "ip -6 addr del $ip_address dev $net_if" },
                Function = (x, y) => BashTool.Execute(x).SplitBash()
            };
            dict["ip6-del-neigh"] = new Command {
                InputType = typeof(string),
                OutputType = typeof(string),
                Arguments = new[] { "ip -6 neigh del $ip_address lladdr $ip_lay_address dev $net_if" },
                Function = (x, y) => BashTool.Execute(x).SplitBash()
            };
            dict["ip6-del-route"] = new Command {
                InputType = typeof(string),
                OutputType = typeof(string),
                Arguments = new[] { "ip -6 route del $ip_address via $gateway" },
                Function = (x, y) => BashTool.Execute(x).SplitBash()
            };
            dict["ip6-del-route-dev"] = new Command {
                InputType = typeof(string),
                OutputType = typeof(string),
                Arguments = new[] { "ip -6 route del $gateway dev $net_if" },
                Function = (x, y) => BashTool.Execute(x).SplitBash()
            };
            dict["ip6-flush-configuration"] = new Command {
                InputType = typeof(string),
                OutputType = typeof(string),
                Arguments = new[] { "ip -6 addr flush dynamic" },
                Function = (x, y) => BashTool.Execute(x).SplitBash()
            };
            dict["ip6-show-if-link"] = new Command {
                InputType = typeof(string),
                OutputType = typeof(string),
                Arguments = new[] { "ip -6 link show $net_if" },
                Function = (x, y) => BashTool.Execute(x).SplitBash()
            };
            dict["ip6-show-if-stats"] = new Command {
                InputType = typeof(string),
                OutputType = typeof(string),
                Arguments = new[] { "ip -6 -s link ls $net_if" },
                Function = (x, y) => BashTool.Execute(x).SplitBash()
            };
            dict["ip6-show-neigh"] = new Command {
                InputType = typeof(string),
                OutputType = typeof(string),
                Arguments = new[] { "ip -6 neigh show dev $net_if" },
                Function = (x, y) => BashTool.Execute(x).SplitBash()
            };
            dict["ip6-show-routes"] = new Command {
                InputType = typeof(string),
                OutputType = typeof(string),
                Arguments = new[] { "ip -6 route show $net_if" },
                Function = (x, y) => BashTool.Execute(x).SplitBash()
            };
            dict["ip6-show-tunnels"] = new Command {
                InputType = typeof(string),
                OutputType = typeof(string),
                Arguments = new[] { "ip -6 tunnel show $net_if" },
                Function = (x, y) => BashTool.Execute(x).SplitBash()
            };
            #endregion

            #region [    Command - Time & Date    ]
            dict["ntpdate"] = new Command {
                InputType = typeof(string),
                OutputType = typeof(string),
                Arguments = new[] { "ntpdate $server" },
                Function = (x, y) => BashTool.Execute(x).SplitBash()
            };
            dict["set-ntpdate"] = new Command {
                InputType = typeof(IEnumerable<string>),
                OutputType = typeof(string),
                Arguments = new[] {
                    "ntpdate $date_server",
                    "timedatectl --no-pager --no-ask-password --adjust-system-clock set-ntp yes"
                },
                Function = (x, y) => BashTool.Execute(x).SplitBash()
            };
            dict["set-timezone"] = new Command {
                InputType = typeof(string),
                OutputType = typeof(string),
                Arguments = new[] { "timedatectl --no-pager --no-ask-password --adjust-system-clock set-timezone $host_timezone" },
                Function = (x, y) => BashTool.Execute(x).SplitBash()
            };
            dict["sync-clock"] = new Command {
                InputType = typeof(IEnumerable<string>),
                OutputType = typeof(string),
                Arguments = new[] {
                    "hwclock --systohc",
                    "hwclock --hctosys"
                },
                Function = (x, y) => BashTool.Execute(x).SplitBash()
            };
            dict["timedatectl"] = new Command {
                InputType = typeof(string),
                OutputType = typeof(IEnumerable<string>),
                Arguments = new[] { "timedatectl" },
                Function = (x, y) => BashTool.Execute(x).SplitBash()
            };
            dict["timedatectl-get-localtime"] = new Command {
                InputType = typeof(string),
                OutputType = typeof(string),
                Arguments = new[] { "timedatectl" },
                Grep = "Local time: ",
                Function = (x, y) => BashTool.Execute(x).Grep(y).First().Print(2, ':').SplitBash()
            };
            dict["timedatectl-get-nettimeon"] = new Command {
                InputType = typeof(string),
                OutputType = typeof(string),
                Arguments = new[] { "timedatectl" },
                Grep = "Network time on: ",
                Function = (x, y) => BashTool.Execute(x).Grep(y).First().Print(2, ':').SplitBash()
            };
            dict["timedatectl-get-ntpsync"] = new Command {
                InputType = typeof(string),
                OutputType = typeof(string),
                Arguments = new[] { "timedatectl" },
                Grep = "NTP synchronized: ",
                Function = (x, y) => BashTool.Execute(x).Grep(y).First().Print(2, ':').SplitBash()
            };
            dict["timedatectl-get-rtcintz"] = new Command {
                InputType = typeof(string),
                OutputType = typeof(string),
                Arguments = new[] { "timedatectl" },
                Grep = "RTC in local TZ: ",
                Function = (x, y) => BashTool.Execute(x).Grep(y).First().Print(2, ':').SplitBash()
            };
            dict["timedatectl-get-rtctime"] = new Command {
                InputType = typeof(string),
                OutputType = typeof(string),
                Arguments = new[] { "timedatectl" },
                Grep = "RTC time: ",
                Function = (x, y) => BashTool.Execute(x).Grep(y).First().Print(2, ':').SplitBash()
            };
            dict["timedatectl-get-timezone"] = new Command {
                InputType = typeof(string),
                OutputType = typeof(string),
                Arguments = new[] { "timedatectl" },
                Grep = "Time zone: ",
                Function = (x, y) => BashTool.Execute(x).Grep(y).First().Print(2, ':').SplitBash()
            };
            dict["timedatectl-get-univtime"] = new Command {
                InputType = typeof(string),
                OutputType = typeof(string),
                Arguments = new[] { "timedatectl" },
                Grep = "Universal time: ",
                Function = (x, y) => BashTool.Execute(x).Grep(y).First().Print(2, ':').SplitBash()
            };
            #endregion

            #region [    Command - Rsync    ]
            dict["rsync"] = new Command {
                InputType = typeof(string),
                OutputType = typeof(string),
                Arguments = new[] { "rsync -aHA $source/ $destination/" },
                Function = (x, y) => BashTool.Execute(x).SplitBash()
            };
            dict["rsync-delete-after"] = new Command {
                InputType = typeof(string),
                OutputType = typeof(string),
                Arguments = new[] { "rsync -aHA --delete-after $source/ $destination/" },
                Function = (x, y) => BashTool.Execute(x).SplitBash()
            };
            dict["rsync-delete-during"] = new Command {
                InputType = typeof(string),
                OutputType = typeof(string),
                Arguments = new[] { "rsync -aHA --delete-during $source/ $destination/" },
                Function = (x, y) => BashTool.Execute(x).SplitBash()
            };
            #endregion

            #region [    Command - Systemctl    ]
            dict["systemctl-start"] = new Command {
                InputType = typeof(string),
                OutputType = typeof(string),
                Arguments = new[] { "systemctl start $service" },
                Function = (x, y) => BashTool.Execute(x).SplitBash()
            };
            dict["systemctl-restart"] = new Command {
                InputType = typeof(string),
                OutputType = typeof(string),
                Arguments = new[] { "systemctl restart $service" },
                Function = (x, y) => BashTool.Execute(x).SplitBash()
            };
            dict["systemctl-stop"] = new Command {
                InputType = typeof(string),
                OutputType = typeof(string),
                Arguments = new[] { "systemctl stop $service" },
                Function = (x, y) => BashTool.Execute(x).SplitBash()
            };
            dict["systemctl-enable"] = new Command {
                InputType = typeof(string),
                OutputType = typeof(string),
                Arguments = new[] { "systemctl enable $service" },
                Function = (x, y) => BashTool.Execute(x).SplitBash()
            };
            dict["systemctl-disable"] = new Command {
                InputType = typeof(string),
                OutputType = typeof(string),
                Arguments = new[] { "systemctl disable $service" },
                Function = (x, y) => BashTool.Execute(x).SplitBash()
            };
            #endregion

            #region [    Command - Virsh    ]
            dict["virsh-destroy"] = new Command {
                InputType = typeof(string),
                OutputType = typeof(string),
                Arguments = new[] { "virsh destroy $domain" },
                Function = (x, y) => BashTool.Execute(x).SplitBash()
            };
            dict["virsh-reboot"] = new Command {
                InputType = typeof(string),
                OutputType = typeof(string),
                Arguments = new[] { "virsh reboot $domain" },
                Function = (x, y) => BashTool.Execute(x).SplitBash()
            };
            dict["virsh-reset"] = new Command {
                InputType = typeof(string),
                OutputType = typeof(string),
                Arguments = new[] { "virsh reset $domain" },
                Function = (x, y) => BashTool.Execute(x).SplitBash()
            };
            dict["virsh-restore"] = new Command {
                InputType = typeof(string),
                OutputType = typeof(string),
                Arguments = new[] { "virsh restore $domain" },
                Function = (x, y) => BashTool.Execute(x).SplitBash()
            };
            dict["virsh-resume"] = new Command {
                InputType = typeof(string),
                OutputType = typeof(string),
                Arguments = new[] { "virsh resume $domain" },
                Function = (x, y) => BashTool.Execute(x).SplitBash()
            };
            dict["virsh-shutdown"] = new Command {
                InputType = typeof(string),
                OutputType = typeof(string),
                Arguments = new[] { "virsh shutdown $domain" },
                Function = (x, y) => BashTool.Execute(x).SplitBash()
            };
            dict["virsh-start"] = new Command {
                InputType = typeof(string),
                OutputType = typeof(string),
                Arguments = new[] { "virsh start $domain" },
                Function = (x, y) => BashTool.Execute(x).SplitBash()
            };
            dict["virsh-suspend"] = new Command {
                InputType = typeof(string),
                OutputType = typeof(string),
                Arguments = new[] { "virsh suspend $domain" },
                Function = (x, y) => BashTool.Execute(x).SplitBash()
            };
            dict["virsh-dompmsuspend"] = new Command {
                InputType = typeof(string),
                OutputType = typeof(string),
                Arguments = new[] { "virsh dompmsuspend $domain" },
                Function = (x, y) => BashTool.Execute(x).SplitBash()
            };
            dict["virsh-dompmwakeup"] = new Command {
                InputType = typeof(string),
                OutputType = typeof(string),
                Arguments = new[] { "virsh dompmwakeup $domain" },
                Function = (x, y) => BashTool.Execute(x).SplitBash()
            };
            #endregion

            #region [    Command - Storage    ]
            dict["zpool-mklabel"] = new Command {
                InputType = typeof(string),
                OutputType = typeof(string),
                Arguments = new[] { "parted /dev/$disk_device mklabel $zpool_label Yes" },
                Function = (x, y) => BashTool.Execute(x).SplitBash()
            };
            dict["zpool-create"] = new Command {
                InputType = typeof(string),
                OutputType = typeof(string),
                Arguments = new[] { "zpool create -f -o altroot=$pool_altroot -o ashift=12 -O casesensitivity=insensitive -O normalization=formD $pool_name $pool_type $disk_byid" },
                Function = (x, y) => BashTool.Execute(x).SplitBash()
            };
            dict["zpool-create-simple"] = new Command {
                InputType = typeof(string),
                OutputType = typeof(string),
                Arguments = new[] { "zpool create -f -o altroot=$pool_altroot $pool_name Storage01 $disk" },
                Function = (x, y) => BashTool.Execute(x).SplitBash()
            };
            dict["zfs-create"] = new Command {
                InputType = typeof(IEnumerable<string>),
                OutputType = typeof(string),
                Arguments = new[] {
                    "zfs create -o compression=lz4 -o atime=off $pool_name/$dataset_name",
                    "zfs set xattr=on $pool_name/$dataset_name",
                    "zfs set acltype=posixacl $pool_name/$dataset_name",
                    "zfs set aclinherit=passthrough-x $pool_name/$dataset_name",
                    "zfs set mountpoint=/$pool_altroot/$pool_name/$dataset_name $pool_name/$dataset_name",
                    "zpool add $pool_name log $disk_log",
                    "zpool add $pool_name cache $disk_cache",
                    "zpool set cachefile=/etc/zfs/zpool.cache $pool_name",
                    "zpool set cachefile=/etc/zfs/zpool.cache $pool_name"
                },
                Function = (x, y) => BashTool.Execute(x).SplitBash()
            };
            dict["df"] = new Command {
                InputType = typeof(string),
                OutputType = typeof(IEnumerable<string>),
                Arguments = new[] { "df $directory" },
                Function = (x, y) => BashTool.Execute(x).SplitBash()
            };
            dict["df-h"] = new Command {
                InputType = typeof(string),
                OutputType = typeof(IEnumerable<string>),
                Arguments = new[] { "df -h $directory" },
                Function = (x, y) => BashTool.Execute(x).SplitBash()
            };
            dict["smartctl"] = new Command {
                InputType = typeof(string),
                OutputType = typeof(IEnumerable<string>),
                Arguments = new[] { "smartctl -a $dev" },
                Function = (x, y) => BashTool.Execute(x).SplitBash()
            };
            #endregion

            #region [    Command - Assets    ]
            dict["wol"] = new Command {
                InputType = typeof(string),
                OutputType = typeof(string),
                Arguments = new[] { "wol $mac" },
                Function = (x, y) => BashTool.Execute(x, false).SplitBash()
            };
            #endregion
            return dict;
        }
    }
}
