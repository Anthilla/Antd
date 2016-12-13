using System.Collections.Generic;

namespace Antd.Network {
    public class NetworkConfigurationModel {
        public string DefaultTxqueuelen { get; set; } = "10000";
        public string DefaultMtu { get; set; } = "6000";
        public List<NetworkInterfaceConfigurationModel> Interfaces { get; set; } = new List<NetworkInterfaceConfigurationModel>();
    }

    public class NetworkInterfaceConfigurationModel {
        public string Guid { get; set; } = System.Guid.NewGuid().ToString();
        public string Interface { get; set; }
        public NetworkInterfaceMode Mode { get; set; } = NetworkInterfaceMode.Dynamic;
        public NetworkInterfaceStatus Status { get; set; } = NetworkInterfaceStatus.Down;
        public string StaticAddres { get; set; }
        public string StaticRange { get; set; }
        public string Txqueuelen { get; set; } = "10000";
        public string Mtu { get; set; } = "6000";
    }
}
