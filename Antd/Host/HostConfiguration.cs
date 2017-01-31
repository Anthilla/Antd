using System;
using System.Linq;
using System.IO;
using antdlib.common;
using Newtonsoft.Json;
using System.Collections.Generic;
using antd.commands;
using antdlib.models;

namespace Antd.Host {
    public class HostConfiguration {

        public string FilePath { get; }
        public string FilePathBackup { get; }
        public HostModel Host { get; private set; }

        public HostConfiguration() {
            FilePath = $"{Parameter.AntdCfg}/host.conf";
            FilePathBackup = $"{Parameter.AntdCfg}/host.conf.bck";
            Host = LoadHostModel();
        }

        private HostModel LoadHostModel() {
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

        public void Export(HostModel model) {
            if(File.Exists(FilePath)) {
                File.Copy(FilePath, FilePathBackup, true);
            }
            File.WriteAllText(FilePath, JsonConvert.SerializeObject(model, Formatting.Indented));
        }

        #region [    repo - Modules    ]

        public string[] GetHostModprobes() {
            Host = LoadHostModel();
            var result = new List<string>();
            foreach(Dictionary<string, string> dict in Host.Modprobes.Select(_ => _.StoredValues)) {
                result.AddRange(dict.Select(_ => _.Value));
            }
            return result.ToArray();
        }

        public void SetHostModprobes(IEnumerable<string> modules) {
            Host = LoadHostModel();
            Host.Modprobes =
                modules.Select(
                    _ =>
                        new HostParameter {
                            SetCmd = "modprobe",
                            StoredValues = new Dictionary<string, string> { { "$package", _ } }
                        }).ToArray();
            Export(Host);
        }

        public void ApplyHostModprobes() {
            Host = LoadHostModel();
            var launcher = new CommandLauncher();
            foreach(var modprobe in Host.Modprobes) {
                launcher.Launch(modprobe.SetCmd, modprobe.StoredValues);
            }
        }

        public string[] GetHostRemoveModules() {
            Host = LoadHostModel();
            return Host.RemoveModules.StoredValues.Select(_ => _.Value).First().SplitToList(" ").ToArray();
        }

        public void SetHostRemoveModules(IEnumerable<string> modules) {
            Host = LoadHostModel();
            Host.RemoveModules = new HostParameter {
                SetCmd = "modprobe",
                StoredValues = new Dictionary<string, string> { { "$package", string.Join(" ", modules) } }
            };
            Export(Host);
        }

        public void ApplyHostRemoveModules() {
            Host = LoadHostModel();
            var launcher = new CommandLauncher();
            launcher.Launch(Host.RemoveModules.SetCmd, Host.RemoveModules.StoredValues);
        }

        public string[] GetHostBlacklistModules() {
            Host = LoadHostModel();
            return Host.ModulesBlacklist.ToArray();
        }

        public void SetHostBlacklistModules(IEnumerable<string> modules) {
            Host = LoadHostModel();
            Host.ModulesBlacklist = modules.ToArray();
            Export(Host);
        }

        public void ApplyHostBlacklistModules() {
            Host = LoadHostModel();
            var existing = File.ReadAllLines("/etc/modprobe.d/blacklist.conf").Select(_ => _.Replace("blacklist", "").Trim());
            var configured = Host.ModulesBlacklist;
            var merge = existing.Union(configured).ToArray();
            Host.ModulesBlacklist = merge;
            File.WriteAllLines("/etc/modprobe.d/blacklist.conf", Host.ModulesBlacklist.Select(_ => $"blacklist {_}"));
            Export(Host);
        }

        #endregion

        #region [    repo - Services    ]

        public string[] GetHostServices() {
            Host = LoadHostModel();
            var result = new List<string>();
            foreach(Dictionary<string, string> dict in Host.Services.Select(_ => _.StoredValues)) {
                result.AddRange(dict.Select(_ => _.Value));
            }
            return result.ToArray();
        }

        public void SetHostServices(IEnumerable<string> services) {
            Host = LoadHostModel();
            Host.Services =
                services.Select(
                    _ =>
                        new HostParameter {
                            SetCmd = "systemctl-restart",
                            StoredValues = new Dictionary<string, string> { { "$service", _ } }
                        }).ToArray();
            Export(Host);
        }

        public void ApplyHostServices() {
            Host = LoadHostModel();
            var launcher = new CommandLauncher();
            foreach(var srvc in Host.Services) {
                launcher.Launch(srvc.SetCmd, srvc.StoredValues);
            }
        }

        #endregion

        #region [    repo - OS Parameters    ]

        public Dictionary<string, string> GetHostOsParameters() {
            Host = LoadHostModel();
            var ks =
                Host.OsParameters.Select(_ => _.StoredValues.Where(__ => __.Key == "$file").Select(__ => __.Value))
                    .SelectMany(x => x)
                    .ToArray();
            var vs =
                Host.OsParameters.Select(_ => _.StoredValues.Where(__ => __.Key == "$value").Select(__ => __.Value))
                    .SelectMany(x => x)
                    .ToArray();
            var dict = new Dictionary<string, string>();
            if(ks.Length != vs.Length)
                return dict;
            for(var i = 0; i < ks.Length; i++) {
                if(!dict.ContainsKey(ks[i])) {
                    dict.Add(ks[i], vs[i]);
                }
            }
            return dict;
        }

        public void SetHostOsParameters(Dictionary<string, string> parameters) {
            Host = LoadHostModel();
            Host.OsParameters = parameters.Select(_ => new HostParameter {
                SetCmd = "echo-write",
                StoredValues = new Dictionary<string, string> {
                    {"$file", _.Key},
                    {"$value", _.Value.Replace("\r", "")}
                }
            }).ToArray();
            Export(Host);
        }

        public void ApplyHostOsParameters() {
            Host = LoadHostModel();
            var bash = new Bash();
            foreach(var modprobe in Host.OsParameters) {
                try {
                    var file = modprobe.StoredValues["$file"];
                    if(!File.Exists(file))
                        continue;
                    var value = modprobe.StoredValues["$value"];
                    bash.Execute($"echo {value} > {file}");
                }
                catch(Exception ex) {
                    ConsoleLogger.Error(ex);
                }
            }
        }
        #endregion

        #region [    repo - Host Info    ]
        public HostInfoModel GetHostInfo() {
            Host = LoadHostModel();
            var host = new HostInfoModel {
                Name = Host.HostName.StoredValues["$host_name"],
                Chassis = Host.HostName.StoredValues["$host_chassis"],
                Deployment = Host.HostName.StoredValues["$host_deployment"],
                Location = Host.HostName.StoredValues["$host_location"]
            };
            return host;
        }

        public void SetHostInfoName(string name) {
            Host = LoadHostModel();
            Host.HostName.StoredValues["$host_name"] = name;
            Export(Host);
        }

        public void SetHostInfoChassis(string chassis) {
            Host = LoadHostModel();
            Host.HostChassis.StoredValues["$host_chassis"] = chassis;
            Export(Host);
        }

        public void SetHostInfoDeployment(string deployment) {
            Host = LoadHostModel();
            Host.HostDeployment.StoredValues["$host_deployment"] = deployment;
            Export(Host);
        }

        public void SetHostInfoLocation(string location) {
            Host = LoadHostModel();
            Host.HostLocation.StoredValues["$host_location"] = location;
            Export(Host);
        }

        public void SetHostInfo(string name, string chassis, string deployment, string location) {
            Host = LoadHostModel();
            Host.HostName.StoredValues["$host_name"] = name;
            Host.HostChassis.StoredValues["$host_chassis"] = chassis;
            Host.HostDeployment.StoredValues["$host_deployment"] = deployment;
            Host.HostLocation.StoredValues["$host_location"] = location;
            Export(Host);
        }

        public void ApplyHostInfo() {
            Host = LoadHostModel();
            var launcher = new CommandLauncher();
            launcher.Launch(Host.HostName.SetCmd, Host.HostName.StoredValues);
            launcher.Launch(Host.HostChassis.SetCmd, Host.HostChassis.StoredValues);
            launcher.Launch(Host.HostDeployment.SetCmd, Host.HostDeployment.StoredValues);
            launcher.Launch(Host.HostLocation.SetCmd, Host.HostLocation.StoredValues);
            var name = Host.HostName.StoredValues["$host_name"];
            File.WriteAllText("/etc/hostname", name);
        }
        #endregion

        #region [    repo - Timezone    ]
        public string GetTimezone() {
            Host = LoadHostModel();
            var timezone = Host.Timezone.StoredValues["$host_timezone"];
            return timezone;
        }

        public void SetTimezone(string timezone) {
            Host = LoadHostModel();
            Host.Timezone.StoredValues["$host_timezone"] = timezone;
            Export(Host);
        }

        public void ApplyTimezone() {
            Host = LoadHostModel();
            var launcher = new CommandLauncher();
            launcher.Launch(Host.Timezone.SetCmd, Host.Timezone.StoredValues);
        }
        #endregion

        #region [    repo - Ntpdate    ]
        public string GetNtpdate() {
            Host = LoadHostModel();
            var ntpdate = Host.NtpdateServer.StoredValues["$server"];
            return ntpdate;
        }

        public void SetNtpdate(string ntpdate) {
            Host = LoadHostModel();
            Host.NtpdateServer.StoredValues["$server"] = ntpdate;
            Export(Host);
        }

        public void ApplyNtpdate() {
            Host = LoadHostModel();
            var launcher = new CommandLauncher();
            launcher.Launch(Host.NtpdateServer.SetCmd, Host.NtpdateServer.StoredValues);
        }
        #endregion

        #region [    repo - NtpD    ]
        public string[] GetNtpd() {
            Host = LoadHostModel();
            var launcher = new CommandLauncher();
            var ntpd = launcher.Launch(Host.Ntpd.GetCmd).ToArray();
            return ntpd;
        }

        public void SetNtpd(string[] ntpd) {
            Host = LoadHostModel();
            Host.NtpdContent = ntpd;
            Host.Ntpd.StoredValues["$value"] = ntpd.JoinToString("\n");
            Export(Host);
        }

        public void ApplyNtpd() {
            Host = LoadHostModel();
            var existing = File.ReadAllLines("/etc/ntp.conf");
            var configured = Host.NtpdContent;
            var merge = existing.Union(configured).ToArray();
            Host.NtpdContent = merge;
            File.WriteAllLines("/etc/ntp.conf", Host.NtpdContent);
            Export(Host);
        }
        #endregion

        #region [    sync time    ]
        public void SyncClock(string ntpServer = "") {
            var launcher = new CommandLauncher();
            ApplyNtpdate();
            if(Systemctl.IsActive("ntpd")) {
                ApplyNtpd();
            }
            launcher.Launch("sync-clock");
        }
        #endregion

        #region [    repo - Name Service - Hosts    ]
        public string[] GetNsHosts() {
            Host = LoadHostModel();
            var launcher = new CommandLauncher();
            var hosts = launcher.Launch(Host.NsHosts.GetCmd).ToArray();
            return hosts;
        }

        public void SetNsHosts(string[] hosts) {
            Host = LoadHostModel();
            Host.NsHostsContent = hosts;
            Export(Host);
        }

        public void ApplyNsHosts() {
            Host = LoadHostModel();
            var existing = File.ReadAllLines("/etc/hosts");
            var configured = Host.NsHostsContent;
            var merge = existing.Union(configured).ToArray();
            Host.NsHostsContent = merge;
            File.WriteAllLines("/etc/hosts", Host.NsHostsContent);
            Export(Host);
        }
        #endregion

        #region [    repo - Name Service - Networks    ]
        public string[] GetNsNetworks() {
            Host = LoadHostModel();
            var launcher = new CommandLauncher();
            var networks = launcher.Launch(Host.NsNetworks.GetCmd).ToArray();
            return networks;
        }

        public void SetNsNetworks(string[] networks) {
            Host = LoadHostModel();
            Host.NsNetworks.StoredValues["$value"] = networks.JoinToString("\n");
            Host.NsNetworksContent = networks;
            Export(Host);
        }

        public void ApplyNsNetworks() {
            Host = LoadHostModel();
            var existing = File.ReadAllLines("/etc/networks");
            var configured = Host.NsNetworksContent;
            var merge = existing.Union(configured).ToArray();
            Host.NsNetworksContent = merge;
            File.WriteAllLines("/etc/networks", Host.NsNetworksContent);
            Export(Host);
        }
        #endregion

        #region [    repo - Name Service - Resolv    ]
        public string[] GetNsResolv() {
            Host = LoadHostModel();
            var launcher = new CommandLauncher();
            var resolv = launcher.Launch(Host.NsResolv.GetCmd).ToArray();
            return resolv;
        }

        public void SetNsResolv(string[] resolv) {
            Host = LoadHostModel();
            Host.NsResolvContent = resolv;
            Host.NsResolv.StoredValues["$value"] = resolv.JoinToString("\n");
            Export(Host);
        }

        public void ApplyNsResolv() {
            Host = LoadHostModel();
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
        public string[] GetNsSwitch() {
            Host = LoadHostModel();
            var launcher = new CommandLauncher();
            var @switch = launcher.Launch(Host.NsSwitch.GetCmd).ToArray();
            return @switch;
        }

        public void SetNsSwitch(string[] @switch) {
            Host = LoadHostModel();
            Host.NsSwitchContent = @switch;
            Host.NsSwitch.StoredValues["$value"] = @switch.JoinToString("\n");
            Export(Host);
        }

        public void ApplyNsSwitch() {
            Host = LoadHostModel();
            var existing = File.ReadAllLines("/etc/nsswitch.conf");
            var configured = Host.NsSwitchContent;
            var merge = existing.Union(configured).ToArray();
            Host.NsSwitchContent = merge;
            File.WriteAllLines("/etc/nsswitch.conf", Host.NsSwitchContent);
            Export(Host);
        }
        #endregion

        #region [    repo - Domain - Internal    ]
        public string GetInternalDomain() {
            Host = LoadHostModel();
            var domain = Host.InternalDomain;
            return domain;
        }

        public void SetInternalDomain(string domain) {
            Host = LoadHostModel();
            Host.InternalDomain = domain;
            Export(Host);
        }

        public void ApplyInternalDomain() {
            throw new NotImplementedException("Edit etc files changing the internal domain value.");
        }
        #endregion

        #region [    repo - Domain - Extenal    ]
        public string GetExtenalDomain() {
            Host = LoadHostModel();
            var domain = Host.ExternalDomain;
            return domain;
        }

        public void SetExtenalDomain(string domain) {
            Host = LoadHostModel();
            Host.ExternalDomain = domain;
            Export(Host);
        }

        public void ApplyExtenalDomain() {
            throw new NotImplementedException("Edit etc files changing the external domain value.");
        }
        #endregion
    }
}
