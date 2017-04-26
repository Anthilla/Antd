using LukeSkywalker.IPNetwork;

namespace antdlib.models {
    public class Host2Model {
        #region [    Host Var    ]
        public string HostName { get; set; } = string.Empty; //srv01
        public string HostChassis { get; set; } = string.Empty;
        public string HostDeployment { get; set; } = string.Empty;
        public string HostLocation { get; set; } = string.Empty;

        public string HostAliasPrimary { get; set; } = string.Empty;

        public string InternalDomainPrimary { get; set; } = string.Empty; //domint.local
        public string InternalHostIpPrimary { get; set; } = string.Empty; //10.11.19.111
        public string InternalNetPrimaryBits { get; set; } = string.Empty; //16
        public string InternalNetPrimary => Cidr.CalcNetwork(InternalHostIpPrimary, InternalNetPrimaryBits).Network.ToString(); //viene calcolato

        public string ExternalDomainPrimary { get; set; } = string.Empty; //domext.local
        public string ExternalHostIpPrimary { get; set; } = string.Empty; //192.168.111.0/24
        public string ExternalNetPrimaryBits { get; set; } = string.Empty; //24
        public string ExternalNetPrimary => Cidr.CalcNetwork(ExternalHostIpPrimary, ExternalNetPrimaryBits).Network.ToString(); //viene calcolato

        public string ResolvNameserver { get; set; } = string.Empty; //ip address
        public string ResolvDomain { get; set; } = string.Empty; //può essere il domain interno o esterno

        #endregion

        #region [    Time Var    ]
        public string Timezone { get; set; } = string.Empty;
        public string NtpdateServer { get; set; } = string.Empty;
        #endregion

        #region [    Asset Var    ]
        public string MachineUid { get; set; } = string.Empty;
        public string Cloud { get; set; } = string.Empty;
        #endregion
    }

    public class Cidr {
        public static IPNetwork CalcNetwork(string ip, string bits) {
            var net = $"{ip}/{bits}";
            var ipnetwork = IPNetwork.Parse(net);
            return ipnetwork;
        }
    }
}
