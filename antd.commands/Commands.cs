using System.Collections.Generic;

namespace antd.commands {
    public class Commands {

        public static Dictionary<string, string[]> List => GetDict();

        private static Dictionary<string, string[]> GetDict() {
            var dict = new Dictionary<string, string[]> {
                {"bond-add-if", new [] {
                    "ifenslave $bond $net_if"
                }},
                {"bond-del-if", new [] {
                    "ifenslave -d $bond $net_if"
                }},
                {"bond-set", new [] {
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
                }},
                {"brctl-add", new [] {
                    "brctl addbr $bridge"
                }},
                {"brctl-add-if", new [] {
                    "brctl addif $bridge $net_if"
                }},
                {"brctl-del", new [] {
                    "brctl delbr $bridge"
                }},
                {"brctl-del-if", new [] {
                    "brctl delif $bridge $net_if"
                }},
                {"brctl-set-pathcost", new [] {
                    "brctl setpathcost $bridge $path $cost set path cost"
                }},
                {"brctl-set-portprio", new [] {
                    "brctl setportprio $bridge $port $priority set port priority"
                }},
                {"brctl-show-br", new [] {
                    "brctl show $bridge | grep -ho \'$bridge.*\'"
                }},
                {"brctl-show-brid", new [] {
                    "brctl show $bridge | grep -ho \'$bridge.*\' | awk \'{print $2}\'"
                }},
                {"brctl-show-brif", new [] {
      "brctl show $bridge | grep -ho '$bridge.*' | awk \'{print $4}\'"
                }},
                {"brctl-show-brstpstatus", new [] {
                    "brctl show $bridge | grep -ho \'$bridge.*\' | awk \'{print $3}\'"
                }},
                {"brctl-show-macs", new [] {
                    "brctl showmacs $bridge"
                }},
                {"brctl-show-stp", new [] {
                    "brctl showstp $bridge"
                }},
                {"brctl-stp-off", new [] {
                    "brctl stp $bridge off"
                }},
                {"brctl-stp-on", new [] {
                    "brctl stp $bridge on"
                }},
                {"cat-etc-gentoorel", new [] {
                    "cat /etc/gentoo-release | grep -v \'^#\' | grep -v \'^$\'"
                }},
                {"cat-etc-hostname", new [] {
                    "cat /etc/hostname | grep -v \'^#\' | grep -v \'^$\'"
                }},
                {"cat-etc-hosts", new [] {
                    "cat /etc/hosts | grep -v \'^#\' | grep -v \'^$\'"
                }},
                {"cat-etc-lsbrel", new [] {
                    "cat /etc/lsb-release | grep -v \'^#\' | grep -v \'^$\'"
                }},
                {"cat-etc-osrel", new [] {
                    "cat /etc/os-release | grep -v \'^#\' | grep -v \'^$\'"
                }},
                {"cat-etc-resolv", new [] {
                    "cat /etc/resolv.conf | grep -v \'^#\' | grep -v \'^$\'"
                }},
                {"dhclient4", new [] {
      "dhclient $net_if"
                }},
                {"dhclient6", new [] {
      "dhclient -6 $net_if"
                }},
                {"echo", new [] {
      "echo $var1 $var2"
                }},
                {"fdisk-print", new [] {
      "echo -e \"p\" | fdisk $disk_device"
                }},
                {"fdisk-set-partition", new [] {
      "echo -e \"n\\n $part_number\\n $part_first_sector\\n $part_size\\n w\\n\" | fdisk $disk_device",
      "parted $disk_device $part_number 1 $name",
      "parted $disk_device align-check opt $part_number"
                }},
                {"hostnamectl-get-arch", new [] {
                    "hostnamectl | grep -ho \'Architecture: .*\' | awk -F: \'{print $2}\'"
                }},
                {"hostnamectl-get-bootid", new [] {
                    "hostnamectl | grep -ho \'Boot ID: .*\' | awk -F: \'{print $2}\'"
                }},
                {"hostnamectl-get-chassis", new [] {
                    "hostnamectl | grep -ho \'Chassis: .*\' | awk -F: \'{print $2}\'"
                }},
                {"hostnamectl-get-deployment", new [] {
                    "hostnamectl | grep -ho 'Deployment: .*' | awk -F: '{print $2}"
                }},
                {"hostnamectl-get-hostname", new [] {
      "hostnamectl | grep -ho \'Static hostname: .*\' | awk -F: \'{print $2}\'"
                }},
                {"hostnamectl-get-iconname", new [] {
                    "hostnamectl | grep -ho \'Icon name: .*\' | awk -F: \'{print $2}\'"
                }},
                {"hostnamectl-get-kernel", new [] {
                    "hostnamectl | grep -ho \'Kernel: .*\' | awk -F: \'{print $2}\'"
                }},
                {"hostnamectl-get-location", new [] {
                    "hostnamectl | grep -ho \'Location: .*\' | awk -F: \'{print $2}\'"
                }},
                {"hostnamectl-get-machineid", new [] {
                    "hostnamectl | grep -ho \'Machine ID: .*\' | awk -F: \'{print $2}\'"
                }},
                {"hostnamectl-get-os", new [] {
                    "hostnamectl | grep -ho \'Operating System: .*\' | awk -F: \'{print $2}\'"
                }},
                {"hostnamectl-get-virtualization", new [] {
                    "hostnamectl | grep -ho \'Virtualization: .*\' | awk -F: \'{print $2}\'"
                }},
                {"ip4-add-addr", new [] {
                    "ip addr add $address/$range dev $net_if"
                }},
                {"ip4-add-addr-broadcast", new [] {
      "ip addr add $address/$range broadcast $broadcast dev $net_if"
                }},
                {"ip4-add-multipath-route", new [] {
      "ip route add default scope global nexthop dev $net1 nexthop dev $net2"
                }},
                {"ip4-add-nat", new [] {
      "ip route add nat $ip_address via $ip_via_address"
                }},
                {"ip4-add-route", new [] {
      "ip route add $ip_address via $gateway dev $net_if"
                }},
                {"ip4-add-tunnel-point-to-point", new [] {
      "ip tunnel add $net_if mode sit ttl $ttl remote $tunnel local $local_address"
                }},
                {"ip4-del-addr", new [] {
      "ip addr del $address/$range dev $net_if"
                }},
                {"ip4-del-addr-broadcast", new [] {
      "ip addr del $address/$range broadcast $broadcast dev $net_if"
                }},
                {"ip4-del-route", new [] {
      "ip route del $ip_address via $gateway dev $net_if"
                }},
                {"ip4-del-tunnel-point-to-point", new [] {
      "ip tunnel del $net_if"
                }},
                {"ip4-disable-if", new [] {
      "ip link set $net_if down"
                }},
                {"ip4-enable-if", new [] {
      "ip link set $net_if up"
                }},
                {"ip4-flush-configuration", new [] {
      "ip addr flush dev $net_if"
                }},
                {"ip4-get-if-addr", new [] {
      "ip addr show $net_if | grep -ho 'inet .*' | awk '{print $2}"
                }},
                {"ip4-get-if-brd", new [] {
      "ip addr show $net_if | grep -ho 'inet .*' | awk '{print $4}"
                }},
                {"ip4-get-if-macaddress", new [] {
      "cat /sys/class/net/$net_if/address"
                }},
                {"ip4-get-if-mtu", new [] {
      "cat /sys/class/net/$net_if/mtu"
                }},
                {"ip4-if-isdown", new [] {
      "ip addr show eth0 | grep -c 'state DOWN"
                }},
                {"ip4-if-isup", new [] {
      "ip addr show eth0 | grep -c 'state UP"
                }},
                {"ip4-set-macaddress", new [] {
       "ip link set $net_if down",
      "ip link set dev $net_if address $address",
      "ip link set $net_if up"
                }},
                {"ip4-set-mtu", new [] {
      "ip link set dev $net_if mtu $mtu"
                }},
                {"ip4-show-if-addr", new [] {
      "ip addr show $net_if"
                }},
                {"ip4-show-if-link", new [] {
      "ip link show $net_if"
                }},
                {"ip4-show-if-stats", new [] {
      "ip -s link ls $net_if"
                }},
                {"ip4-show-routes", new [] {
      "ip route show $net_if"
                }},
                {"ip4-show-tunnels", new [] {
      "ip tunnel show $net_if"
                }},
                {"ip4-show-updown", new [] {
      "ip link show $net_if | grep -ho \' UP \\| DOWN \'| grep -v \'^$\'"
                }},
                {"ip6-add-addr", new [] {
                    "ip -6 addr add $address/$range dev $net_if"
                }},
                {"ip6-add-neigh", new [] {
      "ip -6 neigh add $ip_address lladdr $ip_lay_address dev $net_if"
                }},
                {"ip6-add-new-address", new [] {
      "ip -6 addr add $ip_address dev $net_if"
                }},
                {"ip6-add-route", new [] {
      "ip -6 route add $ip_address via $gateway"
                }},
                {"ip6-add-route-dev", new [] {
      "ip -6 route add $gateway dev $net_if"
                }},
                {"ip6-del-addr", new [] {
      "ip -6 addr del $address/$range dev $net_if"
                }},
                {"ip6-del-address", new [] {
      "ip -6 addr del $ip_address dev $net_if"
                }},
                {"ip6-del-neigh", new [] {
      "ip -6 neigh del $ip_address lladdr $ip_lay_address dev $net_if"
                }},
                {"ip6-del-route", new [] {
      "ip -6 route del $ip_address via $gateway"
                }},
                {"ip6-del-route-dev", new [] {
      "ip -6 route del $gateway dev $net_if"
                }},
                {"ip6-flush-configuration", new [] {
      "ip -6 addr flush dynamic"
                }},
                {"ip6-show-if-link", new [] {
      "ip -6 link show $net_if"
                }},
                {"ip6-show-if-stats", new [] {
      "ip -6 -s link ls $net_if"
                }},
                {"ip6-show-neigh", new [] {
      "ip -6 neigh show dev $net_if"
                }},
                {"ip6-show-routes", new [] {
      "ip -6 route show $net_if"
                }},
                {"ip6-show-tunnels", new [] {
      "ip -6 tunnel show $net_if"
                }},
                {"mono-antdsh-update", new [] {
      "mono /framework/antdsh/antdsh.exe update $context"
                }},
                {"mono-antdsh-update-check", new [] {
      "mono /framework/antdsh/antdsh.exe update check"
                }},
                {"rsync", new [] {
      "rsync -aHA $source/ $destination/"
                }},
                {"rsync-delete-after", new [] {
      "rsync -aHA --delete-after $source/ $destination/"
                }},
                {"rsync-delete-during", new [] {
      "rsync -aHA --delete-during $source/ $destination/"
                }},
                {"set-bond", new [] {
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
                }},
                {"set-chassis", new [] {
      "hostnamectl set-chassis $host_chassis"
                }},
                {"set-deployment", new [] {
      "hostnamectl set-deployment $host_deployment"
                }},
                {"set-hostname", new [] {
      "hostnamectl set-hostname $host_name"
                }},
                {"set-location", new [] {
      "hostnamectl set-location $host_location"
                }},
                {"set-network-interface", new [] {
                     "ip link set dev $interface_name down",
      "ip link set dev $interface_name up",
      "ip link set $interface_name txqueuelen 10000",
      "ip link set dev $interface_name up"
                }},
                {"set-ntpdate", new [] {
                         "ntpdate $date_server",
      "timedatectl --no-pager --no-ask-password --adjust-system-clock set-ntp yes"

                }},
                {"set-timezone", new [] {
      "timedatectl --no-pager --no-ask-password --adjust-system-clock set-timezone $host_timezone"
                }},
                {"set-vlan", new [] {
                    "ip link set $vlan_interface_name down",
      "ip link del $vlan_interface_name",
      "ip link add name $vlan_interface_name link $interface_name type vlan id $vlan_id",
      "ip link set $vlan_interface_name txqueuelen 10000",
      "ip link set $vlan_interface_name up"
                }},
                {"sync-clock", new [] {
          "hwclock --systohc",
      "hwclock --hctosys"
                }},
                {"timedatectl-get-localtime", new [] {
      "timedatectl | grep -ho \'Local time: .*\'| awk -F: \'{print $2}\'"
                }},
                {"timedatectl-get-nettimeon", new [] {
      "timedatectl | grep -ho \'Network time on: .*\'| awk -F: \'{print $2}\'"
                }},
                {"timedatectl-get-ntpsync", new [] {
                    "timedatectl | grep -ho \'NTP synchronized: .*\'| awk -F: \'{print $2}\'"
                }},
                {"timedatectl-get-rtcintz", new [] {
                    "timedatectl | grep -ho \'RTC in local TZ: .*\'| awk -F: \'{print $2}\'"
                }},
                {"timedatectl-get-rtctime", new [] {
                    "timedatectl | grep -ho \'RTC time: .*\'| awk -F: \'{print $2}\'"
                }},
                {"timedatectl-get-timezone", new [] {
                    "timedatectl | grep -ho \'Time zone: .*\'| awk -F: \'{print $2}\'"
                }},
                {"timedatectl-get-univtime", new [] {
                    "timedatectl | grep -ho \'Universal time: .*\'| awk -F: \'{print $2}\'"
                }},
                {"virsh-destroy", new [] {
                    "virsh destroy $domain"
                }},
                {"virsh-reboot", new [] {
      "virsh reboot $domain"
                }},
                {"virsh-reset", new [] {
      "virsh reset $domain"
                }},
                {"virsh-restore", new [] {
      "virsh restore $domain"
                }},
                {"virsh-resume", new [] {
      "virsh resume $domain"
                }},
                {"virsh-shutdown", new [] {
      "virsh shutdown $domain"
                }},
                {"virsh-start", new [] {
      "virsh start $domain"
                }},
                {"virsh-suspend", new [] {
      "virsh suspend $domain"
                }},
                {"virsh-dompmsuspend", new [] {
      "virsh dompmsuspend $domain"
                }},
                {"virsh-dompmwakeup", new [] {
      "virsh dompmwakeup $domain"
                }},
                {"zpool-mklabel", new [] {
      "parted /dev/$disk_device mklabel $zpool_label Yes"
                }},
                {"zpool-create", new [] {
      "zpool create -f -o altroot=$pool_altroot -o ashift=12 -O casesensitivity=insensitive -O normalization=formD $pool_name $pool_type $disk_byid"
                }},
                {"zpool-create-simple", new [] {
      "zpool create -f -o altroot=$pool_altroot $pool_name Storage01 $disk"
                }},
                {"zfs-create", new [] {
                    "zfs create -o compression=lz4 -o atime=off $pool_name/$dataset_name",
      "zfs set xattr=on $pool_name/$dataset_name",
      "zfs set acltype=posixacl $pool_name/$dataset_name",
      "zfs set aclinherit=passthrough-x $pool_name/$dataset_name",
      "zfs set mountpoint=/$pool_altroot/$pool_name/$dataset_name $pool_name/$dataset_name",
      "zpool add $pool_name log $disk_log",
      "zpool add $pool_name cache $disk_cache",
      "zpool set cachefile=/etc/zfs/zpool.cache $pool_name",
      "zpool set cachefile=/etc/zfs/zpool.cache $pool_name"
                }}
            };
            return dict;
        }
    }
}
