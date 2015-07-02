
Show specific an ipv6 address per interface
* ip -6 addr show dev ethx

Add/Delete an ipv6 address
* ip -6 addr add <ipv6address>/<prefixlength> dev <interface> 
* ip -6 addr del <ipv6address>/<prefixlength> dev <interface>

Displaying existing ipv6 routes
* ip -6 route show ethx

Displaying neighbors
* ip -6 neigh show [dev <device>]

Manipulating neighbors table
* ip -6 neigh add <IPv6 address> lladdr <link-layer address> dev <device>
* ip -6 neigh del <IPv6 address> lladdr <link-layer address> dev <device>

Add/Delete an ipv6 route through a gateway
* ip -6 route add <ipv6network>/<prefixlength> via <ipv6address>
* ip -6 route del <ipv6network>/<prefixlength> via <ipv6address>
* 
Add/Delete an ipv6 route through an interface
*ip -6 route del <ipv6network>/<prefixlength> dev <device>
