using System.Collections.Generic;

namespace antd.commands {
    public class Commands {

        public static string RootFrameworkAntdShellScripts => "/framework/antd/ShellScript";

        public static Dictionary<string, string[]> List => GetMegaDict();
        public static Dictionary<string, string[]> PartialList => GetPartDict();

        private static Dictionary<string, string[]> GetMegaDict() {
            var d1 = GetDict();
            var d2 = GetScriptDict();
            var d3 = GetApiDict();
            var dict = new Dictionary<string, string[]>();
            foreach(var d in d1) {
                dict[d.Key] = d.Value;
            }
            foreach(var d in d2) {
                dict[d.Key] = d.Value;
            }
            foreach(var d in d3) {
                dict[d.Key] = d.Value;
            }
            return dict;
        }

        private static Dictionary<string, string[]> GetPartDict() {
            var d2 = GetScriptDict();
            var d3 = GetApiDict();
            var dict = new Dictionary<string, string[]>();
            foreach(var d in d2) {
                dict[d.Key] = d.Value;
            }
            foreach(var d in d3) {
                dict[d.Key] = d.Value;
            }
            return dict;
        }

        private static Dictionary<string, string[]> GetDict() {
            var dict = new Dictionary<string, string[]> {
                {"anthilla", new [] {
                    "$custom"
                }},
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
                    "brctl show $bridge | grep -ho \'$bridge.\'"
                }},
                {"brctl-show-brid", new [] {
                    "brctl show $bridge | grep -ho \'$bridge.\' | awk \'{print $2}\'"
                }},
                {"brctl-show-brif", new [] {
                    "brctl show $bridge | grep -ho '$bridge.' | awk \'{print $4}\'"
                }},
                {"brctl-show-brstpstatus", new [] {
                    "brctl show $bridge | grep -ho \'$bridge.\' | awk \'{print $3}\'"
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
                {"cat", new [] {
                    "cat $file"
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
                {"dhclient-killall", new [] {
                    "killall dhclient"
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
                {"echo-write", new [] {
                    "echo $value > $file"
                }},
                {"echo-append", new [] {
                    "echo $value >> $file"
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
                    "hostnamectl | grep -ho \'Architecture: .\' | awk -F: \'{print $2}\'"
                }},
                {"hostnamectl-get-bootid", new [] {
                    "hostnamectl | grep -ho \'Boot ID: .\' | awk -F: \'{print $2}\'"
                }},
                {"hostnamectl-get-chassis", new [] {
                    "hostnamectl | grep -ho \'Chassis: .\' | awk -F: \'{print $2}\'"
                }},
                {"hostnamectl-get-deployment", new [] {
                    "hostnamectl | grep -ho 'Deployment: .' | awk -F: '{print $2}"
                }},
                {"hostnamectl-get-hostname", new [] {
                    "hostnamectl | grep -ho \'Static hostname: .\' | awk -F: \'{print $2}\'"
                }},
                {"hostnamectl-get-iconname", new [] {
                    "hostnamectl | grep -ho \'Icon name: .\' | awk -F: \'{print $2}\'"
                }},
                {"hostnamectl-get-kernel", new [] {
                    "hostnamectl | grep -ho \'Kernel: .\' | awk -F: \'{print $2}\'"
                }},
                {"hostnamectl-get-location", new [] {
                    "hostnamectl | grep -ho \'Location: .\' | awk -F: \'{print $2}\'"
                }},
                {"hostnamectl-get-machineid", new [] {
                    "hostnamectl | grep -ho \'Machine ID: .\' | awk -F: \'{print $2}\'"
                }},
                {"hostnamectl-get-os", new [] {
                    "hostnamectl | grep -ho \'Operating System: .\' | awk -F: \'{print $2}\'"
                }},
                {"hostnamectl-get-virtualization", new [] {
                    "hostnamectl | grep -ho \'Virtualization: .\' | awk -F: \'{print $2}\'"
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
                    "ip addr show $net_if | grep -ho 'inet .' | awk '{print $2}"
                }},
                {"ip4-get-if-brd", new [] {
                    "ip addr show $net_if | grep -ho 'inet .' | awk '{print $4}"
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
                {"modprobe", new [] {
                    "modprobe $package"
                }},
                {"mono-antdsh-update", new [] {
                    "mono /framework/antdsh/antdsh.exe update $context"
                }},
                {"mono-antdsh-update-check", new [] {
                    "mono /framework/antdsh/antdsh.exe update check"
                }},
                {"ntpdate", new [] {
                    "ntpdate $server"
                }},
                {"rmmod", new [] {
                    "rmmod $modules"
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
                    "hostnamectl set-location \"$host_location\""
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
                {"sysctl-p", new [] {
                    "sysctl -p"
                }},
                {"systemctl-start", new [] {
                    "systemctl start $service"
                }},
                {"systemctl-stop", new [] {
                    "systemctl stop $service"
                }},
                {"systemd-machine-id-setup", new [] {
                    "systemd-machine-id-setup"
                }},
                {"timedatectl-get-localtime", new [] {
                    "timedatectl | grep -ho \'Local time: .\'| awk -F: \'{print $2}\'"
                }},
                {"timedatectl-get-nettimeon", new [] {
                    "timedatectl | grep -ho \'Network time on: .\'| awk -F: \'{print $2}\'"
                }},
                {"timedatectl-get-ntpsync", new [] {
                    "timedatectl | grep -ho \'NTP synchronized: .\'| awk -F: \'{print $2}\'"
                }},
                {"timedatectl-get-rtcintz", new [] {
                    "timedatectl | grep -ho \'RTC in local TZ: .\'| awk -F: \'{print $2}\'"
                }},
                {"timedatectl-get-rtctime", new [] {
                    "timedatectl | grep -ho \'RTC time: .\'| awk -F: \'{print $2}\'"
                }},
                {"timedatectl-get-timezone", new [] {
                    "timedatectl | grep -ho \'Time zone: .\'| awk -F: \'{print $2}\'"
                }},
                {"timedatectl-get-univtime", new [] {
                    "timedatectl | grep -ho \'Universal time: .\'| awk -F: \'{print $2}\'"
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

        private static Dictionary<string, string[]> GetScriptDict() {
            var dict = new Dictionary<string, string[]> {
{ "script-3Gconnect", new [] { $"{RootFrameworkAntdShellScripts}/scripts/3Gconnect $ppp" } },
{ "script-3Gconnect_stop", new [] { $"{RootFrameworkAntdShellScripts}/scripts/3Gconnect_stop $ppp" } },
{ "script-3g_tty_list", new [] { $"{RootFrameworkAntdShellScripts}/scripts/3g_tty_list" } },
{ "script-accounting_start", new [] { $"{RootFrameworkAntdShellScripts}/scripts/accounting_start" } },
{ "script-acct_AddClass", new [] { $"{RootFrameworkAntdShellScripts}/scripts/acct_AddClass $classname $mb $hours $mbits $costm $costh $chargetype" } },
{ "script-acct_addcredit", new [] { $"{RootFrameworkAntdShellScripts}/scripts/acct_addcredit $username $value" } },
{ "script-acct_ChangeClass", new [] { $"{RootFrameworkAntdShellScripts}/scripts/acct_ChangeClass $classname $mb $hours $mbits $costm $costh $chargetype" } },
{ "script-acctClassArray", new [] { $"{RootFrameworkAntdShellScripts}/scripts/acctClassArray" } },
{ "script-acctClassList", new [] { $"{RootFrameworkAntdShellScripts}/scripts/acctClassList" } },
{ "script-acctd", new [] { $"{RootFrameworkAntdShellScripts}/scripts/acctd" } },
{ "script-acctDetails", new [] { $"{RootFrameworkAntdShellScripts}/scripts/acctDetails $user" } },
{ "script-acctDetails.pl", new [] { $"{RootFrameworkAntdShellScripts}/scripts/acctDetails.pl" } },
{ "script-acct_enqueue", new [] { $"{RootFrameworkAntdShellScripts}/scripts/acct_enqueue $type $id" } },
{ "script-acct_enqueue_start", new [] { $"{RootFrameworkAntdShellScripts}/scripts/acct_enqueue_start $id $username $mac $ip" } },
{ "script-acct_enqueue_stop", new [] { $"{RootFrameworkAntdShellScripts}/scripts/acct_enqueue_stop $id $username $mac $inputoct $outputoct $inputpkt $outputpkt $sessiontime $terminatecause" } },
{ "script-acct_enqueue_update", new [] { $"{RootFrameworkAntdShellScripts}/scripts/acct_enqueue_update $id $username $mac $inputoct $outputoct $inputpkt $outputpkt $sessiontime" } },
{ "script-acctEntries", new [] { $"{RootFrameworkAntdShellScripts}/scripts/acctEntries $filter" } },
{ "script-acctEntries.pl", new [] { $"{RootFrameworkAntdShellScripts}/scripts/acctEntries.pl" } },
{ "script-acct_interim_update", new [] { $"{RootFrameworkAntdShellScripts}/scripts/acct_interim_update" } },
{ "script-acct_isLimitsOK", new [] { $"{RootFrameworkAntdShellScripts}/scripts/acct_isLimitsOK" } },
{ "script-acct_limit_msg", new [] { $"{RootFrameworkAntdShellScripts}/scripts/acct_limit_msg $limit" } },
{ "script-acct_process_requests", new [] { $"{RootFrameworkAntdShellScripts}/scripts/acct_process_requests" } },
{ "script-acct_RemoveClass", new [] { $"{RootFrameworkAntdShellScripts}/scripts/acct_RemoveClass $class" } },
{ "script-acct_removeentry", new [] { $"{RootFrameworkAntdShellScripts}/scripts/acct_removeentry $user" } },
{ "script-acct_save", new [] { $"{RootFrameworkAntdShellScripts}/scripts/acct_save $enabled $currency $decimals" } },
{ "script-acctSelectClass", new [] { $"{RootFrameworkAntdShellScripts}/scripts/acctSelectClass" } },
{ "script-acct_start", new [] { $"{RootFrameworkAntdShellScripts}/scripts/acct_start" } },
{ "script-acct_status", new [] { $"{RootFrameworkAntdShellScripts}/scripts/acct_status" } },
{ "script-acct_stop", new [] { $"{RootFrameworkAntdShellScripts}/scripts/acct_stop" } },
{ "script-acct_store", new [] { $"{RootFrameworkAntdShellScripts}/scripts/acct_store" } },
{ "script-acct_userlimits", new [] { $"{RootFrameworkAntdShellScripts}/scripts/acct_userlimits $username" } },
{ "script-acct_writelimits", new [] { $"{RootFrameworkAntdShellScripts}/scripts/acct_writelimits $entry $limits $ip" } },
{ "script-activatedb", new [] { $"{RootFrameworkAntdShellScripts}/scripts/activatedb $dev $db" } },
{ "script-activatefeature_confirm", new [] { $"{RootFrameworkAntdShellScripts}/scripts/activatefeature_confirm $feature $key" } },
{ "script-activatesubscription_confirm", new [] { $"{RootFrameworkAntdShellScripts}/scripts/activatesubscription_confirm $feature $key" } },
{ "script-activation_msg", new [] { $"{RootFrameworkAntdShellScripts}/scripts/activation_msg $pre $net" } },
{ "script-addip", new [] { $"{RootFrameworkAntdShellScripts}/scripts/addip $interfacename $ip $netmask $status" } },
{ "script-addtrust", new [] { $"{RootFrameworkAntdShellScripts}/scripts/addtrust $foreignrealm $direction $pw" } },
{ "script-alert", new [] { $"{RootFrameworkAntdShellScripts}/scripts/alert" } },
{ "script-alert_msg", new [] { $"{RootFrameworkAntdShellScripts}/scripts/alert_msg $event" } },
{ "script-alerts_changestatus", new [] { $"{RootFrameworkAntdShellScripts}/scripts/alerts_changestatus $name $status" } },
{ "script-alerts_event_list", new [] { $"{RootFrameworkAntdShellScripts}/scripts/alerts_event_list $name" } },
{ "script-alerts_event_names", new [] { $"{RootFrameworkAntdShellScripts}/scripts/alerts_event_names $exclude" } },
{ "script-alerts_getstate", new [] { $"{RootFrameworkAntdShellScripts}/scripts/alerts_getstate $event $name" } },
{ "script-alerts_logger", new [] { $"{RootFrameworkAntdShellScripts}/scripts/alerts_logger $id $msg" } },
{ "script-alerts_mailrecipient", new [] { $"{RootFrameworkAntdShellScripts}/scripts/alerts_mailrecipient $severity" } },
{ "script-alerts_mk_default_events", new [] { $"{RootFrameworkAntdShellScripts}/scripts/alerts_mk_default_events" } },
{ "script-alert_sms_status", new [] { $"{RootFrameworkAntdShellScripts}/scripts/alert_sms_status" } },
{ "script-alert_smtp_status", new [] { $"{RootFrameworkAntdShellScripts}/scripts/alert_smtp_status" } },
{ "script-alerts_number", new [] { $"{RootFrameworkAntdShellScripts}/scripts/alerts_number" } },
{ "script-alert_spool", new [] { $"{RootFrameworkAntdShellScripts}/scripts/alert_spool" } },
{ "script-alerts_recipient_delete", new [] { $"{RootFrameworkAntdShellScripts}/scripts/alerts_recipient_delete $name" } },
{ "script-alerts_recipient_desc", new [] { $"{RootFrameworkAntdShellScripts}/scripts/alerts_recipient_desc $item" } },
{ "script-alerts_recipient_list", new [] { $"{RootFrameworkAntdShellScripts}/scripts/alerts_recipient_list $recipientname" } },
{ "script-alerts_recipient_save", new [] { $"{RootFrameworkAntdShellScripts}/scripts/alerts_recipient_save $name" } },
{ "script-alerts_removeevent", new [] { $"{RootFrameworkAntdShellScripts}/scripts/alerts_removeevent $event" } },
{ "script-alerts_resetevent", new [] { $"{RootFrameworkAntdShellScripts}/scripts/alerts_resetevent $event" } },
{ "script-alerts_s2p", new [] { $"{RootFrameworkAntdShellScripts}/scripts/alerts_s2p $severity" } },
{ "script-alerts_setstate", new [] { $"{RootFrameworkAntdShellScripts}/scripts/alerts_setstate $event $name $value" } },
{ "script-alerts_smsrecipient", new [] { $"{RootFrameworkAntdShellScripts}/scripts/alerts_smsrecipient $severity" } },
{ "script-alerts_start", new [] { $"{RootFrameworkAntdShellScripts}/scripts/alerts_start" } },
{ "script-alerts_status_sx", new [] { $"{RootFrameworkAntdShellScripts}/scripts/alerts_status_sx" } },
{ "script-alert_status", new [] { $"{RootFrameworkAntdShellScripts}/scripts/alert_status" } },
{ "script-alerts_test_logs", new [] { $"{RootFrameworkAntdShellScripts}/scripts/alerts_test_logs $1" } },
{ "script-alerts_testscript", new [] { $"{RootFrameworkAntdShellScripts}/scripts/alerts_testscript $event" } },
{ "script-alerts_test_title", new [] { $"{RootFrameworkAntdShellScripts}/scripts/alerts_test_title $type" } },
{ "script-alerts_viewer", new [] { $"{RootFrameworkAntdShellScripts}/scripts/alerts_viewer" } },
{ "script-alerts_viewscript", new [] { $"{RootFrameworkAntdShellScripts}/scripts/alerts_viewscript $event $opt" } },
{ "script-arpcheck", new [] { $"{RootFrameworkAntdShellScripts}/scripts/arpcheck $host" } },
{ "script-autoupdate-check-incompatible", new [] { $"{RootFrameworkAntdShellScripts}/scripts/autoupdate-check-incompatible $id" } },
{ "script-autoupdate-checkInstallerActivity", new [] { $"{RootFrameworkAntdShellScripts}/scripts/autoupdate-checkInstallerActivity $id" } },
{ "script-autoupdate-check-prerequisites", new [] { $"{RootFrameworkAntdShellScripts}/scripts/autoupdate-check-prerequisites $id" } },
{ "script-autoupdate-cleanup", new [] { $"{RootFrameworkAntdShellScripts}/scripts/autoupdate-cleanup $id $op $what" } },
{ "script-autoupdate-elapsed", new [] { $"{RootFrameworkAntdShellScripts}/scripts/autoupdate-elapsed $id $op" } },
{ "script-autoupdate-error", new [] { $"{RootFrameworkAntdShellScripts}/scripts/autoupdate-error $id $op $msg" } },
{ "script-autoupdateGetField", new [] { $"{RootFrameworkAntdShellScripts}/scripts/autoupdateGetField" } },
{ "script-autoupdateGetSize", new [] { $"{RootFrameworkAntdShellScripts}/scripts/autoupdateGetSize" } },
{ "script-autoupdate-installer", new [] { $"{RootFrameworkAntdShellScripts}/scripts/autoupdate-installer $id $op $flags" } },
{ "script-autoupdate-installer-bg", new [] { $"{RootFrameworkAntdShellScripts}/scripts/autoupdate-installer-bg $id $op" } },
{ "script-autoupdate-logger", new [] { $"{RootFrameworkAntdShellScripts}/scripts/autoupdate-logger $id $op $msg" } },
{ "script-autoupdateOperations", new [] { $"{RootFrameworkAntdShellScripts}/scripts/autoupdateOperations $id" } },
{ "script-autoupdate-output", new [] { $"{RootFrameworkAntdShellScripts}/scripts/autoupdate-output $id $op" } },
{ "script-autoupdate-output-finished", new [] { $"{RootFrameworkAntdShellScripts}/scripts/autoupdate-output-finished $id $op" } },
{ "script-autoupdater", new [] { $"{RootFrameworkAntdShellScripts}/scripts/autoupdater " } },
{ "script-autoupdate-sub-status", new [] { $"{RootFrameworkAntdShellScripts}/scripts/autoupdate-sub-status $flags" } },
{ "script-autoupdateTypeDesc", new [] { $"{RootFrameworkAntdShellScripts}/scripts/autoupdateTypeDesc $type" } },
{ "script-availableInterfaces", new [] { $"{RootFrameworkAntdShellScripts}/scripts/availableInterfaces" } },
{ "script-availableNetwork", new [] { $"{RootFrameworkAntdShellScripts}/scripts/availableNetwork" } },
{ "script-backupdb", new [] { $"{RootFrameworkAntdShellScripts}/scripts/backupdb $dev $db $compressed $wologs" } },
{ "script-bckfilename", new [] { $"{RootFrameworkAntdShellScripts}/scripts/bckfilename $dev $db" } },
{ "script-bitrate", new [] { $"{RootFrameworkAntdShellScripts}/scripts/bitrate" } },
{ "script-bondstart", new [] { $"{RootFrameworkAntdShellScripts}/scripts/bondstart" } },
{ "script-bondwatch", new [] { $"{RootFrameworkAntdShellScripts}/scripts/bondwatch $bond" } },
{ "script-bwd_selectinterface", new [] { $"{RootFrameworkAntdShellScripts}/scripts/bwd_selectinterface" } },
{ "script-bwd_start", new [] { $"{RootFrameworkAntdShellScripts}/scripts/bwd_start" } },
{ "script-bwd_status", new [] { $"{RootFrameworkAntdShellScripts}/scripts/bwd_status" } },
{ "script-bw_subnets", new [] { $"{RootFrameworkAntdShellScripts}/scripts/bw_subnets" } },
{ "script-change_c_year", new [] { $"{RootFrameworkAntdShellScripts}/scripts/change_c_year $grub" } },
{ "script-changetime", new [] { $"{RootFrameworkAntdShellScripts}/scripts/changetime $year $month $day $hh $mm $ss" } },
{ "script-changeUTC", new [] { $"{RootFrameworkAntdShellScripts}/scripts/changeUTC $utc" } },
{ "script-chat.sh", new [] { $"{RootFrameworkAntdShellScripts}/scripts/chat.sh" } },
{ "script-checkk5realm", new [] { $"{RootFrameworkAntdShellScripts}/scripts/checkk5realm $realm" } },
{ "script-checkldapbase", new [] { $"{RootFrameworkAntdShellScripts}/scripts/checkldapbase $base" } },
{ "script-checkNetworkOverlap", new [] { $"{RootFrameworkAntdShellScripts}/scripts/checkNetworkOverlap $ip $netmask $interface $vlan" } },
{ "script-checkrip", new [] { $"{RootFrameworkAntdShellScripts}/scripts/checkrip" } },
{ "script-checkUpdates", new [] { $"{RootFrameworkAntdShellScripts}/scripts/checkUpdates" } },
{ "script-checkvpn", new [] { $"{RootFrameworkAntdShellScripts}/scripts/checkvpn $vpn" } },
{ "script-clamav_resetdb", new [] { $"{RootFrameworkAntdShellScripts}/scripts/clamav_resetdb" } },
{ "script-clamav_update", new [] { $"{RootFrameworkAntdShellScripts}/scripts/clamav_update $result" } },
{ "script-ClamAV-Update", new [] { $"{RootFrameworkAntdShellScripts}/scripts/ClamAV-Update" } },
{ "script-clamav_update_status", new [] { $"{RootFrameworkAntdShellScripts}/scripts/clamav_update_status $result" } },
{ "script-cleantmp", new [] { $"{RootFrameworkAntdShellScripts}/scripts/cleantmp" } },
{ "script-clearAdminSession", new [] { $"{RootFrameworkAntdShellScripts}/scripts/clearAdminSession" } },
{ "script-cntop", new [] { $"{RootFrameworkAntdShellScripts}/scripts/cntop" } },
{ "script-cntop.sh", new [] { $"{RootFrameworkAntdShellScripts}/scripts/cntop.sh $options" } },
{ "script-config3G", new [] { $"{RootFrameworkAntdShellScripts}/scripts/config3G $name $description $tty $apn $dial $optional $defaultroute $auto $nat" } },
{ "script-configbond", new [] { $"{RootFrameworkAntdShellScripts}/scripts/configbond $name $description $bondlist $type $primary" } },
{ "script-configbridge", new [] { $"{RootFrameworkAntdShellScripts}/scripts/configbridge $name $description $bridgelist $stp $fwdelay $ageing $bridgeprio $hellotime" } },
{ "script-configPPP", new [] { $"{RootFrameworkAntdShellScripts}/scripts/configPPP $name $description $ethernet $username $pw $defaultroute $auto $servicename $nat" } },
{ "script-configRIP", new [] { $"{RootFrameworkAntdShellScripts}/scripts/configRIP $rippedlist $configuration" } },
{ "script-configtz", new [] { $"{RootFrameworkAntdShellScripts}/scripts/configtz $tz" } },
{ "script-configvpn", new [] { $"{RootFrameworkAntdShellScripts}/scripts/configvpn $name $description $remoteip $port $proto $tlsrole $remotecn $compression $crypto $parameters $authentication $psk $gateway $localip" } },
{ "script-connTrackFlush", new [] { $"{RootFrameworkAntdShellScripts}/scripts/connTrackFlush" } },
{ "script-connTrackList", new [] { $"{RootFrameworkAntdShellScripts}/scripts/connTrackList" } },
{ "script-countrycode2description", new [] { $"{RootFrameworkAntdShellScripts}/scripts/countrycode2description $code" } },
{ "script-cp_acct_start", new [] { $"{RootFrameworkAntdShellScripts}/scripts/cp_acct_start $id $username $clientip" } },
{ "script-cp_acct_stop", new [] { $"{RootFrameworkAntdShellScripts}/scripts/cp_acct_stop $id $username $clientip $rx $tx" } },
{ "script-cp_activatechain", new [] { $"{RootFrameworkAntdShellScripts}/scripts/cp_activatechain" } },
{ "script-cpAddClient", new [] { $"{RootFrameworkAntdShellScripts}/scripts/cpAddClient $desc $ip $mac" } },
{ "script-cpAddClientIPT", new [] { $"{RootFrameworkAntdShellScripts}/scripts/cpAddClientIPT" } },
{ "script-cpAddClient.orig", new [] { $"{RootFrameworkAntdShellScripts}/scripts/cpAddClient.orig $desc $ip $mac" } },
{ "script-cpAddDomain", new [] { $"{RootFrameworkAntdShellScripts}/scripts/cpAddDomain $domain $ type $radiusreq" } },
{ "script-cpAddService", new [] { $"{RootFrameworkAntdShellScripts}/scripts/cpAddService $desc $ip $port $proto" } },
{ "script-cpAddServiceIPT", new [] { $"{RootFrameworkAntdShellScripts}/scripts/cpAddServiceIPT $ip $mac" } },
{ "script-cpart", new [] { $"{RootFrameworkAntdShellScripts}/scripts/cpart $dev $dim" } },
{ "script-cp_as-httpd.shib", new [] { $"{RootFrameworkAntdShellScripts}/scripts/cp_as-httpd.shib" } },
{ "script-cp_authorize_client", new [] { $"{RootFrameworkAntdShellScripts}/scripts/cp_authorize_client $enable $ip $mac" } },
{ "script-cp_auth_start", new [] { $"{RootFrameworkAntdShellScripts}/scripts/cp_auth_start" } },
{ "script-cp_auth_status", new [] { $"{RootFrameworkAntdShellScripts}/scripts/cp_auth_status" } },
{ "script-cp_auto", new [] { $"{RootFrameworkAntdShellScripts}/scripts/cp_auto" } },
{ "script-cpAutoDisconnect", new [] { $"{RootFrameworkAntdShellScripts}/scripts/cpAutoDisconnect $x" } },
{ "script-cp_check_activity", new [] { $"{RootFrameworkAntdShellScripts}/scripts/cp_check_activity $ip $interface" } },
{ "script-cp_check_activity_wrapper", new [] { $"{RootFrameworkAntdShellScripts}/scripts/cp_check_activity_wrapper $entry $interface $ip" } },
{ "script-cp_checkauthenticator", new [] { $"{RootFrameworkAntdShellScripts}/scripts/cp_checkauthenticator $user $realm $ip $authenticator $renew" } },
{ "script-cp_checkInterface", new [] { $"{RootFrameworkAntdShellScripts}/scripts/cp_checkInterface" } },
{ "script-cp_connect", new [] { $"{RootFrameworkAntdShellScripts}/scripts/cp_connect $ip $user" } },
{ "script-cpConnectedClientsIPT", new [] { $"{RootFrameworkAntdShellScripts}/scripts/cpConnectedClientsIPT" } },
{ "script-cp_create_CRL_ipt", new [] { $"{RootFrameworkAntdShellScripts}/scripts/cp_create_CRL_ipt" } },
{ "script-cp_createticket", new [] { $"{RootFrameworkAntdShellScripts}/scripts/cp_createticket $authtype $user $pw $realm $ip $renew" } },
{ "script-cpDefaultDomain", new [] { $"{RootFrameworkAntdShellScripts}/scripts/cpDefaultDomain $domain" } },
{ "script-cp_disconnect", new [] { $"{RootFrameworkAntdShellScripts}/scripts/cp_disconnect $ip $lcode" } },
{ "script-cpFreeClientsIPT", new [] { $"{RootFrameworkAntdShellScripts}/scripts/cpFreeClientsIPT" } },
{ "script-cp_freeCRL", new [] { $"{RootFrameworkAntdShellScripts}/scripts/cp_freeCRL $ip $port" } },
{ "script-cpFreeServicesIPT", new [] { $"{RootFrameworkAntdShellScripts}/scripts/cpFreeServicesIPT" } },
{ "script-cp_getaccounting", new [] { $"{RootFrameworkAntdShellScripts}/scripts/cp_getaccounting $username $realm $ip $noupate" } },
{ "script-cp_getRXTX", new [] { $"{RootFrameworkAntdShellScripts}/scripts/cp_getRXTX $ip $rx" } },
{ "script-cp_getstatus", new [] { $"{RootFrameworkAntdShellScripts}/scripts/cp_getstatus $username $realm $ip" } },
{ "script-cp_interfaces", new [] { $"{RootFrameworkAntdShellScripts}/scripts/cp_interfaces $options" } },
{ "script-cp_interfaces_multi", new [] { $"{RootFrameworkAntdShellScripts}/scripts/cp_interfaces_multi" } },
{ "script-cp_IP2MAC", new [] { $"{RootFrameworkAntdShellScripts}/scripts/cp_IP2MAC $ip" } },
{ "script-cp_language", new [] { $"{RootFrameworkAntdShellScripts}/scripts/cp_language $lang" } },
{ "script-cpListConnected", new [] { $"{RootFrameworkAntdShellScripts}/scripts/cpListConnected" } },
{ "script-cpListDomains", new [] { $"{RootFrameworkAntdShellScripts}/scripts/cpListDomains" } },
{ "script-cpListDomainsOptions", new [] { $"{RootFrameworkAntdShellScripts}/scripts/cpListDomainsOptions" } },
{ "script-cpListFreeClients", new [] { $"{RootFrameworkAntdShellScripts}/scripts/cpListFreeClients" } },
{ "script-cpListFreeServices", new [] { $"{RootFrameworkAntdShellScripts}/scripts/cpListFreeServices" } },
{ "script-cp_lmsg", new [] { $"{RootFrameworkAntdShellScripts}/scripts/cp_lmsg $ip" } },
{ "script-cp_mkfreeCRL", new [] { $"{RootFrameworkAntdShellScripts}/scripts/cp_mkfreeCRL" } },
{ "script-cp_msg", new [] { $"{RootFrameworkAntdShellScripts}/scripts/cp_msg $msg $lang" } },
{ "script-cp_popup_exclusion", new [] { $"{RootFrameworkAntdShellScripts}/scripts/cp_popup_exclusion" } },
{ "script-cpRemoveClient", new [] { $"{RootFrameworkAntdShellScripts}/scripts/cpRemoveClient $clt" } },
{ "script-cpRemoveClientIPT", new [] { $"{RootFrameworkAntdShellScripts}/scripts/cpRemoveClientIPT $ip $mac" } },
{ "script-cpRemoveDomain", new [] { $"{RootFrameworkAntdShellScripts}/scripts/cpRemoveDomain $domain" } },
{ "script-cpRemoveService", new [] { $"{RootFrameworkAntdShellScripts}/scripts/cpRemoveService $srv" } },
{ "script-cpRemoveServiceIPT", new [] { $"{RootFrameworkAntdShellScripts}/scripts/cpRemoveServiceIPT $ip $port $proto" } },
{ "script-cp_renew", new [] { $"{RootFrameworkAntdShellScripts}/scripts/cp_renew" } },
{ "script-cp_rewrite_conds", new [] { $"{RootFrameworkAntdShellScripts}/scripts/cp_rewrite_conds" } },
{ "script-cp_shib_lsconfig", new [] { $"{RootFrameworkAntdShellScripts}/scripts/cp_shib_lsconfig $ext" } },
{ "script-cp_shib_release", new [] { $"{RootFrameworkAntdShellScripts}/scripts/cp_shib_release" } },
{ "script-cp_start", new [] { $"{RootFrameworkAntdShellScripts}/scripts/cp_start" } },
{ "script-cp_template_source", new [] { $"{RootFrameworkAntdShellScripts}/scripts/cp_template_source $tpl" } },
{ "script-cp_template_source_save", new [] { $"{RootFrameworkAntdShellScripts}/scripts/cp_template_source_save" } },
{ "script-cpu_details", new [] { $"{RootFrameworkAntdShellScripts}/scripts/cpu_details" } },
{ "script-cpuinfo", new [] { $"{RootFrameworkAntdShellScripts}/scripts/cpuinfo" } },
{ "script-cpu_model", new [] { $"{RootFrameworkAntdShellScripts}/scripts/cpu_model" } },
{ "script-cpus", new [] { $"{RootFrameworkAntdShellScripts}/scripts/cpus" } },
{ "script-cpw", new [] { $"{RootFrameworkAntdShellScripts}/scripts/cpw $user $cpw $pw $mode" } },
{ "script-cpwhelper", new [] { $"{RootFrameworkAntdShellScripts}/scripts/cpwhelper $princ $pw" } },
{ "script-cpwldap", new [] { $"{RootFrameworkAntdShellScripts}/scripts/cpwldap $user $pw" } },
{ "script-cp_x509CAList", new [] { $"{RootFrameworkAntdShellScripts}/scripts/cp_x509CAList" } },
{ "script-cp_x509_setCAs", new [] { $"{RootFrameworkAntdShellScripts}/scripts/cp_x509_setCAs $list" } },
{ "script-createK5REALM", new [] { $"{RootFrameworkAntdShellScripts}/scripts/createK5REALM $directory $base $action" } },
{ "script-createLDAPDB", new [] { $"{RootFrameworkAntdShellScripts}/scripts/createLDAPDB $directory $base $pw $hostname $action $ip" } },
{ "script-create-profile", new [] { $"{RootFrameworkAntdShellScripts}/scripts/create-profile $dev" } },
{ "script-createvlan", new [] { $"{RootFrameworkAntdShellScripts}/scripts/createvlan $interfacename $tag $decription" } },
{ "script-createvpn", new [] { $"{RootFrameworkAntdShellScripts}/scripts/createvpn $name $decription $remoteip $port $proto $tlsrole $remotecn $compression $crypto $parameters $auth $psk $gateway $localip" } },
{ "script-c_rehashCA", new [] { $"{RootFrameworkAntdShellScripts}/scripts/c_rehashCA" } },
{ "script-crlupdate", new [] { $"{RootFrameworkAntdShellScripts}/scripts/crlupdate" } },
{ "script-crontabgen", new [] { $"{RootFrameworkAntdShellScripts}/scripts/crontabgen" } },
{ "script-ctfilter", new [] { $"{RootFrameworkAntdShellScripts}/scripts/ctfilter" } },
{ "script-ctlogger", new [] { $"{RootFrameworkAntdShellScripts}/scripts/ctlogger" } },
{ "script-ctwatcher", new [] { $"{RootFrameworkAntdShellScripts}/scripts/ctwatcher" } },
{ "script-daemonwatcher", new [] { $"{RootFrameworkAntdShellScripts}/scripts/daemonwatcher" } },
{ "script-ddns2", new [] { $"{RootFrameworkAntdShellScripts}/scripts/ddns2" } },
{ "script-ddns-listregistered", new [] { $"{RootFrameworkAntdShellScripts}/scripts/ddns-listregistered  $username $password" } },
{ "script-deactivatedb", new [] { $"{RootFrameworkAntdShellScripts}/scripts/deactivatedb" } },
{ "script-defaultbridge", new [] { $"{RootFrameworkAntdShellScripts}/scripts/defaultbridge" } },
{ "script-defaultvalue", new [] { $"{RootFrameworkAntdShellScripts}/scripts/defaultvalue $name" } },
{ "script-deletedb", new [] { $"{RootFrameworkAntdShellScripts}/scripts/deletedb" } },
{ "script-deleteImported", new [] { $"{RootFrameworkAntdShellScripts}/scripts/deleteImported $entry" } },
{ "script-deleteTrustedCA", new [] { $"{RootFrameworkAntdShellScripts}/scripts/deleteTrustedCA $entry" } },
{ "script-deletevlan", new [] { $"{RootFrameworkAntdShellScripts}/scripts/deletevlan $iface" } },
{ "script-delpart", new [] { $"{RootFrameworkAntdShellScripts}/scripts/delpart $dev $part $cod" } },
{ "script-dhcp_addstatic", new [] { $"{RootFrameworkAntdShellScripts}/scripts/dhcp_addstatic $subnet $ip $mac $description $param" } },
{ "script-dhcp_advancedopt", new [] { $"{RootFrameworkAntdShellScripts}/scripts/dhcp_advancedopt $subnet $parameters" } },
{ "script-dhcp_checkopt", new [] { $"{RootFrameworkAntdShellScripts}/scripts/dhcp_checkopt $type $parameters" } },
{ "script-dhcp_configfile", new [] { $"{RootFrameworkAntdShellScripts}/scripts/dhcp_configfile" } },
{ "script-dhcp_createconfig", new [] { $"{RootFrameworkAntdShellScripts}/scripts/dhcp_createconfig" } },
{ "script-dhcp_createsubnet", new [] { $"{RootFrameworkAntdShellScripts}/scripts/dhcp_createsubnet $network $netmask" } },
{ "script-dhcp_editstatic", new [] { $"{RootFrameworkAntdShellScripts}/scripts/dhcp_editstatic $subnet $entry $ip $mac $desc $param" } },
{ "script-dhcp_getinterfaces", new [] { $"{RootFrameworkAntdShellScripts}/scripts/dhcp_getinterfaces" } },
{ "script-dhcp_liststatic", new [] { $"{RootFrameworkAntdShellScripts}/scripts/dhcp_liststatic $subnet" } },
{ "script-dhcp_removestatic", new [] { $"{RootFrameworkAntdShellScripts}/scripts/dhcp_removestatic $subnet $entry" } },
{ "script-dhcp_removesubnet", new [] { $"{RootFrameworkAntdShellScripts}/scripts/dhcp_removesubnet $subnet" } },
{ "script-dhcp_savesubnet", new [] { $"{RootFrameworkAntdShellScripts}/scripts/dhcp_savesubnet $subnet $dfleasedays $dfleasehour $dfleasemin $maxlsd $maxlsh $maxlsm $range1min $range1max $range2min $range2max $range3min $range3max $router $dns1 $dns2 $dns3 $domain $nis $ntp1 $ntp2 $wins1 $wins2 $enabled" } },
{ "script-dhcp_showlease", new [] { $"{RootFrameworkAntdShellScripts}/scripts/dhcp_showlease $cmd $ip" } },
{ "script-dhcp_showleasestable", new [] { $"{RootFrameworkAntdShellScripts}/scripts/dhcp_showleasestable $leasefile $onlyactive" } },
{ "script-dhcp_start", new [] { $"{RootFrameworkAntdShellScripts}/scripts/dhcp_start $opt" } },
{ "script-dhcp_subnetenabled", new [] { $"{RootFrameworkAntdShellScripts}/scripts/dhcp_subnetenabled" } },
{ "script-dns_addForwarder", new [] { $"{RootFrameworkAntdShellScripts}/scripts/dns_addForwarder $domain $server" } },
{ "script-dns_addslaveserver", new [] { $"{RootFrameworkAntdShellScripts}/scripts/dns_addslaveserver $server" } },
{ "script-dns_addslavezone", new [] { $"{RootFrameworkAntdShellScripts}/scripts/dns_addslavezone $zone $master" } },
{ "script-dns_allow-query", new [] { $"{RootFrameworkAntdShellScripts}/scripts/dns_allow-query" } },
{ "script-dnscache", new [] { $"{RootFrameworkAntdShellScripts}/scripts/dnscache" } },
{ "script-dns_changemaster", new [] { $"{RootFrameworkAntdShellScripts}/scripts/dns_changemaster $dn $old $new" } },
{ "script-dns_clients_save", new [] { $"{RootFrameworkAntdShellScripts}/scripts/dns_clients_save $local $acl $reject" } },
{ "script-dns_disable", new [] { $"{RootFrameworkAntdShellScripts}/scripts/dns_disable" } },
{ "script-dns_enable", new [] { $"{RootFrameworkAntdShellScripts}/scripts/dns_enable" } },
{ "script-dns_hup", new [] { $"{RootFrameworkAntdShellScripts}/scripts/dns_hup" } },
{ "script-dns_iptables", new [] { $"{RootFrameworkAntdShellScripts}/scripts/dns_iptables $auth" } },
{ "script-dns_notifyserial", new [] { $"{RootFrameworkAntdShellScripts}/scripts/dns_notifyserial" } },
{ "script-dns_query", new [] { $"{RootFrameworkAntdShellScripts}/scripts/dns_query $dns $what $type $error" } },
{ "script-dns_removeForwarder", new [] { $"{RootFrameworkAntdShellScripts}/scripts/dns_removeForwarder $forwarder" } },
{ "script-dns_removeslaveserver $server", new [] { $"{RootFrameworkAntdShellScripts}/scripts/dns_removeslaveserver" } },
{ "script-dns_removeslavezone", new [] { $"{RootFrameworkAntdShellScripts}/scripts/dns_removeslavezone $zone" } },
{ "script-dns_removezone $zone", new [] { $"{RootFrameworkAntdShellScripts}/scripts/dns_removezone" } },
{ "script-dns_showslavezone $zone", new [] { $"{RootFrameworkAntdShellScripts}/scripts/dns_showslavezone" } },
{ "script-dns_slaveserversoptions", new [] { $"{RootFrameworkAntdShellScripts}/scripts/dns_slaveserversoptions" } },
{ "script-dns_slavezonesoptions", new [] { $"{RootFrameworkAntdShellScripts}/scripts/dns_slavezonesoptions" } },
{ "script-dns_start", new [] { $"{RootFrameworkAntdShellScripts}/scripts/dns_start $param" } },
{ "script-dns_status", new [] { $"{RootFrameworkAntdShellScripts}/scripts/dns_status" } },
{ "script-dns_transferzone", new [] { $"{RootFrameworkAntdShellScripts}/scripts/dns_transferzone $zone" } },
{ "script-dns_zoneconfig", new [] { $"{RootFrameworkAntdShellScripts}/scripts/dns_zoneconfig" } },
{ "script-downloadUpdateList", new [] { $"{RootFrameworkAntdShellScripts}/scripts/downloadUpdateList" } },
{ "script-editip", new [] { $"{RootFrameworkAntdShellScripts}/scripts/editip $interfacename $ip $mask $obj $status" } },
{ "script-editvlan", new [] { $"{RootFrameworkAntdShellScripts}/scripts/editvlan $interfacename $tag $$descr" } },
{ "script-enableforwarding", new [] { $"{RootFrameworkAntdShellScripts}/scripts/enableforwarding $value" } },
{ "script-escape", new [] { $"{RootFrameworkAntdShellScripts}/scripts/escape" } },
{ "script-ext_bondname", new [] { $"{RootFrameworkAntdShellScripts}/scripts/ext_bondname $bond" } },
{ "script-ext_bridgename", new [] { $"{RootFrameworkAntdShellScripts}/scripts/ext_bridgename $bridge" } },
{ "script-failoverd", new [] { $"{RootFrameworkAntdShellScripts}/scripts/failoverd" } },
{ "script-failsafe", new [] { $"{RootFrameworkAntdShellScripts}/scripts/failsafe" } },
{ "script-fpart", new [] { $"{RootFrameworkAntdShellScripts}/scripts/fpart $dev $fs $label $cod" } },
{ "script-freebondnum", new [] { $"{RootFrameworkAntdShellScripts}/scripts/freebondnum" } },
{ "script-freebridgenum", new [] { $"{RootFrameworkAntdShellScripts}/scripts/freebridgenum" } },
{ "script-fw_cancelchanges", new [] { $"{RootFrameworkAntdShellScripts}/scripts/fw_cancelchanges $chain" } },
{ "script-fw_changepolicy", new [] { $"{RootFrameworkAntdShellScripts}/scripts/fw_changepolicy $chain $policy" } },
{ "script-fw_cp_chain", new [] { $"{RootFrameworkAntdShellScripts}/scripts/fw_cp_chain" } },
{ "script-fw_deleterule", new [] { $"{RootFrameworkAntdShellScripts}/scripts/fw_deleterule $chain $rule" } },
{ "script-fw_enablechain", new [] { $"{RootFrameworkAntdShellScripts}/scripts/fw_enablechain $chain $enable" } },
{ "script-fw_getpolicy", new [] { $"{RootFrameworkAntdShellScripts}/scripts/fw_getpolicy $chain" } },
{ "script-fw_https_chain", new [] { $"{RootFrameworkAntdShellScripts}/scripts/fw_https_chain" } },
{ "script-fw_initrules", new [] { $"{RootFrameworkAntdShellScripts}/scripts/fw_initrules $chain" } },
{ "script-fw_interfaces", new [] { $"{RootFrameworkAntdShellScripts}/scripts/fw_interfaces" } },
{ "script-fw_listaction", new [] { $"{RootFrameworkAntdShellScripts}/scripts/fw_listaction $chain" } },
{ "script-fw_listchains", new [] { $"{RootFrameworkAntdShellScripts}/scripts/fw_listchains $opt $chain" } },
{ "script-fw_listrules", new [] { $"{RootFrameworkAntdShellScripts}/scripts/fw_listrules $chain" } },
{ "script-fw_makerule", new [] { $"{RootFrameworkAntdShellScripts}/scripts/fw_makerule $chain $rule $opt" } },
{ "script-fw_newchain", new [] { $"{RootFrameworkAntdShellScripts}/scripts/fw_newchain $chain" } },
{ "script-fw_protdes", new [] { $"{RootFrameworkAntdShellScripts}/scripts/fw_protdes" } },
{ "script-fw_protocols", new [] { $"{RootFrameworkAntdShellScripts}/scripts/fw_protocols" } },
{ "script-fw_removechain", new [] { $"{RootFrameworkAntdShellScripts}/scripts/fw_removechain $chain" } },
{ "script-fw_reorder", new [] { $"{RootFrameworkAntdShellScripts}/scripts/fw_reorder $chain" } },
{ "script-fw_savechain", new [] { $"{RootFrameworkAntdShellScripts}/scripts/fw_savechain $chain" } },
{ "script-fw_ssh_chain", new [] { $"{RootFrameworkAntdShellScripts}/scripts/fw_ssh_chain" } },
{ "script-fw_start", new [] { $"{RootFrameworkAntdShellScripts}/scripts/fw_start" } },
{ "script-fw_sys_chain", new [] { $"{RootFrameworkAntdShellScripts}/scripts/fw_sys_chain" } },
{ "script-fw_viewchain", new [] { $"{RootFrameworkAntdShellScripts}/scripts/fw_viewchain $chain" } },
{ "script-getactivedb", new [] { $"{RootFrameworkAntdShellScripts}/scripts/getactivedb" } },
{ "script-getbinderror", new [] { $"{RootFrameworkAntdShellScripts}/scripts/getbinderror" } },
{ "script-getBondType", new [] { $"{RootFrameworkAntdShellScripts}/scripts/getBondType $bond" } },
{ "script-getdefaultgw", new [] { $"{RootFrameworkAntdShellScripts}/scripts/getdefaultgw $db" } },
{ "script-get_environment", new [] { $"{RootFrameworkAntdShellScripts}/scripts/get_environment $db" } },
{ "script-getethoption", new [] { $"{RootFrameworkAntdShellScripts}/scripts/getethoption $selected" } },
{ "script-getfeature", new [] { $"{RootFrameworkAntdShellScripts}/scripts/getfeature $pre $e" } },
{ "script-getFixDeps", new [] { $"{RootFrameworkAntdShellScripts}/scripts/getFixDeps $id" } },
{ "script-getFixInfo", new [] { $"{RootFrameworkAntdShellScripts}/scripts/getFixInfo $id" } },
{ "script-getFixInfoShow", new [] { $"{RootFrameworkAntdShellScripts}/scripts/getFixInfoShow $id" } },
{ "script-getFixInstalled", new [] { $"{RootFrameworkAntdShellScripts}/scripts/getFixInstalled" } },
{ "script-getFixStatus", new [] { $"{RootFrameworkAntdShellScripts}/scripts/getFixStatus $id" } },
{ "script-gethttplan", new [] { $"{RootFrameworkAntdShellScripts}/scripts/gethttplan" } },
{ "script-getintdesc", new [] { $"{RootFrameworkAntdShellScripts}/scripts/getintdesc $interface" } },
{ "script-getInterfaceWeight", new [] { $"{RootFrameworkAntdShellScripts}/scripts/getInterfaceWeight $bond $interface" } },
{ "script-getipdir", new [] { $"{RootFrameworkAntdShellScripts}/scripts/getipdir $interface $ip" } },
{ "script-getkey", new [] { $"{RootFrameworkAntdShellScripts}/scripts/getkey" } },
{ "script-getlinkstatus", new [] { $"{RootFrameworkAntdShellScripts}/scripts/getlinkstatus $interface $mode" } },
{ "script-getreiserfslabel", new [] { $"{RootFrameworkAntdShellScripts}/scripts/getreiserfslabel" } },
{ "script-get_salt", new [] { $"{RootFrameworkAntdShellScripts}/scripts/get_salt $feature" } },
{ "script-getSOA", new [] { $"{RootFrameworkAntdShellScripts}/scripts/getSOA $zone" } },
{ "script-gw_l2checkd", new [] { $"{RootFrameworkAntdShellScripts}/scripts/gw_l2checkd $gw" } },
{ "script-hwclock", new [] { $"{RootFrameworkAntdShellScripts}/scripts/hwclock" } },
{ "script-iconfig", new [] { $"{RootFrameworkAntdShellScripts}/scripts/iconfig $opt" } },
{ "script-idpDiscovery", new [] { $"{RootFrameworkAntdShellScripts}/scripts/idpDiscovery $type $idp" } },
{ "script-idpDiscoveryRoot", new [] { $"{RootFrameworkAntdShellScripts}/scripts/idpDiscoveryRoot $type $idp" } },
{ "script-imageManagerLink", new [] { $"{RootFrameworkAntdShellScripts}/scripts/imageManagerLink $linkpath" } },
{ "script-imagesize", new [] { $"{RootFrameworkAntdShellScripts}/scripts/imagesize $image" } },
{ "script-importCAcert", new [] { $"{RootFrameworkAntdShellScripts}/scripts/importCAcert" } },
{ "script-importCAkey", new [] { $"{RootFrameworkAntdShellScripts}/scripts/importCAkey" } },
{ "script-importCertificate", new [] { $"{RootFrameworkAntdShellScripts}/scripts/importCertificate" } },
{ "script-importTrustedCA", new [] { $"{RootFrameworkAntdShellScripts}/scripts/importTrustedCA" } },
{ "script-importTrustedCRL", new [] { $"{RootFrameworkAntdShellScripts}/scripts/importTrustedCRL" } },
{ "script-inst-manager", new [] { $"{RootFrameworkAntdShellScripts}/scripts/inst-manager $d $p" } },
{ "script-ip_belongs_network", new [] { $"{RootFrameworkAntdShellScripts}/scripts/ip_belongs_network" } },
{ "script-ip-manager", new [] { $"{RootFrameworkAntdShellScripts}/scripts/ip-manager" } },
{ "script-ip_restart_daemon", new [] { $"{RootFrameworkAntdShellScripts}/scripts/ip_restart_daemon" } },
{ "script-ipsec_start", new [] { $"{RootFrameworkAntdShellScripts}/scripts/ipsec_start" } },
{ "script-ipsec_status", new [] { $"{RootFrameworkAntdShellScripts}/scripts/ipsec_status" } },
{ "script-ipsec_stop", new [] { $"{RootFrameworkAntdShellScripts}/scripts/ipsec_stop" } },
{ "script-iwconfig_watch", new [] { $"{RootFrameworkAntdShellScripts}/scripts/iwconfig_watch $iw" } },
{ "script-k5_addRealm", new [] { $"{RootFrameworkAntdShellScripts}/scripts/k5_addRealm $realm $kdc" } },
{ "script-k5_createConf", new [] { $"{RootFrameworkAntdShellScripts}/scripts/k5_createConf" } },
{ "script-k5defaultencryption", new [] { $"{RootFrameworkAntdShellScripts}/scripts/k5defaultencryption $desc" } },
{ "script-k5enclist", new [] { $"{RootFrameworkAntdShellScripts}/scripts/k5enclist $desc" } },
{ "script-k5_isTrusted", new [] { $"{RootFrameworkAntdShellScripts}/scripts/k5_isTrusted $domain $direction" } },
{ "script-k5_removeRealm", new [] { $"{RootFrameworkAntdShellScripts}/scripts/k5_removeRealm $realm" } },
{ "script-kadmind_start", new [] { $"{RootFrameworkAntdShellScripts}/scripts/kadmind_start" } },
{ "script-krb5/", new [] { $"{RootFrameworkAntdShellScripts}/scripts/krb5/" } },
{ "script-l7listgroups", new [] { $"{RootFrameworkAntdShellScripts}/scripts/l7listgroups" } },
{ "script-l7listprotocols", new [] { $"{RootFrameworkAntdShellScripts}/scripts/l7listprotocols $grp" } },
{ "script-l7protocols-prep", new [] { $"{RootFrameworkAntdShellScripts}/scripts/l7protocols-prep" } },
{ "script-l7view", new [] { $"{RootFrameworkAntdShellScripts}/scripts/l7view $pat" } },
{ "script-lastUpdateCheck", new [] { $"{RootFrameworkAntdShellScripts}/scripts/lastUpdateCheck" } },
{ "script-lgwatcher", new [] { $"{RootFrameworkAntdShellScripts}/scripts/lgwatcher" } },
{ "script-listImportedCerts", new [] { $"{RootFrameworkAntdShellScripts}/scripts/listImportedCerts" } },
{ "script-list_local_subnet", new [] { $"{RootFrameworkAntdShellScripts}/scripts/list_local_subnet" } },
{ "script-listTrustedCAs", new [] { $"{RootFrameworkAntdShellScripts}/scripts/listTrustedCAs" } },
{ "script-loadmsgboard", new [] { $"{RootFrameworkAntdShellScripts}/scripts/loadmsgboard" } },
{ "script-localman", new [] { $"{RootFrameworkAntdShellScripts}/scripts/localman" } },
{ "script-localman_activatedb", new [] { $"{RootFrameworkAntdShellScripts}/scripts/localman_activatedb" } },
{ "script-localman_bridge", new [] { $"{RootFrameworkAntdShellScripts}/scripts/localman_bridge" } },
{ "script-localman_deactivatedb", new [] { $"{RootFrameworkAntdShellScripts}/scripts/localman_deactivatedb" } },
{ "script-localman_infodb", new [] { $"{RootFrameworkAntdShellScripts}/scripts/localman_infodb" } },
{ "script-logreduce", new [] { $"{RootFrameworkAntdShellScripts}/scripts/logreduce $day" } },
{ "script-log_syslog", new [] { $"{RootFrameworkAntdShellScripts}/scripts/log_syslog $y $m $d $host $proc $page $filter" } },
{ "script-lspart", new [] { $"{RootFrameworkAntdShellScripts}/scripts/lspart $dev" } },
{ "script-make3G", new [] { $"{RootFrameworkAntdShellScripts}/scripts/make3G $name $desc $tty $apn $dial $optional $dfroute $auto $nat" } },
{ "script-makebond", new [] { $"{RootFrameworkAntdShellScripts}/scripts/makebond $name $desc $bondlist $type $primary" } },
{ "script-makebridge", new [] { $"{RootFrameworkAntdShellScripts}/scripts/makebridge $name $desc $bridgelist $st $fwdelay $ageing $maxage $brdprio $hellotime" } },
{ "script-makedb", new [] { $"{RootFrameworkAntdShellScripts}/scripts/makedb $dev $desc $restore $srcpath $srcdb $hostname $ldapbase $k5realm $pw $interface $ip $netmask $gw" } },
{ "script-make_dbtpl", new [] { $"{RootFrameworkAntdShellScripts}/scripts/make_dbtpl $source" } },
{ "script-makePPP", new [] { $"{RootFrameworkAntdShellScripts}/scripts/makePPP $name $desc $ethernet $username $pw $dfroute $auto $servicename $nat" } },
{ "script-man_page", new [] { $"{RootFrameworkAntdShellScripts}/scripts/man_page $page $section" } },
{ "script-mkbonddev", new [] { $"{RootFrameworkAntdShellScripts}/scripts/mkbonddev $bond $mode" } },
{ "script-mountdefaultdb", new [] { $"{RootFrameworkAntdShellScripts}/scripts/mountdefaultdb" } },
{ "script-mountstorage", new [] { $"{RootFrameworkAntdShellScripts}/scripts/mountstorage" } },
{ "script-mrtg_createcfg", new [] { $"{RootFrameworkAntdShellScripts}/scripts/mrtg_createcfg $int $cl" } },
{ "script-mrtg_reload", new [] { $"{RootFrameworkAntdShellScripts}/scripts/mrtg_reload" } },
{ "script-mrtg_selectinterface", new [] { $"{RootFrameworkAntdShellScripts}/scripts/mrtg_selectinterface $interface $type" } },
{ "script-mrtgtarget", new [] { $"{RootFrameworkAntdShellScripts}/scripts/mrtgtarget $interface $class" } },
{ "script-mrtgtarget2", new [] { $"{RootFrameworkAntdShellScripts}/scripts/mrtgtarget2" } },
{ "script-mrtgtarget3", new [] { $"{RootFrameworkAntdShellScripts}/scripts/mrtgtarget3 $gw" } },
{ "script-mrtgtarget4", new [] { $"{RootFrameworkAntdShellScripts}/scripts/mrtgtarget4" } },
{ "script-mrtgtarget5", new [] { $"{RootFrameworkAntdShellScripts}/scripts/mrtgtarget5" } },
{ "script-mrtgtarget6", new [] { $"{RootFrameworkAntdShellScripts}/scripts/mrtgtarget6" } },
{ "script-nb_addfakegw", new [] { $"{RootFrameworkAntdShellScripts}/scripts/nb_addfakegw" } },
{ "script-nb_addgw", new [] { $"{RootFrameworkAntdShellScripts}/scripts/nb_addgw $desc $enabled $weight $ip $interface $tc" } },
{ "script-nb_autoppp", new [] { $"{RootFrameworkAntdShellScripts}/scripts/nb_autoppp $status" } },
{ "script-nb_changegw", new [] { $"{RootFrameworkAntdShellScripts}/scripts/nb_changegw $gw $desc $enabled $weight $ip $interface $tc" } },
{ "script-nb_connectivity", new [] { $"{RootFrameworkAntdShellScripts}/scripts/nb_connectivity" } },
{ "script-nb_copydefaultgw", new [] { $"{RootFrameworkAntdShellScripts}/scripts/nb_copydefaultgw" } },
{ "script-nb_ct_pre", new [] { $"{RootFrameworkAntdShellScripts}/scripts/nb_ct_pre $op $gw $localip $dev" } },
{ "script-nb_enablegw", new [] { $"{RootFrameworkAntdShellScripts}/scripts/nb_enablegw $gw $enabled" } },
{ "script-nb_fw", new [] { $"{RootFrameworkAntdShellScripts}/scripts/nb_fw" } },
{ "script-nb_gwlist", new [] { $"{RootFrameworkAntdShellScripts}/scripts/nb_gwlist" } },
{ "script-nb_ips_warning", new [] { $"{RootFrameworkAntdShellScripts}/scripts/nb_ips_warning" } },
{ "script-nb_removegw", new [] { $"{RootFrameworkAntdShellScripts}/scripts/nb_removegw $gw" } },
{ "script-nb_save", new [] { $"{RootFrameworkAntdShellScripts}/scripts/nb_save $enabled $mode $icmpcheck $probedown $probeup $timeout $pause $ppprestart $ip1 $enabledip1 $ip2 $ennnabledip2 $ip3 $enabledip3" } },
{ "script-nb_setautomarking", new [] { $"{RootFrameworkAntdShellScripts}/scripts/nb_setautomarking $gw" } },
{ "script-nb_setnexthop", new [] { $"{RootFrameworkAntdShellScripts}/scripts/nb_setnexthop $gw" } },
{ "script-nb_startfo", new [] { $"{RootFrameworkAntdShellScripts}/scripts/nb_startfo $opt" } },
{ "script-nb_statistics", new [] { $"{RootFrameworkAntdShellScripts}/scripts/nb_statistics" } },
{ "script-nb_statusfo", new [] { $"{RootFrameworkAntdShellScripts}/scripts/nb_statusfo" } },
{ "script-nb_statusgw", new [] { $"{RootFrameworkAntdShellScripts}/scripts/nb_statusgw $gw" } },
{ "script-nb_testfo", new [] { $"{RootFrameworkAntdShellScripts}/scripts/nb_testfo" } },
{ "script-nb_vpn", new [] { $"{RootFrameworkAntdShellScripts}/scripts/nb_vpn" } },
{ "script-nDPI_available", new [] { $"{RootFrameworkAntdShellScripts}/scripts/nDPI_available" } },
{ "script-net_bondview", new [] { $"{RootFrameworkAntdShellScripts}/scripts/net_bondview $bond" } },
{ "script-net_bridgeview", new [] { $"{RootFrameworkAntdShellScripts}/scripts/net_bridgeview $bridgeview" } },
{ "script-net_getfreeppp", new [] { $"{RootFrameworkAntdShellScripts}/scripts/net_getfreeppp" } },
{ "script-net.inc", new [] { $"{RootFrameworkAntdShellScripts}/scripts/net.inc" } },
{ "script-net_interfacesbonded", new [] { $"{RootFrameworkAntdShellScripts}/scripts/net_interfacesbonded $bond $fmt" } },
{ "script-net_interfacesbridged", new [] { $"{RootFrameworkAntdShellScripts}/scripts/net_interfacesbridged $brd $combo" } },
{ "script-net_interfacesforbond", new [] { $"{RootFrameworkAntdShellScripts}/scripts/net_interfacesforbond" } },
{ "script-net_interfacesforbridge", new [] { $"{RootFrameworkAntdShellScripts}/scripts/net_interfacesforbridge" } },
{ "script-net_interfacesforPPP", new [] { $"{RootFrameworkAntdShellScripts}/scripts/net_interfacesforPPP" } },
{ "script-net_list", new [] { $"{RootFrameworkAntdShellScripts}/scripts/net_list $type" } },
{ "script-netmask", new [] { $"{RootFrameworkAntdShellScripts}/scripts/netmask $cdir" } },
{ "script-netmask2cidr", new [] { $"{RootFrameworkAntdShellScripts}/scripts/netmask2cidr $subnet" } },
{ "script-net_rawviewinterface", new [] { $"{RootFrameworkAntdShellScripts}/scripts/net_rawviewinterface $interface" } },
{ "script-net_showinterface", new [] { $"{RootFrameworkAntdShellScripts}/scripts/net_showinterface $interface" } },
{ "script-net_updown", new [] { $"{RootFrameworkAntdShellScripts}/scripts/net_updown $obj $status" } },
{ "script-novlan_interfaces", new [] { $"{RootFrameworkAntdShellScripts}/scripts/novlan_interfaces $interface" } },
{ "script-ntpconfig", new [] { $"{RootFrameworkAntdShellScripts}/scripts/ntpconfig $client $server $serverlist" } },
{ "script-ntp_start", new [] { $"{RootFrameworkAntdShellScripts}/scripts/ntp_start" } },
{ "script-onlyRootRW", new [] { $"{RootFrameworkAntdShellScripts}/scripts/onlyRootRW" } },
{ "script-optionByLDAP", new [] { $"{RootFrameworkAntdShellScripts}/scripts/optionByLDAP $type" } },
{ "script-OVAddDomain", new [] { $"{RootFrameworkAntdShellScripts}/scripts/OVAddDomain $domain $type $radiusreq" } },
{ "script-ov_clients", new [] { $"{RootFrameworkAntdShellScripts}/scripts/ov_clients" } },
{ "script-ov_connect", new [] { $"{RootFrameworkAntdShellScripts}/scripts/ov_connect" } },
{ "script-ov_count", new [] { $"{RootFrameworkAntdShellScripts}/scripts/ov_count" } },
{ "script-OVDefaultDomain", new [] { $"{RootFrameworkAntdShellScripts}/scripts/OVDefaultDomain $domain" } },
{ "script-ov_disconnect", new [] { $"{RootFrameworkAntdShellScripts}/scripts/ov_disconnect" } },
{ "script-OVListDomains", new [] { $"{RootFrameworkAntdShellScripts}/scripts/OVListDomains" } },
{ "script-ov_pw_auth", new [] { $"{RootFrameworkAntdShellScripts}/scripts/ov_pw_auth" } },
{ "script-OVRemoveDomain", new [] { $"{RootFrameworkAntdShellScripts}/scripts/OVRemoveDomain $domain" } },
{ "script-ov_status", new [] { $"{RootFrameworkAntdShellScripts}/scripts/ov_status" } },
{ "script-patchlevel", new [] { $"{RootFrameworkAntdShellScripts}/scripts/patchlevel" } },
{ "script-pingcheck", new [] { $"{RootFrameworkAntdShellScripts}/scripts/pingcheck $host $packetsize" } },
{ "script-pppIP", new [] { $"{RootFrameworkAntdShellScripts}/scripts/pppIP" } },
{ "script-pppoe_auto", new [] { $"{RootFrameworkAntdShellScripts}/scripts/pppoe_auto" } },
{ "script-pppoe_config", new [] { $"{RootFrameworkAntdShellScripts}/scripts/pppoe_config $interface" } },
{ "script-pppoe_interfaces", new [] { $"{RootFrameworkAntdShellScripts}/scripts/pppoe_interfaces $options" } },
{ "script-pppoe_interfaces_multi", new [] { $"{RootFrameworkAntdShellScripts}/scripts/pppoe_interfaces_multi" } },
{ "script-pppoe_start", new [] { $"{RootFrameworkAntdShellScripts}/scripts/pppoe_start $interface" } },
{ "script-pppoe_stop", new [] { $"{RootFrameworkAntdShellScripts}/scripts/pppoe_stop $interface" } },
{ "script-pptp_start", new [] { $"{RootFrameworkAntdShellScripts}/scripts/pptp_start" } },
{ "script-pptp_status", new [] { $"{RootFrameworkAntdShellScripts}/scripts/pptp_status" } },
{ "script-pptp_stop", new [] { $"{RootFrameworkAntdShellScripts}/scripts/pptp_stop" } },
{ "script-previous/", new [] { $"{RootFrameworkAntdShellScripts}/scripts/previous/" } },
{ "script-profile-manager", new [] { $"{RootFrameworkAntdShellScripts}/scripts/profile-manager" } },
{ "script-proxyAddObject", new [] { $"{RootFrameworkAntdShellScripts}/scripts/proxyAddObject $action $interface $srcip $dstip" } },
{ "script-proxy_bg", new [] { $"{RootFrameworkAntdShellScripts}/scripts/proxy_bg" } },
{ "script-proxy_fw", new [] { $"{RootFrameworkAntdShellScripts}/scripts/proxy_fw" } },
{ "script-proxy_fw_reset", new [] { $"{RootFrameworkAntdShellScripts}/scripts/proxy_fw_reset" } },
{ "script-proxyObjectList", new [] { $"{RootFrameworkAntdShellScripts}/scripts/proxyObjectList" } },
{ "script-proxyRadiusConfig", new [] { $"{RootFrameworkAntdShellScripts}/scripts/proxyRadiusConfig" } },
{ "script-proxyRadiusTable", new [] { $"{RootFrameworkAntdShellScripts}/scripts/proxyRadiusTable" } },
{ "script-proxy_removeobject", new [] { $"{RootFrameworkAntdShellScripts}/scripts/proxy_removeobject $obj" } },
{ "script-proxy_save_list", new [] { $"{RootFrameworkAntdShellScripts}/scripts/proxy_save_list $list" } },
{ "script-proxy_start", new [] { $"{RootFrameworkAntdShellScripts}/scripts/proxy_start" } },
{ "script-proxy_status", new [] { $"{RootFrameworkAntdShellScripts}/scripts/proxy_status" } },
{ "script-proxy_stop", new [] { $"{RootFrameworkAntdShellScripts}/scripts/proxy_stop" } },
{ "script-qos_activatenewconfig", new [] { $"{RootFrameworkAntdShellScripts}/scripts/qos_activatenewconfig" } },
{ "script-qos_addclass", new [] { $"{RootFrameworkAntdShellScripts}/scripts/qos_addclass $interface $classname" } },
{ "script-qos_changeclass", new [] { $"{RootFrameworkAntdShellScripts}/scripts/qos_changeclass $interface $classname $local $prior $max $maxunit $guaranteed $guaranteedunit $dscp" } },
{ "script-qos_classestoadd", new [] { $"{RootFrameworkAntdShellScripts}/scripts/qos_classestoadd $classname" } },
{ "script-qos_classlist", new [] { $"{RootFrameworkAntdShellScripts}/scripts/qos_classlist $interface" } },
{ "script-qos_cm_changestatus", new [] { $"{RootFrameworkAntdShellScripts}/scripts/qos_cm_changestatus $name $status" } },
{ "script-qos_cm_delete", new [] { $"{RootFrameworkAntdShellScripts}/scripts/qos_cm_delete $name" } },
{ "script-qos_cm_list", new [] { $"{RootFrameworkAntdShellScripts}/scripts/qos_cm_list $classname" } },
{ "script-qos_cm_save", new [] { $"{RootFrameworkAntdShellScripts}/scripts/qos_cm_save $name $desc $max $maxunit $guaranteed $guaranteedunit $prior $dscp" } },
{ "script-qos_deleteclass", new [] { $"{RootFrameworkAntdShellScripts}/scripts/qos_deleteclass $interface $name" } },
{ "script-qos_detectSpeed", new [] { $"{RootFrameworkAntdShellScripts}/scripts/qos_detectSpeed $interface" } },
{ "script-qos_getbw", new [] { $"{RootFrameworkAntdShellScripts}/scripts/qos_getbw $interface $class $type $format $cm" } },
{ "script-qos_getdscp", new [] { $"{RootFrameworkAntdShellScripts}/scripts/qos_getdscp $interface $class" } },
{ "script-qos_getpriority", new [] { $"{RootFrameworkAntdShellScripts}/scripts/qos_getpriority $interface $class" } },
{ "script-qos_getstat", new [] { $"{RootFrameworkAntdShellScripts}/scripts/qos_getstat $interface $class" } },
{ "script-qos_ison", new [] { $"{RootFrameworkAntdShellScripts}/scripts/qos_ison $interface $class" } },
{ "script-qos_list", new [] { $"{RootFrameworkAntdShellScripts}/scripts/qos_list $type" } },
{ "script-qos_notsaved", new [] { $"{RootFrameworkAntdShellScripts}/scripts/qos_notsaved $interface $class" } },
{ "script-qos_printformat", new [] { $"{RootFrameworkAntdShellScripts}/scripts/qos_printformat $kbit" } },
{ "script-qos_printpriority", new [] { $"{RootFrameworkAntdShellScripts}/scripts/qos_printpriority $prio" } },
{ "script-qos_saveneeded", new [] { $"{RootFrameworkAntdShellScripts}/scripts/qos_saveneeded" } },
{ "script-qos_selectinterface", new [] { $"{RootFrameworkAntdShellScripts}/scripts/qos_selectinterface $interface" } },
{ "script-qos_setinterface", new [] { $"{RootFrameworkAntdShellScripts}/scripts/qos_setinterface $interface" } },
{ "script-qos_showinterface", new [] { $"{RootFrameworkAntdShellScripts}/scripts/qos_showinterface$interface" } },
{ "script-qos_statistics", new [] { $"{RootFrameworkAntdShellScripts}/scripts/qos_statistics $interface" } },
{ "script-qos_tot_guaranteed", new [] { $"{RootFrameworkAntdShellScripts}/scripts/qos_tot_guaranteed $interface $name" } },
{ "script-radiusacct", new [] { $"{RootFrameworkAntdShellScripts}/scripts/radiusacct" } },
{ "script-radiusauth", new [] { $"{RootFrameworkAntdShellScripts}/scripts/radiusauth $username $pwd $radiusreq $ip" } },
{ "script-radius_isProxiable", new [] { $"{RootFrameworkAntdShellScripts}/scripts/radius_isProxiable $domain" } },
{ "script-RadiusLog", new [] { $"{RootFrameworkAntdShellScripts}/scripts/RadiusLog" } },
{ "script-radius_proxyadd", new [] { $"{RootFrameworkAntdShellScripts}/scripts/radius_proxyadd $realm $server $authport $accport $secret $nostrip $lb $type $acc" } },
{ "script-radius_proxydel", new [] { $"{RootFrameworkAntdShellScripts}/scripts/radius_proxydel $realm $numserver" } },
{ "script-radius-session-limits", new [] { $"{RootFrameworkAntdShellScripts}/scripts/radius-session-limits" } },
{ "script-radius_start", new [] { $"{RootFrameworkAntdShellScripts}/scripts/radius_start" } },
{ "script-radius_status", new [] { $"{RootFrameworkAntdShellScripts}/scripts/radius_status" } },
{ "script-radius_stop", new [] { $"{RootFrameworkAntdShellScripts}/scripts/radius_stop" } },
{ "script-readvalue", new [] { $"{RootFrameworkAntdShellScripts}/scripts/readvalue" } },
{ "script-rebootRequired", new [] { $"{RootFrameworkAntdShellScripts}/scripts/rebootRequired" } },
{ "script-release", new [] { $"{RootFrameworkAntdShellScripts}/scripts/release" } },
{ "script-remote_addr", new [] { $"{RootFrameworkAntdShellScripts}/scripts/remote_addr" } },
{ "script-remove3G", new [] { $"{RootFrameworkAntdShellScripts}/scripts/remove3G $name" } },
{ "script-removebond", new [] { $"{RootFrameworkAntdShellScripts}/scripts/removebond $name" } },
{ "script-removebridge", new [] { $"{RootFrameworkAntdShellScripts}/scripts/removebridge $name" } },
{ "script-removeeth", new [] { $"{RootFrameworkAntdShellScripts}/scripts/removeeth $name" } },
{ "script-removeip", new [] { $"{RootFrameworkAntdShellScripts}/scripts/removeip $interface $obj" } },
{ "script-removejob", new [] { $"{RootFrameworkAntdShellScripts}/scripts/removejob $script" } },
{ "script-removePPP", new [] { $"{RootFrameworkAntdShellScripts}/scripts/removePPP $name" } },
{ "script-removetrust", new [] { $"{RootFrameworkAntdShellScripts}/scripts/removetrust $realm" } },
{ "script-removevlan", new [] { $"{RootFrameworkAntdShellScripts}/scripts/removevlan $interface $tag" } },
{ "script-removevpn", new [] { $"{RootFrameworkAntdShellScripts}/scripts/removevpn $name" } },
{ "script-rendertemplate", new [] { $"{RootFrameworkAntdShellScripts}/scripts/rendertemplate $file" } },
{ "script-resetvlans", new [] { $"{RootFrameworkAntdShellScripts}/scripts/resetvlans $interface" } },
{ "script-rip_getconfig", new [] { $"{RootFrameworkAntdShellScripts}/scripts/rip_getconfig $conf" } },
{ "script-rip_interfacesforRIP", new [] { $"{RootFrameworkAntdShellScripts}/scripts/rip_interfacesforRIP" } },
{ "script-rip_interfacesRIPPED", new [] { $"{RootFrameworkAntdShellScripts}/scripts/rip_interfacesRIPPED" } },
{ "script-rip_listlearned", new [] { $"{RootFrameworkAntdShellScripts}/scripts/rip_listlearned $by" } },
{ "script-rip_start", new [] { $"{RootFrameworkAntdShellScripts}/scripts/rip_start" } },
{ "script-rmbonding", new [] { $"{RootFrameworkAntdShellScripts}/scripts/rmbonding $file" } },
{ "script-rm_empty_lines", new [] { $"{RootFrameworkAntdShellScripts}/scripts/rm_empty_lines" } },
{ "script-router_addpat", new [] { $"{RootFrameworkAntdShellScripts}/scripts/router_addpat $interface $virtualip $protocol $localport $remoteip $remoteport" } },
{ "script-router_addstatic", new [] { $"{RootFrameworkAntdShellScripts}/scripts/router_addstatic $routepath $dest $netmask $via $gateway $metric $interface $mode" } },
{ "script-router_changestatic", new [] { $"{RootFrameworkAntdShellScripts}/scripts/router_changestatic $routepath $dest $netmask $via $gateway $metric $interface $entry" } },
{ "script-routerconfig", new [] { $"{RootFrameworkAntdShellScripts}/scripts/routerconfig" } },
{ "script-router_deletestatic", new [] { $"{RootFrameworkAntdShellScripts}/scripts/router_deletestatic $entry" } },
{ "script-router_delpat", new [] { $"{RootFrameworkAntdShellScripts}/scripts/router_delpat $entry" } },
{ "script-router_getroutestate", new [] { $"{RootFrameworkAntdShellScripts}/scripts/router_getroutestate" } },
{ "script-router_interfacesforNAT", new [] { $"{RootFrameworkAntdShellScripts}/scripts/router_interfacesforNAT" } },
{ "script-router_interfacesforPAT", new [] { $"{RootFrameworkAntdShellScripts}/scripts/router_interfacesforPAT" } },
{ "script-router_interfacesNAT", new [] { $"{RootFrameworkAntdShellScripts}/scripts/router_interfacesNAT" } },
{ "script-router_liststatic", new [] { $"{RootFrameworkAntdShellScripts}/scripts/router_liststatic" } },
{ "script-router_natcheck", new [] { $"{RootFrameworkAntdShellScripts}/scripts/router_natcheck $interface" } },
{ "script-router_natconfig", new [] { $"{RootFrameworkAntdShellScripts}/scripts/router_natconfig $natlist" } },
{ "script-router_natview", new [] { $"{RootFrameworkAntdShellScripts}/scripts/router_natview $type" } },
{ "script-router_patconfig", new [] { $"{RootFrameworkAntdShellScripts}/scripts/router_patconfig $entry" } },
{ "script-router_patlist", new [] { $"{RootFrameworkAntdShellScripts}/scripts/router_patlist" } },
{ "script-router_showroutetable", new [] { $"{RootFrameworkAntdShellScripts}/scripts/router_showroutetable $by" } },
{ "script-routeupd", new [] { $"{RootFrameworkAntdShellScripts}/scripts/routeupd $gw $opt" } },
{ "script-routeupd_start", new [] { $"{RootFrameworkAntdShellScripts}/scripts/routeupd_start" } },
{ "script-runscript", new [] { $"{RootFrameworkAntdShellScripts}/scripts/runscript $scriptname $opt" } },
{ "script-savefile", new [] { $"{RootFrameworkAntdShellScripts}/scripts/savefile $file" } },
{ "script-select_dnsclients_allow", new [] { $"{RootFrameworkAntdShellScripts}/scripts/select_dnsclients_allow" } },
{ "script-select_forwarder", new [] { $"{RootFrameworkAntdShellScripts}/scripts/select_forwarder" } },
{ "script-SelectHostCert", new [] { $"{RootFrameworkAntdShellScripts}/scripts/SelectHostCert $tls $certype $certselected" } },
{ "script-select_https_allow", new [] { $"{RootFrameworkAntdShellScripts}/scripts/select_https_allow" } },
{ "script-select_ntpserver", new [] { $"{RootFrameworkAntdShellScripts}/scripts/select_ntpserver" } },
{ "script-select_realm", new [] { $"{RootFrameworkAntdShellScripts}/scripts/select_realm" } },
{ "script-select_script", new [] { $"{RootFrameworkAntdShellScripts}/scripts/select_script" } },
{ "script-select_ssh_allow", new [] { $"{RootFrameworkAntdShellScripts}/scripts/select_ssh_allow" } },
{ "script-select_trust", new [] { $"{RootFrameworkAntdShellScripts}/scripts/select_trust" } },
{ "script-select_tz", new [] { $"{RootFrameworkAntdShellScripts}/scripts/select_tz" } },
{ "script-select_vpn_net", new [] { $"{RootFrameworkAntdShellScripts}/scripts/select_vpn_net" } },
{ "script-sendmail", new [] { $"{RootFrameworkAntdShellScripts}/scripts/sendmail $recipient $subject" } },
{ "script-sendsms", new [] { $"{RootFrameworkAntdShellScripts}/scripts/sendsms $to" } },
{ "script-server_name", new [] { $"{RootFrameworkAntdShellScripts}/scripts/server_name" } },
{ "script-setAdminSession", new [] { $"{RootFrameworkAntdShellScripts}/scripts/setAdminSession" } },
{ "script-setautoupdate", new [] { $"{RootFrameworkAntdShellScripts}/scripts/setautoupdate $status $every $autoinstall $lang" } },
{ "script-setautoupdatesetup", new [] { $"{RootFrameworkAntdShellScripts}/scripts/setautoupdatesetup $reboot $isecurity $ibug $iaddon $ifeature $irelease $usecurity $ubug $uaddon $ufeature" } },
{ "script-setbond", new [] { $"{RootFrameworkAntdShellScripts}/scripts/setbond $name" } },
{ "script-setbondedstatus", new [] { $"{RootFrameworkAntdShellScripts}/scripts/setbondedstatus $interface $status" } },
{ "script-setbridge", new [] { $"{RootFrameworkAntdShellScripts}/scripts/setbridge $name" } },
{ "script-setddns", new [] { $"{RootFrameworkAntdShellScripts}/scripts/setddns $enabled $provider $host $domain $username $pw" } },
{ "script-set_dhclient", new [] { $"{RootFrameworkAntdShellScripts}/scripts/set_dhclient $interface $op $opt" } },
{ "script-setfeature", new [] { $"{RootFrameworkAntdShellScripts}/scripts/setfeature $feature $key" } },
{ "script-sethttps", new [] { $"{RootFrameworkAntdShellScripts}/scripts/sethttps $acl $http $https $local" } },
{ "script-setinterface", new [] { $"{RootFrameworkAntdShellScripts}/scripts/setinterface $interfacename" } },
{ "script-setinterfaceforbond", new [] { $"{RootFrameworkAntdShellScripts}/scripts/setinterfaceforbond $interfacename" } },
{ "script-setinterfaceforbridge", new [] { $"{RootFrameworkAntdShellScripts}/scripts/setinterfaceforbridge $interfacename" } },
{ "script-setkey", new [] { $"{RootFrameworkAntdShellScripts}/scripts/setkey $key" } },
{ "script-setldap", new [] { $"{RootFrameworkAntdShellScripts}/scripts/setldap $ldapactive $ldapsactive $nisactive $autosync $addomain $domaincontroller $adou $adpw" } },
{ "script-SetLinkToCert", new [] { $"{RootFrameworkAntdShellScripts}/scripts/SetLinkToCert $tlsconfig $certtype $certselected" } },
{ "script-setpathcost", new [] { $"{RootFrameworkAntdShellScripts}/scripts/setpathcost $interface $pathcost" } },
{ "script-setpppgw", new [] { $"{RootFrameworkAntdShellScripts}/scripts/setpppgw $ppp" } },
{ "script-setssh", new [] { $"{RootFrameworkAntdShellScripts}/scripts/setssh $enabled $acl" } },
{ "script-set_startup", new [] { $"{RootFrameworkAntdShellScripts}/scripts/set_startup $script $enabled" } },
{ "script-set-txpower", new [] { $"{RootFrameworkAntdShellScripts}/scripts/set-txpower $alias $dbm" } },
{ "script-setvlans", new [] { $"{RootFrameworkAntdShellScripts}/scripts/setvlans $interface" } },
{ "script-set-wifi", new [] { $"{RootFrameworkAntdShellScripts}/scripts/set-wifi $alias $opt" } },
{ "script-shib_checkconfig", new [] { $"{RootFrameworkAntdShellScripts}/scripts/shib_checkconfig" } },
{ "script-shib_createfilter", new [] { $"{RootFrameworkAntdShellScripts}/scripts/shib_createfilter" } },
{ "script-shib_create_ipt", new [] { $"{RootFrameworkAntdShellScripts}/scripts/shib_create_ipt" } },
{ "script-shibd_status", new [] { $"{RootFrameworkAntdShellScripts}/scripts/shibd_status" } },
{ "script-shib_filterstatus", new [] { $"{RootFrameworkAntdShellScripts}/scripts/shib_filterstatus" } },
{ "script-shib_freeIdP", new [] { $"{RootFrameworkAntdShellScripts}/scripts/shib_freeIdP" } },
{ "script-shib_getspmetadata", new [] { $"{RootFrameworkAntdShellScripts}/scripts/shib_getspmetadata" } },
{ "script-shib_mkwhitelist", new [] { $"{RootFrameworkAntdShellScripts}/scripts/shib_mkwhitelist" } },
{ "script-shib_printenv", new [] { $"{RootFrameworkAntdShellScripts}/scripts/shib_printenv" } },
{ "script-shib_release", new [] { $"{RootFrameworkAntdShellScripts}/scripts/shib_release" } },
{ "script-shibupload", new [] { $"{RootFrameworkAntdShellScripts}/scripts/shibupload" } },
{ "script-shib_x509_prepare", new [] { $"{RootFrameworkAntdShellScripts}/scripts/shib_x509_prepare" } },
{ "script-shortHostname", new [] { $"{RootFrameworkAntdShellScripts}/scripts/shortHostname" } },
{ "script-show_dhclientip", new [] { $"{RootFrameworkAntdShellScripts}/scripts/show_dhclientip $interface" } },
{ "script-showinterfacestxt", new [] { $"{RootFrameworkAntdShellScripts}/scripts/showinterfacestxt" } },
{ "script-show-wifi-config", new [] { $"{RootFrameworkAntdShellScripts}/scripts/show-wifi-config $interface" } },
{ "script-slapdrestart", new [] { $"{RootFrameworkAntdShellScripts}/scripts/slapdrestart" } },
{ "script-slot_console_list", new [] { $"{RootFrameworkAntdShellScripts}/scripts/slot_console_list" } },
{ "script-slot_desc", new [] { $"{RootFrameworkAntdShellScripts}/scripts/slot_desc $slot" } },
{ "script-slot_kernel_list", new [] { $"{RootFrameworkAntdShellScripts}/scripts/slot_kernel_list $slot $opt" } },
{ "script-slot_list", new [] { $"{RootFrameworkAntdShellScripts}/scripts/slot_list" } },
{ "script-slot_mkgrub", new [] { $"{RootFrameworkAntdShellScripts}/scripts/slot_mkgrub" } },
{ "script-slot_parameters", new [] { $"{RootFrameworkAntdShellScripts}/scripts/slot_parameters $slot" } },
{ "script-slot_save", new [] { $"{RootFrameworkAntdShellScripts}/scripts/slot_save $slot $console $kernel $parameter $kernel2 $parameter2" } },
{ "script-slot_status", new [] { $"{RootFrameworkAntdShellScripts}/scripts/slot_status $slot" } },
{ "script-spaceAvailable", new [] { $"{RootFrameworkAntdShellScripts}/scripts/spaceAvailable" } },
{ "script-spoolerd", new [] { $"{RootFrameworkAntdShellScripts}/scripts/spoolerd" } },
{ "script-spooler_waiting", new [] { $"{RootFrameworkAntdShellScripts}/scripts/spooler_waiting" } },
{ "script-ssh_start", new [] { $"{RootFrameworkAntdShellScripts}/scripts/ssh_start" } },
{ "script-startautoupdater", new [] { $"{RootFrameworkAntdShellScripts}/scripts/startautoupdater" } },
{ "script-startservices", new [] { $"{RootFrameworkAntdShellScripts}/scripts/startservices" } },
{ "script-start-wifi", new [] { $"{RootFrameworkAntdShellScripts}/scripts/start-wifi $opt" } },
{ "script-StatusHostCert", new [] { $"{RootFrameworkAntdShellScripts}/scripts/StatusHostCert $tlsconfig $certtype $certselected" } },
{ "script-stopservices", new [] { $"{RootFrameworkAntdShellScripts}/scripts/stopservices" } },
{ "script-storage", new [] { $"{RootFrameworkAntdShellScripts}/scripts/storage $opt" } },
{ "script-storage_netDB", new [] { $"{RootFrameworkAntdShellScripts}/scripts/storage_netDB" } },
{ "script-sysinfo", new [] { $"{RootFrameworkAntdShellScripts}/scripts/sysinfo" } },
{ "script-syslog_start", new [] { $"{RootFrameworkAntdShellScripts}/scripts/syslog_start" } },
{ "script-tableFixesA", new [] { $"{RootFrameworkAntdShellScripts}/scripts/tableFixesA $type" } },
{ "script-tableFixesI", new [] { $"{RootFrameworkAntdShellScripts}/scripts/tableFixesI $type" } },
{ "script-tapcreate", new [] { $"{RootFrameworkAntdShellScripts}/scripts/tapcreate" } },
{ "script-terminate", new [] { $"{RootFrameworkAntdShellScripts}/scripts/terminate $process $retry" } },
{ "script-tick2time", new [] { $"{RootFrameworkAntdShellScripts}/scripts/tick2time $time" } },
{ "script-tracepathcheck", new [] { $"{RootFrameworkAntdShellScripts}/scripts/tracepathcheck $host" } },
{ "script-ts2date", new [] { $"{RootFrameworkAntdShellScripts}/scripts/ts2date" } },
{ "script-umountdb", new [] { $"{RootFrameworkAntdShellScripts}/scripts/umountdb" } },
{ "script-umountstorage", new [] { $"{RootFrameworkAntdShellScripts}/scripts/umountstorage" } },
{ "script-UploadFill", new [] { $"{RootFrameworkAntdShellScripts}/scripts/UploadFill" } },
{ "script-UploadImage", new [] { $"{RootFrameworkAntdShellScripts}/scripts/UploadImage $image" } },
{ "script-usb_modem_try.sh", new [] { $"{RootFrameworkAntdShellScripts}/scripts/usb_modem_try.sh" } },
{ "script-usenativeshib", new [] { $"{RootFrameworkAntdShellScripts}/scripts/usenativeshib" } },
{ "script-utilities", new [] { $"{RootFrameworkAntdShellScripts}/scripts/utilities" } },
{ "script-viewfile", new [] { $"{RootFrameworkAntdShellScripts}/scripts/viewfile $file" } },
{ "script-viewscript", new [] { $"{RootFrameworkAntdShellScripts}/scripts/viewscript $script $opt" } },
{ "script-VLANList", new [] { $"{RootFrameworkAntdShellScripts}/scripts/VLANList $interface $opt" } },
{ "script-vpn_checknbgw", new [] { $"{RootFrameworkAntdShellScripts}/scripts/vpn_checknbgw $vpn" } },
{ "script-vpn_checkUniquePort", new [] { $"{RootFrameworkAntdShellScripts}/scripts/vpn_checkUniquePort $port" } },
{ "script-vpnconfig", new [] { $"{RootFrameworkAntdShellScripts}/scripts/vpnconfig" } },
{ "script-vpn_count", new [] { $"{RootFrameworkAntdShellScripts}/scripts/vpn_count" } },
{ "script-vpn_ctl", new [] { $"{RootFrameworkAntdShellScripts}/scripts/vpn_ctl" } },
{ "script-vpn_genkey", new [] { $"{RootFrameworkAntdShellScripts}/scripts/vpn_genkey" } },
{ "script-vpn_getx509", new [] { $"{RootFrameworkAntdShellScripts}/scripts/vpn_getx509 $interface" } },
{ "script-vpn_list", new [] { $"{RootFrameworkAntdShellScripts}/scripts/vpn_list" } },
{ "script-vpn_localip", new [] { $"{RootFrameworkAntdShellScripts}/scripts/vpn_localip $vpn" } },
{ "script-vpn_log", new [] { $"{RootFrameworkAntdShellScripts}/scripts/vpn_log $interface" } },
{ "script-vpn_mii", new [] { $"{RootFrameworkAntdShellScripts}/scripts/vpn_mii $interface" } },
{ "script-vpn_restart_x509", new [] { $"{RootFrameworkAntdShellScripts}/scripts/vpn_restart_x509" } },
{ "script-vpnservice", new [] { $"{RootFrameworkAntdShellScripts}/scripts/vpnservice" } },
{ "script-vpn_setnet", new [] { $"{RootFrameworkAntdShellScripts}/scripts/vpn_setnet $setnet" } },
{ "script-vpn_start", new [] { $"{RootFrameworkAntdShellScripts}/scripts/vpn_start" } },
{ "script-vpn_x509CAList", new [] { $"{RootFrameworkAntdShellScripts}/scripts/vpn_x509CAList" } },
{ "script-vpn_x509ln", new [] { $"{RootFrameworkAntdShellScripts}/scripts/vpn_x509ln $interface $x509" } },
{ "script-vpn_x509_setCAs", new [] { $"{RootFrameworkAntdShellScripts}/scripts/vpn_x509_setCAs $list" } },
{ "script-web_shutdown", new [] { $"{RootFrameworkAntdShellScripts}/scripts/web_shutdown $opt" } },
{ "script-wifi-checkpower", new [] { $"{RootFrameworkAntdShellScripts}/scripts/wifi-checkpower $opt" } },
{ "script-wifi-get-regdomain", new [] { $"{RootFrameworkAntdShellScripts}/scripts/wifi-get-regdomain $opt" } },
{ "script-wifi.inc", new [] { $"{RootFrameworkAntdShellScripts}/scripts/wifi.inc" } },
{ "script-wifi-list-regdomain", new [] { $"{RootFrameworkAntdShellScripts}/scripts/wifi-list-regdomain $opt" } },
{ "script-wifi-list-station", new [] { $"{RootFrameworkAntdShellScripts}/scripts/wifi-list-station $interface" } },
{ "script-wifi-manager", new [] { $"{RootFrameworkAntdShellScripts}/scripts/wifi-manager" } },
{ "script-wifi-scanning", new [] { $"{RootFrameworkAntdShellScripts}/scripts/wifi-scanning $xdev" } },
{ "script-x509_cert2pem", new [] { $"{RootFrameworkAntdShellScripts}/scripts/x509_cert2pem" } },
{ "script-x509_createAdminCert", new [] { $"{RootFrameworkAntdShellScripts}/scripts/x509_createAdminCert" } },
{ "script-x509_createDefaultCA", new [] { $"{RootFrameworkAntdShellScripts}/scripts/x509_createDefaultCA" } },
{ "script-x509_createDefaultCert", new [] { $"{RootFrameworkAntdShellScripts}/scripts/x509_createDefaultCert" } },
{ "script-x509_listcerts", new [] { $"{RootFrameworkAntdShellScripts}/scripts/x509_listcerts" } },
{ "script-x509_reset", new [] { $"{RootFrameworkAntdShellScripts}/scripts/x509_reset" } },
{ "script-x509_verify", new [] { $"{RootFrameworkAntdShellScripts}/scripts/x509_verify $pemfile $checkcrl" } },
            };
            return dict;
        }

        private static Dictionary<string, string[]> GetApiDict() {
            var dict = new Dictionary<string, string[]> {
{ "api-BOND_STATUS", new [] { $"{RootFrameworkAntdShellScripts}/api/BOND_STATUS" } },
{ "api-DISKSPACE", new [] { $"{RootFrameworkAntdShellScripts}/api/DISKSPACE" } },
{ "api-GWINFO", new [] { $"{RootFrameworkAntdShellScripts}/api/GWINFO" } },
{ "api-KERNEL", new [] { $"{RootFrameworkAntdShellScripts}/api/KERNEL" } },
{ "api-LOAD15", new [] { $"{RootFrameworkAntdShellScripts}/api/LOAD15" } },
{ "api-MEMINFO", new [] { $"{RootFrameworkAntdShellScripts}/api/MEMINFO" } },
{ "api-NCORES", new [] { $"{RootFrameworkAntdShellScripts}/api/NCORES" } },
{ "api-SLOT", new [] { $"{RootFrameworkAntdShellScripts}/api/SLOT" } },
{ "api-SYSTEM_STARTED", new [] { $"{RootFrameworkAntdShellScripts}/api/SYSTEM_STARTED" } },
{ "api-UPTIME", new [] { $"{RootFrameworkAntdShellScripts}/api/UPTIME" } },
{ "api-VPN_STATUS", new [] { $"{RootFrameworkAntdShellScripts}/api/VPN_STATUS" } },
{ "api-ZEROSHELL", new [] { $"{RootFrameworkAntdShellScripts}/api/ZEROSHELL" } },
            };
            return dict;
        }
    }
}
