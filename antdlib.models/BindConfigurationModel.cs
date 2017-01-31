using System.Collections.Generic;
using antdlib.common;
using Newtonsoft.Json;

namespace antdlib.models {
    public class BindConfigurationModel {
        public bool IsActive { get; set; }

        public string Notify { get; set; } = "no";
        public string MaxCacheSize { get; set; } = "128M";
        public string MaxCacheTtl { get; set; } = "108000";
        public string MaxNcacheTtl { get; set; } = "3";
        public List<string> Forwarders { get; set; } = new List<string> { "8.8.8.8", "4.4.4.4" };
        public List<string> AllowNotify { get; set; } = new List<string> { "iif", "inet" };
        public List<string> AllowTransfer { get; set; } = new List<string> { "iif", "inet" };
        public string Recursion { get; set; } = "yes";
        public string TransferFormat { get; set; } = "many-answers";
        public string QuerySourceAddress { get; set; } = "*";
        public string QuerySourcePort { get; set; } = "*";
        public string Version { get; set; } = "none";
        public List<string> AllowQuery { get; set; } = new List<string> { "loif", "iif", "oif", "lonet", "inet", "onet" };
        public List<string> AllowRecursion { get; set; } = new List<string> { "loif", "iif", "oif", "lonet", "inet", "onet" };
        public string IxfrFromDifferences { get; set; } = "yes";
        public List<string> ListenOnV6 { get; set; } = new List<string> { "none" };
        public List<string> ListenOnPort53 { get; set; } = new List<string> { "loif", "iif", "oif" };
        public string DnssecEnabled { get; set; } = "yes";
        public string DnssecValidation { get; set; } = "yes";
        public string DnssecLookaside { get; set; } = "auto";
        public string AuthNxdomain { get; set; } = "yes";
        public string KeyName { get; set; } = "";
        public string KeySecret { get; set; } = "";
        public string ControlAcl { get; set; } = "inet";
        public string ControlIp { get; set; } = "10.1.19.1";
        public string ControlPort { get; set; } = "953";
        public List<string> ControlAllow { get; set; } = new List<string> { "loif", "iif", "oif" };
        public string LoggingChannel { get; set; } = "syslog";
        public string LoggingDaemon { get; set; } = "syslogsyslog daemon";
        public string LoggingSeverity { get; set; } = "info";
        public string LoggingPrintCategory { get; set; } = "yes";
        public string LoggingPrintSeverity { get; set; } = "yes";
        public string LoggingPrintTime { get; set; } = "yes";
        public string TrustedKeys { get; set; } = "";
        public List<string> AclLocalInterfaces { get; set; } = new List<string> { "127.0.0.1" };
        public List<string> AclInternalInterfaces { get; set; } = new List<string> { "10.1.19.1", "10.99.19.1" };
        public List<string> AclExternalInterfaces { get; set; } = new List<string> { "192.168.111.2", "192.168.222.2" };
        public List<string> AclLocalNetworks { get; set; } = new List<string> { "127.0.0.0/8" };
        public List<string> AclInternalNetworks { get; set; } = new List<string> { "10.1.0.0/16", "10.99.0.0/16" };
        public List<string> AclExternalNetworks { get; set; } = new List<string> { "192.168.111.2/32", "192.168.222.2/32" };

        [JsonIgnore]
        public string ForwardersString => Forwarders.JoinToString(", ");
        [JsonIgnore]
        public string AllowNotifyString => AllowNotify.JoinToString(", ");
        [JsonIgnore]
        public string AllowTransferString => AllowTransfer.JoinToString(", ");
        [JsonIgnore]
        public string AllowQueryString => AllowQuery.JoinToString(", ");
        [JsonIgnore]
        public string AllowRecursionString => AllowRecursion.JoinToString(", ");
        [JsonIgnore]
        public string ListenOnV6String => ListenOnV6.JoinToString(", ");
        [JsonIgnore]
        public string ListenOnPort53String => ListenOnPort53.JoinToString(", ");
        [JsonIgnore]
        public string ControlAllowString => ControlAllow.JoinToString(", ");
        [JsonIgnore]
        public string AclLocalInterfacesString => AclLocalInterfaces.JoinToString(", ");
        [JsonIgnore]
        public string AclInternalInterfacesString => AclInternalInterfaces.JoinToString(", ");
        [JsonIgnore]
        public string AclExternalInterfacesString => AclExternalInterfaces.JoinToString(", ");
        [JsonIgnore]
        public string AclLocalNetworksString => AclLocalNetworks.JoinToString(", ");
        [JsonIgnore]
        public string AclInternalNetworksString => AclInternalNetworks.JoinToString(", ");
        [JsonIgnore]
        public string AclExternalNetworksString => AclExternalNetworks.JoinToString(", ");

        public List<BindConfigurationZoneModel> Zones { get; set; } = new List<BindConfigurationZoneModel>();
        public List<BindConfigurationZoneFileModel> ZoneFiles { get; set; } = new List<BindConfigurationZoneFileModel>();
    }

    public class BindConfigurationZoneModel {
        public string Guid { get; set; } = System.Guid.NewGuid().ToString();
        public string Name { get; set; }
        public string Type { get; set; }
        public string File { get; set; }
        public string SerialUpdateMethod { get; set; } = "unixtime";
        public List<string> AllowUpdate { get; set; } = new List<string> { "loif", "iif", "lonet", "inet", "onet", "key updbindkey" };
        public List<string> AllowQuery { get; set; } = new List<string> { "any" };
        public List<string> AllowTransfer { get; set; } = new List<string> { "loif", "iif", "lonet", "inet", "onet" };

        [JsonIgnore]
        public string AllowUpdateString => AllowUpdate.JoinToString(", ");
        [JsonIgnore]
        public string AllowQueryString => AllowQuery.JoinToString(", ");
        [JsonIgnore]
        public string AllowTransferString => AllowTransfer.JoinToString(", ");
    }

    public class BindConfigurationZoneFileModel {
        public string Guid { get; set; } = System.Guid.NewGuid().ToString();
        public string Name { get; set; }
        public string Configuration { get; set; }
    }
}