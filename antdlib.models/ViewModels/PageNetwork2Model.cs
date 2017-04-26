using System.Collections.Generic;

namespace antdlib.models {
    public class PageNetwork2Model {
        public IEnumerable<string> PhysicalIf { get; set; }
        public IEnumerable<string> BridgeIf { get; set; }
        public IEnumerable<string> BondIf { get; set; }
        public IEnumerable<string> VirtualIf { get; set; }
        public IEnumerable<NetworkInterfaceConfiguration> InterfaceConfigurationList { get; set; }
        public IEnumerable<NetworkGatewayConfiguration> GatewayConfigurationList { get; set; }
        public Host2Model Variables { get; set; }
    }
}
