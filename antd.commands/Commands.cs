using System.Collections.Generic;

namespace antd.commands {
    public class Commands {

        public static Dictionary<string, string[]> List => GetDict();
        public static Dictionary<string, string[]> ScriptList => GetScriptDict();
        public static Dictionary<string, string[]> ApiList => GetApiDict();

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

        private static Dictionary<string, string[]> GetScriptDict() {
            var dict = new Dictionary<string, string[]> {
                { "3Gconnect", new [] { "$param" } },
 { "3Gconnect_stop", new [] { "$param" } },
 { "3g_tty_list", new [] { "$param" } },
 { "accounting_start", new [] { "$param" } },
 { "acct_AddClass", new [] { "$param" } },
 { "acct_addcredit", new [] { "$param" } },
 { "acct_ChangeClass", new [] { "$param" } },
 { "acctClassArray", new [] { "$param" } },
 { "acctClassList", new [] { "$param" } },
 { "acctd", new [] { "$param" } },
 { "acctDetails", new [] { "$param" } },
 { "acctDetails.pl", new [] { "$param" } },
 { "acct_enqueue", new [] { "$param" } },
 { "acct_enqueue_start", new [] { "$param" } },
 { "acct_enqueue_stop", new [] { "$param" } },
 { "acct_enqueue_update", new [] { "$param" } },
 { "acctEntries", new [] { "$param" } },
 { "acctEntries.pl", new [] { "$param" } },
 { "acct_interim_update", new [] { "$param" } },
 { "acct_isLimitsOK", new [] { "$param" } },
 { "acct_limit_msg", new [] { "$param" } },
 { "acct_process_requests", new [] { "$param" } },
 { "acct_RemoveClass", new [] { "$param" } },
 { "acct_removeentry", new [] { "$param" } },
 { "acct_save", new [] { "$param" } },
 { "acctSelectClass", new [] { "$param" } },
 { "acct_start", new [] { "$param" } },
 { "acct_status", new [] { "$param" } },
 { "acct_stop", new [] { "$param" } },
 { "acct_store", new [] { "$param" } },
 { "acct_userlimits", new [] { "$param" } },
 { "acct_writelimits", new [] { "$param" } },
 { "activatedb", new [] { "$param" } },
 { "activatefeature_confirm", new [] { "$param" } },
 { "activatesubscription_confirm", new [] { "$param" } },
 { "activation_msg", new [] { "$param" } },
 { "addip", new [] { "$param" } },
 { "addtrust", new [] { "$param" } },
 { "alert", new [] { "$param" } },
 { "alert_msg", new [] { "$param" } },
 { "alerts_changestatus", new [] { "$param" } },
 { "alerts_event_list", new [] { "$param" } },
 { "alerts_event_names", new [] { "$param" } },
 { "alerts_getstate", new [] { "$param" } },
 { "alerts_logger", new [] { "$param" } },
 { "alerts_mailrecipient", new [] { "$param" } },
 { "alerts_mk_default_events", new [] { "$param" } },
 { "alert_sms_status", new [] { "$param" } },
 { "alert_smtp_status", new [] { "$param" } },
 { "alerts_number", new [] { "$param" } },
 { "alert_spool", new [] { "$param" } },
 { "alerts_recipient_delete", new [] { "$param" } },
 { "alerts_recipient_desc", new [] { "$param" } },
 { "alerts_recipient_list", new [] { "$param" } },
 { "alerts_recipient_save", new [] { "$param" } },
 { "alerts_removeevent", new [] { "$param" } },
 { "alerts_resetevent", new [] { "$param" } },
 { "alerts_s2p", new [] { "$param" } },
 { "alerts_setstate", new [] { "$param" } },
 { "alerts_smsrecipient", new [] { "$param" } },
 { "alerts_start", new [] { "$param" } },
 { "alerts_status_sx", new [] { "$param" } },
 { "alert_status", new [] { "$param" } },
 { "alerts_test_logs", new [] { "$param" } },
 { "alerts_testscript", new [] { "$param" } },
 { "alerts_test_title", new [] { "$param" } },
 { "alerts_viewer", new [] { "$param" } },
 { "alerts_viewscript", new [] { "$param" } },
 { "arpcheck", new [] { "$param" } },
 { "autoupdate-check-incompatible", new [] { "$param" } },
 { "autoupdate-checkInstallerActivity", new [] { "$param" } },
 { "autoupdate-check-prerequisites", new [] { "$param" } },
 { "autoupdate-cleanup", new [] { "$param" } },
 { "autoupdate-elapsed", new [] { "$param" } },
 { "autoupdate-error", new [] { "$param" } },
 { "autoupdateGetField", new [] { "$param" } },
 { "autoupdateGetSize", new [] { "$param" } },
 { "autoupdate-installer", new [] { "$param" } },
 { "autoupdate-installer-bg", new [] { "$param" } },
 { "autoupdate-logger", new [] { "$param" } },
 { "autoupdateOperations", new [] { "$param" } },
 { "autoupdate-output", new [] { "$param" } },
 { "autoupdate-output-finished", new [] { "$param" } },
 { "autoupdater", new [] { "$param" } },
 { "autoupdate-sub-status", new [] { "$param" } },
 { "autoupdateTypeDesc", new [] { "$param" } },
 { "availableInterfaces", new [] { "$param" } },
 { "availableNetwork", new [] { "$param" } },
 { "backupdb", new [] { "$param" } },
 { "bckfilename", new [] { "$param" } },
 { "bitrate", new [] { "$param" } },
 { "bondstart", new [] { "$param" } },
 { "bondwatch", new [] { "$param" } },
 { "bwd_selectinterface", new [] { "$param" } },
 { "bwd_start", new [] { "$param" } },
 { "bwd_status", new [] { "$param" } },
 { "bw_subnets", new [] { "$param" } },
 { "change_c_year", new [] { "$param" } },
 { "changetime", new [] { "$param" } },
 { "changeUTC", new [] { "$param" } },
 { "chat.sh", new [] { "$param" } },
 { "checkk5realm", new [] { "$param" } },
 { "checkldapbase", new [] { "$param" } },
 { "checkNetworkOverlap", new [] { "$param" } },
 { "checkrip", new [] { "$param" } },
 { "checkUpdates", new [] { "$param" } },
 { "checkvpn", new [] { "$param" } },
 { "clamav_resetdb", new [] { "$param" } },
 { "clamav_update", new [] { "$param" } },
 { "ClamAV-Update", new [] { "$param" } },
 { "clamav_update_status", new [] { "$param" } },
 { "cleantmp", new [] { "$param" } },
 { "clearAdminSession", new [] { "$param" } },
 { "cntop", new [] { "$param" } },
 { "cntop.sh", new [] { "$param" } },
 { "config3G", new [] { "$param" } },
 { "configbond", new [] { "$param" } },
 { "configbridge", new [] { "$param" } },
 { "configPPP", new [] { "$param" } },
 { "configRIP", new [] { "$param" } },
 { "configtz", new [] { "$param" } },
 { "configvpn", new [] { "$param" } },
 { "connTrackFlush", new [] { "$param" } },
 { "connTrackList", new [] { "$param" } },
 { "countrycode2description", new [] { "$param" } },
 { "cp_acct_start", new [] { "$param" } },
 { "cp_acct_stop", new [] { "$param" } },
 { "cp_activatechain", new [] { "$param" } },
 { "cpAddClient", new [] { "$param" } },
 { "cpAddClientIPT", new [] { "$param" } },
 { "cpAddClient.orig", new [] { "$param" } },
 { "cpAddDomain", new [] { "$param" } },
 { "cpAddService", new [] { "$param" } },
 { "cpAddServiceIPT", new [] { "$param" } },
 { "cpart", new [] { "$param" } },
 { "cp_as-httpd.shib", new [] { "$param" } },
 { "cp_authorize_client", new [] { "$param" } },
 { "cp_auth_start", new [] { "$param" } },
 { "cp_auth_status", new [] { "$param" } },
 { "cp_auto", new [] { "$param" } },
 { "cpAutoDisconnect", new [] { "$param" } },
 { "cp_check_activity", new [] { "$param" } },
 { "cp_check_activity_wrapper", new [] { "$param" } },
 { "cp_checkauthenticator", new [] { "$param" } },
 { "cp_checkInterface", new [] { "$param" } },
 { "cp_connect", new [] { "$param" } },
 { "cpConnectedClientsIPT", new [] { "$param" } },
 { "cp_create_CRL_ipt", new [] { "$param" } },
 { "cp_createticket", new [] { "$param" } },
 { "cpDefaultDomain", new [] { "$param" } },
 { "cp_disconnect", new [] { "$param" } },
 { "cpFreeClientsIPT", new [] { "$param" } },
 { "cp_freeCRL", new [] { "$param" } },
 { "cpFreeServicesIPT", new [] { "$param" } },
 { "cp_getaccounting", new [] { "$param" } },
 { "cp_getRXTX", new [] { "$param" } },
 { "cp_getstatus", new [] { "$param" } },
 { "cp_interfaces", new [] { "$param" } },
 { "cp_interfaces_multi", new [] { "$param" } },
 { "cp_IP2MAC", new [] { "$param" } },
 { "cp_language", new [] { "$param" } },
 { "cpListConnected", new [] { "$param" } },
 { "cpListDomains", new [] { "$param" } },
 { "cpListDomainsOptions", new [] { "$param" } },
 { "cpListFreeClients", new [] { "$param" } },
 { "cpListFreeServices", new [] { "$param" } },
 { "cp_lmsg", new [] { "$param" } },
 { "cp_mkfreeCRL", new [] { "$param" } },
 { "cp_msg", new [] { "$param" } },
 { "cp_popup_exclusion", new [] { "$param" } },
 { "cpRemoveClient", new [] { "$param" } },
 { "cpRemoveClientIPT", new [] { "$param" } },
 { "cpRemoveDomain", new [] { "$param" } },
 { "cpRemoveService", new [] { "$param" } },
 { "cpRemoveServiceIPT", new [] { "$param" } },
 { "cp_renew", new [] { "$param" } },
 { "cp_rewrite_conds", new [] { "$param" } },
 { "cp_shib_lsconfig", new [] { "$param" } },
 { "cp_shib_release", new [] { "$param" } },
 { "cp_start", new [] { "$param" } },
 { "cp_template_source", new [] { "$param" } },
 { "cp_template_source_save", new [] { "$param" } },
 { "cpu_details", new [] { "$param" } },
 { "cpuinfo", new [] { "$param" } },
 { "cpu_model", new [] { "$param" } },
 { "cpus", new [] { "$param" } },
 { "cpw", new [] { "$param" } },
 { "cpwhelper", new [] { "$param" } },
 { "cpwldap", new [] { "$param" } },
 { "cp_x509CAList", new [] { "$param" } },
 { "cp_x509_setCAs", new [] { "$param" } },
 { "createK5REALM", new [] { "$param" } },
 { "createLDAPDB", new [] { "$param" } },
 { "create-profile", new [] { "$param" } },
 { "createvlan", new [] { "$param" } },
 { "createvpn", new [] { "$param" } },
 { "c_rehashCA", new [] { "$param" } },
 { "crlupdate", new [] { "$param" } },
 { "crontabgen", new [] { "$param" } },
 { "ctfilter", new [] { "$param" } },
 { "ctlogger", new [] { "$param" } },
 { "ctwatcher", new [] { "$param" } },
 { "daemonwatcher", new [] { "$param" } },
 { "ddns2", new [] { "$param" } },
 { "ddns-listregistered", new [] { "$param" } },
 { "deactivatedb", new [] { "$param" } },
 { "defaultbridge", new [] { "$param" } },
 { "defaultvalue", new [] { "$param" } },
 { "deletedb", new [] { "$param" } },
 { "deleteImported", new [] { "$param" } },
 { "deleteTrustedCA", new [] { "$param" } },
 { "deletevlan", new [] { "$param" } },
 { "delpart", new [] { "$param" } },
 { "dhcp_addstatic", new [] { "$param" } },
 { "dhcp_advancedopt", new [] { "$param" } },
 { "dhcp_checkopt", new [] { "$param" } },
 { "dhcp_configfile", new [] { "$param" } },
 { "dhcp_createconfig", new [] { "$param" } },
 { "dhcp_createsubnet", new [] { "$param" } },
 { "dhcp_editstatic", new [] { "$param" } },
 { "dhcp_getinterfaces", new [] { "$param" } },
 { "dhcp_liststatic", new [] { "$param" } },
 { "dhcp_removestatic", new [] { "$param" } },
 { "dhcp_removesubnet", new [] { "$param" } },
 { "dhcp_savesubnet", new [] { "$param" } },
 { "dhcp_showlease", new [] { "$param" } },
 { "dhcp_showleasestable", new [] { "$param" } },
 { "dhcp_start", new [] { "$param" } },
 { "dhcp_subnetenabled", new [] { "$param" } },
 { "dns_addForwarder", new [] { "$param" } },
 { "dns_addslaveserver", new [] { "$param" } },
 { "dns_addslavezone", new [] { "$param" } },
 { "dns_allow-query", new [] { "$param" } },
 { "dnscache", new [] { "$param" } },
 { "dns_changemaster", new [] { "$param" } },
 { "dns_clients_save", new [] { "$param" } },
 { "dns_disable", new [] { "$param" } },
 { "dns_enable", new [] { "$param" } },
 { "dns_hup", new [] { "$param" } },
 { "dns_iptables", new [] { "$param" } },
 { "dns_notifyserial", new [] { "$param" } },
 { "dns_query", new [] { "$param" } },
 { "dns_removeForwarder", new [] { "$param" } },
 { "dns_removeslaveserver", new [] { "$param" } },
 { "dns_removeslavezone", new [] { "$param" } },
 { "dns_removezone", new [] { "$param" } },
 { "dns_showslavezone", new [] { "$param" } },
 { "dns_slaveserversoptions", new [] { "$param" } },
 { "dns_slavezonesoptions", new [] { "$param" } },
 { "dns_start", new [] { "$param" } },
 { "dns_status", new [] { "$param" } },
 { "dns_transferzone", new [] { "$param" } },
 { "dns_zoneconfig", new [] { "$param" } },
 { "downloadUpdateList", new [] { "$param" } },
 { "editip", new [] { "$param" } },
 { "editvlan", new [] { "$param" } },
 { "enableforwarding", new [] { "$param" } },
 { "escape", new [] { "$param" } },
 { "ext_bondname", new [] { "$param" } },
 { "ext_bridgename", new [] { "$param" } },
 { "failoverd", new [] { "$param" } },
 { "failsafe", new [] { "$param" } },
 { "fpart", new [] { "$param" } },
 { "freebondnum", new [] { "$param" } },
 { "freebridgenum", new [] { "$param" } },
 { "fw_cancelchanges", new [] { "$param" } },
 { "fw_changepolicy", new [] { "$param" } },
 { "fw_cp_chain", new [] { "$param" } },
 { "fw_deleterule", new [] { "$param" } },
 { "fw_enablechain", new [] { "$param" } },
 { "fw_getpolicy", new [] { "$param" } },
 { "fw_https_chain", new [] { "$param" } },
 { "fw_initrules", new [] { "$param" } },
 { "fw_interfaces", new [] { "$param" } },
 { "fw_listaction", new [] { "$param" } },
 { "fw_listchains", new [] { "$param" } },
 { "fw_listrules", new [] { "$param" } },
 { "fw_makerule", new [] { "$param" } },
 { "fw_newchain", new [] { "$param" } },
 { "fw_protdes", new [] { "$param" } },
 { "fw_protocols", new [] { "$param" } },
 { "fw_removechain", new [] { "$param" } },
 { "fw_reorder", new [] { "$param" } },
 { "fw_savechain", new [] { "$param" } },
 { "fw_ssh_chain", new [] { "$param" } },
 { "fw_start", new [] { "$param" } },
 { "fw_sys_chain", new [] { "$param" } },
 { "fw_viewchain", new [] { "$param" } },
 { "getactivedb", new [] { "$param" } },
 { "getbinderror", new [] { "$param" } },
 { "getBondType", new [] { "$param" } },
 { "getdefaultgw", new [] { "$param" } },
 { "get_environment", new [] { "$param" } },
 { "getethoption", new [] { "$param" } },
 { "getfeature", new [] { "$param" } },
 { "getFixDeps", new [] { "$param" } },
 { "getFixInfo", new [] { "$param" } },
 { "getFixInfoShow", new [] { "$param" } },
 { "getFixInstalled", new [] { "$param" } },
 { "getFixStatus", new [] { "$param" } },
 { "gethttplan", new [] { "$param" } },
 { "getintdesc", new [] { "$param" } },
 { "getInterfaceWeight", new [] { "$param" } },
 { "getipdir", new [] { "$param" } },
 { "getkey", new [] { "$param" } },
 { "getlinkstatus", new [] { "$param" } },
 { "getreiserfslabel", new [] { "$param" } },
 { "get_salt", new [] { "$param" } },
 { "getSOA", new [] { "$param" } },
 { "gw_l2checkd", new [] { "$param" } },
 { "hwclock", new [] { "$param" } },
 { "iconfig", new [] { "$param" } },
 { "idpDiscovery", new [] { "$param" } },
 { "idpDiscoveryRoot", new [] { "$param" } },
 { "imageManagerLink", new [] { "$param" } },
 { "imagesize", new [] { "$param" } },
 { "importCAcert", new [] { "$param" } },
 { "importCAkey", new [] { "$param" } },
 { "importCertificate", new [] { "$param" } },
 { "importTrustedCA", new [] { "$param" } },
 { "importTrustedCRL", new [] { "$param" } },
 { "inst-manager", new [] { "$param" } },
 { "ip_belongs_network", new [] { "$param" } },
 { "ip-manager", new [] { "$param" } },
 { "ip_restart_daemon", new [] { "$param" } },
 { "ipsec_start", new [] { "$param" } },
 { "ipsec_status", new [] { "$param" } },
 { "ipsec_stop", new [] { "$param" } },
 { "iwconfig_watch", new [] { "$param" } },
 { "k5_addRealm", new [] { "$param" } },
 { "k5_createConf", new [] { "$param" } },
 { "k5defaultencryption", new [] { "$param" } },
 { "k5enclist", new [] { "$param" } },
 { "k5_isTrusted", new [] { "$param" } },
 { "k5_removeRealm", new [] { "$param" } },
 { "kadmind_start", new [] { "$param" } },
 { "krb5", new [] { "$param" } },
 { "l7listgroups", new [] { "$param" } },
 { "l7listprotocols", new [] { "$param" } },
 { "l7protocols-prep", new [] { "$param" } },
 { "l7view", new [] { "$param" } },
 { "lastUpdateCheck", new [] { "$param" } },
 { "lgwatcher", new [] { "$param" } },
 { "listImportedCerts", new [] { "$param" } },
 { "list_local_subnet", new [] { "$param" } },
 { "listTrustedCAs", new [] { "$param" } },
 { "loadmsgboard", new [] { "$param" } },
 { "localman", new [] { "$param" } },
 { "localman_activatedb", new [] { "$param" } },
 { "localman_bridge", new [] { "$param" } },
 { "localman_deactivatedb", new [] { "$param" } },
 { "localman_infodb", new [] { "$param" } },
 { "logreduce", new [] { "$param" } },
 { "log_syslog", new [] { "$param" } },
 { "lspart", new [] { "$param" } },
 { "make3G", new [] { "$param" } },
 { "makebond", new [] { "$param" } },
 { "makebridge", new [] { "$param" } },
 { "makedb", new [] { "$param" } },
 { "make_dbtpl", new [] { "$param" } },
 { "makePPP", new [] { "$param" } },
 { "man_page", new [] { "$param" } },
 { "mkbonddev", new [] { "$param" } },
 { "mountdefaultdb", new [] { "$param" } },
 { "mountstorage", new [] { "$param" } },
 { "mrtg_createcfg", new [] { "$param" } },
 { "mrtg_reload", new [] { "$param" } },
 { "mrtg_selectinterface", new [] { "$param" } },
 { "mrtgtarget", new [] { "$param" } },
 { "mrtgtarget2", new [] { "$param" } },
 { "mrtgtarget3", new [] { "$param" } },
 { "mrtgtarget4", new [] { "$param" } },
 { "mrtgtarget5", new [] { "$param" } },
 { "mrtgtarget6", new [] { "$param" } },
 { "nb_addfakegw", new [] { "$param" } },
 { "nb_addgw", new [] { "$param" } },
 { "nb_autoppp", new [] { "$param" } },
 { "nb_changegw", new [] { "$param" } },
 { "nb_connectivity", new [] { "$param" } },
 { "nb_copydefaultgw", new [] { "$param" } },
 { "nb_ct_pre", new [] { "$param" } },
 { "nb_enablegw", new [] { "$param" } },
 { "nb_fw", new [] { "$param" } },
 { "nb_gwlist", new [] { "$param" } },
 { "nb_ips_warning", new [] { "$param" } },
 { "nb_removegw", new [] { "$param" } },
 { "nb_save", new [] { "$param" } },
 { "nb_setautomarking", new [] { "$param" } },
 { "nb_setnexthop", new [] { "$param" } },
 { "nb_startfo", new [] { "$param" } },
 { "nb_statistics", new [] { "$param" } },
 { "nb_statusfo", new [] { "$param" } },
 { "nb_statusgw", new [] { "$param" } },
 { "nb_testfo", new [] { "$param" } },
 { "nb_vpn", new [] { "$param" } },
 { "nDPI_available", new [] { "$param" } },
 { "net_bondview", new [] { "$param" } },
 { "net_bridgeview", new [] { "$param" } },
 { "net_getfreeppp", new [] { "$param" } },
 { "net.inc", new [] { "$param" } },
 { "net_interfacesbonded", new [] { "$param" } },
 { "net_interfacesbridged", new [] { "$param" } },
 { "net_interfacesforbond", new [] { "$param" } },
 { "net_interfacesforbridge", new [] { "$param" } },
 { "net_interfacesforPPP", new [] { "$param" } },
 { "net_list", new [] { "$param" } },
 { "netmask", new [] { "$param" } },
 { "netmask2cidr", new [] { "$param" } },
 { "net_rawviewinterface", new [] { "$param" } },
 { "net_showinterface", new [] { "$param" } },
 { "net_updown", new [] { "$param" } },
 { "novlan_interfaces", new [] { "$param" } },
 { "ntpconfig", new [] { "$param" } },
 { "ntp_start", new [] { "$param" } },
 { "onlyRootRW", new [] { "$param" } },
 { "optionByLDAP", new [] { "$param" } },
 { "OVAddDomain", new [] { "$param" } },
 { "ov_clients", new [] { "$param" } },
 { "ov_connect", new [] { "$param" } },
 { "ov_count", new [] { "$param" } },
 { "OVDefaultDomain", new [] { "$param" } },
 { "ov_disconnect", new [] { "$param" } },
 { "OVListDomains", new [] { "$param" } },
 { "ov_pw_auth", new [] { "$param" } },
 { "OVRemoveDomain", new [] { "$param" } },
 { "ov_status", new [] { "$param" } },
 { "patchlevel", new [] { "$param" } },
 { "pingcheck", new [] { "$param" } },
 { "pppIP", new [] { "$param" } },
 { "pppoe_auto", new [] { "$param" } },
 { "pppoe_config", new [] { "$param" } },
 { "pppoe_interfaces", new [] { "$param" } },
 { "pppoe_interfaces_multi", new [] { "$param" } },
 { "pppoe_start", new [] { "$param" } },
 { "pppoe_stop", new [] { "$param" } },
 { "pptp_start", new [] { "$param" } },
 { "pptp_status", new [] { "$param" } },
 { "pptp_stop", new [] { "$param" } },
 { "previous", new [] { "$param" } },
 { "profile-manager", new [] { "$param" } },
 { "proxyAddObject", new [] { "$param" } },
 { "proxy_bg", new [] { "$param" } },
 { "proxy_fw", new [] { "$param" } },
 { "proxy_fw_reset", new [] { "$param" } },
 { "proxyObjectList", new [] { "$param" } },
 { "proxyRadiusConfig", new [] { "$param" } },
 { "proxyRadiusTable", new [] { "$param" } },
 { "proxy_removeobject", new [] { "$param" } },
 { "proxy_save_list", new [] { "$param" } },
 { "proxy_start", new [] { "$param" } },
 { "proxy_status", new [] { "$param" } },
 { "proxy_stop", new [] { "$param" } },
 { "qos_activatenewconfig", new [] { "$param" } },
 { "qos_addclass", new [] { "$param" } },
 { "qos_changeclass", new [] { "$param" } },
 { "qos_classestoadd", new [] { "$param" } },
 { "qos_classlist", new [] { "$param" } },
 { "qos_cm_changestatus", new [] { "$param" } },
 { "qos_cm_delete", new [] { "$param" } },
 { "qos_cm_list", new [] { "$param" } },
 { "qos_cm_save", new [] { "$param" } },
 { "qos_deleteclass", new [] { "$param" } },
 { "qos_detectSpeed", new [] { "$param" } },
 { "qos_getbw", new [] { "$param" } },
 { "qos_getdscp", new [] { "$param" } },
 { "qos_getpriority", new [] { "$param" } },
 { "qos_getstat", new [] { "$param" } },
 { "qos_ison", new [] { "$param" } },
 { "qos_list", new [] { "$param" } },
 { "qos_notsaved", new [] { "$param" } },
 { "qos_printformat", new [] { "$param" } },
 { "qos_printpriority", new [] { "$param" } },
 { "qos_saveneeded", new [] { "$param" } },
 { "qos_selectinterface", new [] { "$param" } },
 { "qos_setinterface", new [] { "$param" } },
 { "qos_showinterface", new [] { "$param" } },
 { "qos_statistics", new [] { "$param" } },
 { "qos_tot_guaranteed", new [] { "$param" } },
 { "radiusacct", new [] { "$param" } },
 { "radiusauth", new [] { "$param" } },
 { "radius_isProxiable", new [] { "$param" } },
 { "RadiusLog", new [] { "$param" } },
 { "radius_proxyadd", new [] { "$param" } },
 { "radius_proxydel", new [] { "$param" } },
 { "radius-session-limits", new [] { "$param" } },
 { "radius_start", new [] { "$param" } },
 { "radius_status", new [] { "$param" } },
 { "radius_stop", new [] { "$param" } },
 { "readvalue", new [] { "$param" } },
 { "rebootRequired", new [] { "$param" } },
 { "release", new [] { "$param" } },
 { "remote_addr", new [] { "$param" } },
 { "remove3G", new [] { "$param" } },
 { "removebond", new [] { "$param" } },
 { "removebridge", new [] { "$param" } },
 { "removeeth", new [] { "$param" } },
 { "removeip", new [] { "$param" } },
 { "removejob", new [] { "$param" } },
 { "removePPP", new [] { "$param" } },
 { "removetrust", new [] { "$param" } },
 { "removevlan", new [] { "$param" } },
 { "removevpn", new [] { "$param" } },
 { "rendertemplate", new [] { "$param" } },
 { "resetvlans", new [] { "$param" } },
 { "rip_getconfig", new [] { "$param" } },
 { "rip_interfacesforRIP", new [] { "$param" } },
 { "rip_interfacesRIPPED", new [] { "$param" } },
 { "rip_listlearned", new [] { "$param" } },
 { "rip_start", new [] { "$param" } },
 { "rmbonding", new [] { "$param" } },
 { "rm_empty_lines", new [] { "$param" } },
 { "router_addpat", new [] { "$param" } },
 { "router_addstatic", new [] { "$param" } },
 { "router_changestatic", new [] { "$param" } },
 { "routerconfig", new [] { "$param" } },
 { "router_deletestatic", new [] { "$param" } },
 { "router_delpat", new [] { "$param" } },
 { "router_getroutestate", new [] { "$param" } },
 { "router_interfacesforNAT", new [] { "$param" } },
 { "router_interfacesforPAT", new [] { "$param" } },
 { "router_interfacesNAT", new [] { "$param" } },
 { "router_liststatic", new [] { "$param" } },
 { "router_natcheck", new [] { "$param" } },
 { "router_natconfig", new [] { "$param" } },
 { "router_natview", new [] { "$param" } },
 { "router_patconfig", new [] { "$param" } },
 { "router_patlist", new [] { "$param" } },
 { "router_showroutetable", new [] { "$param" } },
 { "routeupd", new [] { "$param" } },
 { "routeupd_start", new [] { "$param" } },
 { "runscript", new [] { "$param" } },
 { "savefile", new [] { "$param" } },
 { "select_dnsclients_allow", new [] { "$param" } },
 { "select_forwarder", new [] { "$param" } },
 { "SelectHostCert", new [] { "$param" } },
 { "select_https_allow", new [] { "$param" } },
 { "select_ntpserver", new [] { "$param" } },
 { "select_realm", new [] { "$param" } },
 { "select_script", new [] { "$param" } },
 { "select_ssh_allow", new [] { "$param" } },
 { "select_trust", new [] { "$param" } },
 { "select_tz", new [] { "$param" } },
 { "select_vpn_net", new [] { "$param" } },
 { "sendmail", new [] { "$param" } },
 { "sendsms", new [] { "$param" } },
 { "server_name", new [] { "$param" } },
 { "setAdminSession", new [] { "$param" } },
 { "setautoupdate", new [] { "$param" } },
 { "setautoupdatesetup", new [] { "$param" } },
 { "setbond", new [] { "$param" } },
 { "setbondedstatus", new [] { "$param" } },
 { "setbridge", new [] { "$param" } },
 { "setddns", new [] { "$param" } },
 { "set_dhclient", new [] { "$param" } },
 { "setfeature", new [] { "$param" } },
 { "sethttps", new [] { "$param" } },
 { "setinterface", new [] { "$param" } },
 { "setinterfaceforbond", new [] { "$param" } },
 { "setinterfaceforbridge", new [] { "$param" } },
 { "setkey", new [] { "$param" } },
 { "setldap", new [] { "$param" } },
 { "SetLinkToCert", new [] { "$param" } },
 { "setpathcost", new [] { "$param" } },
 { "setpppgw", new [] { "$param" } },
 { "setssh", new [] { "$param" } },
 { "set_startup", new [] { "$param" } },
 { "set-txpower", new [] { "$param" } },
 { "setvlans", new [] { "$param" } },
 { "set-wifi", new [] { "$param" } },
 { "shib_checkconfig", new [] { "$param" } },
 { "shib_createfilter", new [] { "$param" } },
 { "shib_create_ipt", new [] { "$param" } },
 { "shibd_status", new [] { "$param" } },
 { "shib_filterstatus", new [] { "$param" } },
 { "shib_freeIdP", new [] { "$param" } },
 { "shib_getspmetadata", new [] { "$param" } },
 { "shib_mkwhitelist", new [] { "$param" } },
 { "shib_printenv", new [] { "$param" } },
 { "shib_release", new [] { "$param" } },
 { "shibupload", new [] { "$param" } },
 { "shib_x509_prepare", new [] { "$param" } },
 { "shortHostname", new [] { "$param" } },
 { "show_dhclientip", new [] { "$param" } },
 { "showinterfacestxt", new [] { "$param" } },
 { "show-wifi-config", new [] { "$param" } },
 { "slapdrestart", new [] { "$param" } },
 { "slot_console_list", new [] { "$param" } },
 { "slot_desc", new [] { "$param" } },
 { "slot_kernel_list", new [] { "$param" } },
 { "slot_list", new [] { "$param" } },
 { "slot_mkgrub", new [] { "$param" } },
 { "slot_parameters", new [] { "$param" } },
 { "slot_save", new [] { "$param" } },
 { "slot_status", new [] { "$param" } },
 { "spaceAvailable", new [] { "$param" } },
 { "spoolerd", new [] { "$param" } },
 { "spooler_waiting", new [] { "$param" } },
 { "ssh_start", new [] { "$param" } },
 { "startautoupdater", new [] { "$param" } },
 { "startservices", new [] { "$param" } },
 { "start-wifi", new [] { "$param" } },
 { "StatusHostCert", new [] { "$param" } },
 { "stopservices", new [] { "$param" } },
 { "storage", new [] { "$param" } },
 { "storage_netDB", new [] { "$param" } },
 { "sysinfo", new [] { "$param" } },
 { "syslog_start", new [] { "$param" } },
 { "tableFixesA", new [] { "$param" } },
 { "tableFixesI", new [] { "$param" } },
 { "tapcreate", new [] { "$param" } },
 { "terminate", new [] { "$param" } },
 { "tick2time", new [] { "$param" } },
 { "tracepathcheck", new [] { "$param" } },
 { "ts2date", new [] { "$param" } },
 { "umountdb", new [] { "$param" } },
 { "umountstorage", new [] { "$param" } },
 { "UploadFill", new [] { "$param" } },
 { "UploadImage", new [] { "$param" } },
 { "usb_modem_try.sh", new [] { "$param" } },
 { "usenativeshib", new [] { "$param" } },
 { "utilities", new [] { "$param" } },
 { "viewfile", new [] { "$param" } },
 { "viewscript", new [] { "$param" } },
 { "VLANList", new [] { "$param" } },
 { "vpn_checknbgw", new [] { "$param" } },
 { "vpn_checkUniquePort", new [] { "$param" } },
 { "vpnconfig", new [] { "$param" } },
 { "vpn_count", new [] { "$param" } },
 { "vpn_ctl", new [] { "$param" } },
 { "vpn_genkey", new [] { "$param" } },
 { "vpn_getx509", new [] { "$param" } },
 { "vpn_list", new [] { "$param" } },
 { "vpn_localip", new [] { "$param" } },
 { "vpn_log", new [] { "$param" } },
 { "vpn_mii", new [] { "$param" } },
 { "vpn_restart_x509", new [] { "$param" } },
 { "vpnservice", new [] { "$param" } },
 { "vpn_setnet", new [] { "$param" } },
 { "vpn_start", new [] { "$param" } },
 { "vpn_x509CAList", new [] { "$param" } },
 { "vpn_x509ln", new [] { "$param" } },
 { "vpn_x509_setCAs", new [] { "$param" } },
 { "web_shutdown", new [] { "$param" } },
 { "wifi-checkpower", new [] { "$param" } },
 { "wifi-get-regdomain", new [] { "$param" } },
 { "wifi.inc", new [] { "$param" } },
 { "wifi-list-regdomain", new [] { "$param" } },
 { "wifi-list-station", new [] { "$param" } },
 { "wifi-manager", new [] { "$param" } },
 { "wifi-scanning", new [] { "$param" } },
 { "x509_cert2pem", new [] { "$param" } },
 { "x509_createAdminCert", new [] { "$param" } },
 { "x509_createDefaultCA", new [] { "$param" } },
 { "x509_createDefaultCert", new [] { "$param" } },
 { "x509_listcerts", new [] { "$param" } },
 { "x509_reset", new [] { "$param" } },
 { "x509_verify", new [] { "$param" } }


            };
            return dict;
        }

        private static Dictionary<string, string[]> GetApiDict() {
            var dict = new Dictionary<string, string[]> {
                { "BOND_STATUS", new [] { "$param" }},
                { "DISKSPACE", new [] { "$param" }},
                { "GWINFO", new [] { "$param" }},
                { "KERNEL", new [] {"$param" }},
                { "LOAD15", new [] { "$param" }},
                { "MEMINFO", new [] { "$param" }},
                { "NCORES", new [] { "$param" }},
                { "SLOT", new [] { "$param" }},
                { "SYSTEM_STARTED", new [] { "$param" }},
                { "UPTIME", new [] { "$param" }},
                { "VPN_STATUS", new [] { "$param" }},
                { "ZEROSHELL", new [] { "$param" }}
            };
            return dict;
        }
    }
}
