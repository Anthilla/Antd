using System.Collections.Generic;

namespace antdlib.models {
    public class PageNetworkModel {
        public IEnumerable<NetworkInterfaceConfigurationModel> NetworkPhysicalIf { get; set; }
        public IEnumerable<NetworkInterfaceConfigurationModel> NetworkBridgeIf { get; set; }
        public IEnumerable<NetworkInterfaceConfigurationModel> NetworkBondIf { get; set; }
        public IEnumerable<NetworkInterfaceConfigurationModel> NetworkVirtualIf { get; set; }
        public IEnumerable<string> NetworkIfList { get; set; }
    }
}
