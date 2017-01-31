using System.Collections.Generic;
using System.IO;
using System.Linq;
using antdlib.common;

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
                Arguments = new[] { "$obj" },
                Function = (x, y) => {
                    var list = new List<string> {
                        $"element 1: {x.FirstOrDefault()}",
                        $"element 2: {x.FirstOrDefault()}",
                        $"element 3: {x.FirstOrDefault()}",
                    };
                    return list;
                }
            };

            dict["test-sub-list"] = new Command {
                Arguments = new[] { "$obj", "prova $obj", "$value is another value" },
                Function = (x, y) => {
                    var list = new List<string> {
                        $"element 1: {x.FirstOrDefault()}",
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
                Arguments = new[] { "$custom" },
                Function = (x, y) => BashTool.Execute(x).SplitBash()
            };
            dict["link-s"] = new Command {
                Arguments = new[] { "ln -s $link $file" },
                Function = (x, y) => BashTool.Execute(x).SplitBash()
            };
            dict["modprobe"] = new Command {
                Arguments = new[] { "modprobe $package" },
                Function = (x, y) => BashTool.Execute(x).SplitBash()
            };
            dict["mono-antdsh-update"] = new Command {
                Arguments = new[] { "mono /framework/antdsh/antdsh.exe update $context" },
                Function = (x, y) => BashTool.Execute(x).SplitBash()
            };
            dict["mono-antdsh-update-check"] = new Command {
                Arguments = new[] { "mono /framework/antdsh/antdsh.exe update check" },
                Function = (x, y) => BashTool.Execute(x).SplitBash()
            };
            dict["journactl-service"] = new Command {
                Arguments = new[] { "journalctl --no-pager -u $service" },
                Function = (x, y) => BashTool.Execute(x).SplitBash()
            };
            dict["rmmod"] = new Command {
                Arguments = new[] { "rmmod $modules" },
                Function = (x, y) => BashTool.Execute(x).SplitBash()
            };
            dict["set-vlan"] = new Command {
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
                Arguments = new[] { "sysctl -p" },
                Function = (x, y) => BashTool.Execute(x).SplitBash()
            };
            dict["systemd-machine-id-setup"] = new Command {
                Arguments = new[] { "systemd-machine-id-setup" },
                Function = (x, y) => BashTool.Execute(x).SplitBash()
            };
            dict["mkpasswd"] = new Command {
                Arguments = new[] { "mkpasswd -m sha-512 $password" },
                Function = (x, y) => BashTool.Execute(x).SplitBash()
            };
            dict["ssh-keyscan-getkeys"] = new Command {
                Arguments = new[] { "ssh-keyscan -H $ip >> ~/.ssh/known_hosts" },
                Function = (x, y) => BashTool.Execute(x).SplitBash()
            };
            dict["dmidecode"] = new Command {
                Arguments = new[] { "dmidecode" },
                Function = (x, y) => BashTool.Execute(x).SplitBash()
            };
            #endregion

            #region [    Command - Bond    ]
            dict["bond-add-if"] = new Command {
                Arguments = new[] { "ifenslave $bond $net_if" },
                Function = (x, y) => BashTool.Execute(x, false).SplitBash()
            };
            dict["bond-del-if"] = new Command {
                Arguments = new[] { "ifenslave -d $bond $net_if" },
                Function = (x, y) => BashTool.Execute(x, false).SplitBash()
            };
            dict["bond-set"] = new Command {
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
                Arguments = new[] { "brctl addbr $bridge" },
                Function = (x, y) => BashTool.Execute(x, false).SplitBash()
            };
            dict["brctl-add-if"] = new Command {
                Arguments = new[] { "brctl addif $bridge $net_if" },
                Function = (x, y) => BashTool.Execute(x, false).SplitBash()
            };
            dict["brctl-del"] = new Command {
                Arguments = new[] { "brctl delbr $bridge" },
                Function = (x, y) => BashTool.Execute(x, false).SplitBash()
            };
            dict["brctl-del-if"] = new Command {
                Arguments = new[] { "brctl delif $bridge $net_if" },
                Function = (x, y) => BashTool.Execute(x, false).SplitBash()
            };
            dict["brctl-set-pathcost"] = new Command {
                Arguments = new[] { "brctl setpathcost $bridge $path $cost set path cost" },
                Function = (x, y) => BashTool.Execute(x, false).SplitBash()
            };
            dict["brctl-set-portprio"] = new Command {
                Arguments = new[] { "brctl setportprio $bridge $port $priority set port priority" },
                Function = (x, y) => BashTool.Execute(x, false).SplitBash()
            };
            dict["brctl-show-br"] = new Command {
                Arguments = new[] { "brctl show $bridge" },
                Grep = "$bridge",
                Function = (x, y) => BashTool.Execute(x).Grep(y)
            };
            dict["brctl-show-brid"] = new Command {
                Arguments = new[] { "brctl show $bridge" },
                Grep = "$bridge",
                Function = (x, y) => BashTool.Execute(x).Grep(y).Print(2, " ")
            };
            dict["brctl-show-brif"] = new Command {
                Arguments = new[] { "brctl show $bridge" },
                Grep = "$bridge",
                Function = (x, y) => BashTool.Execute(x).Grep(y).Print(4, " ")
            };
            dict["brctl-show-brstpstatus"] = new Command {
                Arguments = new[] { "brctl show $bridge" },
                Grep = "$bridge",
                Function = (x, y) => BashTool.Execute(x).Grep(y).Print(3, " ")
            };
            dict["brctl-show-macs"] = new Command {
                Arguments = new[] { "brctl showmacs $bridge" },
                Function = (x, y) => BashTool.Execute(x).Grep(y)
            };
            dict["brctl-show-stp"] = new Command {
                Arguments = new[] { "brctl showstp $bridge" },
                Function = (x, y) => BashTool.Execute(x).Grep(y)
            };
            dict["brctl-stp-off"] = new Command {
                Arguments = new[] { "brctl stp $bridge off" },
                Function = (x, y) => BashTool.Execute(x).Grep(y)
            };
            dict["brctl-stp-on"] = new Command {
                Arguments = new[] { "brctl stp $bridge on" },
                Function = (x, y) => BashTool.Execute(x).Grep(y)
            };
            #endregion

            #region [    Command - Cat    ]
            dict["cat"] = new Command {
                Arguments = new[] { "$file" },
                Function = (x, y) => ReadTool.FileLines(x.FirstOrDefault())
            };
            dict["cat-etc-gentoorel"] = new Command {
                Arguments = new[] { "/etc/gentoo-release" },
                Function = (x, y) => ReadTool.FileLines(x.FirstOrDefault()).Where(_ => !string.IsNullOrEmpty(_))
            };
            dict["cat-etc-hostname"] = new Command {
                Arguments = new[] { "/etc/hostname" },
                Function = (x, y) => ReadTool.FileLines(x.FirstOrDefault()).Where(_ => !string.IsNullOrEmpty(_))
            };
            dict["cat-etc-hosts"] = new Command {
                Arguments = new[] { "/etc/hosts" },
                Function = (x, y) => ReadTool.FileLines(x.FirstOrDefault()).Where(_ => !string.IsNullOrEmpty(_))
            };
            dict["cat-etc-lsbrel"] = new Command {
                Arguments = new[] { "/etc/lsb-release" },
                Function = (x, y) => ReadTool.FileLines(x.FirstOrDefault()).Where(_ => !string.IsNullOrEmpty(_))
            };
            dict["cat-etc-osrel"] = new Command {
                Arguments = new[] { "/etc/os-release" },
                Function = (x, y) => ReadTool.FileLines(x.FirstOrDefault()).Where(_ => !string.IsNullOrEmpty(_))
            };
            dict["cat-etc-resolv"] = new Command {
                Arguments = new[] { "/etc/resolv.conf" },
                Function = (x, y) => ReadTool.FileLines(x.FirstOrDefault()).Where(_ => !string.IsNullOrEmpty(_)).GrepIgnore("#")
            };
            dict["cat-etc-nsswitch"] = new Command {
                Arguments = new[] { "/etc/nsswitch.conf" },
                Function = (x, y) => ReadTool.FileLines(x.FirstOrDefault()).Where(_ => !string.IsNullOrEmpty(_)).GrepIgnore("#")
            };
            dict["cat-etc-networks"] = new Command {
                Arguments = new[] { "/etc/networks" },
                Function = (x, y) => ReadTool.FileLines(x.FirstOrDefault()).Where(_ => !string.IsNullOrEmpty(_)).GrepIgnore("#")
            };
            dict["cat-etc-ntp"] = new Command {
                Arguments = new[] { "/etc/ntp.conf" },
                Function = (x, y) => ReadTool.FileLines(x.FirstOrDefault()).Where(_ => !string.IsNullOrEmpty(_)).GrepIgnore("#")
            };
            #endregion

            #region [    Command - Dhclient    ]
            dict["dhclient-killall"] = new Command {
                Arguments = new[] { "killall dhclient" },
                Function = (x, y) => BashTool.Execute(x).SplitBash()
            };
            dict["dhclient4"] = new Command {
                Arguments = new[] { "dhclient $net_if" },
                Function = (x, y) => BashTool.Execute(x).SplitBash()
            };
            dict["dhclient6"] = new Command {
                Arguments = new[] { "dhclient -6 $net_if" },
                Function = (x, y) => BashTool.Execute(x).SplitBash()
            };
            #endregion

            #region [    Command - Nftables    ]
            dict["nft-f"] = new Command {
                Arguments = new[] { "nft-f $file" },
                Function = (x, y) => BashTool.Execute(x).SplitBash()
            };
            dict["nft-tables"] = new Command {
                Arguments = new[] { "nft list tables" },
                Function = (x, y) => BashTool.Execute(x).SplitBash()
            };
            dict["nft-table"] = new Command {
                Arguments = new[] { "nft list table $type $table" },
                Function = (x, y) => BashTool.Execute(x).SplitBash()
            };
            dict["nft-set"] = new Command {
                Arguments = new[] { "nft list set $type $table $set" },
                Function = (x, y) => BashTool.Execute(x).SplitBash()
            };
            dict["nft-chain"] = new Command {
                Arguments = new[] { "nft list chain $type $table $chain" },
                Function = (x, y) => BashTool.Execute(x).SplitBash()
            };
            #endregion

            #region [    Command - Echo (write/append)    ]
            dict["echo-write"] = new Command {
                Arguments = new[] { "$file" },
                Grep = "$value",
                Function = (x, y) => {
                    if(File.Exists(x.FirstOrDefault())) {
                        File.Delete(x.FirstOrDefault());
                    }
                    WriteTool.WriteFile(x.FirstOrDefault(), y);
                    return new List<string>();
                }
            };
            dict["echo-write-all"] = new Command {
                Arguments = new[] { "$file" },
                Grep = "$value",
                Function = (x, y) => {
                    if(!System.IO.File.ReadAllText(y).Contains(x.FirstOrDefault())) {
                        WriteTool.WriteFileLines(x.FirstOrDefault(), y.SplitToList("\n"));
                    }
                    return new List<string>();
                }
            };
            dict["echo-append"] = new Command {
                Arguments = new[] { "$file" },
                Grep = "$value",
                Function = (x, y) => WriteTool.AppendFile(x.FirstOrDefault(), y)
            };
            dict["echo-append-rm"] = new Command {
                Arguments = new[] { "$file" },
                Grep = "$value",
                Function = (x, y) => {
                    if(!System.IO.File.ReadAllText(y).Contains(x.FirstOrDefault())) {
                        WriteTool.AppendFile(x.FirstOrDefault(), y);
                    }
                    return new List<string>();
                }
            };
            #endregion

            #region [    Command - Fdisk    ]
            dict["fdisk-print"] = new Command {
                Arguments = new[] { "echo -e \"p\" | fdisk $disk_device" },
                Function = (x, y) => BashTool.Execute(x).SplitBash()
            };
            dict["fdisk-set-partition"] = new Command {
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
                Arguments = new[] { "hostnamectl" },
                Function = (x, y) => BashTool.Execute(x).SplitBash()
            };
            dict["hostnamectl-get-arch"] = new Command {
                Arguments = new[] { "hostnamectl" },
                Grep = "Architecture: ",
                Function = (x, y) => BashTool.Execute(x).Grep(y).FirstOrDefault().Print(2, ':').SplitBash()
            };
            dict["hostnamectl-get-bootid"] = new Command {
                Arguments = new[] { "hostnamectl" },
                Grep = "Boot ID: ",
                Function = (x, y) => BashTool.Execute(x).Grep(y).FirstOrDefault().Print(2, ':').SplitBash()
            };
            dict["hostnamectl-get-chassis"] = new Command {
                Arguments = new[] { "hostnamectl" },
                Grep = "Chassis: ",
                Function = (x, y) => BashTool.Execute(x).Grep(y).FirstOrDefault().Print(2, ':').SplitBash()
            };
            dict["hostnamectl-get-deployment"] = new Command {
                Arguments = new[] { "hostnamectl" },
                Grep = "Deployment: ",
                Function = (x, y) => BashTool.Execute(x).Grep(y).FirstOrDefault().Print(2, ':').SplitBash()
            };
            dict["hostnamectl-get-hostname"] = new Command {
                Arguments = new[] { "hostnamectl" },
                Grep = "Transient hostname: ",
                Function = (x, y) => BashTool.Execute(x).Grep(y).FirstOrDefault().Print(2, ':').SplitBash()
            };
            dict["hostnamectl-get-iconname"] = new Command {
                Arguments = new[] { "hostnamectl" },
                Grep = "Icon name: ",
                Function = (x, y) => BashTool.Execute(x).Grep(y).FirstOrDefault().Print(2, ':').SplitBash()
            };
            dict["hostnamectl-get-kernel"] = new Command {
                Arguments = new[] { "hostnamectl" },
                Grep = "Kernel: ",
                Function = (x, y) => BashTool.Execute(x).Grep(y).FirstOrDefault().Print(2, ':').SplitBash()
            };
            dict["hostnamectl-get-location"] = new Command {
                Arguments = new[] { "hostnamectl" },
                Grep = "Location: ",
                Function = (x, y) => BashTool.Execute(x).Grep(y).FirstOrDefault().Print(2, ':').SplitBash()
            };
            dict["hostnamectl-get-machineid"] = new Command {
                Arguments = new[] { "hostnamectl" },
                Grep = "Machine ID: ",
                Function = (x, y) => BashTool.Execute(x).Grep(y).FirstOrDefault().Print(2, ':').SplitBash()
            };
            dict["hostnamectl-get-os"] = new Command {
                Arguments = new[] { "hostnamectl" },
                Grep = "Operating System: ",
                Function = (x, y) => BashTool.Execute(x).Grep(y).FirstOrDefault().Print(2, ':').SplitBash()
            };
            dict["hostnamectl-get-virtualization"] = new Command {
                Arguments = new[] { "hostnamectl" },
                Grep = "Virtualization: ",
                Function = (x, y) => BashTool.Execute(x).Grep(y).FirstOrDefault().Print(2, ':').SplitBash()
            };
            dict["set-chassis"] = new Command {
                Arguments = new[] { "hostnamectl set-chassis $host_chassis" },
                Function = (x, y) => BashTool.Execute(x).SplitBash()
            };
            dict["set-deployment"] = new Command {
                Arguments = new[] { "hostnamectl set-deployment $host_deployment" },
                Function = (x, y) => BashTool.Execute(x).SplitBash()
            };
            dict["set-hostname"] = new Command {
                Arguments = new[] { "hostnamectl set-hostname \"$host_name\" --pretty --static --transient" },
                Function = (x, y) => BashTool.Execute(x).SplitBash()
            };
            dict["set-location"] = new Command {
                Arguments = new[] { "hostnamectl set-location \"$host_location\"" },
                Function = (x, y) => BashTool.Execute(x).SplitBash()
            };
            #endregion

            #region [    Command - Network    ]
            dict["nmap-ip"] = new Command {
                Arguments = new[] { "nmap $ip" },
                Function = (x, y) => BashTool.Execute(x).SplitBash()
            };
            dict["nmap-ip-fast"] = new Command {
                Arguments = new[] { "nmap -F $ip" },
                Function = (x, y) => BashTool.Execute(x).SplitBash()
            };
            dict["nmap-snmp-interfaces"] = new Command {
                Arguments = new[] { "nmap -sU -p 161 -T4 -d -v -n -Pn --script snmp-interfaces $ip" },
                Function = (x, y) => BashTool.Execute(x).SplitBash()
            };
            dict["nmap-ip-sp"] = new Command {
                Arguments = new[] { "nmap -sP $subnet -oG -" },
                Function = (x, y) => BashTool.Execute(x).SplitBash()
            };
            dict["ping-c"] = new Command {
                Arguments = new[] { "ping -c3 -w10 $ip" },
                Function = (x, y) => BashTool.Execute(x).SplitBash()
            };
            dict["arp"] = new Command {
                Arguments = new[] { "arp $ip" },
                Function = (x, y) => BashTool.Execute(x).SplitBash()
            };
            dict["ifconfig"] = new Command {
                Arguments = new[] { "ifconfig" },
                Function = (x, y) => BashTool.Execute(x).SplitBash()
            };
            dict["ifconfig-if"] = new Command {
                Arguments = new[] { "ifconfig $if" },
                Function = (x, y) => BashTool.Execute(x).SplitBash()
            };
            dict["net-carrier"] = new Command {
                Arguments = new[] { "/sys/class/net/$if/carrier" },
                Function = (x, y) => ReadTool.FileLines(x.FirstOrDefault())
            };
            #endregion

            #region [    Command - Ipv4    ]
            dict["set-network-interface"] = new Command {
                Arguments = new[] {
                    "ip link set dev $interface_name down",
                    "ip link set dev $interface_name up",
                    "ip link set $interface_name txqueuelen 10000",
                    "ip link set dev $interface_name up"
                },
                Function = (x, y) => BashTool.Execute(x).SplitBash()
            };
            dict["ip4-add-addr"] = new Command {
                Arguments = new[] { "ip addr add $address/$range dev $net_if" },
                Function = (x, y) => BashTool.Execute(x).SplitBash()
            };
            dict["ip4-add-addr-broadcast"] = new Command {
                Arguments = new[] { "ip addr add $address/$range broadcast $broadcast dev $net_if" },
                Function = (x, y) => BashTool.Execute(x).SplitBash()
            };
            dict["ip4-add-multipath-route"] = new Command {
                Arguments = new[] { "ip route add default scope global nexthop dev $net1 nexthop dev $net2" },
                Function = (x, y) => BashTool.Execute(x).SplitBash()
            };
            dict["ip4-add-nat"] = new Command {
                Arguments = new[] { "ip route add nat $ip_address via $ip_via_address" },
                Function = (x, y) => BashTool.Execute(x).SplitBash()
            };
            dict["ip4-add-route"] = new Command {
                Arguments = new[] { "ip route add $ip_address via $gateway dev $net_if" },
                Function = (x, y) => BashTool.Execute(x).SplitBash()
            };
            dict["ip4-add-tunnel-point-to-point"] = new Command {
                Arguments = new[] { "ip tunnel add $net_if mode sit ttl $ttl remote $tunnel local $local_address" },
                Function = (x, y) => BashTool.Execute(x).SplitBash()
            };
            dict["ip4-del-addr"] = new Command {
                Arguments = new[] { "ip addr del $address/$range dev $net_if" },
                Function = (x, y) => BashTool.Execute(x).SplitBash()
            };
            dict["ip4-del-addr-broadcast"] = new Command {
                Arguments = new[] { "ip addr del $address/$range broadcast $broadcast dev $net_if" },
                Function = (x, y) => BashTool.Execute(x).SplitBash()
            };
            dict["ip4-del-route"] = new Command {
                Arguments = new[] { "ip route del $ip_address via $gateway dev $net_if" },
                Function = (x, y) => BashTool.Execute(x).SplitBash()
            };
            dict["ip4-del-tunnel-point-to-point"] = new Command {
                Arguments = new[] { "ip tunnel del $net_if" },
                Function = (x, y) => BashTool.Execute(x).SplitBash()
            };
            dict["ip4-disable-if"] = new Command {
                Arguments = new[] { "ip link set $net_if down" },
                Function = (x, y) => BashTool.Execute(x).SplitBash()
            };
            dict["ip4-enable-if"] = new Command {
                Arguments = new[] { "ip link set $net_if up" },
                Function = (x, y) => BashTool.Execute(x).SplitBash()
            };
            dict["ip4-flush-configuration"] = new Command {
                Arguments = new[] { "ip addr flush dev $net_if" },
                Function = (x, y) => BashTool.Execute(x).SplitBash()
            };
            dict["ip4-get-if-addr"] = new Command {
                Arguments = new[] { "ip addr show $net_if" },
                Function = (x, y) => BashTool.Execute(x).Grep("inet .").FirstOrDefault().Print(2, " ").SplitBash()
            };
            dict["ip4-get-if-brd"] = new Command {
                Arguments = new[] { "ip addr show $net_if" },
                Function = (x, y) => BashTool.Execute(x).Grep("inet .").FirstOrDefault().Print(4, " ").SplitBash()
            };
            dict["ip4-get-if-macaddress"] = new Command {
                Arguments = new[] { "/sys/class/net/$net_if/address" },
                Function = (x, y) => ReadTool.FileLines(x.FirstOrDefault())
            };
            dict["ip4-get-if-mtu"] = new Command {
                Arguments = new[] { "/sys/class/net/$net_if/mtu" },
                Function = (x, y) => ReadTool.FileLines(x.FirstOrDefault())
            };
            dict["ip4-if-isdown"] = new Command {
                Arguments = new[] { "ip addr show $net_if" },
                Function = (x, y) => BashTool.Execute(x).Grep("state DOWN").FirstOrDefault().SplitBash()
            };
            dict["ip4-if-isup"] = new Command {
                Arguments = new[] { "ip addr show $net_if" },
                Function = (x, y) => BashTool.Execute(x).Grep("state UP").FirstOrDefault().SplitBash()
            };
            dict["ip4-set-macaddress"] = new Command {
                Arguments = new[] {
                    "ip link set $net_if down",
                    "ip link set dev $net_if address $address",
                    "ip link set $net_if up"
                },
                Function = (x, y) => BashTool.Execute(x).SplitBash()
            };
            dict["ip4-set-mtu"] = new Command {
                Arguments = new[] { "ip link set dev $net_if mtu $mtu" },
                Function = (x, y) => BashTool.Execute(x).SplitBash()
            };
            dict["ip4-set-txqueuelen"] = new Command {
                Arguments = new[] { "ip link set $net_if txqueuelen $txqueuelen" },
                Function = (x, y) => BashTool.Execute(x).SplitBash()
            };
            dict["ip4-show-if-addr"] = new Command {
                Arguments = new[] { "ip addr show $net_if" },
                Function = (x, y) => BashTool.Execute(x).SplitBash()
            };
            dict["ip4-show-if-link"] = new Command {
                Arguments = new[] { "ip -s link ls $net_if" },
                Function = (x, y) => BashTool.Execute(x).SplitBash()
            };
            dict["ip4-show-routes"] = new Command {
                Arguments = new[] { "ip route show $net_if" },
                Function = (x, y) => BashTool.Execute(x).SplitBash()
            };
            dict["ip4-show-tunnels"] = new Command {
                Arguments = new[] { "ip tunnel show $net_if" },
                Function = (x, y) => BashTool.Execute(x).SplitBash()
            };
            dict["ip4-show-updown"] = new Command {
                Arguments = new[] { "ip link show $net_if" },
                //todo fare multigrep  | grep -ho \' UP \\| DOWN \'
                Function = (x, y) => BashTool.Execute(x).Grep("UP").FirstOrDefault().SplitBash()
            };
            #endregion

            #region [    Command - Ipv6    ]
            dict["ip6-add-addr"] = new Command {
                Arguments = new[] { "ip -6 addr add $address/$range dev $net_if" },
                Function = (x, y) => BashTool.Execute(x).SplitBash()
            };
            dict["ip6-add-neigh"] = new Command {
                Arguments = new[] { "ip -6 neigh add $ip_address lladdr $ip_lay_address dev $net_if" },
                Function = (x, y) => BashTool.Execute(x).SplitBash()
            };
            dict["ip6-add-new-address"] = new Command {
                Arguments = new[] { "ip -6 addr add $ip_address dev $net_if" },
                Function = (x, y) => BashTool.Execute(x).SplitBash()
            };
            dict["ip6-add-route"] = new Command {
                Arguments = new[] { "ip -6 route add $ip_address via $gateway" },
                Function = (x, y) => BashTool.Execute(x).SplitBash()
            };
            dict["ip6-add-route-dev"] = new Command {
                Arguments = new[] { "ip -6 addr del $address/$range dev $net_if" },
                Function = (x, y) => BashTool.Execute(x).SplitBash()
            };
            dict["ip6-del-addr"] = new Command {
                Arguments = new[] { "ip -6 addr del $address/$range dev $net_if" },
                Function = (x, y) => BashTool.Execute(x).SplitBash()
            };
            dict["ip6-del-address"] = new Command {
                Arguments = new[] { "ip -6 addr del $ip_address dev $net_if" },
                Function = (x, y) => BashTool.Execute(x).SplitBash()
            };
            dict["ip6-del-neigh"] = new Command {
                Arguments = new[] { "ip -6 neigh del $ip_address lladdr $ip_lay_address dev $net_if" },
                Function = (x, y) => BashTool.Execute(x).SplitBash()
            };
            dict["ip6-del-route"] = new Command {
                Arguments = new[] { "ip -6 route del $ip_address via $gateway" },
                Function = (x, y) => BashTool.Execute(x).SplitBash()
            };
            dict["ip6-del-route-dev"] = new Command {
                Arguments = new[] { "ip -6 route del $gateway dev $net_if" },
                Function = (x, y) => BashTool.Execute(x).SplitBash()
            };
            dict["ip6-flush-configuration"] = new Command {
                Arguments = new[] { "ip -6 addr flush dynamic" },
                Function = (x, y) => BashTool.Execute(x).SplitBash()
            };
            dict["ip6-show-if-link"] = new Command {
                Arguments = new[] { "ip -6 link show $net_if" },
                Function = (x, y) => BashTool.Execute(x).SplitBash()
            };
            dict["ip6-show-if-stats"] = new Command {
                Arguments = new[] { "ip -6 -s link ls $net_if" },
                Function = (x, y) => BashTool.Execute(x).SplitBash()
            };
            dict["ip6-show-neigh"] = new Command {
                Arguments = new[] { "ip -6 neigh show dev $net_if" },
                Function = (x, y) => BashTool.Execute(x).SplitBash()
            };
            dict["ip6-show-routes"] = new Command {
                Arguments = new[] { "ip -6 route show $net_if" },
                Function = (x, y) => BashTool.Execute(x).SplitBash()
            };
            dict["ip6-show-tunnels"] = new Command {
                Arguments = new[] { "ip -6 tunnel show $net_if" },
                Function = (x, y) => BashTool.Execute(x).SplitBash()
            };
            #endregion

            #region [    Command - Time & Date    ]
            dict["ntpdate"] = new Command {
                Arguments = new[] { "ntpdate $server" },
                Function = (x, y) => BashTool.Execute(x).SplitBash()
            };
            dict["set-ntpdate"] = new Command {
                Arguments = new[] {
                    "ntpdate $date_server",
                    "timedatectl --no-pager --no-ask-password --adjust-system-clock set-ntp yes"
                },
                Function = (x, y) => BashTool.Execute(x).SplitBash()
            };
            dict["set-timezone"] = new Command {
                Arguments = new[] { "timedatectl --no-pager --no-ask-password --adjust-system-clock set-timezone $host_timezone" },
                Function = (x, y) => BashTool.Execute(x).SplitBash()
            };
            dict["sync-clock"] = new Command {
                Arguments = new[] {
                    "hwclock --systohc",
                    "hwclock --hctosys"
                },
                Function = (x, y) => BashTool.Execute(x).SplitBash()
            };
            dict["timedatectl"] = new Command {
                Arguments = new[] { "timedatectl" },
                Function = (x, y) => BashTool.Execute(x).SplitBash()
            };
            dict["timedatectl-get-localtime"] = new Command {
                Arguments = new[] { "timedatectl" },
                Grep = "Local time: ",
                Function = (x, y) => BashTool.Execute(x).Grep(y).FirstOrDefault().Print(2, ':').SplitBash()
            };
            dict["timedatectl-get-nettimeon"] = new Command {
                Arguments = new[] { "timedatectl" },
                Grep = "Network time on: ",
                Function = (x, y) => BashTool.Execute(x).Grep(y).FirstOrDefault().Print(2, ':').SplitBash()
            };
            dict["timedatectl-get-ntpsync"] = new Command {
                Arguments = new[] { "timedatectl" },
                Grep = "NTP synchronized: ",
                Function = (x, y) => BashTool.Execute(x).Grep(y).FirstOrDefault().Print(2, ':').SplitBash()
            };
            dict["timedatectl-get-rtcintz"] = new Command {
                Arguments = new[] { "timedatectl" },
                Grep = "RTC in local TZ: ",
                Function = (x, y) => BashTool.Execute(x).Grep(y).FirstOrDefault().Print(2, ':').SplitBash()
            };
            dict["timedatectl-get-rtctime"] = new Command {
                Arguments = new[] { "timedatectl" },
                Grep = "RTC time: ",
                Function = (x, y) => BashTool.Execute(x).Grep(y).FirstOrDefault().Print(2, ':').SplitBash()
            };
            dict["timedatectl-get-timezone"] = new Command {
                Arguments = new[] { "timedatectl" },
                Grep = "Time zone: ",
                Function = (x, y) => BashTool.Execute(x).Grep(y).FirstOrDefault().Print(2, ':').SplitBash()
            };
            dict["timedatectl-get-univtime"] = new Command {
                Arguments = new[] { "timedatectl" },
                Grep = "Universal time: ",
                Function = (x, y) => BashTool.Execute(x).Grep(y).FirstOrDefault().Print(2, ':').SplitBash()
            };
            #endregion

            #region [    Command - Rsync    ]
            dict["rsync"] = new Command {
                Arguments = new[] { "rsync -aHA $source $destination" },
                Function = (x, y) => BashTool.Execute(x).SplitBash()
            };
            dict["rsync-delete-after"] = new Command {
                Arguments = new[] { "rsync -aHA --delete-after $source $destination" },
                Function = (x, y) => BashTool.Execute(x).SplitBash()
            };
            dict["rsync-delete-during"] = new Command {
                Arguments = new[] { "rsync -aHA --delete-during $source $destination" },
                Function = (x, y) => BashTool.Execute(x).SplitBash()
            };
            #endregion

            #region [    Command - Systemctl    ]
            dict["systemctl-daemonreload"] = new Command {
                Arguments = new[] { "systemctl daemon-reload" },
                Function = (x, y) => BashTool.Execute(x).SplitBash()
            };
            dict["systemctl-start"] = new Command {
                Arguments = new[] { "systemctl start $service" },
                Function = (x, y) => BashTool.Execute(x).SplitBash()
            };
            dict["systemctl-restart"] = new Command {
                Arguments = new[] { "systemctl restart $service" },
                Function = (x, y) => BashTool.Execute(x).SplitBash()
            };
            dict["systemctl-stop"] = new Command {
                Arguments = new[] { "systemctl stop $service" },
                Function = (x, y) => BashTool.Execute(x).SplitBash()
            };
            dict["systemctl-enable"] = new Command {
                Arguments = new[] { "systemctl enable $service" },
                Function = (x, y) => BashTool.Execute(x).SplitBash()
            };
            dict["systemctl-disable"] = new Command {
                Arguments = new[] { "systemctl disable $service" },
                Function = (x, y) => BashTool.Execute(x).SplitBash()
            };
            #endregion

            #region [    Command - Virsh    ]
            dict["virsh-destroy"] = new Command {
                Arguments = new[] { "virsh destroy $domain" },
                Function = (x, y) => BashTool.Execute(x).SplitBash()
            };
            dict["virsh-reboot"] = new Command {
                Arguments = new[] { "virsh reboot $domain" },
                Function = (x, y) => BashTool.Execute(x).SplitBash()
            };
            dict["virsh-reset"] = new Command {
                Arguments = new[] { "virsh reset $domain" },
                Function = (x, y) => BashTool.Execute(x).SplitBash()
            };
            dict["virsh-restore"] = new Command {
                Arguments = new[] { "virsh restore $domain" },
                Function = (x, y) => BashTool.Execute(x).SplitBash()
            };
            dict["virsh-resume"] = new Command {
                Arguments = new[] { "virsh resume $domain" },
                Function = (x, y) => BashTool.Execute(x).SplitBash()
            };
            dict["virsh-shutdown"] = new Command {
                Arguments = new[] { "virsh shutdown $domain" },
                Function = (x, y) => BashTool.Execute(x).SplitBash()
            };
            dict["virsh-start"] = new Command {
                Arguments = new[] { "virsh start $domain" },
                Function = (x, y) => BashTool.Execute(x).SplitBash()
            };
            dict["virsh-suspend"] = new Command {
                Arguments = new[] { "virsh suspend $domain" },
                Function = (x, y) => BashTool.Execute(x).SplitBash()
            };
            dict["virsh-dompmsuspend"] = new Command {
                Arguments = new[] { "virsh dompmsuspend $domain" },
                Function = (x, y) => BashTool.Execute(x).SplitBash()
            };
            dict["virsh-dompmwakeup"] = new Command {
                Arguments = new[] { "virsh dompmwakeup $domain" },
                Function = (x, y) => BashTool.Execute(x).SplitBash()
            };
            #endregion

            #region [    Command - Storage    ]
            dict["zpool-mklabel"] = new Command {
                Arguments = new[] { "parted /dev/$disk_device mklabel $zpool_label Yes" },
                Function = (x, y) => BashTool.Execute(x).SplitBash()
            };
            dict["zpool-create"] = new Command {
                Arguments = new[] { "zpool create -f " +
                                        "-o altroot=$pool_altroot " +
                                        "-o ashift=12 " +
                                        "-O casesensitivity=insensitive " +
                                        "-O normalization=formD " +
                                        "$pool_name $pool_type $disk" },
                Function = (x, y) => BashTool.Execute(x).SplitBash()
            };
            dict["zpool-create-2"] = new Command {
                Arguments = new[] { "zpool create -f " +
                                        "-o altroot=$pool_altroot " +
                                        "-o ashift=12 " +
                                        "-O casesensitivity=insensitive " +
                                        "-O normalization=formD " +
                                        "$pool_name $disk" },
                Function = (x, y) => BashTool.Execute(x).SplitBash()
            };
            dict["zpool-create-simple"] = new Command {
                Arguments = new[] { "zpool create -f -o altroot=$pool_altroot $pool_name Storage01 $disk" },
                Function = (x, y) => BashTool.Execute(x).SplitBash()
            };
            dict["zfs-create"] = new Command {
                Arguments = new[] {
                    "zfs create " +
                        "-o atime=off " +
                        "-o compression=lz4 " +
                        "-o checksum=fletcher4 "+
                        "-o dedup=off "+
                        "-o xattr=on " +
                        "-o acltype=posixacl " +
                        "-o aclinherit=passthrough-x " +
                        "-o mountpoint=/$pool_altroot/$pool_name/$dataset_name " +
                        "$pool_name/$dataset_name"
                },
                Function = (x, y) => BashTool.Execute(x).SplitBash()
            };
            dict["zfs-create-dedupverify"] = new Command {
                Arguments = new[] {
                    "zfs create " +
                        "-o atime=off " +
                        "-o compression=lz4 " +
                        "-o checksum=fletcher4 "+
                        "-o dedup=verify "+
                        "-o xattr=on " +
                        "-o acltype=posixacl " +
                        "-o aclinherit=passthrough-x " +
                        "-o mountpoint=/$pool_altroot/$pool_name/$dataset_name " +
                        "$pool_name/$dataset_name"
                },
                Function = (x, y) => BashTool.Execute(x).SplitBash()
            };
            dict["zfs-add-log-disk"] = new Command {
                Arguments = new[] { "zpool add $pool_name log $disk_log" },
                Function = (x, y) => BashTool.Execute(x).SplitBash()
            };
            dict["zfs-add-log-cache"] = new Command {
                Arguments = new[] { "zpool add $pool_name cache $disk_cache" },
                Function = (x, y) => BashTool.Execute(x).SplitBash()
            };
            dict["zfs-set-cachefile"] = new Command {
                Arguments = new[] { "zpool set cachefile=/etc/zfs/zpool.cache $pool_name" },
                Function = (x, y) => BashTool.Execute(x).SplitBash()
            };
            dict["df"] = new Command {
                Arguments = new[] { "df $directory" },
                Function = (x, y) => BashTool.Execute(x).SplitBash()
            };
            dict["df-h"] = new Command {
                Arguments = new[] { "df -h $directory" },
                Function = (x, y) => BashTool.Execute(x).SplitBash()
            };
            dict["smartctl"] = new Command {
                Arguments = new[] { "smartctl -a $dev" },
                Function = (x, y) => BashTool.Execute(x).SplitBash()
            };
            #endregion

            #region [    Command - Assets    ]
            dict["wol"] = new Command {
                Arguments = new[] { "wol $mac" },
                Function = (x, y) => BashTool.Execute(x, false).SplitBash()
            };
            #endregion

            #region [    Command - Users & Groups Management    ]
            dict["getent-passwd"] = new Command {
                Arguments = new[] { "getent passwd" },
                Function = (x, y) => BashTool.Execute(x, false).SplitBash()
            };
            #endregion
            return dict;
        }
    }
}
