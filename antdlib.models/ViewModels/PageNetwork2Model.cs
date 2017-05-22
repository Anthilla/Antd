using System.Collections.Generic;

namespace antdlib.models {
    public class PageNetwork2Model {
        public IEnumerable<string> PhysicalIf { get; set; }
        public IEnumerable<string> BridgeIf { get; set; }
        public IEnumerable<string> BondIf { get; set; }
        public IEnumerable<string> VirtualIf { get; set; }
        public IEnumerable<string> AllIfs { get; set; }
        public IEnumerable<NetworkInterfaceConfiguration> InterfaceConfigurationList { get; set; }
        public IEnumerable<NetworkGatewayConfiguration> GatewayConfigurationList { get; set; }
        public IEnumerable<NetworkRouteConfiguration> RouteConfigurationList { get; set; }
        public IEnumerable<DnsConfiguration> DnsConfigurationList { get; set; }
        public List<NetworkInterface> Configuration { get; set; }
        public List<NetworkHardwareConfiguration> NetworkHardwareConfigurationList { get; set; }
        public List<NetworkAggregatedInterfaceConfiguration> LagConfigurationList { get; set; }
        public Host2Model Variables { get; set; }
        public string ActiveDnsConfiguration { get; set; }
    }
}
