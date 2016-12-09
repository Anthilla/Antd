namespace Antd.Network {
    public class NetworkConfigurationModel {
        public string DefaultTxqueuelen { get; set; } = "10000";
        public string DefaultMtu { get; set; } = "6000";
    }

    public class NetworkInterfaceConfigurationModel {
        public string Interface { get; set; }
        /// <summary>
        /// up down
        /// </summary>
        public string Status { get; set; } 
        public string Txqueuelen { get; set; }
        public string Mtu { get; set; } 
    }
}
