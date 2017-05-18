using antdlib.common;
using antdlib.models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace antdlib.config {
    public class BindConfiguration {

        private static BindConfigurationModel ServiceModel => Load();

        private static readonly string CfgFile = $"{Parameter.AntdCfgServices}/bind.conf";
        private static readonly string CfgFileBackup = $"{Parameter.AntdCfgServices}/bind.conf.bck";
        private const string ServiceName = "named.service";
        private const string MainFilePath = "/etc/bind/named.conf";
        private const string MainFilePathBackup = "/etc/bind/.named.conf";

        private static BindConfigurationModel Load() {
            if(!File.Exists(CfgFile)) {
                return new BindConfigurationModel();
            }
            try {
                var text = File.ReadAllText(CfgFile);
                var obj = JsonConvert.DeserializeObject<BindConfigurationModel>(text);
                return obj;
            }
            catch(Exception) {
                return new BindConfigurationModel();
            }
        }

        public static void Save(BindConfigurationModel model) {
            var text = JsonConvert.SerializeObject(model, Formatting.Indented);
            if(File.Exists(CfgFile)) {
                File.Copy(CfgFile, CfgFileBackup, true);
            }
            FileWithAcl.WriteAllText(CfgFile, text, "644", "root", "wheel");
            ConsoleLogger.Log("[bind] configuration saved");
        }

        public static void Set() {
            Stop();
            #region [    named.conf generation    ]
            if(File.Exists(MainFilePath)) {
                if(File.Exists(MainFilePathBackup)) {
                    File.Delete(MainFilePathBackup);
                }
                File.Copy(MainFilePath, MainFilePathBackup);
            }
            var lines = new List<string> {
                "options {"
            };
            var options = ServiceModel;
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

            var zones = options.Zones;
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
            FileWithAcl.WriteAllLines(MainFilePath, lines, "644", "root", "wheel");
            #endregion
            Start();
            RndcReconfig();
        }

        public static bool IsActive() {
            if(!File.Exists(CfgFile)) {
                return false;
            }
            return ServiceModel != null && ServiceModel.IsActive;
        }

        public static BindConfigurationModel Get() {
            return ServiceModel;
        }

        public static void Enable() {
            var mo = ServiceModel;
            mo.IsActive = true;
            Save(mo);
            ConsoleLogger.Log("[bind] enabled");
        }

        public static void Disable() {
            var mo = ServiceModel;
            mo.IsActive = false;
            Save(mo);
            ConsoleLogger.Log("[bind] disabled");
        }

        public static void Stop() {
            Systemctl.Stop(ServiceName);
            ConsoleLogger.Log("[bind] stop");
        }

        public static void Start() {
            if(Systemctl.IsEnabled(ServiceName) == false) {
                Systemctl.Enable(ServiceName);
            }
            if(Systemctl.IsActive(ServiceName) == false) {
                Systemctl.Restart(ServiceName);
            }
            ConsoleLogger.Log("[bind] start");
        }

        public static void RndcReconfig() {
            Bash.Execute("rndc reconfig");
        }

        public static void RndcReload() {
            Bash.Execute("rndc reload");
        }

        public static void AddZone(BindConfigurationZoneModel model) {
            var zones = ServiceModel.Zones;
            if(zones.Any(_ => _.Name == model.Name)) {
                return;
            }
            zones.Add(model);
            ServiceModel.Zones = zones;
            Save(ServiceModel);
        }

        public static void RemoveZone(string guid) {
            var zones = ServiceModel.Zones;
            var model = zones.First(_ => _.Guid == guid);
            if(model == null) {
                return;
            }
            zones.Remove(model);
            ServiceModel.Zones = zones;
            Save(ServiceModel);
        }

        public static List<string> GetHostZone(string hostname, string domain, string ip) {
            var list = new List<string> {"$ORIGIN .",
                "$TTL 3600	; 1 hour",
                $"{domain}			IN SOA	{hostname}.{domain}. hostmaster.{domain}. (",
                "				1000	   ; serial",
                "				900        ; refresh (15 minutes)",
                "				600        ; retry (10 minutes)",
                "				86400      ; expire (1 day)",
                "				3600       ; minimum (1 hour)",
                "				)",
                $"			NS	{hostname}.{domain}.",
                "$TTL 600	; 10 minutes",
                $"			A	{ip}",
                $"$ORIGIN _tcp.DefaultSite._sites.{domain}.",
                $"_gc			SRV	0 100 3268 {hostname}.{domain}.",
                $"_kerberos		SRV	0 100 88 {hostname}.{domain}.",
                $"_ldap			SRV	0 100 389 {hostname}.{domain}.",
                $"$ORIGIN _tcp.{domain}.",
                $"_gc			SRV	0 100 3268 {hostname}.{domain}.",
                $"_kerberos		SRV	0 100 88 {hostname}.{domain}.",
                $"_kpasswd		SRV	0 100 464 {hostname}.{domain}.",
                $"_ldap			SRV	0 100 389 {hostname}.{domain}.",
                $"$ORIGIN _udp.{domain}.",
                $"_kerberos		SRV	0 100 88 {hostname}.{domain}.",
                $"_kpasswd		SRV	0 100 464 {hostname}.{domain}.",
                $"$ORIGIN {domain}.",
                "$TTL 600	; 10 minutes",
                $"domaindnszones		A	{ip}",
                $"$ORIGIN domaindnszones.{domain}.",
                $"_ldap._tcp.DefaultSite._sites	SRV	0 100 389 {hostname}.{domain}.",
                $"_ldap._tcp		SRV	0 100 389 {hostname}.{domain}.",
                $"$ORIGIN {domain}.",
                "$TTL 1200	; 20 minutes",
                $"forestdnszones		A	{ip}",
                $"$ORIGIN forestdnszones.{domain}.",
                $"_ldap._tcp.DefaultSite._sites	SRV	0 100 389 {hostname}.{domain}.",
                $"_ldap._tcp		SRV	0 100 389 {hostname}.{domain}.",
                $"$ORIGIN _tcp.DefaultSite._sites.dc._msdcs.{domain}.",
                "$TTL 3600       ; 1 hour",
                $"_kerberos		SRV	0 100 88 {hostname}.{domain}.",
                $"_ldap			SRV	0 100 389 {hostname}.{domain}.",
                $"$ORIGIN _tcp.dc._msdcs.{domain}.",
                "$TTL 3600       ; 1 hour",
                $"_kerberos		SRV	0 100 88 {hostname}.{domain}.",
                $"_ldap			SRV	0 100 389 {hostname}.{domain}.",
                $"$ORIGIN _msdcs.{domain}.",
                "$TTL 3600       ; 1 hour",
                $"_ldap._tcp		SRV 0 100 389 {hostname}.{domain}.",
                $"$ORIGIN gc._msdcs.{domain}.",
                "$TTL 3600       ; 1 hour",
                $"_ldap._tcp.DefaultSite._sites SRV	0 100 3268 {hostname}.{domain}.",
                $"_ldap._tcp		SRV	0 100 3268 {hostname}.{domain}.",
                $"$ORIGIN _msdcs.{domain}.",
                "$TTL 3600       ; 1 hour",
                $"_ldap._tcp.pdc		SRV	0 100 389 {hostname}.{domain}.",
                $"$ORIGIN {domain}.",};
            return list;
        }

        public static List<string> GetReverseZone(string hostname, string domain, string arpaNet, string arpaIp) {
            var list = new List<string> {
                "$ORIGIN .",
                "$TTL 3600	; 1 hour",
                $"{arpaNet}.in-addr.arpa   IN SOA	{hostname}.{domain}. hostmaster.{domain}. (",
                "				1111	   ; serial",
                "				900        ; refresh (15 minutes)",
                "				600        ; retry (10 minutes)",
                "				86400      ; expire (1 day)",
                "				3600       ; minimum (1 hour)",
                "				900        ; refresh (15 minutes)",
                "				)",
                $"			NS  {hostname}.{domain}.",
                $"$ORIGIN {arpaNet}.in-addr.arpa.",
                $"{arpaIp} PTR	{hostname}.{domain}."
            };

            return list;
        }
    }
}
