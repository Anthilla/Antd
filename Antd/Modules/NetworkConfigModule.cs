//-------------------------------------------------------------------------------------
//     Copyright (c) 2014, Anthilla S.r.l. (http://www.anthilla.com)
//     All rights reserved.
//
//     Redistribution and use in source and binary forms, with or without
//     modification, are permitted provided that the following conditions are met:
//         * Redistributions of source code must retain the above copyright
//           notice, this list of conditions and the following disclaimer.
//         * Redistributions in binary form must reproduce the above copyright
//           notice, this list of conditions and the following disclaimer in the
//           documentation and/or other materials provided with the distribution.
//         * Neither the name of the Anthilla S.r.l. nor the
//           names of its contributors may be used to endorse or promote products
//           derived from this software without specific prior written permission.
//
//     THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
//     ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
//     WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
//     DISCLAIMED. IN NO EVENT SHALL ANTHILLA S.R.L. BE LIABLE FOR ANY
//     DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
//     (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
//     LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
//     ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
//     (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
//     SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
//
//     20141110
//-------------------------------------------------------------------------------------

using antdlib.Network;
using Nancy;
using Nancy.Security;

namespace Antd.Modules {

    public class NetworkConfigModule : NancyModule {

        public NetworkConfigModule()
            : base("/network/config") {
            this.RequiresAuthentication();

            #region Repository
            Get["/repo/all"] = x => {
                var result = NetworkConfigRepository.GetAll();
                return Response.AsJson(result);
            };

            Post["/repo/enable"] = x => {
                string guid = Request.Form.Guid;
                NetworkConfigRepository.Enable(guid);
                return Response.AsJson(true);
            };

            Post["/repo/disable"] = x => {
                string guid = Request.Form.Guid;
                NetworkConfigRepository.Disable(guid);
                return Response.AsJson(true);
            };

            Post["/repo/delete"] = x => {
                string guid = Request.Form.Guid;
                NetworkConfigRepository.Delete(guid);
                return Response.AsJson(true);
            };

            Post["/repo/export"] = x => {
                NetworkConfigRepository.ExportToFile();
                return Response.AsJson(true);
            };
            #endregion Repository

            #region IPV4
            Post["/ipv4/add/address"] = x => {
                string address = Request.Form.Address;
                string range = Request.Form.Range;
                string interfaceName = Request.Form.Interface;
                string broadcast = Request.Form.Broadcast;
                var result = NetworkConfig.Iproute2.AddNewAddressIpv4(address, range, interfaceName, broadcast);
                return Response.AsJson(result);
            };

            Post["/ipv4/del/address"] = x => {
                string address = Request.Form.Address;
                string range = Request.Form.Range;
                string interfaceName = Request.Form.Interface;
                string broadcast = Request.Form.Broadcast;
                var result = NetworkConfig.Iproute2.DeleteAddressIpv4(address, range, interfaceName, broadcast);
                return Response.AsJson(result);
            };

            Post["/ipv4/flush"] = x => {
                string interfaceName = Request.Form.Interface;
                var i = (interfaceName.Length > 0) ? interfaceName : null;
                var result = NetworkConfig.Iproute2.FlushConfigurationIpv4(i);
                return Response.AsJson(result);
            };

            Get["/ipv4/address/{interfaceName}"] = x => {
                string interfaceName = x.interfaceName;
                var result = NetworkConfig.Iproute2.ShowInterfaceAddr(interfaceName);
                return Response.AsJson(result);
            };

            Get["/ipv4/link/{interfaceName}"] = x => {
                string interfaceName = x.interfaceName;
                var result = NetworkConfig.Iproute2.ShowInterfaceLink(interfaceName);
                return Response.AsJson(result);
            };

            Get["/ipv4/stats/{interfaceName}"] = x => {
                string interfaceName = x.interfaceName;
                var result = NetworkConfig.Iproute2.ShowInterfaceStats(interfaceName);
                return Response.AsJson(result);
            };

            Post["/ipv4/add/route"] = x => {
                string gateway = Request.Form.Gateway;
                string destination = Request.Form.Destination;
                var i = (destination.Length > 0) ? destination : null;
                var result = NetworkConfig.Iproute2.AddRouteIpv4(gateway, i);
                return Response.AsJson(result);
            };

            Post["/ipv4/del/route"] = x => {
                string gateway = Request.Form.Gateway;
                string destination = Request.Form.Destination;
                var i = (destination.Length > 0) ? destination : null;
                var result = NetworkConfig.Iproute2.DeleteRouteIpv4(gateway, i);
                return Response.AsJson(result);
            };

            Post["/ipv4/add/route/multipath"] = x => {
                string network1 = Request.Form.Network1;
                string network2 = Request.Form.Network2;
                var result = NetworkConfig.Iproute2.AddMultipathRoute(network1, network2);
                return Response.AsJson(result);
            };

            Post["/ipv4/add/nat"] = x => {
                string address = Request.Form.Address;
                string via = Request.Form.Via;
                var result = NetworkConfig.Iproute2.AddNat(address, via);
                return Response.AsJson(result);
            };

            Get["/ipv4/routes/{interfaceName}"] = x => {
                string interfaceName = x.interfaceName;
                var result = NetworkConfig.Iproute2.ShowRoutes(interfaceName);
                return Response.AsJson(result);
            };

            Post["/ipv4/enable/if"] = x => {
                string interfaceName = Request.Form.Interface;
                var result = NetworkConfig.Iproute2.EnableInterface(interfaceName);
                return Response.AsJson(result);
            };

            Post["/ipv4/disable/if"] = x => {
                string interfaceName = Request.Form.Interface;
                var result = NetworkConfig.Iproute2.DisableInterface(interfaceName);
                return Response.AsJson(result);
            };

            Post["/ipv4/add/tunnel"] = x => {
                string interfaceName = Request.Form.Interface;
                string ttl = Request.Form.Ttl;
                string foreignTunnel = Request.Form.Tunnel;
                string address = Request.Form.Address;
                var result = NetworkConfig.Iproute2.AddTunnelPointToPointIpv4(interfaceName, ttl, foreignTunnel, address);
                return Response.AsJson(result);
            };

            Post["/ipv4/del/tunnel"] = x => {
                string interfaceName = Request.Form.Interface;
                var result = NetworkConfig.Iproute2.DeleteTunnelPointToPointIpv4(interfaceName);
                return Response.AsJson(result);
            };

            Get["/ipv4/tunnels/{interfaceName?}"] = x => {
                string interfaceName = x.interfaceName;
                var result = NetworkConfig.Iproute2.ShowTunnelsIpv4(interfaceName);
                return Response.AsJson(result);
            };
            #endregion

            #region IPV6
            Post["/ipv6/add/address"] = x => {
                string address = Request.Form.Address;
                string interfaceName = Request.Form.Interface;
                var result = NetworkConfig.Iproute2.AddNewAddressIpv6(address, interfaceName);
                return Response.AsJson(result);
            };

            Post["/ipv6/del/address"] = x => {
                string address = Request.Form.Address;
                string interfaceName = Request.Form.Interface;
                var result = NetworkConfig.Iproute2.DeleteAddressIpv6(address, interfaceName);
                return Response.AsJson(result);
            };

            Post["/ipv6/flush"] = x => {
                var result = NetworkConfig.Iproute2.FlushConfigurationIpv6();
                return Response.AsJson(result);
            };

            Get["/ipv6/neigh/{interfaceName}"] = x => {
                string interfaceName = x.interfaceName;
                var result = NetworkConfig.Iproute2.ShowNeighborsIpv6(interfaceName);
                return Response.AsJson(result);
            };

            Post["/ipv6/add/neigh"] = x => {
                string address = Request.Form.Address;
                string layer = Request.Form.Layer;
                string interfaceName = Request.Form.Interface;
                var result = NetworkConfig.Iproute2.AddNeighborsIpv6(address, layer, interfaceName);
                return Response.AsJson(result);
            };

            Post["/ipv6/del/neigh"] = x => {
                string address = Request.Form.Address;
                string layer = Request.Form.Layer;
                string interfaceName = Request.Form.Interface;
                var result = NetworkConfig.Iproute2.DeleteNeighborsIpv6(address, layer, interfaceName);
                return Response.AsJson(result);
            };

            Post["/ipv6/add/route/gw"] = x => {
                string address = Request.Form.Address;
                string gateway = Request.Form.Gateway;
                var g = (gateway.Length > 0) ? gateway : null;
                var result = NetworkConfig.Iproute2.AddRouteIpv6Gateway(address, g);
                return Response.AsJson(result);
            };

            Post["/ipv6/del/route/gw"] = x => {
                string address = Request.Form.Address;
                string gateway = Request.Form.Gateway;
                var g = (gateway.Length > 0) ? gateway : null;
                var result = NetworkConfig.Iproute2.DeleteRouteIpv6Gateway(address, g);
                return Response.AsJson(result);
            };

            Post["/ipv6/add/route/if"] = x => {
                string address = Request.Form.Address;
                string interfaceName = Request.Form.Interface;
                var i = (interfaceName.Length > 0) ? interfaceName : null;
                var result = NetworkConfig.Iproute2.AddRouteIpv6Interface(address, i);
                return Response.AsJson(result);
            };

            Post["/ipv6/del/route/if"] = x => {
                string address = Request.Form.Address;
                string interfaceName = Request.Form.Interface;
                var i = (interfaceName.Length > 0) ? interfaceName : null;
                var result = NetworkConfig.Iproute2.DeleteRouteIpv6Interface(address, i);
                return Response.AsJson(result);
            };

            Get["/ipv6/tunnels/{interfaceName?}"] = x => {
                string interfaceName = x.interfaceName;
                var result = NetworkConfig.Iproute2.ShowTunnelsIpv6(interfaceName);
                return Response.AsJson(result);
            };

            #endregion

            #region BRIDGE
            Post["/br/add"] = x => {
                string bridgeName = Request.Form.Bridge;
                var result = NetworkConfig.Brctl.AddBridgeName(bridgeName);
                return Response.AsJson(result);
            };

            Post["/br/del"] = x => {
                string bridgeName = Request.Form.Bridge;
                var result = NetworkConfig.Brctl.DeleteBridgeName(bridgeName);
                return Response.AsJson(result);
            };

            Post["/br/add/if"] = x => {
                string bridgeName = Request.Form.Bridge;
                string interfaceName = Request.Form.Interface;
                var result = NetworkConfig.Brctl.AddNetworkInterfaceToBridge(bridgeName, interfaceName);
                return Response.AsJson(result);
            };

            Post["/br/del/if"] = x => {
                string bridgeName = Request.Form.Bridge;
                string interfaceName = Request.Form.Interface;
                var result = NetworkConfig.Brctl.DeleteNetworkInterfaceToBridge(bridgeName, interfaceName);
                return Response.AsJson(result);
            };

            Post["/br/stp/on/bridge"] = x => {
                string bridgeName = Request.Form.Bridge;
                var result = NetworkConfig.Brctl.EnableStpOnBridge(bridgeName);
                return Response.AsJson(result);
            };

            Post["/br/stp/off/bridge"] = x => {
                string bridgeName = Request.Form.Bridge;
                var result = NetworkConfig.Brctl.DisableStpOnBridge(bridgeName);
                return Response.AsJson(result);
            };

            Get["/br/macs/{bridgeName}"] = x => {
                string bridgeName = x.bridgeName;
                var result = NetworkConfig.Brctl.ShowBridgeMacs(bridgeName);
                return Response.AsJson(result);
            };

            Get["/br/stp/{bridgeName}"] = x => {
                string bridgeName = x.bridgeName;
                var result = NetworkConfig.Brctl.ShowBridgeStp(bridgeName);
                return Response.AsJson(result);
            };

            Post["/br/path/cost"] = x => {
                string bridgeName = Request.Form.Bridge;
                string path = Request.Form.Path;
                string cost = Request.Form.Cost;
                var result = NetworkConfig.Brctl.SetBridgePathCost(bridgeName, path, cost);
                return Response.AsJson(result);
            };

            Post["/br/port/prio"] = x => {
                string bridgeName = Request.Form.Bridge;
                string port = Request.Form.Port;
                string prio = Request.Form.Priority;
                var result = NetworkConfig.Brctl.SetBridgePortPriority(bridgeName, port, prio);
                return Response.AsJson(result);
            };

            #endregion
        }
    }
}