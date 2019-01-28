using anthilla.core;

namespace Antd {
    public class Default {

        private const string primaryDomain = "domain.com";
        private const string hostName = "box01";
        private const string primaryIp = "10.11.200.201";
        private const string primaryNetwork = "10.11.0.0";
        private const byte primaryNetworkRange = 16;
        private const string secondaryNetwork = "192.168.111.0";
        private const byte secondaryNetworkRange = 24;
        private const string primaryNetworkAdapter = "eth0";
        private const string primaryNetworkAdapterBridge = "br0";
        private const string primaryGateway = "10.11.19.111";

        public static Network Network() {
            var network = new Network() {
                PrimaryDomain = primaryDomain,
                KnownDns = new DnsClientConfiguration()
            };

            network.KnownHosts = new KnownHost[] {
                new KnownHost() { IpAddr = primaryIp, CommonNames = new[] { CommonString.Append(hostName, ".", primaryDomain), CommonString.Append(hostName) } },
            };

            network.KnownNetworks = new KnownNetwork[] {
                new KnownNetwork() { Label = "primary-network", NetAddr = primaryNetwork },
                new KnownNetwork() { Label = "secondary-network", NetAddr = secondaryNetwork }
            };

            network.InternalNetwork = new SubNetwork(true) {
                Label = "Internal Network",
                Domain = primaryDomain,
                NetworkAdapter = primaryNetworkAdapterBridge,
                StaticAddress = true,
                NetworkRange = primaryNetworkRange,
                IpAddress = primaryIp
            };

            network.ExternalNetwork = new SubNetwork(false);

            network.NetworkInterfaces = new NetInterface[0];
            //network.NetworkInterfaces = new NetInterface[] {
            //    new NetInterface() {
            //        Active = true,
            //        Id = primaryNetworkAdapter,
            //        Name = primaryNetworkAdapter,
            //        Type = models.NetworkAdapterType.Physical,
            //        Membership = NetworkAdapterMembership.InternalNetwork,
            //        NetworkClass = primaryNetwork,
            //        HardwareConfiguration = new NetworkAdapterInfo.HardwareConfiguration() {
            //           Mtu = 6000,
            //           Txqueuelen = 10000,
            //           Promisc = true
            //        },
            //    },
            //    new NetInterface() {
            //        Active = true,
            //        Id = primaryNetworkAdapterBridge,
            //        Name = primaryNetworkAdapterBridge,
            //        Type = models.NetworkAdapterType.Bridge,
            //        Membership = NetworkAdapterMembership.InternalNetwork,
            //        NetworkClass = primaryNetwork,
            //        PrimaryAddressConfiguration = new NetworkAdapterInfo.AddressConfiguration() {
            //            StaticAddress = true,
            //            IpAddr = primaryIp,
            //            NetworkRange = primaryNetworkRange
            //        },
            //        HardwareConfiguration = new NetworkAdapterInfo.HardwareConfiguration() {
            //           Mtu = 6000,
            //           Txqueuelen = 10000,
            //           Promisc = true
            //        },
            //        LowerInterfaces = new [] { primaryNetworkAdapter }
            //    }
            //};

            network.Routing = new NetRoute[] {
                new NetRoute() { Default = true, Destination = "default", Gateway = primaryGateway, Device= primaryNetworkAdapterBridge }
            };

            return network;
        }
    }
}
