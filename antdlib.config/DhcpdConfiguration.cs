using antdlib.common;
using antdlib.models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using antdlib.config.Parsers;
using anthilla.core;
using Parameter = antdlib.common.Parameter;

namespace antdlib.config {
    public class DhcpdConfiguration {

        private static DhcpdConfigurationModel ServiceModel => Load();

        private static readonly string CfgFile = $"{Parameter.AntdCfgServices}/dhcpd.conf";
        private static readonly string CfgFileBackup = $"{Parameter.AntdCfgServices}/dhcpd.conf.bck";
        private const string ServiceName = "dhcpd4.service";
        private const string MainFilePath = "/etc/dhcp/dhcpd.conf";

        public static void TryImport() {
            if(File.Exists(CfgFile)) {
                return;
            }
            if(!File.Exists(MainFilePath)) {
                return;
            }
            var text = File.ReadAllText(MainFilePath);
            var model = Parse(text);
            Save(model);
            ConsoleLogger.Log("[dhcpd] import existing configuration");
        }

        private static DhcpdConfigurationModel Parse(string text) {
            var model = new DhcpdConfigurationModel { IsActive = false };
            var allow = DhcpdParser.ParseAllow(text);
            model.Allow = allow;
            model = DhcpdParser.ParseParameters(model, text);
            model = DhcpdParser.ParseKeySecret(model, text);
            var reservations = DhcpdParser.ParseReservation(text);
            model.Reservations = reservations;
            var classes = DhcpdParser.ParseClass(text);
            model.Classes = classes;
            var subnets = DhcpdParser.ParseSubnet(text);
            model.Subnets = subnets;
            return model;
        }

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
            var lines = new List<string>();
            var options = ServiceModel;

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
                lines.Add($"match if binary-to-ascii(16,8,\":\",substring(hardware, 1, 2)) = \"{cls.VendorMacAddress}\";");
                lines.Add("}");
            }
            lines.Add("");
            lines.Add("authoritative;");

            foreach(var allow in options.Allow) {
                lines.Add($"allow {allow};");
            }
            if(!string.IsNullOrEmpty(options.UpdateStaticLeases)) { lines.Add($"update-static-leases {options.UpdateStaticLeases};"); }
            if(!string.IsNullOrEmpty(options.UpdateConflictDetection)) { lines.Add($"update-conflict-detection {options.UpdateConflictDetection};"); }
            if(!string.IsNullOrEmpty(options.UseHostDeclNames)) { lines.Add($"use-host-decl-names {options.UseHostDeclNames};"); }
            if(!string.IsNullOrEmpty(options.DoForwardUpdates)) { lines.Add($"do-forward-updates {options.DoForwardUpdates};"); }
            if(!string.IsNullOrEmpty(options.DoReverseUpdates)) { lines.Add($"do-reverse-updates {options.DoReverseUpdates};"); }
            if(!string.IsNullOrEmpty(options.LogFacility)) { lines.Add($"log-facility {options.LogFacility};"); }

            if(!string.IsNullOrEmpty(options.OptionRouters)) { lines.Add($"option routers {options.OptionRouters};"); }
            if(!string.IsNullOrEmpty(options.OptionLocalProxy)) { lines.Add($"option local-proxy-config code {options.OptionLocalProxy};"); }
            if(!string.IsNullOrEmpty(options.OptionDomainName)) { lines.Add($"option domain-name \"{options.OptionDomainName}\";"); }

            if(!string.IsNullOrEmpty(options.ZoneName) && !string.IsNullOrEmpty(options.ZonePrimaryAddress)) { lines.Add($"zone {options.ZoneName} {{ primary {options.ZonePrimaryAddress}; }}"); }
            if(!string.IsNullOrEmpty(options.DdnsUpdateStyle)) { lines.Add($"ddns-update-style {options.DdnsUpdateStyle};"); }
            if(!string.IsNullOrEmpty(options.DdnsUpdates)) { lines.Add($"ddns-updates {options.DdnsUpdates};"); }
            if(!string.IsNullOrEmpty(options.DdnsDomainName)) { lines.Add($"ddns-domainname \"{options.DdnsDomainName}\";"); }
            if(!string.IsNullOrEmpty(options.DdnsRevDomainName)) { lines.Add($"ddns-rev-domainname \"{options.DdnsRevDomainName}\";"); }
            if(!string.IsNullOrEmpty(options.DefaultLeaseTime)) { lines.Add($"default-lease-time {options.DefaultLeaseTime};"); }
            if(!string.IsNullOrEmpty(options.MaxLeaseTime)) { lines.Add($"max-lease-time {options.MaxLeaseTime};"); }
            lines.Add("");

            foreach(var subnet in options.Subnets) {
                lines.Add($"subnet {subnet.SubnetIpFamily} netmask {subnet.SubnetIpMask} {{");
                if(!string.IsNullOrEmpty(subnet.SubnetOptionRouters)) { lines.Add($"option routers {subnet.SubnetOptionRouters}"); }
                if(!string.IsNullOrEmpty(subnet.SubnetNtpServers)) { lines.Add($"option ntp-servers {subnet.SubnetNtpServers}"); }
                if(!string.IsNullOrEmpty(subnet.SubnetTimeServers)) { lines.Add($"option time-servers {subnet.SubnetTimeServers}"); }
                if(!string.IsNullOrEmpty(subnet.SubnetDomainNameServers)) { lines.Add($"option domain-name-servers {subnet.SubnetDomainNameServers}"); }
                if(!string.IsNullOrEmpty(subnet.SubnetBroadcastAddress)) { lines.Add($"option broadcast-address {subnet.SubnetBroadcastAddress}"); }
                if(!string.IsNullOrEmpty(subnet.SubnetMask)) { lines.Add($"option subnet-mask {subnet.SubnetMask}"); }
                if(!string.IsNullOrEmpty(subnet.ZoneName) && !string.IsNullOrEmpty(subnet.ZonePrimaryAddress)) { lines.Add($"zone {subnet.ZoneName} {{ primary {subnet.ZonePrimaryAddress}; }}"); }
                lines.Add("");

                var pools = subnet.Pools;
                foreach(var pool in pools) {
                    lines.Add("pool {");
                    if(!string.IsNullOrEmpty(pool.ClassName)) { lines.Add($"allow members of \"{pool.ClassName}\";"); }
                    if(!string.IsNullOrEmpty(pool.PoolRangeEnd) && !string.IsNullOrEmpty(pool.PoolRangeStart)) { lines.Add($"range {pool.PoolRangeStart} {pool.PoolRangeEnd};"); }
                    lines.Add("}");
                }
                lines.Add("");

                lines.Add("pool {");
                if(!string.IsNullOrEmpty(subnet.PoolDynamicRangeEnd) && !string.IsNullOrEmpty(subnet.PoolDynamicRangeStart)) { lines.Add($"range dynamic-bootp {subnet.PoolDynamicRangeStart} {subnet.PoolDynamicRangeEnd};"); }
                lines.Add("}");
                lines.Add("}");
            }

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

        public static void AddClass(DhcpdClass model) {
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

        //public static void AddPool(DhcpdPool model) {
        //    var pool = ServiceModel.Pools;
        //    if(pool.Any(_ => _.Guid == model.Guid)) {
        //        return;
        //    }
        //    pool.Add(model);
        //    ServiceModel.Pools = pool;
        //    Save(ServiceModel);
        //}

        //public static void RemovePool(string guid) {
        //    var pool = ServiceModel.Pools;
        //    var model = pool.First(_ => _.Guid == guid);
        //    if(model == null) {
        //        return;
        //    }
        //    pool.Remove(model);
        //    ServiceModel.Pools = pool;
        //    Save(ServiceModel);
        //}

        public static void AddReservation(DhcpdReservation model) {
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
