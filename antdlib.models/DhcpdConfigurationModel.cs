using System.Collections.Generic;

namespace antdlib.models {
    public class DhcpdConfigurationModel {
        public bool IsActive { get; set; }

        public List<string> Allow { get; set; } = new List<string>(); //{ "client-updates", "unknown-clients" };

        public string UpdateStaticLeases { get; set; } = "on";
        public string UpdateConflictDetection { get; set; } = "false";
        public string UseHostDeclNames { get; set; } = "on";
        public string DoForwardUpdates { get; set; } = "on";
        public string DoReverseUpdates { get; set; } = "on";
        public string LogFacility { get; set; } = "local7";
        public string ZoneName { get; set; } = "";
        public string ZonePrimaryAddress { get; set; } = "";
        public string ZonePrimaryKey { get; set; } = "";
        public string DdnsUpdateStyle { get; set; } = "interim";
        public string DdnsUpdates { get; set; } = "on";
        public string DdnsDomainName { get; set; } = "";
        public string DdnsRevDomainName { get; set; } = "in-addr.arpa.";
        public string DefaultLeaseTime { get; set; } = "7200";
        public string MaxLeaseTime { get; set; } = "7200";
        public string OptionRouters { get; set; } = "7200";
        public string OptionLocalProxy { get; set; } = "7200";
        public string OptionDomainName { get; set; } = "7200";
        public string KeyName { get; set; } = "updbindkey";
        public string KeySecret { get; set; } = "";

        public List<DhcpdSubnet> Subnets { get; set; } = new List<DhcpdSubnet>();
        public List<DhcpdClass> Classes { get; set; } = new List<DhcpdClass>();
        public List<DhcpdReservation> Reservations { get; set; } = new List<DhcpdReservation>();
    }

    public class DhcpdSubnet {
        public string Guid { get; set; } = System.Guid.NewGuid().ToString();
        public string SubnetIpFamily { get; set; } = "";
        public string SubnetIpMask { get; set; } = "255.255.0.0";
        public string SubnetOptionRouters { get; set; } = "";
        public string SubnetNtpServers { get; set; } = "";
        public string SubnetTimeServers { get; set; } = "";
        public string SubnetDomainNameServers { get; set; } = "";
        public string SubnetBroadcastAddress { get; set; } = "";
        public string SubnetMask { get; set; } = "255.255.0.0";
        public string ZoneName { get; set; } = "";
        public string ZonePrimaryAddress { get; set; } = "";
        public string ZonePrimaryKey { get; set; } = "";
        public string PoolDynamicRangeStart { get; set; } = "";
        public string PoolDynamicRangeEnd { get; set; } = "";
        public List<DhcpdPool> Pools { get; set; } = new List<DhcpdPool>();
    }

    public class DhcpdPool {
        public string Guid { get; set; } = System.Guid.NewGuid().ToString();
        public string ClassName { get; set; } = "";
        public string PoolRangeStart { get; set; } = "";
        public string PoolRangeEnd { get; set; } = "";
    }

    public class DhcpdClass {
        public string Guid { get; set; } = System.Guid.NewGuid().ToString();
        public string Name { get; set; }
        public string VendorMacAddress { get; set; }
    }

    public class DhcpdReservation {
        public string Guid { get; set; } = System.Guid.NewGuid().ToString();
        public string HostName { get; set; }
        public string MacAddress { get; set; }
        public string IpAddress { get; set; }
    }
}