namespace Antd.SyslogNg {
    public class SyslogNgConfigurationModel {
        public bool IsActive { get; set; }
        public string Threaded { get; set; } = "yes";
        public string ChainHostname { get; set; } = "no";
        public string StatsFrequency { get; set; } = "43200";
        public string MarkFrequency { get; set; } = "3600";
        public string CheckHostname { get; set; } = "yes";
        public string CreateDirectories { get; set; } = "yes";
        public string DnsCache { get; set; } = "yes";
        public string KeepHostname { get; set; } = "yes";
        public string DirAcl { get; set; } = "0755";
        public string Acl { get; set; } = "0644";
        public string UseDns { get; set; } = "yes";
        public string UseFqdn { get; set; } = "yes";
        public string RootPath { get; set; } = "/var/log";
        public string PortLevelApplication { get; set; } = "511";
        public string PortLevelSecurity { get; set; } = "512";
        public string PortLevelSystem { get; set; } = "513";
    }
}