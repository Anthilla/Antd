using antdlib.common;
using System.Collections.Generic;
using System.IO;

namespace antdlib.models {
    public class Network2ConfigurationModel {
        public List<NetworkInterface> Interfaces { get; set; } = new List<NetworkInterface>();
        public string ActiveDnsConfiguration { get; set; } = string.Empty; //Id
    }

    public class NetworkInterface {
        public string Device { get; set; } = string.Empty;
        public string Configuration { get; set; } = string.Empty; //Id
        public List<string> AdditionalConfigurations { get; set; } = new List<string>(); //Ids
        public string GatewayConfiguration { get; set; } = string.Empty; //Id
        public string Txqueuelen { get; set; } = "10000";
        public string Mtu { get; set; } = "6000";
    }

    public class NetworkInterfaceConfiguration {
        public string Id { get; set; } = string.Empty;
        public NetworkInterfaceType Type { get; set; } = NetworkInterfaceType.Null;
        public string Hostname { get; set; } = string.Empty; //srv01ext.domext.local    
        public int Index { get; set; } //indice Extenal / Internal
        public string Description { get; set; } = string.Empty; //Internet
        public NetworkRoleVerb RoleVerb { get; set; }
        public string Alias { get; set; } = string.Empty; //extif
        public NetworkInterfaceMode Mode { get; set; } = NetworkInterfaceMode.Static;
        public NetworkInterfaceStatus Status { get; set; } = NetworkInterfaceStatus.Up;
        public string Ip { get; set; } = string.Empty; //192.168.111.2
        public string Subnet { get; set; } = string.Empty; //24 -> 192.168.111.0 --> /24
        public string Broadcast { get; set; } = string.Empty; //192.168.111.255
        public NetworkAdapterType Adapter { get; set; } = NetworkAdapterType.Physical;
        public List<string> ChildrenIf { get; set; } = new List<string>(); // bridge / bond
        public bool IsUsed { get; set; } //viene calcolato
    }

    public class NetworkGatewayConfiguration {
        public string Id { get; set; } = string.Empty;
        public string Route { get; set; } = "default";
        public string GatewayAddress { get; set; } = string.Empty;
        public bool IsUsed { get; set; } //viene calcolato
    }

    public class DnsConfiguration {
        public string Id { get; set; } = string.Empty;
        public DnsType Type { get; set; } = DnsType.Public;
        public DnsMode Mode { get; set; } = DnsMode.Dynamic;
        public DnsDestination Dest { get; set; } = DnsDestination.Internal;
        public string Domain { get; set; } = string.Empty;
        public string Ip { get; set; } = string.Empty;
        public bool AuthenticationEnabled { get; set; } = true;
    }

    public class NsUpdateConfiguration {
        public string Id { get; set; } = string.Empty;
        public string ServerName { get; set; } = string.Empty;
        public string ServerPort { get; set; } = string.Empty;
        public string LocalAddress { get; set; } = string.Empty;
        public string LocalPort { get; set; } = string.Empty;
        public string ZoneName { get; set; } = string.Empty;
        public string ClassName { get; set; } = string.Empty;
        public string KeyName { get; set; } = "upbindkey";
        public string KeySecret => File.ReadAllText(Parameter.AntdCfgSecret);
        public string NxDomain { get; set; } = string.Empty;
        public string YxDomain { get; set; } = string.Empty;
        public string NxRrset { get; set; } = string.Empty;
        public string YxRrset { get; set; } = string.Empty;
        public string Delete { get; set; } = string.Empty;
        public string Add { get; set; } = string.Empty;
    }
}
