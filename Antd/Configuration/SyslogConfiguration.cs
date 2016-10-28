using System.Collections.Generic;
using System.IO;
using antdlib.common;
using antdlib.Systemd;
using antdlib.views;
using Antd.Database;

namespace Antd.Configuration {
    public class SyslogConfiguration {

        private const string ConfigurationPath = "/etc/syslog-ng/syslog-ng.conf";
        private const string ServiceName = "syslog-ng.service";
        private static readonly SyslogRepository SyslogRepository = new SyslogRepository();

        public static bool Set() {
            Directory.CreateDirectory(Parameter.RepoConfig);
            var config = SyslogRepository.Get();
            if (config == null) {
                return false;
            }
            var model = new SyslogModel(config);
            WriteFile(model);
            RestartService();
            return true;
        }

        private static void WriteFile(SyslogModel model) {
            if (File.Exists(ConfigurationPath)) {

            }
            var headLines = new List<string> {
            "@version: 3.7",
            "@include \"scl.conf\"",
            "",
            "options {",
            "threaded(yes);",
            "chain_hostnames(no);",
            "stats_freq(43200);",
            "mark_freq(3600);",
            "bad_hostname(\"^gconfd$\");",
            "check_hostname(yes);",
            "create_dirs(yes);",
            "dir_perm(0755);",
            "dns_cache(yes);",
            "#group(wheel);",
            "keep_hostname(yes);",
            "#log_fifo_size(2048);",
            "perm(0644);",
            "time_reap(30);",
            "time_reopen(10);",
            "use_dns(yes);",
            "use_fqdn(yes);",
            "flush_lines(0);",
            "};",
            ""
            };

            var sourceLines = new List<string> {
                "source s_Int0 {internal();};",
                "source s_Net1 {",
                $"udp(port({model.PortNet1}) flags(\"sanitize-utf8\",\"syslog-protocol\"));",
                $"tcp(port({model.PortNet1}) flags(\"sanitize-utf8\",\"syslog-protocol\"));",
                "};",
                "source s_Net2 {",
                $"udp(port({model.PortNet2}) flags(\"sanitize-utf8\",\"syslog-protocol\"));",
                $"tcp(port({model.PortNet2}) flags(\"sanitize-utf8\",\"syslog-protocol\"));",
                "};",
                "source s_Net3 {",
                $"udp(port({model.PortNet3}) flags(\"sanitize-utf8\",\"syslog-protocol\"));",
                $"tcp(port({model.PortNet3}) flags(\"sanitize-utf8\",\"syslog-protocol\"));",
                "};"
            };
            foreach (var service in model.Services) {
                sourceLines.Add($"source s_{service.Key.Trim()} {{");
                sourceLines.Add($"udp(port({service.Value}) flags(\"sanitize-utf8\",\"syslog-protocol\"));");
                sourceLines.Add($"tcp(port({service.Value}) flags(\"sanitize-utf8\",\"syslog-protocol\"));");
                sourceLines.Add("};");
            }
            sourceLines.Add("");
            headLines.AddRange(sourceLines);

            var destinationLines = new List<string> {
                $"destination d_Net1 {{ file(\"{model.RootPath}/$HOST/$YEAR/$MONTH/$DAY/Application.log\" owner(root) group(wheel) perm(0644) dir_perm(0755) create_dirs(yes)); }};",
                $"destination d_Net2 {{ file(\"{model.RootPath}/$HOST/$YEAR/$MONTH/$DAY/Security.log\" owner(root) group(wheel) perm(0644) dir_perm(0755) create_dirs(yes)); }};",
                $"destination d_Net3 {{ file(\"{model.RootPath}/$HOST/$YEAR/$MONTH/$DAY/System.log\" owner(root) group(wheel) perm(0644) dir_perm(0755) create_dirs(yes)); }};",
                $"destination d_Int0 {{ file(\"{model.RootPath}/$HOST/$YEAR/$MONTH/$DAY/Int0.log\" owner(root) group(wheel) perm(0644) dir_perm(0755) create_dirs(yes)); }};"
            };
            foreach (var service in model.Services) {
                destinationLines.Add($"destination d_{service.Key.Trim()} {{ file(\"{model.RootPath}/$HOST/$YEAR/$MONTH/$DAY/{service.Key}.log\" owner(root) group(wheel) perm(0644) dir_perm(0755) create_dirs(yes)); }};");
            }
            destinationLines.Add("");
            headLines.AddRange(destinationLines);

            var logLines = new List<string> {
                "log {source(s_Net1); destination(d_Net1);};",
                "log {source(s_Net2); destination(d_Net2);};",
                "log {source(s_Net3); destination(d_Net3);};",
                "log {source(s_Int0); destination(d_Int0);};"
            };
            foreach (var service in model.Services) {
                logLines.Add($"log {{source(s_{service.Key.Trim()}); destination(d_{service.Key.Trim()});}};");
            }
            logLines.Add("");
            headLines.AddRange(logLines);

            File.WriteAllLines(ConfigurationPath, headLines);
        }

        public static void RestartService() {
            if (Systemctl.IsEnabled(ServiceName) == false) {
                Systemctl.Enable(ServiceName);
            }
            Systemctl.Restart(ServiceName);
        }
    }
}
