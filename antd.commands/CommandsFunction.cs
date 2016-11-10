using System.Collections.Generic;
using System.Linq;
using antdlib.common.Tool;

namespace antd.commands {
    public class CommandsFunction {

        public static string RootFrameworkAntdShellScripts => "/framework/antd/ShellScript";
        private static readonly Bash BashTool = new Bash();
        private static readonly Read ReadTool = new Read();
        private static readonly Write WriteTool = new Write();

        public static Dictionary<string, ICommand> List => GetDict();

        private static Dictionary<string, ICommand> GetDict() {
            var dict = new Dictionary<string, ICommand>();

            #region [    Command - Misc    ]
            dict["anthilla"] = new Command<string, string> {
                Arguments = "$custom",
                Functions = (x, y) => BashTool.Execute(x)
            };
            dict["modprobe"] = new Command<string, string> {
                Arguments = "modprobe $package",
                Functions = (x, y) => BashTool.Execute(x)
            };
            dict["mono-antdsh-update"] = new Command<string, string> {
                Arguments = "mono /framework/antdsh/antdsh.exe update $context",
                Functions = (x, y) => BashTool.Execute(x)
            };
            dict["mono-antdsh-update-check"] = new Command<string, string> {
                Arguments = "mono /framework/antdsh/antdsh.exe update check",
                Functions = (x, y) => BashTool.Execute(x)
            };
            dict["rmmod"] = new Command<string, string> {
                Arguments = "rmmod $modules",
                Functions = (x, y) => BashTool.Execute(x)
            };
            dict["set-vlan"] = new Command<IEnumerable<string>, string> {
                Arguments = new[] {
                    "ip link set $vlan_interface_name down",
                    "ip link del $vlan_interface_name",
                    "ip link add name $vlan_interface_name link $interface_name type vlan id $vlan_id",
                    "ip link set $vlan_interface_name txqueuelen 10000",
                    "ip link set $vlan_interface_name up"
                },
                Functions = (x, y) => BashTool.Execute(x)
            };
            dict["sysctl-p"] = new Command<string, string> {
                Arguments = "sysctl -p",
                Functions = (x, y) => BashTool.Execute(x)
            };
            dict["systemd-machine-id-setup"] = new Command<string, string> {
                Arguments = "systemd-machine-id-setup",
                Functions = (x, y) => BashTool.Execute(x)
            };
            #endregion

            #region [    Command - Bond    ]
            dict["bond-add-if"] = new Command<string, string> {
                Arguments = "ifenslave $bond $net_if",
                Functions = (x, y) => BashTool.Execute(x, false)
            };
            dict["bond-del-if"] = new Command<string, string> {
                Arguments = "ifenslave -d $bond $net_if",
                Functions = (x, y) => BashTool.Execute(x, false)
            };
            dict["bond-set"] = new Command<IEnumerable<string>, string> {
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
                Functions = (x, y) => BashTool.Execute(x, false)
            };
            dict["set-bond"] = new Command<IEnumerable<string>, string> {
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
                Functions = (x, y) => BashTool.Execute(x, false)
            };
            #endregion

            #region [    Command - Brctl    ]
            dict["brctl-add"] = new Command<string, string> {
                Arguments = "brctl addbr $bridge",
                Functions = (x, y) => BashTool.Execute(x, false)
            };
            dict["brctl-add-if"] = new Command<string, string> {
                Arguments = "brctl addif $bridge $net_if",
                Functions = (x, y) => BashTool.Execute(x, false)
            };
            dict["brctl-del"] = new Command<string, string> {
                Arguments = "brctl delbr $bridge",
                Functions = (x, y) => BashTool.Execute(x, false)
            };
            dict["brctl-del-if"] = new Command<string, string> {
                Arguments = "brctl delif $bridge $net_if",
                Functions = (x, y) => BashTool.Execute(x, false)
            };
            dict["brctl-set-pathcost"] = new Command<string, string> {
                Arguments = "brctl setpathcost $bridge $path $cost set path cost",
                Functions = (x, y) => BashTool.Execute(x, false)
            };
            dict["brctl-set-portprio"] = new Command<string, string> {
                Arguments = "brctl setportprio $bridge $port $priority set port priority",
                Functions = (x, y) => BashTool.Execute(x, false)
            };
            dict["brctl-show-br"] = new Command<string, IEnumerable<string>> {
                Arguments = "brctl show $bridge",
                Grep = "$bridge",
                Functions = (x, y) => BashTool.Execute(x).Grep(y)
            };
            dict["brctl-show-brid"] = new Command<string, IEnumerable<string>> {
                Arguments = "brctl show $bridge",
                Grep = "$bridge",
                Functions = (x, y) => BashTool.Execute(x).Grep(y).Print(2)
            };
            dict["brctl-show-brif"] = new Command<string, IEnumerable<string>> {
                Arguments = "brctl show $bridge",
                Grep = "$bridge",
                Functions = (x, y) => BashTool.Execute(x).Grep(y).Print(4)
            };
            dict["brctl-show-brstpstatus"] = new Command<string, IEnumerable<string>> {
                Arguments = "brctl show $bridge",
                Grep = "$bridge",
                Functions = (x, y) => BashTool.Execute(x).Grep(y).Print(3)
            };
            dict["brctl-show-macs"] = new Command<string, IEnumerable<string>> {
                Arguments = "brctl showmacs $bridge",
                Functions = (x, y) => BashTool.Execute(x).Grep(y)
            };
            dict["brctl-show-stp"] = new Command<string, IEnumerable<string>> {
                Arguments = "brctl showstp $bridge",
                Functions = (x, y) => BashTool.Execute(x).Grep(y)
            };
            dict["brctl-stp-off"] = new Command<string, IEnumerable<string>> {
                Arguments = "brctl stp $bridge off",
                Functions = (x, y) => BashTool.Execute(x).Grep(y)
            };
            dict["brctl-stp-on"] = new Command<string, IEnumerable<string>> {
                Arguments = "brctl stp $bridge on",
                Functions = (x, y) => BashTool.Execute(x).Grep(y)
            };
            #endregion

            #region [    Command - Cat    ]
            dict["cat"] = new Command<string, string> {
                Arguments = "$file",
                Functions = (x, y) => ReadTool.File(x)
            };
            dict["cat-etc-gentoorel"] = new Command<string, string> {
                Arguments = "/etc/gentoo-release",
                Functions = (x, y) => ReadTool.File(x)
            };
            dict["cat-etc-hostname"] = new Command<string, string> {
                Arguments = "/etc/hostname",
                Functions = (x, y) => ReadTool.File(x)
            };
            dict["cat-etc-hosts"] = new Command<string, string> {
                Arguments = "/etc/hosts",
                Functions = (x, y) => ReadTool.File(x)
            };
            dict["cat-etc-lsbrel"] = new Command<string, string> {
                Arguments = "/etc/lsb-release",
                Functions = (x, y) => ReadTool.File(x)
            };
            dict["cat-etc-osrel"] = new Command<string, string> {
                Arguments = "/etc/os-release",
                Functions = (x, y) => ReadTool.File(x)
            };
            dict["cat-etc-resolv"] = new Command<string, string> {
                Arguments = "/etc/resolv.conf",
                Functions = (x, y) => ReadTool.File(x)
            };
            #endregion

            #region [    Command - Dhclient    ]
            dict["dhclient-killall"] = new Command<string, string> {
                Arguments = "killall dhclient",
                Functions = (x, y) => BashTool.Execute(x)
            };
            dict["dhclient4"] = new Command<string, string> {
                Arguments = "dhclient $net_if",
                Functions = (x, y) => BashTool.Execute(x)
            };
            dict["dhclient6"] = new Command<string, string> {
                Arguments = "dhclient -6 $net_if",
                Functions = (x, y) => BashTool.Execute(x)
            };
            #endregion

            #region [    Command - Echo (write/append)    ]
            dict["echo-write"] = new Command<string, string> {
                Arguments = "$file",
                Grep = "$value",
                Functions = (x, y) => WriteTool.WriteFile(x, y)
            };
            dict["echo-append"] = new Command<string, string> {
                Arguments = "$file",
                Grep = "$value",
                Functions = (x, y) => WriteTool.AppendFile(x, y)
            };
            #endregion

            #region [    Command - Fdisk    ]
            dict["fdisk-print"] = new Command<string, string> {
                Arguments = "echo -e \"p\" | fdisk $disk_device",
                Functions = (x, y) => BashTool.Execute(x)
            };
            dict["fdisk-set-partition"] = new Command<IEnumerable<string>, string> {
                Arguments = new[] {
                    "echo -e \"n\\n $part_number\\n $part_first_sector\\n $part_size\\n w\\n\" | fdisk $disk_device",
                    "parted $disk_device $part_number 1 $name",
                    "parted $disk_device align-check opt $part_number"
                },
                Functions = (x, y) => BashTool.Execute(x)
            };
            #endregion

            #region [    Command - Hostnamectl    ]
            dict["hostnamectl-get-arch"] = new Command<string, string> {
                Arguments = "hostnamectl",
                Grep = "Architecture: ",
                Functions = (x, y) => BashTool.Execute(x).Grep(y).First().Print(2, ':')
            };
            dict["hostnamectl-get-bootid"] = new Command<string, string> {
                Arguments = "hostnamectl",
                Grep = "Boot ID: ",
                Functions = (x, y) => BashTool.Execute(x).Grep(y).First().Print(2, ':')
            };
            dict["hostnamectl-get-chassis"] = new Command<string, string> {
                Arguments = "hostnamectl",
                Grep = "Chassis: ",
                Functions = (x, y) => BashTool.Execute(x).Grep(y).First().Print(2, ':')
            };
            dict["hostnamectl-get-deployment"] = new Command<string, string> {
                Arguments = "hostnamectl",
                Grep = "Deployment: ",
                Functions = (x, y) => BashTool.Execute(x).Grep(y).First().Print(2, ':')
            };
            dict["hostnamectl-get-hostname"] = new Command<string, string> {
                Arguments = "hostnamectl",
                Grep = "Static hostname: ",
                Functions = (x, y) => BashTool.Execute(x).Grep(y).First().Print(2, ':')
            };
            dict["hostnamectl-get-iconname"] = new Command<string, string> {
                Arguments = "hostnamectl",
                Grep = "Icon name: ",
                Functions = (x, y) => BashTool.Execute(x).Grep(y).First().Print(2, ':')
            };
            dict["hostnamectl-get-kernel"] = new Command<string, string> {
                Arguments = "hostnamectl",
                Grep = "Kernel: ",
                Functions = (x, y) => BashTool.Execute(x).Grep(y).First().Print(2, ':')
            };
            dict["hostnamectl-get-location"] = new Command<string, string> {
                Arguments = "hostnamectl",
                Grep = "Location: ",
                Functions = (x, y) => BashTool.Execute(x).Grep(y).First().Print(2, ':')
            };
            dict["hostnamectl-get-machineid"] = new Command<string, string> {
                Arguments = "hostnamectl",
                Grep = "Machine ID: ",
                Functions = (x, y) => BashTool.Execute(x).Grep(y).First().Print(2, ':')
            };
            dict["hostnamectl-get-os"] = new Command<string, string> {
                Arguments = "hostnamectl",
                Grep = "Operating System: ",
                Functions = (x, y) => BashTool.Execute(x).Grep(y).First().Print(2, ':')
            };
            dict["hostnamectl-get-virtualization"] = new Command<string, string> {
                Arguments = "hostnamectl",
                Grep = "Virtualization: ",
                Functions = (x, y) => BashTool.Execute(x).Grep(y).First().Print(2, ':')
            };
            dict["set-chassis"] = new Command<string, string> {
                Arguments = "hostnamectl set-chassis $host_chassis",
                Functions = (x, y) => BashTool.Execute(x)
            };
            dict["set-deployment"] = new Command<string, string> {
                Arguments = "hostnamectl set-deployment $host_deployment",
                Functions = (x, y) => BashTool.Execute(x)
            };
            dict["set-hostname"] = new Command<string, string> {
                Arguments = "hostnamectl set-hostname $host_name",
                Functions = (x, y) => BashTool.Execute(x)
            };
            dict["set-location"] = new Command<string, string> {
                Arguments = "hostnamectl set-location \"$host_location\"",
                Functions = (x, y) => BashTool.Execute(x)
            };
            #endregion

            #region [    Command - Ipv4    ]
            dict["set-network-interface"] = new Command<IEnumerable<string>, string> {
                Arguments = new[] {
                    "ip link set dev $interface_name down",
                    "ip link set dev $interface_name up",
                    "ip link set $interface_name txqueuelen 10000",
                    "ip link set dev $interface_name up"
                },
                Functions = (x, y) => BashTool.Execute(x)
            };
            dict["ip4-add-addr"] = new Command<string, string> {
                Arguments = "ip addr add $address/$range dev $net_if",
                Functions = (x, y) => BashTool.Execute(x)
            };
            dict["ip4-add-addr-broadcast"] = new Command<string, string> {
                Arguments = "ip addr add $address/$range broadcast $broadcast dev $net_if",
                Functions = (x, y) => BashTool.Execute(x)
            };
            dict["ip4-add-multipath-route"] = new Command<string, string> {
                Arguments = "ip route add default scope global nexthop dev $net1 nexthop dev $net2",
                Functions = (x, y) => BashTool.Execute(x)
            };
            dict["ip4-add-nat"] = new Command<string, string> {
                Arguments = "ip route add nat $ip_address via $ip_via_address",
                Functions = (x, y) => BashTool.Execute(x)
            };
            dict["ip4-add-route"] = new Command<string, string> {
                Arguments = "ip route add $ip_address via $gateway dev $net_if",
                Functions = (x, y) => BashTool.Execute(x)
            };
            dict["ip4-add-tunnel-point-to-point"] = new Command<string, string> {
                Arguments = "ip tunnel add $net_if mode sit ttl $ttl remote $tunnel local $local_address",
                Functions = (x, y) => BashTool.Execute(x)
            };
            dict["ip4-del-addr"] = new Command<string, string> {
                Arguments = "ip addr del $address/$range dev $net_if",
                Functions = (x, y) => BashTool.Execute(x)
            };
            dict["ip4-del-addr-broadcast"] = new Command<string, string> {
                Arguments = "ip addr del $address/$range broadcast $broadcast dev $net_if",
                Functions = (x, y) => BashTool.Execute(x)
            };
            dict["ip4-del-route"] = new Command<string, string> {
                Arguments = "ip route del $ip_address via $gateway dev $net_if",
                Functions = (x, y) => BashTool.Execute(x)
            };
            dict["ip4-del-tunnel-point-to-point"] = new Command<string, string> {
                Arguments = "ip tunnel del $net_if",
                Functions = (x, y) => BashTool.Execute(x)
            };
            dict["ip4-disable-if"] = new Command<string, string> {
                Arguments = "ip link set $net_if down",
                Functions = (x, y) => BashTool.Execute(x)
            };
            dict["ip4-enable-if"] = new Command<string, string> {
                Arguments = "ip link set $net_if up",
                Functions = (x, y) => BashTool.Execute(x)
            };
            dict["ip4-flush-configuration"] = new Command<string, string> {
                Arguments = "ip addr flush dev $net_if",
                Functions = (x, y) => BashTool.Execute(x)
            };
            dict["ip4-get-if-addr"] = new Command<string, string> {
                Arguments = "ip addr show $net_if",
                Functions = (x, y) => BashTool.Execute(x).Grep("inet .").First().Print(2)
            };
            dict["ip4-get-if-brd"] = new Command<string, string> {
                Arguments = "ip addr show $net_if",
                Functions = (x, y) => BashTool.Execute(x).Grep("inet .").First().Print(4)
            };
            dict["ip4-get-if-macaddress"] = new Command<string, string> {
                Arguments = "/sys/class/net/$net_if/address",
                Functions = (x, y) => ReadTool.File(x)
            };
            dict["ip4-get-if-mtu"] = new Command<string, string> {
                Arguments = "/sys/class/net/$net_if/mtu",
                Functions = (x, y) => ReadTool.File(x)
            };
            dict["ip4-if-isdown"] = new Command<string, string> {
                Arguments = "ip addr show $net_if",
                Functions = (x, y) => BashTool.Execute(x).Grep("state DOWN").First()
            };
            dict["ip4-if-isup"] = new Command<string, string> {
                Arguments = "ip addr show $net_if",
                Functions = (x, y) => BashTool.Execute(x).Grep("state UP").First()
            };
            dict["ip4-set-macaddress"] = new Command<IEnumerable<string>, string> {
                Arguments = new[] {
                    "ip link set $net_if down",
                    "ip link set dev $net_if address $address",
                    "ip link set $net_if up"
                },
                Functions = (x, y) => BashTool.Execute(x)
            };
            dict["ip4-set-mtu"] = new Command<string, string> {
                Arguments = "ip link set dev $net_if mtu $mtu",
                Functions = (x, y) => BashTool.Execute(x)
            };
            dict["ip4-show-if-addr"] = new Command<string, string> {
                Arguments = "ip addr show $net_if",
                Functions = (x, y) => BashTool.Execute(x)
            };
            dict["ip4-show-if-link"] = new Command<string, string> {
                Arguments = "ip -s link ls $net_if",
                Functions = (x, y) => BashTool.Execute(x)
            };
            dict["ip4-show-routes"] = new Command<string, string> {
                Arguments = "ip route show $net_if",
                Functions = (x, y) => BashTool.Execute(x)
            };
            dict["ip4-show-tunnels"] = new Command<string, string> {
                Arguments = "ip tunnel show $net_if",
                Functions = (x, y) => BashTool.Execute(x)
            };
            dict["ip4-show-updown"] = new Command<string, string> {
                Arguments = "ip link show $net_if",
                //todo fare multigrep  | grep -ho \' UP \\| DOWN \'
                Functions = (x, y) => BashTool.Execute(x).Grep("UP").First()
            };
            #endregion

            #region [    Command - Ipv6    ]
            dict["ip6-add-addr"] = new Command<string, string> {
                Arguments = "ip -6 addr add $address/$range dev $net_if",
                Functions = (x, y) => BashTool.Execute(x)
            };
            dict["ip6-add-neigh"] = new Command<string, string> {
                Arguments = "ip -6 neigh add $ip_address lladdr $ip_lay_address dev $net_if",
                Functions = (x, y) => BashTool.Execute(x)
            };
            dict["ip6-add-new-address"] = new Command<string, string> {
                Arguments = "ip -6 addr add $ip_address dev $net_if",
                Functions = (x, y) => BashTool.Execute(x)
            };
            dict["ip6-add-route"] = new Command<string, string> {
                Arguments = "ip -6 route add $ip_address via $gateway",
                Functions = (x, y) => BashTool.Execute(x)
            };
            dict["ip6-add-route-dev"] = new Command<string, string> {
                Arguments = "ip -6 addr del $address/$range dev $net_if",
                Functions = (x, y) => BashTool.Execute(x)
            };
            dict["ip6-del-addr"] = new Command<string, string> {
                Arguments = "ip -6 addr del $address/$range dev $net_if",
                Functions = (x, y) => BashTool.Execute(x)
            };
            dict["ip6-del-address"] = new Command<string, string> {
                Arguments = "ip -6 addr del $ip_address dev $net_if",
                Functions = (x, y) => BashTool.Execute(x)
            };
            dict["ip6-del-neigh"] = new Command<string, string> {
                Arguments = "ip -6 neigh del $ip_address lladdr $ip_lay_address dev $net_if",
                Functions = (x, y) => BashTool.Execute(x)
            };
            dict["ip6-del-route"] = new Command<string, string> {
                Arguments = "ip -6 route del $ip_address via $gateway",
                Functions = (x, y) => BashTool.Execute(x)
            };
            dict["ip6-del-route-dev"] = new Command<string, string> {
                Arguments = "ip -6 route del $gateway dev $net_if",
                Functions = (x, y) => BashTool.Execute(x)
            };
            dict["ip6-flush-configuration"] = new Command<string, string> {
                Arguments = "ip -6 addr flush dynamic",
                Functions = (x, y) => BashTool.Execute(x)
            };
            dict["ip6-show-if-link"] = new Command<string, string> {
                Arguments = "ip -6 link show $net_if",
                Functions = (x, y) => BashTool.Execute(x)
            };
            dict["ip6-show-if-stats"] = new Command<string, string> {
                Arguments = "ip -6 -s link ls $net_if",
                Functions = (x, y) => BashTool.Execute(x)
            };
            dict["ip6-show-neigh"] = new Command<string, string> {
                Arguments = "ip -6 neigh show dev $net_if",
                Functions = (x, y) => BashTool.Execute(x)
            };
            dict["ip6-show-routes"] = new Command<string, string> {
                Arguments = "ip -6 route show $net_if",
                Functions = (x, y) => BashTool.Execute(x)
            };
            dict["ip6-show-tunnels"] = new Command<string, string> {
                Arguments = "ip -6 tunnel show $net_if",
                Functions = (x, y) => BashTool.Execute(x)
            };
            #endregion

            #region [    Command - Time & Date    ]
            dict["ntpdate"] = new Command<string, string> {
                Arguments = "ntpdate $server",
                Functions = (x, y) => BashTool.Execute(x)
            };
            dict["set-ntpdate"] = new Command<IEnumerable<string>, string> {
                Arguments = new[] {
                    "ntpdate $date_server",
                    "timedatectl --no-pager --no-ask-password --adjust-system-clock set-ntp yes"
                },
                Functions = (x, y) => BashTool.Execute(x)
            };
            dict["set-timezone"] = new Command<string, string> {
                Arguments = "timedatectl --no-pager --no-ask-password --adjust-system-clock set-timezone $host_timezone",
                Functions = (x, y) => BashTool.Execute(x)
            };
            dict["sync-clock"] = new Command<IEnumerable<string>, string> {
                Arguments = new[] {
                    "hwclock --systohc",
                    "hwclock --hctosys"
                },
                Functions = (x, y) => BashTool.Execute(x)
            };
            dict["timedatectl-get-localtime"] = new Command<string, string> {
                Arguments = "timedatectl",
                Grep = "Local time: ",
                Functions = (x, y) => BashTool.Execute(x).Grep(y).First().Print(2, ':')
            };
            dict["timedatectl-get-nettimeon"] = new Command<string, string> {
                Arguments = "timedatectl",
                Grep = "Network time on: ",
                Functions = (x, y) => BashTool.Execute(x).Grep(y).First().Print(2, ':')
            };
            dict["timedatectl-get-ntpsync"] = new Command<string, string> {
                Arguments = "timedatectl",
                Grep = "NTP synchronized: ",
                Functions = (x, y) => BashTool.Execute(x).Grep(y).First().Print(2, ':')
            };
            dict["timedatectl-get-rtcintz"] = new Command<string, string> {
                Arguments = "timedatectl",
                Grep = "RTC in local TZ: ",
                Functions = (x, y) => BashTool.Execute(x).Grep(y).First().Print(2, ':')
            };
            dict["timedatectl-get-rtctime"] = new Command<string, string> {
                Arguments = "timedatectl",
                Grep = "RTC time: ",
                Functions = (x, y) => BashTool.Execute(x).Grep(y).First().Print(2, ':')
            };
            dict["timedatectl-get-timezone"] = new Command<string, string> {
                Arguments = "timedatectl",
                Grep = "Time zone: ",
                Functions = (x, y) => BashTool.Execute(x).Grep(y).First().Print(2, ':')
            };
            dict["timedatectl-get-univtime"] = new Command<string, string> {
                Arguments = "timedatectl",
                Grep = "Universal time: ",
                Functions = (x, y) => BashTool.Execute(x).Grep(y).First().Print(2, ':')
            };
            #endregion

            #region [    Command - Rsync    ]
            dict["rsync"] = new Command<string, string> {
                Arguments = "rsync -aHA $source/ $destination/",
                Functions = (x, y) => BashTool.Execute(x)
            };
            dict["rsync-delete-after"] = new Command<string, string> {
                Arguments = "rsync -aHA --delete-after $source/ $destination/",
                Functions = (x, y) => BashTool.Execute(x)
            };
            dict["rsync-delete-during"] = new Command<string, string> {
                Arguments = "rsync -aHA --delete-during $source/ $destination/",
                Functions = (x, y) => BashTool.Execute(x)
            };
            #endregion

            #region [    Command - Systemctl    ]
            dict["systemctl-start"] = new Command<string, string> {
                Arguments = "systemctl start $service",
                Functions = (x, y) => BashTool.Execute(x)
            };
            dict["systemctl-stop"] = new Command<string, string> {
                Arguments = "systemctl stop $service",
                Functions = (x, y) => BashTool.Execute(x)
            };
            #endregion

            #region [    Command - Virsh    ]
            dict["virsh-destroy"] = new Command<string, string> {
                Arguments = "virsh destroy $domain",
                Functions = (x, y) => BashTool.Execute(x)
            };
            dict["virsh-reboot"] = new Command<string, string> {
                Arguments = "virsh reboot $domain",
                Functions = (x, y) => BashTool.Execute(x)
            };
            dict["virsh-reset"] = new Command<string, string> {
                Arguments = "virsh reset $domain",
                Functions = (x, y) => BashTool.Execute(x)
            };
            dict["virsh-restore"] = new Command<string, string> {
                Arguments = "virsh restore $domain",
                Functions = (x, y) => BashTool.Execute(x)
            };
            dict["virsh-resume"] = new Command<string, string> {
                Arguments = "virsh resume $domain",
                Functions = (x, y) => BashTool.Execute(x)
            };
            dict["virsh-shutdown"] = new Command<string, string> {
                Arguments = "virsh shutdown $domain",
                Functions = (x, y) => BashTool.Execute(x)
            };
            dict["virsh-start"] = new Command<string, string> {
                Arguments = "virsh start $domain",
                Functions = (x, y) => BashTool.Execute(x)
            };
            dict["virsh-suspend"] = new Command<string, string> {
                Arguments = "virsh suspend $domain",
                Functions = (x, y) => BashTool.Execute(x)
            };
            dict["virsh-dompmsuspend"] = new Command<string, string> {
                Arguments = "virsh dompmsuspend $domain",
                Functions = (x, y) => BashTool.Execute(x)
            };
            dict["virsh-dompmwakeup"] = new Command<string, string> {
                Arguments = "virsh dompmwakeup $domain",
                Functions = (x, y) => BashTool.Execute(x)
            };
            #endregion

            #region [    Command - Storage    ]
            dict["zpool-mklabel"] = new Command<string, string> {
                Arguments = "parted /dev/$disk_device mklabel $zpool_label Yes",
                Functions = (x, y) => BashTool.Execute(x)
            };
            dict["zpool-create"] = new Command<string, string> {
                Arguments = "zpool create -f -o altroot=$pool_altroot -o ashift=12 -O casesensitivity=insensitive -O normalization=formD $pool_name $pool_type $disk_byid",
                Functions = (x, y) => BashTool.Execute(x)
            };
            dict["zpool-create-simple"] = new Command<string, string> {
                Arguments = "zpool create -f -o altroot=$pool_altroot $pool_name Storage01 $disk",
                Functions = (x, y) => BashTool.Execute(x)
            };
            dict["zfs-create"] = new Command<IEnumerable<string>, string> {
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
                Functions = (x, y) => BashTool.Execute(x)
            };
            #endregion

            dict["bond-add-if"] = new Command<string, string> {
                Arguments = "ifenslave $bond $net_if",
                Functions = (x, y) => BashTool.Execute(x)
            };

            return dict;
        }
    }
}
