using Newtonsoft.Json;
using System.Collections.Generic;
using anthilla.core;

namespace antdlib.models {
    public class DhcpdConfigurationModel {
        public bool IsActive { get; set; }

        public List<string> Allow { get; set; } = new List<string> { "client-updates", "unknown-clients" };
        public string UpdateStaticLeases { get; set; } = "on";
        public string UpdateConflictDetection { get; set; } = "false";
        public string UseHostDeclNames { get; set; } = "on";
        public string DoForwardUpdates { get; set; } = "on";
        public string DoReverseUpdates { get; set; } = "on";
        public string LogFacility { get; set; } = "local7";
        public List<string> Option { get; set; } = new List<string> { "routers eth0", "local-proxy-config code 252 = text" };
        public string ZoneName { get; set; } = "";
        public string ZonePrimaryAddress { get; set; } = "";
        public string DdnsUpdateStyle { get; set; } = "interim";
        public string DdnsUpdates { get; set; } = "on";
        public string DdnsDomainName { get; set; } = "";
        public string DdnsRevDomainName { get; set; } = "in-addr.arpa.";
        public string DefaultLeaseTime { get; set; } = "7200";
        public string MaxLeaseTime { get; set; } = "7200";
        public string KeyName { get; set; } = "updbindkey";
        public string KeySecret { get; set; } = "";

        public string SubnetIpFamily { get; set; } = "";
        public string SubnetIpMask { get; set; } = "255.255.0.0";
        public string SubnetOptionRouters { get; set; } = "";
        public string SubnetNtpServers { get; set; } = "";
        public string SubnetTimeServers { get; set; } = "";
        public string SubnetDomainNameServers { get; set; } = "";
        public string SubnetBroadcastAddress { get; set; } = "";
        public string SubnetMask { get; set; } = "255.255.0.0";

        [JsonIgnore]
        public string AllowString => Allow.JoinToString(", ");
        [JsonIgnore]
        public string OptionString => Option.JoinToString(", ");

        public List<DhcpConfigurationClassModel> Classes { get; set; } = new List<DhcpConfigurationClassModel>();
        public List<DhcpConfigurationPoolModel> Pools { get; set; } = new List<DhcpConfigurationPoolModel>();
        public List<DhcpConfigurationReservationModel> Reservations { get; set; } = new List<DhcpConfigurationReservationModel>();
    }

    public class DhcpConfigurationClassModel {
        public string Guid { get; set; } = System.Guid.NewGuid().ToString();
        public string Name { get; set; }
        public string MacVendor { get; set; }
    }

    public class DhcpConfigurationPoolModel {
        public string Guid { get; set; } = System.Guid.NewGuid().ToString();
        public List<string> Options { get; set; } = new List<string>();

        [JsonIgnore]
        public string OptionsString => Options.JoinToString(", ");
    }

    public class DhcpConfigurationReservationModel {
        public string Guid { get; set; } = System.Guid.NewGuid().ToString();
        public string HostName { get; set; }
        public string MacAddress { get; set; }
        public string IpAddress { get; set; }
    }
}