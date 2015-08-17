### EXAMPLE 5 WORKING

#### IPROUTE2, BRIDGE/SWITCH (2xLAN) + 2xWAN ####
```
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
```
#### NFTABLE FIREWALL, NAT, ROUTER, BRIDGE/SWITCH, WITH 2 WAN NOT REDUNDANT ####
```
flush ruleset;

table ip filter {
        chain input {
                 type filter hook input priority 0;
                 ct state { related, established} accept
                 ct state invalid drop
                 ip protocol { icmp, esp, ah, udp } accept
                 icmp type echo-request accept
                 udp dport { isakmp, l2tp, ipsec-nat-t} accept
                 iif { lo, br0, br1} accept
                 tcp dport { http, ssh, 953, 53} accept
                 ip daddr { 192.168.111.2, 192.168.222.2 } ct state { related, established} accept
                 ip saddr { 10.1.19.1, 10.99.19.1, 192.168.111.2, 192.168.222.2 } accept
                 ip saddr { 10.1.0.0/24, 10.99.0.0/24 } accept
                 ip daddr { 192.168.111.2, 192.168.222.2 } tcp dport { https, http } accept
                 drop
        }

        chain output {
                 type filter hook output priority 0;
                 ct state { established, related } accept
                 ct state invalid counter log drop
                 ip protocol { esp, ah, udp, icmp } accept
                 icmp type echo-request accept
                 udp dport { ipsec-nat-t, isakmp, l2tp} accept
                 iif { lo, br0, br1 } ct state new accept
                 oif { lo, eth10, eth11 } ct state new accept
                 ip saddr { 127.0.0.1, 192.168.111.2, 192.168.222.2, 10.1.19.1, 10.99.19.1} accept
                 ip saddr { 10.1.0.0/16, 10.99.0.0/16} accept
                 counter log drop
        }

        chain forward {
                 type filter hook forward priority 0;
                 ct state { established, related} accept
                 ct state invalid counter log drop
                 ip protocol { udp, gre, esp, icmp} accept
                 icmp type echo-request accept
                 udp dport { ipsec-nat-t, isakmp, l2tp} accept
                 iif { lo } accept
                 iif { lo } oif { lo } accept
                 iif { br0, br1} oif { br0, br1, eth10, eth11} accept
                 iif { lo, br0, br1 } tcp dport { 953 } accept
                 ip saddr { 127.0.0.1 } tcp sport { 953 } accept
                 ip daddr { 127.0.0.1 } tcp dport { 953 } accept
                 ip daddr { 127.0.0.1 } udp dport { 953 } accept
                 ip saddr { 127.0.0.1, 10.1.19.1, 10.99.19.1} tcp sport { http, https, ssh, domain } accept
                 ip daddr { 127.0.0.1, 10.1.19.1, 10.99.19.1} tcp dport { http, https, ssh, domain } accept
                 ip daddr { 127.0.0.1, 10.99.19.1, 10.1.19.1} udp dport { domain } accept
                 ip daddr { 10.1.19.1, 10.99.19.1} iif { br0, br1} accept
                 meta iif { eth10, eth11 } meta oif { br0, br1} ip daddr 10.1.19.4 tcp dport 8081 accept
                 meta iif { eth10, eth11 } ct state new counter log drop
                 counter log drop
        }
}
table ip nat {
        chain prerouting {
                 type nat hook prerouting priority 0;
                 iif { eth10, eth11 } tcp dport { 80 } dnat 10.1.19.4:8081
                 tcp dport 680 redirect to 555
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
                 ip saddr { 10.1.0.0/16, 10.99.0.0/16 } oif { eth10 } snat 192.168.111.2
                 ip saddr { 10.1.0.0/16, 10.99.0.0/16 } oif { eth11 } snat 192.168.222.2
                 masquerade
                 oif { eth10, eth11 } ip saddr { 10.1.0.0/16, 10.99.0.0/16 } masquerade
                 accept
        }
}
table ip6 filter6 {
        chain input {
                 type filter hook input priority 0;
                 ct state { related, established} accept
                 ct state invalid counter log drop
                 ip6 nexthdr ipv6-icmp accept
                 icmpv6 type { nd-neighbor-solicit, echo-request} accept
                 iif { lo, br0, br1} accept
                 iif { lo, br0, br1} ct state new accept
                 iif { br0, br1} tcp sport { ftp, ftp-data} ct state new accept
                 tcp dport { http, ssh} accept
                 counter log drop
        }

        chain output {
                 type filter hook output priority 0;
                 ct state { established, related } accept
                 ct state invalid counter log drop
                 iif { lo, br0, br1 } accept
                 iif { lo, br0, br1 } ct state new accept
                 oif { lo, eth10, eth11 } accept
                 counter log drop
    }

        chain forward {
                 type filter hook forward priority 0;
                 counter log drop
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
```
