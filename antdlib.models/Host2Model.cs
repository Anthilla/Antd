using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
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
        public string InternalNetPrimary => Cidr.CalcNetwork(InternalHostIpPrimary, InternalNetPrimaryBits)?.Network.ToString(); //viene calcolato
        public string InternalNetMaskPrimary => Cidr.CalcNetwork(InternalHostIpPrimary, InternalNetPrimaryBits)?.Netmask.ToString(); //viene calcolato
        public string InternalBroadcastPrimary => Cidr.CalcNetwork(InternalHostIpPrimary, InternalNetPrimaryBits)?.Broadcast.ToString(); //viene calcolato
        public string InternalArpaPrimary => Cidr.IpArpaAnnotation(InternalHostIpPrimary, InternalNetPrimaryBits); //viene calcolato

        public string ExternalDomainPrimary { get; set; } = string.Empty; //domext.local
        public string ExternalHostIpPrimary { get; set; } = string.Empty; //192.168.111.0/24
        public string ExternalNetPrimaryBits { get; set; } = string.Empty; //24
        public string ExternalNetPrimary => Cidr.CalcNetwork(ExternalHostIpPrimary, ExternalNetPrimaryBits)?.Network.ToString(); //viene calcolato
        public string ExternalNetMaskPrimary => Cidr.CalcNetwork(ExternalHostIpPrimary, InternalNetPrimaryBits)?.Netmask.ToString(); //viene calcolato
        public string ExternalBroadcastPrimary => Cidr.CalcNetwork(ExternalHostIpPrimary, InternalNetPrimaryBits)?.Broadcast.ToString(); //viene calcolato
        public string ExternalArpaPrimary => Cidr.IpArpaAnnotation(ExternalHostIpPrimary, InternalNetPrimaryBits); //viene calcolato

        public string ResolvNameserver { get; set; } = string.Empty; //ip address
        public string ResolvDomain { get; set; } = string.Empty; //può essere il domain interno o esterno

        public string Secret { get; set; }

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

    public class Secret {
        public static string Gen(int length = 64) {
            var key = new byte[64];
            using(var rng = new RNGCryptoServiceProvider()) {
                rng.GetBytes(key);
            }
            var c = new byte[length];
            using(var rng = new RNGCryptoServiceProvider()) {
                rng.GetBytes(c);
            }
            byte[] hashValue;
            using(var hmac = new HMACMD5(key)) {
                hashValue = hmac.ComputeHash(c);
            }
            return Convert.ToBase64String(hashValue);
        }
    }

    public class Cidr {
        public static IPNetwork CalcNetwork(string ip, string bits) {
            if(string.IsNullOrEmpty(ip) || string.IsNullOrEmpty(bits)) {
                return null;
            }
            var net = $"{ip}/{bits}";
            var ipnetwork = IPNetwork.Parse(net);
            return ipnetwork;
        }

        public static string IpArpaAnnotation(string ip, string bits) {
            var ipNetwork = CalcNetwork(ip, bits);
            if(ipNetwork == null) {
                return null;
            }
            var subnet = ipNetwork.Network;
            //subnet    es 10.11.0.0
            //arr       es { "10", "11", "0", "0" }
            var arr = subnet.ToString().Split('.');
            //sarr      es { "10", "11" }
            var sarr = arr.Where(_ => _ != "0");
            //sarr      es { "11", "10" }
            var rra2 = sarr.Reverse();
            //join      es 11.10
            var join = string.Join(".", rra2);
            return join;
        }
    }
}
