using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using antdlib.common;
using Newtonsoft.Json;
using IoDir = System.IO.Directory;

namespace Antd.Dhcpd {
    public class DhcpdConfiguration {

        private readonly DhcpdConfigurationModel _serviceModel;

        private readonly string _cfgFile = $"{Parameter.AntdCfgServices}/dhcp.conf";
        private readonly string _cfgFileBackup = $"{Parameter.AntdCfgServices}/dhcp.conf.bck";
        private const string ServiceName = "dhcpd4.service";
        private const string MainFilePath = "/etc/dhcp/dhcpd.conf";
        private const string MainFilePathBackup = "/etc/dhcp/.dhcpd.conf";

        public DhcpdConfiguration() {
            IoDir.CreateDirectory(Parameter.AntdCfgServices);
            if(!File.Exists(_cfgFile)) {
                _serviceModel = new DhcpdConfigurationModel();
            }
            else {
                try {
                    var text = File.ReadAllText(_cfgFile);
                    var obj = JsonConvert.DeserializeObject<DhcpdConfigurationModel>(text);
                    _serviceModel = obj;
                }
                catch(Exception) {
                    _serviceModel = new DhcpdConfigurationModel();
                }

            }
        }

        public void Save(DhcpdConfigurationModel model) {
            var text = JsonConvert.SerializeObject(model, Formatting.Indented);
            if(File.Exists(_cfgFile)) {
                File.Copy(_cfgFile, _cfgFileBackup, true);
            }
            File.WriteAllText(_cfgFile, text);
            ConsoleLogger.Log("[dhcpd] configuration saved");
        }

        public void Set() {
            Stop();
            #region [    dhcpd.conf generation    ]
            if(File.Exists(MainFilePath)) {
                if(File.Exists(MainFilePathBackup)) {
                    File.Delete(MainFilePathBackup);
                }
                File.Copy(MainFilePath, MainFilePathBackup);
            }
            var lines = new List<string> {
                "authoritative;"
            };
            var options = _serviceModel;
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

        public DhcpdConfigurationModel Get() {
            return _serviceModel;
        }

        public void Enable() {
            _serviceModel.IsActive = true;
            Save(_serviceModel);
            ConsoleLogger.Log("[dhcpd] enabled");
        }

        public void Disable() {
            _serviceModel.IsActive = false;
            Save(_serviceModel);
            ConsoleLogger.Log("[dhcpd] disabled");
        }

        public void Stop() {
            Systemctl.Stop(ServiceName);
            ConsoleLogger.Log("[dhcpd] stop");
        }

        public void Start() {
            if(Systemctl.IsEnabled(ServiceName) == false) {
                Systemctl.Enable(ServiceName);
            }
            if(Systemctl.IsActive(ServiceName) == false) {
                Systemctl.Restart(ServiceName);
            }
            ConsoleLogger.Log("[dhcpd] start");
        }

        public void AddClass(DhcpConfigurationClassModel model) {
            var cls = _serviceModel.Classes;
            if(cls.Any(_ => _.Name == model.Name)) {
                return;
            }
            cls.Add(model);
            _serviceModel.Classes = cls;
            Save(_serviceModel);
        }

        public void RemoveClass(string guid) {
            var cls = _serviceModel.Classes;
            var model = cls.First(_ => _.Guid == guid);
            if(model == null) {
                return;
            }
            cls.Remove(model);
            _serviceModel.Classes = cls;
            Save(_serviceModel);
        }

        public void AddPool(DhcpConfigurationPoolModel model) {
            var pool = _serviceModel.Pools;
            if(pool.Any(_ => _.Guid == model.Guid)) {
                return;
            }
            pool.Add(model);
            _serviceModel.Pools = pool;
            Save(_serviceModel);
        }

        public void RemovePool(string guid) {
            var pool = _serviceModel.Pools;
            var model = pool.First(_ => _.Guid == guid);
            if(model == null) {
                return;
            }
            pool.Remove(model);
            _serviceModel.Pools = pool;
            Save(_serviceModel);
        }

        public void AddReservation(DhcpConfigurationReservationModel model) {
            var hostres = _serviceModel.Reservations;
            if(hostres.Any(_ => _.Guid == model.Guid)) {
                return;
            }
            hostres.Add(model);
            _serviceModel.Reservations = hostres;
            Save(_serviceModel);
        }

        public void RemoveReservation(string guid) {
            var hostres = _serviceModel.Reservations;
            var model = hostres.First(_ => _.Guid == guid);
            if(model == null) {
                return;
            }
            hostres.Remove(model);
            _serviceModel.Reservations = hostres;
            Save(_serviceModel);
        }
    }
}
