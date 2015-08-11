#### IPROUTE2, BRIDGE/SWITCH (2xLAN) + 2xWAN ####

killall dhclient
systemctl stop systemd-networkd.service
systemctl stop systemd-networkd.socket
systemctl stop systemd-networkd.service

echo nameserver 192.168.111.1 > /etc/resolv.conf
echo nameserver 192.168.222.1 >> /etc/resolv.conf

ip link set up dev eth10
ip addr add 192.168.111.2/24 dev eth10
ip link set up dev eth11
ip addr add 192.168.222.2/24 dev eth11

ip route add default via 192.168.111.1

ip link set up dev eth0
ip link set up dev eth1
ip link set up dev eth2
ip link set up dev eth3
ip link set up dev eth4
ip link set up dev eth5
ip link set up dev eth6
ip link set up dev eth7
ip link set up dev eth8
ip link set up dev eth9
ip link set dev eth0 promisc on
ip link set dev eth1 promisc on
ip link set dev eth2 promisc on
ip link set dev eth3 promisc on
ip link set dev eth4 promisc on
ip link set dev eth5 promisc on
ip link set dev eth6 promisc on
ip link set dev eth7 promisc on
ip link set dev eth8 promisc on
ip link set dev eth9 promisc on
ip link set dev eth10 promisc on
ip link set dev eth11 promisc on

brctl addbr br0
ip link set up dev br0
brctl addif br0 eth0
brctl addif br0 eth1
brctl addif br0 eth2
brctl addif br0 eth3
brctl stp br0 off
ip addr add 10.99.19.1/16 dev br0
ip link set up dev br0

brctl addbr br1
ip link set up dev br1
brctl addif br1 eth4
brctl addif br1 eth5
brctl addif br1 eth6
brctl addif br1 eth7
brctl addif br0 eth8
brctl addif br0 eth9
brctl stp br1 off
ip addr add 10.1.19.1/16 dev br1
ip link set up dev br1

#### NFTABLE FIREWALL, NAT, ROUTER, BRIDGE/SWITCH, WITH 2 WAN NOT REDUNDANT ####

flush ruleset

table ip filter {
        chain input {
                 type filter hook input priority 0;
                 ip protocol icmp accept
                 ip protocol esp accept
                 ip protocol ah accept
                 ip protocol udp accept
                 udp dport { l2tp, ipsec-nat-t, isakmp} accept
                 ct state { established, related} accept
                 ct state invalid drop
                 iifname "lo" accept
                 iif lo ct state new accept
                 iifname { "eth6", "eth3", "eth0", "eth10", "eth5", "eth8", "br1", "br0", "eth9", "eth7", "eth2", "eth11", "eth1", "eth4"} accept
                 iif { eth1, eth2, br1, eth0, eth9, eth5, eth3, eth8, eth4, eth7, br0, eth6} ct state new accept
                 icmp type echo-request accept
                 tcp dport { ssh, http} accept
                 iifname "lo" accept
                 iifname { "br1", "br0"} tcp sport { ftp, ftp-data} ct state new accept
                 iifname { "br0", "br1"} ip protocol icmp accept
                 iifname { "br1", "br0"} accept
                 ip saddr { 127.0.0.1} iifname "lo" accept
                 ip saddr { 10.1.19.1} iifname "lo" accept
                 ip saddr { 10.1.0.0/24} ip daddr { 192.168.111.2} accept
                 ip saddr { 10.99.19.1} iifname "lo" accept
                 ip saddr { 10.99.0.0/24} ip daddr { 192.168.111.2} accept
                 ip saddr { 192.168.111.2} iifname "lo" accept
                 ip daddr { 192.168.111.2} tcp dport { http, https, 27000} accept
                 ip daddr { 192.168.111.2} ct state { established, related} accept
                 ip saddr { 192.168.222.2} iifname "lo" accept
                 ip daddr { 192.168.222.2} tcp dport { 27000, http, https} accept
                 ip daddr { 192.168.222.2} ct state { established, related} accept
                 drop
        }

        chain output {
                 type filter hook output priority 0;
                 ip protocol icmp accept
                 ct state { related, established} accept
                 ct state invalid drop
                 iifname "lo" accept
                 iif lo ct state new accept
                 iifname { "br1", "eth9", "eth11", "eth8", "eth4", "eth10", "eth7", "eth6", "eth5", "eth1", "br0", "eth0", "eth2", "eth3"} accept
                 iif { eth0, eth6, eth7, eth4, eth2, eth9, eth8, eth3, br0, eth5, eth1, eth11, br1, eth10} ct state new accept
                 icmp type echo-request accept
                 oif lo accept
                 oif { eth11, eth10} accept
                 ip protocol icmp accept
                 udp dport { l2tp, isakmp, ipsec-nat-t} accept
                 ip protocol { udp, esp, ah} accept
                 ip saddr { 10.1.19.1, 10.99.19.1} accept
                 ip saddr { 10.1.0.0/16, 10.99.0.0/16} accept
                 ip saddr { 192.168.111.2} accept
                 ip saddr { 192.168.222.2} accept
                 ip saddr { 127.0.0.1} accept
                 drop
        }

        chain forward {
                 type filter hook forward priority 0;
                 ct state { related, established} accept
                 ip protocol icmp accept
                 ct state invalid drop
                 iifname "lo" accept
                 iif lo ct state new accept
                 iifname { "eth2", "br1", "eth4", "eth11", "eth3", "eth6", "eth7", "eth9", "br0", "eth8", "eth1", "eth5", "eth0", "eth10"} accept
                 iif { eth7, eth0, eth6, eth5, eth8, eth2, eth3, eth9, br0, br1, eth1, eth4, eth11, eth10} ct state new accept
                 icmp type echo-request accept
                 ip protocol icmp accept
                 udp dport { isakmp, ipsec-nat-t, l2tp} accept
                 ip protocol { esp, gre, udp} accept
                 iifname { "eth7", "eth9", "eth4", "eth1", "eth6", "br1", "eth2", "eth5", "eth3", "eth0", "eth8", "br0"} oif { eth1, eth0, br1, eth2, eth9, eth10, br0, eth5, eth4, eth8, eth11, eth3, eth7, eth6} accept
                 ip saddr { 10.1.19.1, 10.99.19.1} tcp sport { 27000, https, http} accept
                 ip daddr { 10.1.19.1, 10.99.19.1} tcp dport { http, https, 27000} accept
                 ip daddr { 10.1.19.1, 10.99.19.1} tcp dport domain accept
                 ip daddr { 10.1.19.1, 10.99.19.1} udp dport domain accept
                 ip daddr { 10.1.19.1, 10.99.19.1} iifname { "br1", "br0"} accept
                 ip daddr { 0.0.0.0/1} accept
                 ip saddr { 0.0.0.0/1} accept
                 oif { br1, br0} ct state { related, established} accept
                 iifname { "br1", "br0" } ct state { established, related } accept
                 iifname { "br0", "br1" } oif { br0, br1 } ip protocol icmp accept
                 drop
        }
}
table ip nat {
        chain prerouting {
                 type nat hook prerouting priority 0;
                 accept
        }

        chain input {
                 type nat hook input priority 0;
                 accept
        }

        chain output {
                 type nat hook output priority 0;
                 accept
        }

        chain postrouting {
                 type nat hook postrouting priority 0;
                 ip saddr 10.1.0.0/16 oif eth10 snat 192.168.111.2
                 ip saddr 10.99.0.0/16 oif eth10 snat 192.168.111.2
                 ip saddr 10.1.0.0/16 oif eth11 snat 192.168.222.2
                 ip saddr 10.99.0.0/16 oif eth11 snat 192.168.222.2
                 masquerade
                 accept
        }
}
table ip6 filter6 {
        chain input {
                 type filter hook input priority 0;
                 ip6 nexthdr ipv6-icmp accept
                 ct state { established, related} accept
                 ct state invalid drop
                 iifname "lo" accept
                 iif lo ct state new accept
                 iifname { "eth5", "eth2", "br0", "eth3", "eth7", "eth1", "eth8", "br1", "eth9", "eth0", "eth6", "eth4"} accept
                 iif { eth6, eth4, eth0, br1, eth5, eth2, eth8, eth3, eth1, eth9, eth7, br0} ct state new accept
                 icmpv6 type { nd-neighbor-solicit, echo-request} accept
                 tcp dport { ssh, http} accept
                 reject
                 drop
        }

        chain output {
                 type filter hook output priority 0;
                 iifname { "eth5", "eth7", "lo", "eth6", "eth9", "eth1", "eth2", "eth4", "br0", "eth3", "eth0", "eth10", "eth11", "br1", "eth8"} accept
                 iif { eth1, eth3, eth4, eth10, br0, eth0, eth2, eth11, br1, eth5, eth8, eth7, eth6, eth9} ct state new accept
                 accept
        }

        chain forward {
                 type filter hook forward priority 0;
                 drop
        }
}
table ip6 nat6 {
        chain prerouting {
                 type nat hook prerouting priority 0;
                 accept
        }

        chain input {
                 type nat hook input priority 0;
                 accept
        }

        chain output {
                 type nat hook output priority 0;
                 accept
        }

        chain postrouting {
                 type nat hook postrouting priority 0;
                 accept
        }
}
