using System.Collections.Generic;

namespace Antd.Dhcpd {
    public class DhcpdConfigurationModel {
        public bool IsActive { get; set; }

        public List<string> Allow { get; set; }
        public string UpdateStaticLeases { get; set; } //on off
        public string UpdateConflictDetection { get; set; }
        public string UseHostDeclNames { get; set; } //on off
        public string DoForwardUpdates { get; set; } //on off
        public string DoReverseUpdates { get; set; } //on off
        public string LogFacility { get; set; }
        public List<string> Option { get; set; }
        public string ZoneName { get; set; }
        public string ZonePrimaryAddress { get; set; }
        public string DdnsUpdateStyle { get; set; }
        public string DdnsUpdates { get; set; }
        public string DdnsDomainName { get; set; }
        public string DdnsRevDomainName { get; set; }
        public string DefaultLeaseTime { get; set; }
        public string MaxLeaseTime { get; set; }
        public string KeyName { get; set; }
        public string KeySecret { get; set; }

        public string SubnetIpFamily { get; set; }
        public string SubnetIpMask { get; set; }
        public string SubnetOptionRouters { get; set; }
        public string SubnetNtpServers { get; set; }
        public string SubnetTimeServers { get; set; }
        public string SubnetDomainNameServers { get; set; }
        public string SubnetBroadcastAddress { get; set; }
        public string SubnetMask { get; set; }

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
        public IEnumerable<string> Options { get; set; }
    }

    public class DhcpConfigurationReservationModel {
        public string Guid { get; set; } = System.Guid.NewGuid().ToString();
        public string HostName { get; set; }
        public string MacAddress { get; set; }
        public string IpAddress { get; set; }
    }
}