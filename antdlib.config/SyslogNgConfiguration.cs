using antdlib.common;
using antdlib.models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using IoDir = System.IO.Directory;

namespace antdlib.config {
    public class SyslogNgConfiguration {

        private readonly SyslogNgConfigurationModel _serviceModel;

        private readonly string _cfgFile = $"{Parameter.AntdCfgServices}/syslogng.conf";
        private readonly string _cfgFileBackup = $"{Parameter.AntdCfgServices}/syslogng.conf.bck";
        private const string ServiceName = "syslog-ng.service";
        private const string MainFilePath = "/etc/syslog-ng/syslog-ng.conf";
        private const string MainFilePathBackup = "/etc/syslog-ng/.syslog-ng.conf";

        public SyslogNgConfiguration() {
            IoDir.CreateDirectory(Parameter.AntdCfgServices);
            if(!File.Exists(_cfgFile)) {
                _serviceModel = new SyslogNgConfigurationModel();
            }
            else {
                try {
                    var text = File.ReadAllText(_cfgFile);
                    var obj = JsonConvert.DeserializeObject<SyslogNgConfigurationModel>(text);
                    _serviceModel = obj;
                }
                catch(Exception) {
                    _serviceModel = new SyslogNgConfigurationModel();
                }

            }
        }

        public void Save(SyslogNgConfigurationModel model) {
            var text = JsonConvert.SerializeObject(model, Formatting.Indented);
            if(File.Exists(_cfgFile)) {
                File.Copy(_cfgFile, _cfgFileBackup, true);
            }
            File.WriteAllText(_cfgFile, text);
            ConsoleLogger.Log("[syslogng] configuration saved");
        }

        public void Set() {
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
            var options = _serviceModel;
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

            File.WriteAllLines(MainFilePath, lines);
            #endregion
            Start();
        }

        public bool IsActive() {
            if(!File.Exists(_cfgFile)) {
                return false;
            }
            return _serviceModel != null && _serviceModel.IsActive;
        }

        public SyslogNgConfigurationModel Get() {
            return _serviceModel;
        }

        public void Enable() {
            _serviceModel.IsActive = true;
            Save(_serviceModel);
            ConsoleLogger.Log("[syslogng] enabled");
        }

        public void Disable() {
            _serviceModel.IsActive = false;
            Save(_serviceModel);
            ConsoleLogger.Log("[syslogng] disabled");
        }

        public void Stop() {
            Systemctl.Stop(ServiceName);
            ConsoleLogger.Log("[syslogng] stop");
        }

        public void Start() {
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
