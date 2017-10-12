using System.Collections.Generic;
using System.IO;
using anthilla.core;
using Antd.parsing;
using Antd.models;

namespace Antd.cmds {

    public class Dhcpd {

        private const string serviceName = "dhcpd4.service";
        private const string dhcpdFile = "/etc/dhcp/dhcpd.conf";

        public static void Parse() {
            if(!File.Exists(dhcpdFile)) {
                return;
            }
            var content = File.ReadAllText(dhcpdFile);
            var model = new DhcpdModel() { Active = false };
            var allow = DhcpdParser.ParseAllow(content);
            model.Allow = allow;
            model = DhcpdParser.ParseParameters(model, content);
            model = DhcpdParser.ParseKeySecret(model, content);
            var reservations = DhcpdParser.ParseReservation(content);
            model.Reservations = reservations;
            var classes = DhcpdParser.ParseClass(content);
            model.Classes = classes;
            var subnets = DhcpdParser.ParseSubnet(content);
            model.Subnets = subnets;
            Application.CurrentConfiguration.Services.Dhcpd = model;
            ConsoleLogger.Log("[dhcpd] import existing configuration");
        }

        public static void Apply() {
            var options = Application.CurrentConfiguration.Services.Dhcpd;
            if(options == null) {
                return;
            }
            Stop();
            #region [    dhcpd.conf generation    ]
            var lines = new List<string>();
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
            File.WriteAllLines(dhcpdFile, lines);
            #endregion
            Start();
        }

        public static void Stop() {
            Systemctl.Stop(serviceName);
            ConsoleLogger.Log("[dhcpd] stop");
        }

        public static void Start() {
            if(Systemctl.IsEnabled(serviceName) == false) {
                Systemctl.Enable(serviceName);
            }
            if(Systemctl.IsActive(serviceName) == false) {
                Systemctl.Restart(serviceName);
            }
            ConsoleLogger.Log("[dhcpd] start");
        }
    }
}