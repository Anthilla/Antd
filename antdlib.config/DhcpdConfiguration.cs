using antdlib.common;
using antdlib.models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace antdlib.config {
    public class DhcpdConfiguration {

        private static DhcpdConfigurationModel ServiceModel => Load();

        private static readonly string CfgFile = $"{Parameter.AntdCfgServices}/dhcp.conf";
        private static readonly string CfgFileBackup = $"{Parameter.AntdCfgServices}/dhcp.conf.bck";
        private const string ServiceName = "dhcpd4.service";
        private const string MainFilePath = "/etc/dhcp/dhcpd.conf";

        private static DhcpdConfigurationModel Load() {
            if(!File.Exists(CfgFile)) {
                return new DhcpdConfigurationModel();
            }
            try {
                var text = File.ReadAllText(CfgFile);
                var obj = JsonConvert.DeserializeObject<DhcpdConfigurationModel>(text);
                return obj;
            }
            catch(Exception) {
                return new DhcpdConfigurationModel();
            }
        }

        public static void Save(DhcpdConfigurationModel model) {
            var text = JsonConvert.SerializeObject(model, Formatting.Indented);
            if(File.Exists(CfgFile)) {
                File.Copy(CfgFile, CfgFileBackup, true);
            }
            FileWithAcl.WriteAllText(CfgFile, text, "644", "root", "wheel");
            ConsoleLogger.Log("[dhcpd] configuration saved");
        }

        public static void Set() {
            Stop();
            #region [    dhcpd.conf generation    ]
            var lines = new List<string> {
                "authoritative;"
            };
            var options = ServiceModel;
            foreach(var allow in options.Allow) {
                lines.Add($"allow {allow};");
            }
            if(!string.IsNullOrEmpty(options.UpdateStaticLeases)) { lines.Add($"update-static-leases {options.UpdateStaticLeases}"); }
            if(!string.IsNullOrEmpty(options.UpdateConflictDetection)) { lines.Add($"update-conflict-detection {options.UpdateConflictDetection}"); }
            if(!string.IsNullOrEmpty(options.UseHostDeclNames)) { lines.Add($"use-host-decl-names {options.UseHostDeclNames}"); }
            if(!string.IsNullOrEmpty(options.DoForwardUpdates)) { lines.Add($"do-forward-updates {options.DoForwardUpdates}"); }
            if(!string.IsNullOrEmpty(options.DoReverseUpdates)) { lines.Add($"do-reverse-updates {options.DoReverseUpdates}"); }
            if(!string.IsNullOrEmpty(options.LogFacility)) { lines.Add($"log-facility {options.LogFacility}"); }
            foreach(var option in options.Option) {
                lines.Add($"option {option};");
            }
            if(!string.IsNullOrEmpty(options.ZoneName) && !string.IsNullOrEmpty(options.ZonePrimaryAddress)) { lines.Add($"zone {options.ZoneName} {{ primary {options.ZonePrimaryAddress}; }}"); }
            if(!string.IsNullOrEmpty(options.DdnsUpdateStyle)) { lines.Add($"ddns-update-style {options.DdnsUpdateStyle}"); }
            if(!string.IsNullOrEmpty(options.DdnsUpdates)) { lines.Add($"ddns-updates {options.DdnsUpdates}"); }
            if(!string.IsNullOrEmpty(options.DdnsDomainName)) { lines.Add($"ddns-domainname \"{options.DdnsDomainName}\""); }
            if(!string.IsNullOrEmpty(options.DdnsRevDomainName)) { lines.Add($"ddns-rev-domainname \"{options.DdnsRevDomainName}\""); }
            if(!string.IsNullOrEmpty(options.DefaultLeaseTime)) { lines.Add($"default-lease-time {options.DefaultLeaseTime}"); }
            if(!string.IsNullOrEmpty(options.MaxLeaseTime)) { lines.Add($"max-lease-time {options.MaxLeaseTime}"); }
            lines.Add("");
            if(!string.IsNullOrEmpty(options.KeyName) && !string.IsNullOrEmpty(options.KeySecret)) {
                lines.Add($"key \"{options.KeyName}\" {{");
                lines.Add("algorithm hmac-md5;");
                lines.Add($"secret \"{options.KeySecret}\";");
                lines.Add("};");
            }
            lines.Add("");
            var classes = options.Classes;
            foreach(var cls in classes) {
                lines.Add($"class \"{cls.Name}\" {{");
                lines.Add($"match if binary-to-ascii(16,8,\":\",substring(hardware, 1, 2)) = \"{cls.MacVendor}\";");
                lines.Add("}");
            }
            lines.Add("");
            lines.Add($"subnet {options.SubnetIpFamily} netmask {options.SubnetIpMask} {{");
            if(!string.IsNullOrEmpty(options.SubnetOptionRouters)) { lines.Add($"option routers {options.SubnetOptionRouters}"); }
            if(!string.IsNullOrEmpty(options.SubnetNtpServers)) { lines.Add($"option ntp-servers {options.SubnetNtpServers}"); }
            if(!string.IsNullOrEmpty(options.SubnetTimeServers)) { lines.Add($"option time-servers {options.SubnetTimeServers}"); }
            if(!string.IsNullOrEmpty(options.SubnetDomainNameServers)) { lines.Add($"option domain-name-servers {options.SubnetDomainNameServers}"); }
            if(!string.IsNullOrEmpty(options.SubnetBroadcastAddress)) { lines.Add($"option broadcast-address {options.SubnetBroadcastAddress}"); }
            if(!string.IsNullOrEmpty(options.SubnetMask)) { lines.Add($"option subnet-mask {options.SubnetMask}"); }
            if(!string.IsNullOrEmpty(options.ZoneName) && !string.IsNullOrEmpty(options.ZonePrimaryAddress)) { lines.Add($"zone {options.ZoneName} {{ primary {options.ZonePrimaryAddress}; }}"); }
            var pools = options.Pools;
            foreach(var pool in pools) {
                lines.Add("pool {");
                foreach(var opt in pool.Options) {
                    lines.Add(opt + (opt.EndsWith(";") ? "" : ";"));
                }
                lines.Add("}");
            }
            lines.Add("}");
            lines.Add("");
            var reservations = options.Reservations;
            foreach(var reservation in reservations) {
                lines.Add($"host {reservation.HostName} {{ hardware ethernet {reservation.MacAddress}; fixed-address {reservation.IpAddress}; }}");
            }
            FileWithAcl.WriteAllLines(MainFilePath, lines, "644", "root", "wheel");
            #endregion
            Start();
        }

        public static void DefaultSet() {

        }

        public static bool IsActive() {
            if(!File.Exists(CfgFile)) {
                return false;
            }
            return ServiceModel != null && ServiceModel.IsActive;
        }

        public static DhcpdConfigurationModel Get() {
            return ServiceModel;
        }

        public static void Enable() {
            ServiceModel.IsActive = true;
            Save(ServiceModel);
            ConsoleLogger.Log("[dhcpd] enabled");
        }

        public static void Disable() {
            ServiceModel.IsActive = false;
            Save(ServiceModel);
            ConsoleLogger.Log("[dhcpd] disabled");
        }

        public static void Stop() {
            Systemctl.Stop(ServiceName);
            ConsoleLogger.Log("[dhcpd] stop");
        }

        public static void Start() {
            if(Systemctl.IsEnabled(ServiceName) == false) {
                Systemctl.Enable(ServiceName);
            }
            if(Systemctl.IsActive(ServiceName) == false) {
                Systemctl.Restart(ServiceName);
            }
            ConsoleLogger.Log("[dhcpd] start");
        }

        public static void AddClass(DhcpConfigurationClassModel model) {
            var cls = ServiceModel.Classes;
            if(cls.Any(_ => _.Name == model.Name)) {
                return;
            }
            cls.Add(model);
            ServiceModel.Classes = cls;
            Save(ServiceModel);
        }

        public static void RemoveClass(string guid) {
            var cls = ServiceModel.Classes;
            var model = cls.First(_ => _.Guid == guid);
            if(model == null) {
                return;
            }
            cls.Remove(model);
            ServiceModel.Classes = cls;
            Save(ServiceModel);
        }

        public static void AddPool(DhcpConfigurationPoolModel model) {
            var pool = ServiceModel.Pools;
            if(pool.Any(_ => _.Guid == model.Guid)) {
                return;
            }
            pool.Add(model);
            ServiceModel.Pools = pool;
            Save(ServiceModel);
        }

        public static void RemovePool(string guid) {
            var pool = ServiceModel.Pools;
            var model = pool.First(_ => _.Guid == guid);
            if(model == null) {
                return;
            }
            pool.Remove(model);
            ServiceModel.Pools = pool;
            Save(ServiceModel);
        }

        public static void AddReservation(DhcpConfigurationReservationModel model) {
            var hostres = ServiceModel.Reservations;
            if(hostres.Any(_ => _.Guid == model.Guid)) {
                return;
            }
            hostres.Add(model);
            ServiceModel.Reservations = hostres;
            Save(ServiceModel);
        }

        public static void RemoveReservation(string guid) {
            var hostres = ServiceModel.Reservations;
            var model = hostres.First(_ => _.Guid == guid);
            if(model == null) {
                return;
            }
            hostres.Remove(model);
            ServiceModel.Reservations = hostres;
            Save(ServiceModel);
        }
    }
}
