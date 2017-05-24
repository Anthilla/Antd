using System.Collections.Generic;

namespace antdlib.models {
    public class BindConfigurationModel {
        public bool IsActive { get; set; }

        public string Notify { get; set; } = "no";
        public string MaxCacheSize { get; set; } = "128M";
        public string MaxCacheTtl { get; set; } = "108000";
        public string MaxNcacheTtl { get; set; } = "3";
        public List<string> Forwarders { get; set; } = new List<string>(); // { "8.8.8.8", "8.8.4.4" };
        public List<string> AllowNotify { get; set; } = new List<string>(); // { "iif", "inet" };
        public List<string> AllowTransfer { get; set; } = new List<string>(); //  { "iif", "inet" };
        public string Recursion { get; set; } = "yes";
        public string TransferFormat { get; set; } = "many-answers";
        public string QuerySourceAddress { get; set; } = "*";
        public string QuerySourcePort { get; set; } = "*";
        public string Version { get; set; } = "none";
        public List<string> AllowQuery { get; set; } = new List<string>(); //  { "loif", "iif", "oif", "lonet", "inet", "onet" };
        public List<string> AllowRecursion { get; set; } = new List<string>(); // { "loif", "iif", "oif", "lonet", "inet", "onet" };
        public string IxfrFromDifferences { get; set; } = "yes";
        public List<string> ListenOnV6 { get; set; } = new List<string>(); //  { "none" };
        public List<string> ListenOnPort53 { get; set; } = new List<string>(); //  { "loif", "iif", "oif" };
        public string DnssecEnabled { get; set; } = "yes";
        public string DnssecValidation { get; set; } = "yes";
        public string DnssecLookaside { get; set; } = "auto";
        public string AuthNxdomain { get; set; } = "yes";

        public string KeyName { get; set; } = "";
        public string KeySecret { get; set; } = "";

        public string ControlIp { get; set; } = "10.11.19.1";
        public string ControlPort { get; set; } = "953";
        public List<string> ControlAllow { get; set; } = new List<string>(); //  { "loif", "iif", "oif" };
        public List<string> ControlKeys { get; set; } = new List<string>(); //  { "updbindkey" };

        public string SyslogSeverity { get; set; } = "info";
        public string SyslogPrintCategory { get; set; } = "yes";
        public string SyslogPrintSeverity { get; set; } = "yes";
        public string SyslogPrintTime { get; set; } = "yes";

        public string TrustedKeys { get; set; } = "";

        public List<BindConfigurationAcl> AclList { get; set; } = new List<BindConfigurationAcl>();

        public List<BindConfigurationZoneModel> Zones { get; set; } = new List<BindConfigurationZoneModel>();

        public List<string> IncludeFiles { get; set; } = new List<string>();

        public List<BindConfigurationZoneFileModel> ZoneFiles { get; set; } = new List<BindConfigurationZoneFileModel>();
    }

    public class BindConfigurationAcl {
        public string Guid { get; set; }
        public string Name { get; set; }
        public List<string> InterfaceList { get; set; } = new List<string>();
    }

    public class BindConfigurationZoneModel {
        public string Guid { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string File { get; set; }
        public string SerialUpdateMethod { get; set; } = "unixtime";
        public List<string> AllowUpdate { get; set; } = new List<string>(); //  { "loif", "iif", "lonet", "inet", "onet", "key updbindkey" };
        public List<string> AllowQuery { get; set; } = new List<string>(); //  { "any" };
        public List<string> AllowTransfer { get; set; } = new List<string>(); //   { "loif", "iif", "lonet", "inet", "onet" };
    }

    public class BindConfigurationZoneFileModel {
        public string Guid { get; set; } = System.Guid.NewGuid().ToString();
        public string Name { get; set; }
        public string Configuration { get; set; }
    }
}