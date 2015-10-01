///-------------------------------------------------------------------------------------
///     Copyright (c) 2014, Anthilla S.r.l. (http://www.anthilla.com)
///     All rights reserved.
///
///     Redistribution and use in source and binary forms, with or without
///     modification, are permitted provided that the following conditions are met:
///         * Redistributions of source code must retain the above copyright
///           notice, this list of conditions and the following disclaimer.
///         * Redistributions in binary form must reproduce the above copyright
///           notice, this list of conditions and the following disclaimer in the
///           documentation and/or other materials provided with the distribution.
///         * Neither the name of the Anthilla S.r.l. nor the
///           names of its contributors may be used to endorse or promote products
///           derived from this software without specific prior written permission.
///
///     THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
///     ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
///     WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
///     DISCLAIMED. IN NO EVENT SHALL ANTHILLA S.R.L. BE LIABLE FOR ANY
///     DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
///     (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
///     LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
///     ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
///     (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
///     SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
///
///     20141110
///-------------------------------------------------------------------------------------

namespace antdlib.Network {
    public class NetworkConfig {
        public class Iproute2 {

            public static string AddNewAddressIPV4(string range, string address, string interfaceName) {
                var cmd = $"ip addr add {range} broadcast {address} dev {interfaceName}";
                NetworkConfigRepository.Create(cmd);
                return Terminal.Execute(cmd);
            }

            public static string DeleteAddressIPV4(string range, string address, string interfaceName) {
                var cmd = $"ip addr del {range} broadcast {address} dev {interfaceName}";
                NetworkConfigRepository.Create(cmd);
                return Terminal.Execute(cmd);
            }

            public static string FlushConfigurationIPV4(string interfaceName = null) {
                var i = (interfaceName == null) ? "label \"eth *\"" : "dev {interfaceName}";
                var cmd = $"ip addr flush {i}";
                NetworkConfigRepository.Create(cmd);
                return Terminal.Execute(cmd);
            }

            public static string ShowInterfaceAddr(string interfaceName) {
                var cmd = $"ip addr show {interfaceName}";
                return Terminal.Execute(cmd);
            }

            public static string ShowInterfaceLink(string interfaceName) {
                var cmd = $"ip link show {interfaceName}";
                return Terminal.Execute(cmd);
            }

            public static string ShowInterfaceStats(string interfaceName) {
                var cmd = $"ip -s link ls {interfaceName}";
                return Terminal.Execute(cmd);
            }

            public static string AddRouteIPV4(string address, string gateway = null) {
                if (gateway == null) {
                    var cmd = $"ip route add default via {address}";
                    NetworkConfigRepository.Create(cmd);
                    return Terminal.Execute(cmd);
                }
                else {
                    var cmd = $"ip route add {gateway} via {address}";
                    NetworkConfigRepository.Create(cmd);
                    return Terminal.Execute(cmd);
                }
            }

            public static string AddMultipathRoute(string net1, string net2) {
                var cmd = $"ip route add default scope global nexthop dev {net1} nexthop dev {net2}";
                NetworkConfigRepository.Create(cmd);
                return Terminal.Execute(cmd);
            }

            public static string AddNat(string address, string viaAddress) {
                var cmd = $"ip route add nat {address} via {viaAddress}";
                NetworkConfigRepository.Create(cmd);
                return Terminal.Execute(cmd);
            }

            public static string DeleteRouteIPV4(string address, string gateway = null) {
                if (gateway == null) {
                    var cmd = $"ip route del default via {address}";
                    NetworkConfigRepository.Create(cmd);
                    return Terminal.Execute(cmd);
                }
                else {
                    var cmd = $"ip route del {gateway} via {address}";
                    NetworkConfigRepository.Create(cmd);
                    return Terminal.Execute(cmd);
                }
            }

            public static string ShowRoutes(string interfaceName = "") {
                var cmd = $"ip route show {interfaceName}";
                return Terminal.Execute(cmd);
            }

            public static string EnableInterface(string interfaceName) {
                var cmd = $"ip link set {interfaceName} up";
                NetworkConfigRepository.Create(cmd);
                return Terminal.Execute(cmd);
            }

            public static string DisableInterface(string interfaceName) {
                var cmd = $"ip link set {interfaceName} down";
                NetworkConfigRepository.Create(cmd);
                return Terminal.Execute(cmd);
            }

            public static string AddTunnelPointToPointIPV4(string interfaceName, string ttl, string foreignTunnel, string address) {
                var cmd = $"ip tunnel add {interfaceName} mode sit ttl {ttl} remote {foreignTunnel} local {address}";
                NetworkConfigRepository.Create(cmd);
                return Terminal.Execute(cmd);
            }

            public static string DeleteTunnelPointToPointIPV4(string interfaceName) {
                var cmd = $"ip tunnel del {interfaceName}";
                NetworkConfigRepository.Create(cmd);
                return Terminal.Execute(cmd);
            }

            public static string ShowTunnelsIPV4(string interfaceName) {
                var i = (interfaceName == null) ? "" : $"dev {interfaceName}";
                var cmd = $"ip tunnel show {i}";
                return Terminal.Execute(cmd);
            }

            #region IPV6 Related
            public static string AddNewAddressIPV6(string address, string interfaceName) {
                var cmd = $"ip -6 addr add {address} dev {interfaceName}";
                NetworkConfigRepository.Create(cmd);
                return Terminal.Execute(cmd);
            }

            public static string DeleteAddressIPV6(string address, string interfaceName) {
                var cmd = $"ip -6 addr del {address} dev {interfaceName}";
                NetworkConfigRepository.Create(cmd);
                return Terminal.Execute(cmd);
            }

            public static string ShowRoutesIPV6(string interfaceName = "") {
                var cmd = $"ip -6 route show {interfaceName}";
                return Terminal.Execute(cmd);
            }

            public static string FlushConfigurationIPV6() {
                var cmd = $"ip addr flush dynamic";
                NetworkConfigRepository.Create(cmd);
                return Terminal.Execute(cmd);
            }

            public static string ShowNeighborsIPV6(string interfaceName = null) {
                var i = (interfaceName == null) ? "" : $"dev {interfaceName}";
                var cmd = $"ip -6 neigh show {i}";
                return Terminal.Execute(cmd);
            }

            public static string AddNeighborsIPV6(string address, string layerAddress, string interfaceName) {
                var cmd = $"ip -6 neigh add {address} lladdr {layerAddress} dev {interfaceName}";
                NetworkConfigRepository.Create(cmd);
                return Terminal.Execute(cmd);
            }

            public static string DeleteNeighborsIPV6(string address, string layerAddress, string interfaceName) {
                var cmd = $"ip -6 neigh del {address} lladdr {layerAddress} dev {interfaceName}";
                NetworkConfigRepository.Create(cmd);
                return Terminal.Execute(cmd);
            }

            public static string AddRouteIPV6Gateway(string address, string gateway = null) {
                if (gateway == null) {
                    var cmd = $"ip -6 route add default via {address}";
                    NetworkConfigRepository.Create(cmd);
                    return Terminal.Execute(cmd);
                }
                else {
                    var cmd = $"ip -6 route add {gateway} via {address}";
                    NetworkConfigRepository.Create(cmd);
                    return Terminal.Execute(cmd);
                }
            }

            public static string DeleteRouteIPV6Gateway(string address, string gateway = null) {
                if (gateway == null) {
                    var cmd = $"ip -6 route del default via {address}";
                    NetworkConfigRepository.Create(cmd);
                    return Terminal.Execute(cmd);
                }
                else {
                    var cmd = $"ip -6 route del {gateway} via {address}";
                    NetworkConfigRepository.Create(cmd);
                    return Terminal.Execute(cmd);
                }
            }

            public static string AddRouteIPV6Interface(string interfaceName, string gateway = null) {
                if (gateway == null) {
                    var cmd = $"ip -6 route add default dev {interfaceName}";
                    NetworkConfigRepository.Create(cmd);
                    return Terminal.Execute(cmd);
                }
                else {
                    var cmd = $"ip -6 route add {gateway} dev {interfaceName}";
                    NetworkConfigRepository.Create(cmd);
                    return Terminal.Execute(cmd);
                }
            }

            public static string DeleteRouteIPV6Interface(string interfaceName, string gateway = null) {
                if (gateway == null) {
                    var cmd = $"ip -6 route del default dev {interfaceName}";
                    NetworkConfigRepository.Create(cmd);
                    return Terminal.Execute(cmd);
                }
                else {
                    var cmd = $"ip -6 route del {gateway} dev {interfaceName}";
                    NetworkConfigRepository.Create(cmd);
                    return Terminal.Execute(cmd);
                }
            }

            public static string ShowTunnelsIPV6(string interfaceName) {
                var i = (interfaceName == null) ? "" : $"dev {interfaceName}";
                var cmd = $"ip -6 tunnel show {i}";
                return Terminal.Execute(cmd);
            }
            #endregion
        }

        public class Brctl {
            public static string AddBridgeName(string bridgeName) {
                var cmd = $"brctl addbr {bridgeName}";
                NetworkConfigRepository.Create(cmd);
                return Terminal.Execute(cmd);
            }

            public static string DeleteBridgeName(string bridgeName) {
                var cmd = $"brctl delbr {bridgeName}";
                NetworkConfigRepository.Create(cmd);
                return Terminal.Execute(cmd);
            }

            public static string AddNetworkInterfaceToBridge(string bridgeName, string interfaceName) {
                var cmd = $"brctl addif {bridgeName} {interfaceName}";
                NetworkConfigRepository.Create(cmd);
                return Terminal.Execute(cmd);
            }

            public static string DeleteNetworkInterfaceToBridge(string bridgeName, string interfaceName) {
                var cmd = $"brctl delif {bridgeName} {interfaceName}";
                NetworkConfigRepository.Create(cmd);
                return Terminal.Execute(cmd);
            }

            public static string EnableStpOnBridge(string bridgeName) {
                var cmd = $"brctl stp {bridgeName} on";
                NetworkConfigRepository.Create(cmd);
                return Terminal.Execute(cmd);
            }

            public static string DisableStpOnBridge(string bridgeName) {
                var cmd = $"brctl stp {bridgeName} off";
                NetworkConfigRepository.Create(cmd);
                return Terminal.Execute(cmd);
            }

            public static string ShowBridgeMACS(string bridgeName) {
                var cmd = $"brctl showmacs {bridgeName}";
                return Terminal.Execute(cmd);
            }

            public static string ShowBridgeSTP(string bridgeName) {
                var cmd = $"brctl showstp {bridgeName}";
                return Terminal.Execute(cmd);
            }

            public static string SetBridgePathCost(string bridgeName, string path, string cost) {
                var cmd = $"brctl setpathcost {bridgeName} {path} {cost} set path cost";
                NetworkConfigRepository.Create(cmd);
                return Terminal.Execute(cmd);
            }

            public static string SetBridgePortPriority(string bridgeName, string port, string priority) {
                var cmd = $"brctl setportprio {bridgeName} {port} {priority} set port priority";
                NetworkConfigRepository.Create(cmd);
                return Terminal.Execute(cmd);
            }
        }
    }
}