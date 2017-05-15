using antdlib.common;
using antdlib.models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace antdlib.config {
    public static class SyslogNgConfiguration {

        private static SyslogNgConfigurationModel ServiceModel => Load();

        private static readonly string CfgFile = $"{Parameter.AntdCfgServices}/syslogng.conf";
        private const string ServiceName = "syslog-ng.service";
        private const string MainFilePath = "/etc/syslog-ng/syslog-ng.conf";
        private const string MainFilePathBackup = "/etc/syslog-ng/.syslog-ng.conf";

        private static SyslogNgConfigurationModel Load() {
            if(!File.Exists(CfgFile)) {
                return new SyslogNgConfigurationModel();
            }
            try {
                var text = File.ReadAllText(CfgFile);
                var obj = JsonConvert.DeserializeObject<SyslogNgConfigurationModel>(text);
                return obj;
            }
            catch(Exception) {
                return new SyslogNgConfigurationModel();
            }
        }

        public static void Save(SyslogNgConfigurationModel model) {
            var text = JsonConvert.SerializeObject(model, Formatting.Indented);
            FileWithAcl.WriteAllText(CfgFile, text);
            ConsoleLogger.Log("[syslogng] configuration saved");
        }

        public static void Set() {
            Enable();
            Stop();
            #region [    syslog-ng.conf generation    ]
            if(File.Exists(MainFilePath)) {
                if(File.Exists(MainFilePathBackup)) {
                    File.Delete(MainFilePathBackup);
                }
                File.Copy(MainFilePath, MainFilePathBackup);
            }
            var lines = new List<string> {
                "@version: 3.7",
                "@include \"scl.conf\"",
                "options {"
            };
            var options = ServiceModel;
            lines.Add($"threaded({options.Threaded});");
            lines.Add($"chain_hostnames({options.ChainHostname});");
            lines.Add($"stats_freq({options.StatsFrequency});");
            lines.Add($"mark_freq({options.MarkFrequency});");
            lines.Add("bad_hostname(\"^gconfd$\");");
            lines.Add($"check_hostname({options.CheckHostname});");
            lines.Add($"create_dirs({options.CreateDirectories});");
            lines.Add($"dir_perm({options.DirAcl});");
            lines.Add($"dns_cache({options.DnsCache});");
            lines.Add($"keep_hostname({options.KeepHostname});");
            lines.Add($"perm({options.Acl});");
            lines.Add("time_reap(30);");
            lines.Add("time_reopen(10);");
            lines.Add($"use_dns({options.UseDns});");
            lines.Add($"use_fqdn({options.UseFqdn});");
            lines.Add("flush_lines(0);");
            lines.Add("};");
            lines.Add("");
            lines.Add("source s_Int0 {internal();};");

            lines.Add("source s_Net1 {");
            lines.Add($"udp(port({options.PortLevelApplication}) flags(\"sanitize-utf8\",\"syslog-protocol\"));");
            lines.Add($"tcp(port({options.PortLevelApplication}) flags(\"sanitize-utf8\",\"syslog-protocol\"));");
            lines.Add("};");
            lines.Add("source s_Net2 {");
            lines.Add($"udp(port({options.PortLevelSecurity}) flags(\"sanitize-utf8\",\"syslog-protocol\"));");
            lines.Add($"tcp(port({options.PortLevelSecurity}) flags(\"sanitize-utf8\",\"syslog-protocol\"));");
            lines.Add("};");
            lines.Add("source s_Net3 {");
            lines.Add($"udp(port({options.PortLevelSystem}) flags(\"sanitize-utf8\",\"syslog-protocol\"));");
            lines.Add($"tcp(port({options.PortLevelSystem}) flags(\"sanitize-utf8\",\"syslog-protocol\"));");
            lines.Add("};");
            lines.Add("");

            lines.Add("destination d_Net1 {{ file(\"/var/log/00_HOSTS_00/$HOST/$YEAR/$MONTH/$DAY/Application.log\" owner(root) group(wheel) perm(0644) dir_perm(0755) create_dirs(yes)); }};");
            lines.Add("destination d_Net2 { file(\"/var/log/00_HOSTS_00/$HOST/$YEAR/$MONTH/$DAY/Security.log\" owner(root) group(wheel) perm(0644) dir_perm(0755) create_dirs(yes)); };");
            lines.Add("destination d_Net3 { file(\"/var/log/00_HOSTS_00/$HOST/$YEAR/$MONTH/$DAY/System.log\" owner(root) group(wheel) perm(0644) dir_perm(0755) create_dirs(yes)); };");
            lines.Add("destination d_Int0 { file(\"/var/log/00_HOSTS_00/$HOST/$YEAR/$MONTH/$DAY/Int0.log\" owner(root) group(wheel) perm(0644) dir_perm(0755) create_dirs(yes)); };");
            lines.Add("");

            lines.Add("log {source(s_Net1); destination(d_Net1);};");
            lines.Add("log {source(s_Net2); destination(d_Net2);};");
            lines.Add("log {source(s_Net3); destination(d_Net3);};");
            lines.Add("log {source(s_Int0); destination(d_Int0);};");
            lines.Add("");

            FileWithAcl.WriteAllLines(MainFilePath, lines, "644", "root", "wheel");
            #endregion
            Start();
        }

        public static bool IsActive() {
            if(!File.Exists(CfgFile)) {
                return false;
            }
            return ServiceModel != null && ServiceModel.IsActive;
        }

        public static SyslogNgConfigurationModel Get() {
            return ServiceModel;
        }

        public static void Enable() {
            ServiceModel.IsActive = true;
            Save(ServiceModel);
            ConsoleLogger.Log("[syslogng] enabled");
        }

        public static void Disable() {
            ServiceModel.IsActive = false;
            Save(ServiceModel);
            ConsoleLogger.Log("[syslogng] disabled");
        }

        public static void Stop() {
            Systemctl.Stop(ServiceName);
            ConsoleLogger.Log("[syslogng] stop");
        }

        public static void Start() {
            if(Systemctl.IsEnabled(ServiceName) == false) {
                Systemctl.Enable(ServiceName);
            }
            if(Systemctl.IsActive(ServiceName) == false) {
                Systemctl.Restart(ServiceName);
            }
            ConsoleLogger.Log("[syslogng] start");
        }
    }
}
