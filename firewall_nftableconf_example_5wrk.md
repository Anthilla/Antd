
### EXAMPLE5 WORKING
'''
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
'''
