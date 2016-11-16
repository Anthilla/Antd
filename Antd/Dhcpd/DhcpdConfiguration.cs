using System.Collections.Generic;
using System.IO;
using System.Linq;
using antdlib.Systemd;
using antdlib.views;
using Antd.Database;
using IoDir = System.IO.Directory;

namespace Antd.Dhcpd {
    public class DhcpdConfiguration {

        private const string Directory = "/etc/dhcp";
        private const string ServiceName = "dhcpd4.service";
        private const string MainFilePath = "/etc/dhcp/dhcpd.conf";
        private const string MainFilePathBackup = "/etc/dhcp/.dhcpd.conf";
        private readonly DhcpServerOptionsRepository _dhcpServerOptionsRepository = new DhcpServerOptionsRepository();
        private readonly DhcpServerSubnetRepository _dhcpServerSubnetRepository = new DhcpServerSubnetRepository();
        private readonly DhcpServerClassRepository _dhcpServerClassRepository = new DhcpServerClassRepository();
        private readonly DhcpServerPoolRepository _dhcpServerPoolRepository = new DhcpServerPoolRepository();
        private readonly DhcpServerReservationRepository _dhcpServerReservationRepository = new DhcpServerReservationRepository();

        public void Set() {
            if(!IoDir.Exists(Directory)) {
                IoDir.CreateDirectory(Directory);
            }
            Enable();
            Stop();

            #region [    dhcpd.conf generation    ]
            var o = _dhcpServerOptionsRepository.Get();
            var s = _dhcpServerSubnetRepository.Get();
            if(o == null || s == null) {
                return;
            }
            if(File.Exists(MainFilePath)) {
                if(File.Exists(MainFilePathBackup)) {
                    File.Delete(MainFilePathBackup);
                }
                File.Copy(MainFilePath, MainFilePathBackup);
            }
            var lines = new List<string> {
                "authoritative;"
            };
            var options = new DhcpServerOptionsModel(o);
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
            var classes = _dhcpServerClassRepository.GetAll();
            foreach(var cls in classes) {
                lines.Add($"class \"{cls.Name}\" {{");
                lines.Add($"match if binary-to-ascii(16,8,\":\",substring(hardware, 1, 2)) = \"{cls.MacVendor}\";");
                lines.Add("}");
            }
            lines.Add("");
            var subnet = new DhcpServerSubnetModel(s);
            lines.Add($"subnet {subnet.IpFamily} netmask {subnet.IpMask} {{");
            if(!string.IsNullOrEmpty(subnet.OptionRouters)) { lines.Add($"option routers {subnet.OptionRouters}"); }
            if(!string.IsNullOrEmpty(subnet.NtpServers)) { lines.Add($"option ntp-servers {subnet.NtpServers}"); }
            if(!string.IsNullOrEmpty(subnet.TimeServers)) { lines.Add($"option time-servers {subnet.TimeServers}"); }
            if(!string.IsNullOrEmpty(subnet.DomainNameServers)) { lines.Add($"option domain-name-servers {subnet.DomainNameServers}"); }
            if(!string.IsNullOrEmpty(subnet.BroadcastAddress)) { lines.Add($"option broadcast-address {subnet.BroadcastAddress}"); }
            if(!string.IsNullOrEmpty(subnet.SubnetMask)) { lines.Add($"option subnet-mask {subnet.SubnetMask}"); }
            if(!string.IsNullOrEmpty(subnet.ZoneName) && !string.IsNullOrEmpty(subnet.ZonePrimaryAddress)) { lines.Add($"zone {subnet.ZoneName} {{ primary {subnet.ZonePrimaryAddress}; }}"); }
            var pools = _dhcpServerPoolRepository.GetAll().Select(_ => new DhcpServerPoolModel(_)).ToList();
            foreach(var pool in pools) {
                lines.Add("pool {");
                foreach(var opt in pool.Options) {
                    lines.Add(opt + (opt.EndsWith(";") ? "" : ";"));
                }
                lines.Add("}");
            }
            lines.Add("}");
            lines.Add("");
            var reservations = _dhcpServerReservationRepository.GetAll().Select(_ => new DhcpServerReservationModel(_)).ToList();
            foreach(var reservation in reservations) {
                lines.Add($"host {reservation.HostName} {{ hardware ethernet {reservation.MacAddress}; fixed-address {reservation.IpAddress}; }}");
            }
            File.WriteAllLines(MainFilePath, lines);
            #endregion

            Restart();
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
    }
}
