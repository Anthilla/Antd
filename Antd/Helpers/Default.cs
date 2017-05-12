using System.Collections.Generic;
using System.Linq;
using antdlib.common;
using antdlib.config;
using antdlib.models;

namespace Antd {

    public class Default {

        public static NetworkInterfaceConfiguration InternalPhysicalInterfaceConfiguration(string customIp = "") {
            var host = Host2Configuration.Host;
            var ip = string.IsNullOrEmpty(customIp) ? "10.11.254.254" : customIp;
            const string subnet = "16";
            const NetworkRoleVerb verb = NetworkRoleVerb.iif;
            var hostname = $"{host.HostName}{verb}.{host.InternalDomainPrimary}";
            var alias = $"{verb}00";
            var broadcast = Cidr.CalcNetwork(ip, subnet).Broadcast.ToString();
            var networkInterfaceConfiguration = new NetworkInterfaceConfiguration {
                Id = $"{customIp}gfEUxrgNe0qVR4HWpT1U2A",
                Type = NetworkInterfaceType.Internal,
                Hostname = hostname,
                Index = 0,
                Description = "default interface configuration",
                RoleVerb = verb,
                Alias = alias,
                Mode = NetworkInterfaceMode.Static,
                Status = NetworkInterfaceStatus.Up,
                Ip = ip,
                Subnet = subnet,
                Broadcast = broadcast,
                Adapter = NetworkAdapterType.Physical,
                ChildrenIf = new List<string>()
            };
            return networkInterfaceConfiguration;
        }

        public static NetworkInterfaceConfiguration ExternalPhysicalInterfaceConfiguration(string customIp = "") {
            var host = Host2Configuration.Host;
            var ip = string.IsNullOrEmpty(customIp) ? "10.11.254.254" : customIp;
            const string subnet = "16";
            const NetworkRoleVerb verb = NetworkRoleVerb.eif;
            var hostname = $"{host.HostName}{verb}.{host.InternalDomainPrimary}";
            var alias = $"{verb}00";
            var broadcast = Cidr.CalcNetwork(ip, subnet).Broadcast.ToString();
            var networkInterfaceConfiguration = new NetworkInterfaceConfiguration {
                Id = $"{customIp}gfEUxrgNe0qVR4HWpT1U2A",
                Type = NetworkInterfaceType.External,
                Hostname = hostname,
                Index = 0,
                Description = "default interface configuration",
                RoleVerb = verb,
                Alias = alias,
                Mode = NetworkInterfaceMode.Static,
                Status = NetworkInterfaceStatus.Up,
                Ip = ip,
                Subnet = subnet,
                Broadcast = broadcast,
                Adapter = NetworkAdapterType.Physical,
                ChildrenIf = new List<string>()
            };
            return networkInterfaceConfiguration;
        }

        public static NetworkInterfaceConfiguration InternalBridgeInterfaceConfiguration(string customIp = "") {
            var host = Host2Configuration.Host;
            var ip = string.IsNullOrEmpty(customIp) ? "10.11.254.254" : customIp;
            const string subnet = "16";
            const NetworkRoleVerb verb = NetworkRoleVerb.iif;
            var hostname = $"{host.HostName}{verb}.{host.InternalDomainPrimary}";
            var alias = $"{verb}00";
            var broadcast = Cidr.CalcNetwork(ip, subnet).Broadcast.ToString();
            var networkInterfaceConfiguration = new NetworkInterfaceConfiguration {
                Id = $"{customIp}gfEUxrgNe0qVR4HWpT1U2A",
                Type = NetworkInterfaceType.Internal,
                Hostname = hostname,
                Index = 0,
                Description = "default interface configuration",
                RoleVerb = verb,
                Alias = alias,
                Mode = NetworkInterfaceMode.Static,
                Status = NetworkInterfaceStatus.Up,
                Ip = ip,
                Subnet = subnet,
                Broadcast = broadcast,
                Adapter = NetworkAdapterType.Bridge,
                ChildrenIf = Network2Configuration.InterfacePhysical.ToList()
            };
            return networkInterfaceConfiguration;
        }

        public static NetworkInterfaceConfiguration ExternalBridgeInterfaceConfiguration(string customIp = "") {
            var host = Host2Configuration.Host;
            var ip = string.IsNullOrEmpty(customIp) ? "10.11.254.254" : customIp;
            const string subnet = "16";
            const NetworkRoleVerb verb = NetworkRoleVerb.eif;
            var hostname = $"{host.HostName}{verb}.{host.InternalDomainPrimary}";
            var alias = $"{verb}00";
            var broadcast = Cidr.CalcNetwork(ip, subnet).Broadcast.ToString();
            var networkInterfaceConfiguration = new NetworkInterfaceConfiguration {
                Id = $"{customIp}gfEUxrgNe0qVR4HWpT1U2A",
                Type = NetworkInterfaceType.External,
                Hostname = hostname,
                Index = 0,
                Description = "default interface configuration",
                RoleVerb = verb,
                Alias = alias,
                Mode = NetworkInterfaceMode.Static,
                Status = NetworkInterfaceStatus.Up,
                Ip = ip,
                Subnet = subnet,
                Broadcast = broadcast,
                Adapter = NetworkAdapterType.Bridge,
                ChildrenIf = Network2Configuration.InterfacePhysical.ToList()
            };
            return networkInterfaceConfiguration;
        }

        public static NetworkGatewayConfiguration GatewayConfiguration(string customIp = "") {
            var ip = string.IsNullOrEmpty(customIp) ? "10.11.254.254" : customIp;
            var gatewayConfiguration = new NetworkGatewayConfiguration {
                Id = "BpwyyChBvkalUcdxNIL8yQ",
                Route = "default",
                GatewayAddress = ip
            };
            return gatewayConfiguration;
        }

        public static DnsConfiguration PublicDnsConfiguration() {
            var dnsConfiguration = new DnsConfiguration {
                Id = "J8Ho6CGgFUydXUuWf58J-A",
                Type = DnsType.Public,
                Mode = DnsMode.Dynamic,
                AuthenticationEnabled = false,
                Dest = DnsDestination.External,
                Domain = "",
                Ip = "8.8.8.8"
            };
            return dnsConfiguration;
        }

        public static DnsConfiguration PrivateInternalDnsConfiguration() {
            var host = Host2Configuration.Host;
            var dnsConfiguration = new DnsConfiguration {
                Id = "BhfPEkstWUmACT6rL5vO-w",
                Type = DnsType.Private,
                Mode = DnsMode.Dynamic,
                AuthenticationEnabled = false,
                Dest = DnsDestination.Internal,
                Domain = host.InternalDomainPrimary,
                Ip = host.InternalHostIpPrimary
            };
            return dnsConfiguration;
        }

        public static DnsConfiguration PrivateExternalDnsConfiguration() {
            var host = Host2Configuration.Host;
            var dnsConfiguration = new DnsConfiguration {
                Id = "FD1tT-TJZku93WFXt9Ldfw",
                Type = DnsType.Private,
                Mode = DnsMode.Dynamic,
                AuthenticationEnabled = false,
                Dest = DnsDestination.External,
                Domain = host.ExternalDomainPrimary,
                Ip = host.ExternalHostIpPrimary
            };
            return dnsConfiguration;
        }
    }
}
