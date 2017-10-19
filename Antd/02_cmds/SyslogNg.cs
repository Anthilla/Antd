using System.Collections.Generic;
using System.IO;
using anthilla.core;

namespace Antd.cmds {

    public class SyslogNg {

        private const string syslogngConfFile = "/etc/syslog-ng/syslog-ng.conf";
        //private const string syslogngTmpConfFile = "/etc/syslog-ng/syslog-ng.conf";
        private const string sclConfFile = "/etc/syslog-ng/scl.conf";
        private const string sclTmpConfFile = "/etc/syslog-ng/scl.conf.tmp";
        private const string serviceName = "syslog-ng.service";

        public static void Apply() {
            var options = Application.CurrentConfiguration.Services.SyslogNg;
            if(options == null) {
                return;
            }
            WriteSclFile();
            var lines = new List<string> {
                "@version: 3.7",
                "@include \"scl.conf\"",
                "",
                "options {",
                "    threaded(yes);",
                "    chain_hostnames(no);",
                "    stats_freq(43200);",
                "    mark_freq(3600);",
                "    bad_hostname(\"^gconfd$\",",
                "    check_hostname(yes);",
                "    create_dirs(yes);",
                "    dir_perm(yes);",
                "    dns_cache(yes);",
                "    keep_hostname(yes);",
                "    perm(0644);",
                "    time_reap(30);",
                "    time_reopen(10);",
                "    use_dns(yes);",
                "    use_fqdn(yes);",
                "    flush_lines(0);",
                "};",
                "",
                "source s_Int0 {internal();};",
                "",
                "source s_Net1 {",
                CommonString.Append("    udp(port(", options.PortLevelApplication, ") flags(\"sanitize-utf8\",\"syslog-protocol\"));"),
                CommonString.Append("    tcp(port(", options.PortLevelApplication, ") flags(\"sanitize-utf8\",\"syslog-protocol\"));"),
                "};",
                "",
                "source s_Net2 {",
                CommonString.Append("    udp(port(", options.PortLevelSecurity, ") flags(\"sanitize-utf8\",\"syslog-protocol\"));"),
                CommonString.Append("    tcp(port(", options.PortLevelSecurity, ") flags(\"sanitize-utf8\",\"syslog-protocol\"));"),
                "};",
                "",
                "source s_Net3 {",
                CommonString.Append("    udp(port(", options.PortLevelSystem, ") flags(\"sanitize-utf8\",\"syslog-protocol\"));"),
                CommonString.Append("    tcp(port(", options.PortLevelSystem, ") flags(\"sanitize-utf8\",\"syslog-protocol\"));"),
                "};",
                "",
                CommonString.Append("destination d_Net1 { file(\"", options.RootPath,"/00_HOSTS_00/$HOST/$YEAR/$MONTH/$DAY/Application.log\" owner(root) group(wheel) perm(0644) dir_perm(0755) create_dirs(yes)); };"),
                CommonString.Append("destination d_Net2 { file(\"", options.RootPath,"/00_HOSTS_00/$HOST/$YEAR/$MONTH/$DAY/Security.log\" owner(root) group(wheel) perm(0644) dir_perm(0755) create_dirs(yes)); };"),
                CommonString.Append("destination d_Net3 { file(\"", options.RootPath,"/00_HOSTS_00/$HOST/$YEAR/$MONTH/$DAY/System.log\" owner(root) group(wheel) perm(0644) dir_perm(0755) create_dirs(yes)); };"),
                CommonString.Append("destination d_Int0 { file(\"", options.RootPath,"/00_HOSTS_00/$HOST/$YEAR/$MONTH/$DAY/Int0.log\" owner(root) group(wheel) perm(0644) dir_perm(0755) create_dirs(yes)); };"),
                "",
                "log {source(s_Net1); destination(d_Net1);};",
                "log {source(s_Net2); destination(d_Net2);};",
                "log {source(s_Net3); destination(d_Net3);};",
                "log {source(s_Int0); destination(d_Int0);};",
                ""
            };
            File.WriteAllLines(syslogngConfFile, lines);
            //var newHash = CommonFile.GetHash(syslogngTmpConfFile);
            //var existingHash = File.Exists(syslogngConfFile) ? CommonFile.GetHash(syslogngConfFile) : string.Empty;
            //if(CommonString.AreEquals(existingHash, newHash) == false) {
            //    File.Copy(syslogngTmpConfFile, syslogngConfFile, true);
            //}
            //if(File.Exists(syslogngTmpConfFile)) {
            //    File.Delete(syslogngTmpConfFile);
            //}
            ConsoleLogger.Log("[syslogng] apply conf");
            Start();
        }

        private static void WriteSclFile() {
            var lines = new string[] {
                "@define scl-root \"`syslog-ng-data`/include/scl\"",
                "@define include-path \"`include-path`:`syslog-ng-data`/include\"",
                "",
                "@include 'scl/*/*.conf'"
            };
            File.WriteAllLines(sclTmpConfFile, lines);
            var newHash = CommonFile.GetHash(sclTmpConfFile);
            var existingHash = File.Exists(sclConfFile) ? CommonFile.GetHash(sclConfFile) : string.Empty;
            if(CommonString.AreEquals(existingHash, newHash) == false) {
                File.Copy(sclTmpConfFile, sclConfFile, true);
            }
            if(File.Exists(sclTmpConfFile)) {
                File.Delete(sclTmpConfFile);
            }
        }

        public static void Start() {
            if(Systemctl.IsEnabled(serviceName) == false) {
                Systemctl.Enable(serviceName);
            }
            if(Systemctl.IsActive(serviceName) == false) {
                Systemctl.Restart(serviceName);
            }
            ConsoleLogger.Log("[syslogng] start service");
        }
    }
}