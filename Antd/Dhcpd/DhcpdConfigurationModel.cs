using System.Collections.Generic;
using antdlib.common;
using Newtonsoft.Json;

namespace Antd.Dhcpd {
    public class DhcpdConfigurationModel {
        public bool IsActive { get; set; }

        public List<string> Allow { get; set; } = new List<string>();
        public string UpdateStaticLeases { get; set; } = "";
        public string UpdateConflictDetection { get; set; } = "";
        public string UseHostDeclNames { get; set; } = "";
        public string DoForwardUpdates { get; set; } = "";
        public string DoReverseUpdates { get; set; } = "";
        public string LogFacility { get; set; } = "";
        public List<string> Option { get; set; } = new List<string>();
        public string ZoneName { get; set; } = "";
        public string ZonePrimaryAddress { get; set; } = "";
        public string DdnsUpdateStyle { get; set; } = "";
        public string DdnsUpdates { get; set; } = "";
        public string DdnsDomainName { get; set; } = "";
        public string DdnsRevDomainName { get; set; } = "";
        public string DefaultLeaseTime { get; set; } = "";
        public string MaxLeaseTime { get; set; } = "";
        public string KeyName { get; set; } = "";
        public string KeySecret { get; set; } = "";

        public string SubnetIpFamily { get; set; } = "";
        public string SubnetIpMask { get; set; } = "";
        public string SubnetOptionRouters { get; set; } = "";
        public string SubnetNtpServers { get; set; } = "";
        public string SubnetTimeServers { get; set; } = "";
        public string SubnetDomainNameServers { get; set; } = "";
        public string SubnetBroadcastAddress { get; set; } = "";
        public string SubnetMask { get; set; } = "";

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