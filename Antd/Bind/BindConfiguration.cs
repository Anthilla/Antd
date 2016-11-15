using System.Collections.Generic;
using System.IO;
using System.Linq;
using antdlib.common;
using antdlib.common.Tool;
using antdlib.Systemd;
using antdlib.views;
using Antd.Database;

namespace Antd.Bind {
    public class BindConfiguration {

        private const string ServiceName = "named.service";
        private const string MainFilePath = "/etc/bind/named.conf";
        private const string MainFilePathBackup = "/etc/bind/.named.conf";
        private readonly BindServerOptionsRepository _bindServerOptionsRepository = new BindServerOptionsRepository();
        private readonly BindServerZoneRepository _bindServerZoneRepository = new BindServerZoneRepository();
        private readonly BindServerZoneFileRepository _bindServerZoneFileRepository = new BindServerZoneFileRepository();
        private readonly Bash _bash = new Bash();

        public void Set() {
            var o = _bindServerOptionsRepository.Get();
            if(o == null) {
                return;
            }
            if(File.Exists(MainFilePath)) {
                if(File.Exists(MainFilePathBackup)) {
                    File.Delete(MainFilePathBackup);
                }
                File.Copy(MainFilePath, MainFilePathBackup);
            }
            var lines = new List<string> {
                "options {"
            };
            var options = new BindServerOptionsModel(o);
            lines.Add($"notify {options.Notify};");
            lines.Add($"max-cache-size {options.MaxCacheSize};");
            lines.Add($"max-cache-ttl {options.MaxCacheTtl};");
            lines.Add($"max-ncache-ttl {options.MaxNcacheTtl};");
            if(options.Forwarders.Any()) {
                lines.Add("forwarders {");
                foreach(var fwd in options.Forwarders) {
                    lines.Add($"{fwd};");
                }
                lines.Add("}");
            }
            lines.Add($"forwarders {{ {options.Forwarders.JoinToString("; ")} }}");
            lines.Add($"allow-notify {{ {options.AllowNotify.JoinToString("; ")} }}");
            lines.Add($"allow-transfer {{ {options.AllowTransfer.JoinToString("; ")} }}");
            lines.Add($"recursion {options.Recursion};");
            lines.Add($"transfer-format {options.TransferFormat};");
            lines.Add($"query-source address {options.QuerySourceAddress} port {options.QuerySourcePort};");
            lines.Add($"version {options.Version};");
            lines.Add($"allow-query {{ {options.AllowQuery.JoinToString("; ")} }}");
            lines.Add($"allow-recursion {{ {options.AllowRecursion.JoinToString("; ")} }}");
            lines.Add($"ixfr-from-differences {options.IxfrFromDifferences};");
            lines.Add($"listen-on-v6 {{ {options.ListenOnV6.JoinToString("; ")} }}");
            lines.Add($"listen-on port 53 {{ {options.ListenOnPort53.JoinToString("; ")} }}");
            lines.Add($"dnssec-enable {options.DnssecEnabled};");
            lines.Add($"dnssec-validation {options.DnssecValidation};");
            lines.Add($"dnssec-lookaside {options.DnssecLookaside};");
            lines.Add($"auth-nxdomain {options.AuthNxdomain};");
            lines.Add("};");
            lines.Add("");

            lines.Add($"key \"{options.KeyName}\" {{");
            lines.Add("algorithm hmac-md5;");
            lines.Add($"secret \"{options.KeySecret}\";");
            lines.Add("};");
            lines.Add("");

            lines.Add($"controls {{ {options.ControlAcl} {options.ControlIp} port {options.ControlPort} allow {{ {options.ControlAllow.JoinToString("; ")} }}");
            lines.Add("");

            lines.Add($"acl loif {{ {options.AclLocalInterfaces.JoinToString("; ")} }}");
            lines.Add($"acl iif {{ {options.AclInternalInterfaces.JoinToString("; ")} }}");
            lines.Add($"acl oif {{ {options.AclExternalInterfaces.JoinToString("; ")} }}");
            lines.Add($"acl lonet {{ {options.AclLocalNetworks.JoinToString("; ")} }}");
            lines.Add($"acl inet {{ {options.AclInternalNetworks.JoinToString("; ")} }}");
            lines.Add($"acl onet {{ {options.AclExternalNetworks.JoinToString("; ")} }}");
            lines.Add("");

            lines.Add("logging {");
            lines.Add($"channel {options.LoggingChannel} {{");
            lines.Add($"{options.LoggingDaemon};");
            lines.Add($"severity {options.LoggingSeverity};");
            lines.Add($"print-category {options.LoggingPrintCategory};");
            lines.Add($"print-severity {options.LoggingPrintSeverity};");
            lines.Add($"print-time {options.LoggingPrintTime};");
            lines.Add("};");
            lines.Add($"category client {{ {options.LoggingChannel} }};");
            lines.Add($"category config {{ {options.LoggingChannel} }};");
            lines.Add($"category database {{ {options.LoggingChannel} }};");
            lines.Add($"category default {{ {options.LoggingChannel} }};");
            lines.Add($"category delegation-only {{ {options.LoggingChannel} }};");
            lines.Add($"category dispatch {{ {options.LoggingChannel} }};");
            lines.Add($"category dnssec {{ {options.LoggingChannel} }};");
            lines.Add($"category general {{ {options.LoggingChannel} }};");
            lines.Add($"category lame-servers {{ {options.LoggingChannel} }};");
            lines.Add($"category network {{ {options.LoggingChannel} }};");
            lines.Add($"category notify {{ {options.LoggingChannel} }};");
            lines.Add($"category queries {{ {options.LoggingChannel} }};");
            lines.Add($"category resolver {{ {options.LoggingChannel} }};");
            lines.Add($"category rpz {{ {options.LoggingChannel} }};");
            lines.Add($"category rate-limit {{ {options.LoggingChannel} }};");
            lines.Add($"category security {{ {options.LoggingChannel} }};");
            lines.Add($"category unmatched {{ {options.LoggingChannel} }};");
            lines.Add($"category update {{ {options.LoggingChannel} }};");
            lines.Add($"category update-security {{ {options.LoggingChannel} }};");
            lines.Add($"category xfer-in {{ {options.LoggingChannel} }};");
            lines.Add($"category xfer-out {{ {options.LoggingChannel} }};");
            lines.Add("};");
            lines.Add("");

            lines.Add("trusted-keys {");
            lines.Add(options.TrustedKeys);
            lines.Add("};");
            lines.Add("");

            var zones = _bindServerZoneRepository.GetAll().Select(_ => new BindServerZoneModel(_));
            foreach(var zone in zones) {
                lines.Add($"zone \"{zone.Name}\" {{");
                lines.Add($"type {zone.Type};");
                lines.Add($"file \"{zone.File}\";");
                if(!string.IsNullOrEmpty(zone.SerialUpdateMethod)) {
                    lines.Add($"serial-update-method {zone.SerialUpdateMethod};");
                }
                if(zone.AllowUpdate.Any()) {
                    lines.Add($"allow-update {{ {zone.AllowUpdate.JoinToString("; ")} }}");
                }
                if(zone.AllowQuery.Any()) {
                    lines.Add($"allow-query {{ {zone.AllowQuery.JoinToString("; ")} }}");
                }
                if(zone.AllowTransfer.Any()) {
                    lines.Add($"allow-transfer {{ {zone.AllowTransfer.JoinToString("; ")} }}");
                    lines.Add($"allow-transfer {zone.AllowTransfer};");
                }
                lines.Add("};");
            }
            lines.Add("");

            lines.Add("include \"/etc/bind/master/blackhole.zones\";");
            File.WriteAllLines(MainFilePath, lines);
        }

        public void Enable() {
            if(Systemctl.IsEnabled(ServiceName) == false) {
                Systemctl.Enable(ServiceName);
            }
        }

        public void Disable() {
            Systemctl.Disable(ServiceName);
        }

        public void Stop() {
            Systemctl.Stop(ServiceName);
        }

        public void Restart() {
            if(Systemctl.IsEnabled(ServiceName) == false) {
                Systemctl.Enable(ServiceName);
            }
            if(Systemctl.IsActive(ServiceName) == false) {
                Systemctl.Restart(ServiceName);
            }
        }

        public void RndcReconfig() {
            _bash.Execute("rndc reconfig");
        }

        public void RndcReload() {
            _bash.Execute("rndc reload");
        }
    }
}
