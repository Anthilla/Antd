using antdlib.common;
using antdlib.models;
using anthilla.commands;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;

namespace antdlib.config {
    public static class HostConfiguration {

        public static string FilePath => $"{Parameter.AntdCfg}/host.conf";
        public static string FilePathBackup => $"{Parameter.AntdCfg}/host.conf.bck";
        public static HostModel Host => LoadHostModel();

        private static HostModel LoadHostModel() {
            if(!File.Exists(FilePath)) {
                return new HostModel();
            }
            try {
                return JsonConvert.DeserializeObject<HostModel>(File.ReadAllText(FilePath));
            }
            catch(Exception) {
                return new HostModel();
            }
        }

        public static void Export(HostModel model) {
            if(File.Exists(FilePath)) {
                File.Copy(FilePath, $"{FilePath}.bck", true);
            }
            File.WriteAllText(FilePath, JsonConvert.SerializeObject(model, Formatting.Indented));
        }

        #region [    repo - Confirm Configuration    ]
        private static readonly string _isConfiguredPath = $"{Parameter.AntdCfg}/configured";

        public static bool IsHostConfiguredByUser() {
            if(!File.Exists(_isConfiguredPath)) {
                return false;
            }
            var text = File.ReadAllBytes(_isConfiguredPath);
            return text.Length > 0;
        }

        public static void SetHostAsConfigured() {
            if(File.Exists(_isConfiguredPath)) {
                return;
            }
            File.WriteAllBytes(_isConfiguredPath, new[] { (byte)0 });
        }
        #endregion

        #region [    repo - Host Info    ]
        public static HostInfoModel GetHostInfo() {
            var host = new HostInfoModel {
                Name = Host.HostName.StoredValues["$host_name"],
                Chassis = Host.HostName.StoredValues["$host_chassis"],
                Deployment = Host.HostName.StoredValues["$host_deployment"],
                Location = Host.HostName.StoredValues["$host_location"]
            };
            return host;
        }

        public static void SetHostInfoName(string name) {
            Host.HostName.StoredValues["$host_name"] = name;
            Export(Host);
        }

        public static void SetHostInfoChassis(string chassis) {
            Host.HostChassis.StoredValues["$host_chassis"] = chassis;
            Export(Host);
        }

        public static void SetHostInfoDeployment(string deployment) {
            Host.HostDeployment.StoredValues["$host_deployment"] = deployment;
            Export(Host);
        }

        public static void SetHostInfoLocation(string location) {
            Host.HostLocation.StoredValues["$host_location"] = location;
            Export(Host);
        }

        public static void SetHostInfo(string name, string chassis, string deployment, string location) {
            Host.HostName.StoredValues["$host_name"] = name;
            Host.HostChassis.StoredValues["$host_chassis"] = chassis;
            Host.HostDeployment.StoredValues["$host_deployment"] = deployment;
            Host.HostLocation.StoredValues["$host_location"] = location;
            Export(Host);
        }

        public static void ApplyHostInfo() {
            CommandLauncher.Launch(Host.HostName.SetCmd, Host.HostName.StoredValues);
            CommandLauncher.Launch(Host.HostChassis.SetCmd, Host.HostChassis.StoredValues);
            CommandLauncher.Launch(Host.HostDeployment.SetCmd, Host.HostDeployment.StoredValues);
            CommandLauncher.Launch(Host.HostLocation.SetCmd, Host.HostLocation.StoredValues);
            var name = Host.HostName.StoredValues["$host_name"];
            File.WriteAllText("/etc/hostname", name);
        }
        #endregion

        #region [    repo - Timezone    ]
        public static string GetTimezone() {
            var timezone = Host.Timezone.StoredValues["$host_timezone"];
            return timezone;
        }

        public static void SetTimezone(string timezone) {
            Host.Timezone.StoredValues["$host_timezone"] = timezone;
            Export(Host);
        }

        public static void ApplyTimezone() {
            CommandLauncher.Launch(Host.Timezone.SetCmd, Host.Timezone.StoredValues);
        }
        #endregion

        #region [    repo - Ntpdate    ]
        public static void SetNtpdate(string ntpdate) {
            Host.NtpdateServer.StoredValues["$server"] = ntpdate;
            Export(Host);
        }

        public static void ApplyNtpdate() {
            CommandLauncher.Launch(Host.NtpdateServer.SetCmd, Host.NtpdateServer.StoredValues);
        }
        #endregion

        #region [    sync time    ]
        public static void SyncClock(string ntpServer = "") {
            ApplyNtpdate();
            CommandLauncher.Launch("sync-clock");
        }
        #endregion

        #region [    repo - Name Service - Hosts    ]
        public static string[] GetNsHosts() {
            var hosts = CommandLauncher.Launch(Host.NsHosts.GetCmd).ToArray();
            return hosts;
        }

        public static void SetNsHosts(string[] hosts) {
            Host.NsHostsContent = hosts;
            Export(Host);
        }

        public static void ApplyNsHosts() {
            var existing = File.ReadAllLines("/etc/hosts");
            var configured = Host.NsHostsContent;
            var merge = existing.Union(configured).ToArray();
            Host.NsHostsContent = merge;
            File.WriteAllLines("/etc/hosts", Host.NsHostsContent);
            Export(Host);
        }
        #endregion

        #region [    repo - Name Service - Networks    ]
        public static string[] GetNsNetworks() {
            var networks = CommandLauncher.Launch(Host.NsNetworks.GetCmd).ToArray();
            return networks;
        }

        public static void SetNsNetworks(string[] networks) {
            Host.NsNetworks.StoredValues["$value"] = networks.JoinToString("\n");
            Host.NsNetworksContent = networks;
            Export(Host);
        }

        public static void ApplyNsNetworks() {
            var existing = File.ReadAllLines("/etc/networks");
            var configured = Host.NsNetworksContent;
            var merge = existing.Union(configured).ToArray();
            Host.NsNetworksContent = merge;
            File.WriteAllLines("/etc/networks", Host.NsNetworksContent);
            Export(Host);
        }
        #endregion

        #region [    repo - Name Service - Resolv    ]
        public static string[] GetNsResolv() {
            var resolv = CommandLauncher.Launch(Host.NsResolv.GetCmd).ToArray();
            return resolv;
        }

        public static void SetNsResolv(string[] resolv) {
            Host.NsResolvContent = resolv;
            Host.NsResolv.StoredValues["$value"] = resolv.JoinToString("\n");
            Export(Host);
        }

        public static void ApplyNsResolv() {
            if(!File.Exists("/etc/resolv.conf")) {
                File.WriteAllText("/etc/resolv.conf", "");
            }
            try {
                var existing = File.ReadAllLines("/etc/resolv.conf");
                var configured = Host.NsResolvContent;
                var merge = existing.Union(configured).ToArray();
                Host.NsResolvContent = merge;
                File.WriteAllLines("/etc/resolv.conf", Host.NsResolvContent);
                Export(Host);
            }
            catch(Exception) {
                return;
            }
        }
        #endregion

        #region [    repo - Name Service - Switch    ]
        public static string[] GetNsSwitch() {
            var @switch = CommandLauncher.Launch(Host.NsSwitch.GetCmd).ToArray();
            return @switch;
        }

        public static void SetNsSwitch(string[] @switch) {
            Host.NsSwitchContent = @switch;
            Host.NsSwitch.StoredValues["$value"] = @switch.JoinToString("\n");
            Export(Host);
        }

        public static void ApplyNsSwitch() {
            var existing = File.ReadAllLines("/etc/nsswitch.conf");
            var configured = Host.NsSwitchContent;
            var merge = existing.Union(configured).ToArray();
            Host.NsSwitchContent = merge;
            File.WriteAllLines("/etc/nsswitch.conf", Host.NsSwitchContent);
            Export(Host);
        }
        #endregion

        #region [    repo - Domain - Internal    ]
        public static string GetInternalDomain() {
            var domain = Host.InternalDomain;
            return domain;
        }

        public static void SetInternalDomain(string domain) {
            Host.InternalDomain = domain;
            Export(Host);
        }

        public static void ApplyInternalDomain() {
            throw new NotImplementedException("Edit etc files changing the internal domain value.");
        }
        #endregion

        #region [    repo - Domain - Extenal    ]
        public static string GetExtenalDomain() {
            var domain = Host.ExternalDomain;
            return domain;
        }

        public static void SetExtenalDomain(string domain) {
            Host.ExternalDomain = domain;
            Export(Host);
        }

        public static void ApplyExtenalDomain() {
            throw new NotImplementedException("Edit etc files changing the external domain value.");
        }
        #endregion
    }
}
