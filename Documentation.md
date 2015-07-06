### S1: Network:

### Important Notes:
- for each command mind to sign all *(S)CRUD* operations. Thanks. (https://en.wikipedia.org/wiki/Create,_read,_update_and_delete)
- maintain uptime, flushes and restart/reloads, at 95% take it down.
- with iproute2, virtual interfaces eth0:X are not necessary.
- with iproute2, you can add/remove many ip set from interfaces.
- Refer *Always* to OSI Stack level (!!!)
- in iproute2 in general interface are L1, and ip link refer to L2, ip addr refer to L3
- Netfilter diagram (https://upload.wikimedia.org/wikipedia/commons/thumb/3/37/Netfilter-packet-flow.svg/600px-Netfilter-packet-flow.svg.png) 
- OSI stack diagram (http://www.windowsnetworking.com/img/upl/image0011210155736818.jpg)

### IPROUTE2:
- Add new address (ipv4):
``` 
ip addr add xx.xx.xx.xx/xx broadcast xx.xx.xx.xx dev ethx
``` 
- Add new address (ipv6):
``` 
ip -6 addr add ipv6/64 dev ethx 
``` 
*(should be possible change the netmask for obvious reasons)*
- Add ipv6 gateway:
``` 
ip -6 route add default via ipv6_gateway dev ethx
``` 
- Delete (prefer instead of flush!)
``` 
ip addr del xx.xx.xx.xx/xx broadcast xx.xx.xx.xx dev ethx
``` 
- Flush configuration:
``` 
ip addr flush dev ethx
ip -stat -stat addr flush to 10/8 
``` 
*(delete range 10.0.0.0/8 and so on)*
``` 
ip -4 addr flush label "eth*" 
``` 
*(flush all ethx addresses)*
``` 
ip -6 addr flush dynamic
``` 
- Show interfaces L1,L2,L3:
``` 
ip addr show
``` 
- Show a interface L1,L2,L3:
``` 
ip addr show ethX
``` 
- Show interfaces L1,L2:
``` 
ip link show
``` 
- Show a interface L1,L2:
``` 
ip link show ethX
``` 
- Show network card statistics L1,L2:
``` 
ip -s link ls ethx
``` 
- Create/Add route:
``` 
ip route add default via xx.xx.xx.xx
ip route add xx.xx.xx.xx/24 via xx.xx.xx.xx
``` 
*(possible many netmasks)*
- Create/Add default multipath route splitting load between ppp0 and ppp1 *(or other networks)*
``` 
ip route add default scope global nexthop dev ppp0 nexthop dev ppp1
``` 
- Natting:
``` 
ip route add nat xx.xx.xx.xx via xx.xx.xx.xx
``` 
- Delete: 
``` 
ip route del default via xx.xx.xx.xx
ip route del xx.xx.xx.xx via xx.xx.xx.xx
``` 
- Show routes:
``` 
ip route show
``` 
- Enable/disable, Take Up/Down interfaces:
``` 
ip link set ethx up | down
``` 
### BRCTL:
- Create/Add brdige name:
``` 
brctl addbr name
``` 
- Delete bridge name:
``` 
brctl delbr name
``` 
- Create/Add network interfaces to brctl interface:
``` 
brctl addif name ethx 
``` 
*(for instance, eth0 - eth1 and so on)*

- Delete network interfaces to brctl interface:
``` 
brctl delif name ethx
``` 
- Enable/Disable stp on bridge:
``` 
brctl stp name on | off
``` 
- Show MACs and stp:
``` 
brctl showmacs name
brctl showstp name
``` 
- Another important option could be the priority port and the cost path:
``` 
brctl setpathcost name <port> <cost> set path cost
brctl setportprio name <port> <prio> set port priority
``` 
### TEAMCTL:
- sample configuration with active/backup mode using teamnl and ip:
``` 
ip link add team0 type bond (many other)
teamnl team0 setoption mode activebackup
ip link set dev eth0 master team0
ip link set dev eth1 master team0
ip addr add xxx.xxx.xxx.xxx/XX broadcast yyy.yyy.yyy.yyy dev team0
ip link set team0 up
``` 
-View teamnl mode:
``` 
teamnl name getoption mode
``` 
-View ports associated:
``` 
teamnl team0 ports
``` 
### IFENSLAVE:

### PSEUDO FS proc AND sys operations. :

### CONVERSIONS:

- Conversion from standard interface to a Bridge one at runtime without loosing connections
 *in a not interactive shell, aka Screen or HOHUP* execute this procedure:

``` 
brctl addbr br0
brctl addif br0 eth0
ip link set eth0 up
ip link set br0 up
ip addr add xxx.xxx.xxx.xxx/XX dev br0
ip addr del xxx.xxx.xxx.xxx/XX dev eth0
ip route del default via yyy.yyy.yyy.yyy dev eth0
ip route add default via yyy.yyy.yyy.yyy dev br0
``` 
- Conversion from standard interface to a Bond one at runtime without loosing connections
- Conversion from standard interface to a Team one at runtime without loosing connections

### IPV6 Related:
- Show specific an ipv6 address per interface:
``` 
ip -6 addr show dev ethx
``` 
- Create/Add and Delete an ipv6 address:
``` 
ip -6 addr add <ipv6address>/<prefixlength> dev <interface> 
ip -6 addr del <ipv6address>/<prefixlength> dev <interface>
``` 
- Displaying existing ipv6 routes:
``` 
ip -6 route show ethx
``` 
- Displaying neighbors:
``` 
ip -6 neigh show [dev <device>]
``` 
- Manipulating neighbors table:
``` 
ip -6 neigh add <IPv6 address> lladdr <link-layer address> dev <device>
ip -6 neigh del <IPv6 address> lladdr <link-layer address> dev <device>
``` 
- Add/Delete an ipv6 route through a gateway:
``` 
ip -6 route add <ipv6network>/<prefixlength> via <ipv6address>
ip -6 route del <ipv6network>/<prefixlength> via <ipv6address>
``` 
- Add/Delete an ipv6 route through an interface:
``` 
ip -6 route del <ipv6network>/<prefixlength> dev <device>
``` 
- Show ipv6 tunnels
```
ip -6 tunnel show [<device>]
```

- Point-to-Point tunneling
```
ip tunnel add <device> mode sit ttl <ttldefault> remote <ipv4addressofforeigntunnel> local <ipv4addresslocal>
```

example:

```
ip tunnel add sit1 mode sit ttl <ttldefault> remote <ipv4addressofforeigntunnel1> local <ipv4addresslocal>
ip link set dev sit1 up
ip -6 route add <prefixtoroute1> dev sit1 metric 1
```
- Removing poin-to-point tunnels
```
ip tunnel del <device>
```

example:
```
ip -6 route del <prefixtoroute1> dev sit1
ip link set sit1 down
ip tunnel del sit1
```

- 6to4 tunnels
```
ip tunnel add tun6to4 mode sit ttl <ttldefault> remote any local <localipv4address> 
ip link set dev tun6to4 up 
ip -6 addr add <local6to4address>/16 dev tun6to4 
ip -6 route add 2000::/3 via ::192.88.99.1 dev tun6to4 metric 1
```

- Remove a 6to4 tunnels
```
ip -6 route flush dev tun6to4
ip link set dev tun6to4 down
ip tunnel del tun6to4 
```

- IPv4-in-IPv6 tunnels
```
ip tunnel add <device> mode ip4ip6 remote <ipv6addressofforeigntunnel> local <ipv6addresslocal>
```


example:
```
ip -6 tunnel add ip6tnl1 mode ip4ip6 remote <ipv6addressofforeigntunnel1> local <ipv6addresslocal>
ip link set dev ip6tnl1 up 
ip -6 route add <prefixtoroute1> dev ip6tnl1 metric 1
```

### VLAN

- Configure basic vlan for its interface
```
ip link add link eth0 name eth0.100 type vlan id 100
```
- Configure trunk
```
ip link add link eth0 name eth0.10 type vlan id 10
ip link add link eth0 name eth0.20 type vlan id 20
ip link add link eth0 name eth0.30 type vlan id 30
```

there is no conventional name for the ethernet/gigabit device. 
we can also use eth0_100 or something else. 
after that it is necessary set up an ip address for the eth0 interface (in this case)

- View the vlan-id
```
ip -d link show eth0.10 
```
it is depends of the name assigned for each interface (eth0.10, eth0.20 and so on)

### S2: Failover:

a virtual IP with 2 ethernet interfaces in 2 different hosts
the interfaces will have a password and will be identified.

example:
```
ip addr add 10.1.1.252/24 dev ethX
ucarp -v 42 -p anthilla -a 10.1.1.252 -s 10.1.1.1 &
ucarp -v 42 -p anthilla -a 10.1.1.252 -s 10.1.1.2 &
```
in this case the identified is 42 and the password is anthilla
