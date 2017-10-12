using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using LukeSkywalker.IPNetwork;
using Newtonsoft.Json;
using anthilla.core;

namespace antdlib.models {
    public class Host2Model {
        #region [    Host Var    ]

        public string HostName { get; set; } = "box01";
        public string HostChassis { get; set; } = "server";
        public string HostDeployment { get; set; } = "developement";
        public string HostLocation { get; set; } = "onEarth";

        public string HostAliasPrimary { get; set; } = "box01.domint.local";

        public string InternalDomainPrimary { get; set; } = "domint.local";
        public string InternalHostIpPrimary { get; set; } = "10.11.19.111"; 
        public string InternalNetPrimaryBits { get; set; } = "16"; 
        [JsonIgnore]
        public string InternalNetPrimary => Cidr.CalcNetwork(InternalHostIpPrimary, InternalNetPrimaryBits)?.Network.ToString();
        [JsonIgnore]
        public string InternalNetMaskPrimary => Cidr.CalcNetwork(InternalHostIpPrimary, InternalNetPrimaryBits)?.Netmask.ToString();
        [JsonIgnore]
        public string InternalBroadcastPrimary => Cidr.CalcNetwork(InternalHostIpPrimary, InternalNetPrimaryBits)?.Broadcast.ToString();
        [JsonIgnore]
        public string InternalArpaPrimary => Cidr.IpArpaAnnotation(InternalHostIpPrimary, InternalNetPrimaryBits);

        public string ExternalDomainPrimary { get; set; } = "domext.local";
        public string ExternalHostIpPrimary { get; set; } = "192.168.111.0"; 
        public string ExternalNetPrimaryBits { get; set; } = "24";
        [JsonIgnore]
        public string ExternalNetPrimary => Cidr.CalcNetwork(ExternalHostIpPrimary, ExternalNetPrimaryBits)?.Network.ToString();
        [JsonIgnore]
        public string ExternalNetMaskPrimary => Cidr.CalcNetwork(ExternalHostIpPrimary, InternalNetPrimaryBits)?.Netmask.ToString();
        [JsonIgnore]
        public string ExternalBroadcastPrimary => Cidr.CalcNetwork(ExternalHostIpPrimary, InternalNetPrimaryBits)?.Broadcast.ToString();
        [JsonIgnore]
        public string ExternalArpaPrimary => Cidr.IpArpaAnnotation(ExternalHostIpPrimary, InternalNetPrimaryBits);

        public string ResolvNameserver { get; set; } = "8.8.8.8";
        public string ResolvDomain { get; set; } = "domint.local"; 

        public string Secret => File.ReadAllText(Parameter.AntdCfgSecret);

        #endregion

        #region [    Time Var    ]
        public string Timezone { get; set; } = "Europe/Rome";
        public string NtpdateServer { get; set; } = "0.it.pool.ntp.org";
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
