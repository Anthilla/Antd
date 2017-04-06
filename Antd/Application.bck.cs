//-------------------------------------------------------------------------------------
//     Copyright (c) 2014, Anthilla S.r.l. (http://www.anthilla.com)
//     All rights reserved.
//
//     Redistribution and use in source and binary forms, with or without
//     modification, are permitted provided that the following conditions are met:
//         * Redistributions of source code must retain the above copyright
//           notice, this list of conditions and the following disclaimer.
//         * Redistributions in binary form must reproduce the above copyright
//           notice, this list of conditions and the following disclaimer in the
//           documentation and/or other materials provided with the distribution.
//         * Neither the name of the Anthilla S.r.l. nor the
//           names of its contributors may be used to endorse or promote products
//           derived from this software without specific prior written permission.
//
//     THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
//     ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
//     WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
//     DISCLAIMED. IN NO EVENT SHALL ANTHILLA S.R.L. BE LIABLE FOR ANY
//     DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
//     (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
//     LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
//     ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
//     (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
//     SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
//
//     20141110
//-------------------------------------------------------------------------------------

using Antd.Apps;
using Antd.Asset;
using Antd.Cloud;
using Antd.License;
using Antd.Overlay;
using Antd.Storage;
using Antd.SystemdTimer;
using Antd.Timer;
using Antd.Ui;
using Antd.Users;
using antdlib.common;
using antdlib.common.Helpers;
using antdlib.config;
using antdlib.config.shared;
using antdlib.models;
using anthilla.crypto;
using Nancy;
using Nancy.Hosting.Self;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using HostConfiguration = antdlib.config.HostConfiguration;

namespace Antd {
    internal class Application {

        #region [    private classes init    ]
        private static readonly AclConfiguration AclConfiguration = new AclConfiguration();
        private static readonly AppsConfiguration AppsConfiguration = new AppsConfiguration();
        private static readonly AppTarget AppTarget = new AppTarget();
        private static readonly Bash Bash = new Bash();
        private static readonly BindConfiguration BindConfiguration = new BindConfiguration();
        private static readonly CaConfiguration CaConfiguration = new CaConfiguration();
        private static readonly DhcpdConfiguration DhcpdConfiguration = new DhcpdConfiguration();
        private static readonly FirewallConfiguration FirewallConfiguration = new FirewallConfiguration();
        private static readonly GlusterConfiguration GlusterConfiguration = new GlusterConfiguration();
        private static readonly HostConfiguration HostConfiguration = new HostConfiguration();
        private static readonly MountManagement Mount = new MountManagement();
        private static readonly NetworkConfiguration NetworkConfiguration = new NetworkConfiguration();
        private static readonly RsyncConfiguration RsyncConfiguration = new RsyncConfiguration();
        private static readonly SambaConfiguration SambaConfiguration = new SambaConfiguration();
        private static readonly SetupConfiguration SetupConfiguration = new SetupConfiguration();
        private static readonly SshdConfiguration SshdConfiguration = new SshdConfiguration();
        private static readonly SyslogNgConfiguration SyslogNgConfiguration = new SyslogNgConfiguration();
        private static readonly Timers Timers = new Timers();
        private static readonly UserConfiguration UserConfiguration = new UserConfiguration();
        private static readonly Zpool Zpool = new Zpool();
        private static readonly JournaldConfiguration JournaldConfiguration = new JournaldConfiguration();
        private static readonly SyncMachineConfiguration SyncMachineConfiguration = new SyncMachineConfiguration();
        #endregion

        public static string KeyName = "antd";
        public static anthilla.logger.Logger Logger;

        private static void Main() {
            Logger = new anthilla.logger.Logger(KeyName, $"{Parameter.AntdCfg}/log.txt", true);
            Logger.Info("starting antd");
            var startTime = DateTime.Now;

            if(Parameter.IsUnix) {
                Logger.Info("[config] core procedures");
                List<Task> storedCoreProcedureTasks = new List<Task>();
                foreach(var action in StoredCoreProcedures()) {
                    Task t = Task.Run(() => {
                        action.Invoke();
                    });
                    storedCoreProcedureTasks.Add(t);
                }
                Task.WaitAll(storedCoreProcedureTasks.ToArray());

                var isConfigured = HostConfiguration.IsHostConfiguredByUser();
                Logger.Info($"[config] antd is {(isConfigured == false ? "NOT " : "")}configured");
                Logger.ConsoleOnly = false;

                if(isConfigured) {
                    Procedures();
                }
                else {
                    FallbackProcedures();
                }

                PostProcedures();

                if(isConfigured) {
                    ManagedProcedures();
                }
            }

            #region [    Host Init    ]
            var app = new AppConfiguration().Get();
            var port = app.AntdPort;
            var uri = $"http://localhost:{app.AntdPort}/";
            var host = new NancyHost(new Uri(uri));
            host.Start();
            Logger.Info("host ready");
            StaticConfiguration.DisableErrorTraces = false;
            Logger.Info($"http port: {port}");
            Logger.Info("antd is running");
            Logger.Info($"loaded in: {DateTime.Now - startTime}");
            #endregion

            WorkingProcedures();

            #region [    Test    ]
#if DEBUG
            Test();
#endif
            #endregion

            KeepAlive();
            Logger.Info("antd is closing");
            host.Stop();
            Console.WriteLine("host shutdown");
        }

        private static void Test() {
        }

        #region [    Core Procedures    ]
        private static void CoreProcedures() {
            Logger.Info("[config] core procedures");
            if(!Parameter.IsUnix)
                return;
            #region [    Remove Limits    ]
            const string limitsFile = "/etc/security/limits.conf";
            if(File.Exists(limitsFile)) {
                if(!File.ReadAllText(limitsFile).Contains("root - nofile 1024000")) {
                    File.AppendAllLines(limitsFile, new[] { "root - nofile 1024000" });
                }
            }
            Bash.Execute("ulimit -n 1024000", false);
            #endregion

            #region [    Overlay Watcher    ]
            if(Directory.Exists(Parameter.Overlay)) {
                new OverlayWatcher().StartWatching();
                Logger.Info("overlay watcher ready");
            }
            #endregion

            #region [    Working Directories    ]
            Directory.CreateDirectory("/cfg/antd");
            Directory.CreateDirectory("/cfg/antd/database");
            Directory.CreateDirectory("/cfg/antd/services");
            Directory.CreateDirectory("/mnt/cdrom/DIRS");
            if(Parameter.IsUnix) {
                Mount.WorkingDirectories();
            }
            Logger.Info("working directories ready");
            #endregion

            #region [    Host Prepare Configuration    ]
            var tmpHost = HostConfiguration.Host;
            HostConfiguration.Export(tmpHost);
            #endregion

            #region [    Mounts    ]
            if(MountHelper.IsAlreadyMounted("/mnt/cdrom/Kernel/active-firmware", "/lib64/firmware") == false) {
                Bash.Execute("mount /mnt/cdrom/Kernel/active-firmware /lib64/firmware", false);
            }
            var kernelRelease = Bash.Execute("uname -r").Trim();
            var linkedRelease = Bash.Execute("file /mnt/cdrom/Kernel/active-modules").Trim();
            if(MountHelper.IsAlreadyMounted("/mnt/cdrom/Kernel/active-modules") == false &&
                linkedRelease.Contains(kernelRelease)) {
                var moduleDir = $"/lib64/modules/{kernelRelease}/";
                Directory.CreateDirectory(moduleDir);
                Bash.Execute($"mount /mnt/cdrom/Kernel/active-modules {moduleDir}", false);
            }
            Bash.Execute("systemctl restart systemd-modules-load.service", false);
            Mount.AllDirectories();
            Logger.Info("mounts ready");
            #endregion

            #region [    Application Keys    ]
            var ak = new AsymmetricKeys(Parameter.AntdCfgKeys, KeyName);
            var pub = ak.PublicKey;
            #endregion

            #region [    License Management    ]
            var machineId = Machine.MachineId.Get;
            var licenseManagement = new LicenseManagement();
            licenseManagement.Download("Antd", machineId, pub);
            Logger.Info($"[machineid] {machineId}");
            var licenseStatus = licenseManagement.Check("Antd", machineId, pub);
            if(licenseStatus == null) {
                Logger.Info("[license] license results null");
            }
            else {
                Logger.Info($"[license] {licenseStatus.Status} - {licenseStatus.Message}");
            }
            #endregion

            #region [    OS Parameters    ]
            HostConfiguration.ApplyHostOsParameters();
            Logger.Info("os parameters ready");
            #endregion

            #region [    Modules    ]
            HostConfiguration.ApplyHostBlacklistModules();
            HostConfiguration.ApplyHostModprobes();
            HostConfiguration.ApplyHostRemoveModules();
            Logger.Info("modules ready");
            #endregion

            #region [    Time & Date    ]
            HostConfiguration.ApplyNtpdate();
            HostConfiguration.ApplyTimezone();
            HostConfiguration.ApplyNtpd();
            Logger.Info("time and date configured");
            #endregion

            #region [    JournalD    ]
            if(JournaldConfiguration.IsActive()) {
                JournaldConfiguration.Set();
            }
            #endregion
        }

        private static List<Action> StoredCoreProcedures() {
            var actions = new List<Action>();

            #region [    Remove Limits    ]
            actions.Add(() => {
                const string limitsFile = "/etc/security/limits.conf";
                if(File.Exists(limitsFile)) {
                    if(!File.ReadAllText(limitsFile).Contains("root - nofile 1024000")) {
                        File.AppendAllLines(limitsFile, new[] { "root - nofile 1024000" });
                    }
                }
                Bash.Execute("ulimit -n 1024000", false);
            });
            #endregion

            #region [    Overlay Watcher    ]
            actions.Add(() => {
                if(Directory.Exists(Parameter.Overlay)) {
                    new OverlayWatcher().StartWatching();
                    Logger.Info("overlay watcher ready");
                }
            });
            #endregion

            #region [    Working Directories    ]
            actions.Add(() => {
                Directory.CreateDirectory("/cfg/antd");
                Directory.CreateDirectory("/cfg/antd/database");
                Directory.CreateDirectory("/cfg/antd/services");
                Directory.CreateDirectory("/mnt/cdrom/DIRS");
                if(Parameter.IsUnix) {
                    Mount.WorkingDirectories();
                }
                Logger.Info("working directories ready");
            });
            #endregion

            #region [    Host Prepare Configuration    ]
            actions.Add(() => {
                var tmpHost = HostConfiguration.Host;
                HostConfiguration.Export(tmpHost);
            });
            #endregion

            #region [    Mounts    ]
            actions.Add(() => {
                if(MountHelper.IsAlreadyMounted("/mnt/cdrom/Kernel/active-firmware", "/lib64/firmware") == false) {
                    Bash.Execute("mount /mnt/cdrom/Kernel/active-firmware /lib64/firmware", false);
                }
                var kernelRelease = Bash.Execute("uname -r").Trim();
                var linkedRelease = Bash.Execute("file /mnt/cdrom/Kernel/active-modules").Trim();
                if(MountHelper.IsAlreadyMounted("/mnt/cdrom/Kernel/active-modules") == false &&
                    linkedRelease.Contains(kernelRelease)) {
                    var moduleDir = $"/lib64/modules/{kernelRelease}/";
                    Directory.CreateDirectory(moduleDir);
                    Bash.Execute($"mount /mnt/cdrom/Kernel/active-modules {moduleDir}", false);
                }
                Bash.Execute("systemctl restart systemd-modules-load.service", false);
                Mount.AllDirectories();
                Logger.Info("mounts ready");
            });
            #endregion

            //#region [    Application Keys    ]
            //actions.Add(() => {
            //    var ak = new AsymmetricKeys(Parameter.AntdCfgKeys, KeyName);
            //    PublicKey = ak.PublicKey;
            //});
            //#endregion

            #region [    License Management    ]
            actions.Add(() => {
                var ak = new AsymmetricKeys(Parameter.AntdCfgKeys, KeyName);
                var pk = ak.PublicKey;
                var machineId = Machine.MachineId.Get;
                var licenseManagement = new LicenseManagement();
                licenseManagement.Download("Antd", machineId, pk);
                Logger.Info($"[machineid] {machineId}");
                var licenseStatus = licenseManagement.Check("Antd", machineId, pk);
                if(licenseStatus == null) {
                    Logger.Info("[license] license results null");
                }
                else {
                    Logger.Info($"[license] {licenseStatus.Status} - {licenseStatus.Message}");
                }
            });
            #endregion

            #region [    OS Parameters    ]
            actions.Add(() => {
                HostConfiguration.ApplyHostOsParameters();
                Logger.Info("os parameters ready");
            });
            #endregion

            #region [    Modules    ]
            actions.Add(() => {
                HostConfiguration.ApplyHostBlacklistModules();
                HostConfiguration.ApplyHostModprobes();
                HostConfiguration.ApplyHostRemoveModules();
                Logger.Info("modules ready");
            });
            #endregion

            #region [    Time & Date    ]
            actions.Add(() => {
                HostConfiguration.ApplyNtpdate();
                HostConfiguration.ApplyTimezone();
                HostConfiguration.ApplyNtpd();
                Logger.Info("time and date configured");
            });
            #endregion

            #region [    JournalD    ]
            actions.Add(() => {
                if(JournaldConfiguration.IsActive()) {
                    JournaldConfiguration.Set();
                }
            });
            #endregion

            return actions;
        }
        #endregion


        #region [    Procedures    ]
        private static void Procedures() {
            Logger.Info("[config] procedures");
            if(!Parameter.IsUnix)
                return;

            #region [    Users    ]
            var manageMaster = new ManageMaster();
            manageMaster.Setup();
            if(Parameter.IsUnix) {
                UserConfiguration.Import();
                UserConfiguration.Set();
            }
            Logger.Info("users config ready");
            #endregion

            #region [    Host Configuration    ]
            HostConfiguration.ApplyHostInfo();
            Logger.Info("host configured");
            #endregion

            #region [    Name Service    ]
            HostConfiguration.ApplyNsHosts();
            HostConfiguration.ApplyNsNetworks();
            HostConfiguration.ApplyNsResolv();
            HostConfiguration.ApplyNsSwitch();
            Logger.Info("name service ready");
            #endregion

            #region [    Network    ]
            NetworkConfiguration.Start();
            NetworkConfiguration.ApplyDefaultInterfaceSetting();
            #endregion

            #region [    Firewall    ]
            if(FirewallConfiguration.IsActive()) {
                FirewallConfiguration.Set();
            }
            #endregion

            #region [    Dhcpd    ]
            if(DhcpdConfiguration.IsActive()) {
                DhcpdConfiguration.Set();
            }
            #endregion

            #region [    Bind    ]
            if(BindConfiguration.IsActive()) {
                BindConfiguration.Set();
            }
            #endregion
        }

        private static List<Action> StoredProcedures() {
            var actions = new List<Action>();

            #region [    Users    ]
            actions.Add(() => {
                var manageMaster = new ManageMaster();
                manageMaster.Setup();
                UserConfiguration.Import();
                UserConfiguration.Set();
                Logger.Info("users config ready");
            });
            #endregion

            #region [    Host Configuration    ]
            actions.Add(() => {
                HostConfiguration.ApplyHostInfo();
                Logger.Info("host configured");
            });
            #endregion

            #region [    Name Service    ]
            actions.Add(() => {
                HostConfiguration.ApplyNsHosts();
                HostConfiguration.ApplyNsNetworks();
                HostConfiguration.ApplyNsResolv();
                HostConfiguration.ApplyNsSwitch();
                Logger.Info("name service ready");
            });
            #endregion

            #region [    Network    ]
            actions.Add(() => {
                NetworkConfiguration.Start();
                NetworkConfiguration.ApplyDefaultInterfaceSetting();
            });
            #endregion

            #region [    Firewall    ]
            actions.Add(() => {
                if(FirewallConfiguration.IsActive()) {
                    FirewallConfiguration.Set();
                }
            });
            #endregion

            #region [    Dhcpd    ]
            actions.Add(() => {
                if(DhcpdConfiguration.IsActive()) {
                    DhcpdConfiguration.Set();
                }
            });
            #endregion

            #region [    Bind    ]
            actions.Add(() => {
                if(BindConfiguration.IsActive()) {
                    BindConfiguration.Set();
                }
            });
            #endregion

            return actions;
        }
        #endregion


        #region [    Managed Procedures    ]
        private static void ManagedProcedures() {
            Logger.Info("[config] managed procedures");
            if(!Parameter.IsUnix)
                return;

            #region [    Samba    ]
            if(SambaConfiguration.IsActive()) {
                SambaConfiguration.Set();
            }
            #endregion

            #region [    Syslog    ]
            if(SyslogNgConfiguration.IsActive()) {
                SyslogNgConfiguration.Set();
            }
            #endregion

            #region [    Storage    ]
            foreach(var pool in Zpool.ImportList().ToList()) {
                if(string.IsNullOrEmpty(pool))
                    continue;
                Logger.Info($"pool {pool} imported");
                Zpool.Import(pool);
            }
            Logger.Info("storage ready");
            #endregion

            #region [    Scheduler    ]
            Timers.Setup();
            Timers.Import();
            Timers.Export();
            foreach(var zp in Zpool.List()) {
                Timers.Create(zp.Name.ToLower() + "snap", "hourly", $"/sbin/zfs snap -r {zp.Name}@${{TTDATE}}");
            }
            Timers.StartAll();
            new SnapshotCleanup().Start(new TimeSpan(2, 00, 00));
            new SyncTime().Start(new TimeSpan(0, 42, 00));
            new RemoveUnusedModules().Start(new TimeSpan(2, 15, 00));
            Logger.Info("scheduled events ready");
            #endregion

            #region [    Acl    ]
            if(AclConfiguration.IsActive()) {
                AclConfiguration.Set();
                AclConfiguration.ScriptSetup();
            }
            #endregion

            #region [    Sync    ]
            if(GlusterConfiguration.IsActive()) {
                GlusterConfiguration.Set();
            }
            if(RsyncConfiguration.IsActive()) {
                RsyncConfiguration.Set();
            }
            #endregion

            #region [    SyncMachine    ]
            if(SyncMachineConfiguration.IsActive()) {
                SyncMachineConfiguration.Set();
            }
            #endregion

            #region [    C A    ]
            if(CaConfiguration.IsActive()) {
                CaConfiguration.Set();
            }
            #endregion

            #region [    Apps    ]
            AppTarget.Setup();
            var apps = AppsConfiguration.Get().Apps;
            foreach(var app in apps) {
                var units = app.UnitLauncher;
                foreach(var unit in units) {
                    if(Systemctl.IsActive(unit) == false) {
                        Systemctl.Restart(unit);
                    }
                }
            }
            //AppTarget.StartAll();
            Logger.Info("apps ready");
            #endregion
        }

        private static List<Action> StoredManagedProcedures() {
            var actions = new List<Action>();

            #region [    Samba    ]
            actions.Add(() => {
                if(SambaConfiguration.IsActive()) {
                    SambaConfiguration.Set();
                }
            });
            #endregion

            #region [    Syslog    ]
            actions.Add(() => {
                if(SyslogNgConfiguration.IsActive()) {
                    SyslogNgConfiguration.Set();
                }
            });
            #endregion

            #region [    Storage    ]
            actions.Add(() => {
                foreach(var pool in Zpool.ImportList().ToList()) {
                    if(string.IsNullOrEmpty(pool))
                        continue;
                    Logger.Info($"pool {pool} imported");
                    Zpool.Import(pool);
                }
                Logger.Info("storage ready");
            });
            #endregion

            #region [    Scheduler    ]
            actions.Add(() => {
                Timers.Setup();
                Timers.Import();
                Timers.Export();
                foreach(var zp in Zpool.List()) {
                    Timers.Create(zp.Name.ToLower() + "snap", "hourly", $"/sbin/zfs snap -r {zp.Name}@${{TTDATE}}");
                }
                Timers.StartAll();
                new SnapshotCleanup().Start(new TimeSpan(2, 00, 00));
                new SyncTime().Start(new TimeSpan(0, 42, 00));
                new RemoveUnusedModules().Start(new TimeSpan(2, 15, 00));
                Logger.Info("scheduled events ready");
            });
            #endregion

            #region [    Acl    ]
            actions.Add(() => {
                if(AclConfiguration.IsActive()) {
                    AclConfiguration.Set();
                    AclConfiguration.ScriptSetup();
                }
            });
            #endregion

            #region [    Sync    ]
            actions.Add(() => {
                if(GlusterConfiguration.IsActive()) {
                    GlusterConfiguration.Set();
                }
                if(RsyncConfiguration.IsActive()) {
                    RsyncConfiguration.Set();
                }
            });
            #endregion

            #region [    SyncMachine    ]
            actions.Add(() => {
                if(SyncMachineConfiguration.IsActive()) {
                    SyncMachineConfiguration.Set();
                }
            });
            #endregion

            #region [    C A    ]
            actions.Add(() => {
                if(CaConfiguration.IsActive()) {
                    CaConfiguration.Set();
                }
            });
            #endregion

            #region [    Apps    ]
            actions.Add(() => {
                AppTarget.Setup();
                var apps = AppsConfiguration.Get().Apps;
                foreach(var app in apps) {
                    var units = app.UnitLauncher;
                    foreach(var unit in units) {
                        if(Systemctl.IsActive(unit) == false) {
                            Systemctl.Restart(unit);
                        }
                    }
                }
                //AppTarget.StartAll();
                Logger.Info("apps ready");
            });
            #endregion

            return actions;
        }
        #endregion


        #region [    Fallback Procedures    ]
        private static void FallbackProcedures() {
            Logger.Info("[config] fallback procedures");
            if(!Parameter.IsUnix)
                return;

            const string localNetwork = "10.11.0.0";
            const string localIp = "10.11.254.254";
            const string localRange = "16";
            const string localHostname = "box01";
            const string localDomain = "install.local";

            #region [    Host Configuration    ]
            HostConfiguration.SetHostInfoName(localHostname);
            HostConfiguration.ApplyHostInfo();
            Logger.Info("host configured");
            #endregion

            #region [    Name Service    ]
            HostConfiguration.SetNsHosts(new[] {
                "127.0.0.1 localhost",
                $"{localIp} {localHostname}.{localDomain} {localHostname}"
            });
            HostConfiguration.ApplyNsHosts();
            HostConfiguration.SetNsNetworks(new[] {
                "loopback 127.0.0.0",
                "link-local 169.254.0.0",
                $"{localDomain} {localNetwork}"

            });
            HostConfiguration.ApplyNsNetworks();
            HostConfiguration.SetNsResolv(new[] {
                $"nameserver {localIp}",
                $"search {localDomain}",
                $"domain {localDomain}"
            });
            HostConfiguration.ApplyNsResolv();
            HostConfiguration.SetNsSwitch(new[] {
                "passwd: compat db files nis",
                "shadow: compat db files nis",
                "group: compat db files nis",
                "hosts: files dns",
                "networks: files dns",
                "services: db files",
                "protocols: db files",
                "rpc: db files",
                "ethers: db files",
                "netmasks: files",
                "netgroup: files",
                "bootparams: files",
                "automount: files",
                "aliases: files"
            });
            HostConfiguration.ApplyNsSwitch();
            Logger.Info("name service ready");
            #endregion

            #region [    Network    ]
            var npi = NetworkConfiguration.InterfacePhysical;
            var nifs = NetworkConfiguration.Get().Interfaces;
            const string nifName = "br0";
            var tryget = nifs?.FirstOrDefault(_ => _.Interface == nifName);
            if(tryget == null) {
                NetworkConfiguration.AddInterfaceSetting(new NetworkInterfaceConfigurationModel {
                    Interface = nifName,
                    Mode = NetworkInterfaceMode.Static,
                    Status = NetworkInterfaceStatus.Up,
                    StaticAddress = localIp,
                    StaticRange = localRange,
                    Type = NetworkInterfaceType.Bridge,
                    InterfaceList = npi.ToList()
                });
            }
            NetworkConfiguration.ApplyDefaultInterfaceSetting();
            #endregion

            #region [    Dhcpd    ]
            DhcpdConfiguration.Save(new DhcpdConfigurationModel {
                ZoneName = localDomain,
                ZonePrimaryAddress = localIp,
                DdnsDomainName = $"{localDomain}.",
                Option = new List<string> { $"domain-name \"{localDomain}\"", "routers eth0", "local-proxy-config code 252 = text" },
                KeySecret = "ND991KFHCCA9tUrafsf29uxDM3ZKfnrVR4f1I2J27Ow=",
                SubnetNtpServers = localIp,
                SubnetTimeServers = localIp,
                SubnetOptionRouters = localIp,
                SubnetDomainNameServers = localIp,
                SubnetIpMask = "255.255.0.0",
                SubnetMask = "255.255.0.0",
                SubnetBroadcastAddress = "10.11.255.255",
                SubnetIpFamily = localNetwork
            });
            DhcpdConfiguration.Set();
            #endregion

            #region [    Bind    ]
            BindConfiguration.Save(new BindConfigurationModel {
                ControlIp = localIp,
                AclInternalInterfaces = new List<string> { localIp },
                AclInternalNetworks = new List<string> { $"{localNetwork}/{localRange}" },
                Zones = new List<BindConfigurationZoneModel> {
                    new BindConfigurationZoneModel {
                        Name = "11.10.in-addr.arpa",
                        Type = "master",
                        File = "" //todo crea e gestisci file della zona
                    },
                    new BindConfigurationZoneModel {
                        Name = localDomain,
                        Type = "master",
                        File = "" //todo crea e gestisci file della zona
                    },
                }
            });
            BindConfiguration.Set();
            #endregion
        }

        private static List<Action> StoredFallbackProcedures() {
            var actions = new List<Action>();

            const string localNetwork = "10.11.0.0";
            const string localIp = "10.11.254.254";
            const string localRange = "16";
            const string localHostname = "box01";
            const string localDomain = "install.local";

            #region [    Host Configuration    ]
            actions.Add(() => {
                HostConfiguration.SetHostInfoName(localHostname);
                HostConfiguration.ApplyHostInfo();
                Logger.Info("host configured");
            });
            #endregion

            #region [    Name Service    ]
            actions.Add(() => {
                HostConfiguration.SetNsHosts(new[] {
                "127.0.0.1 localhost",
                $"{localIp} {localHostname}.{localDomain} {localHostname}"
            });
                HostConfiguration.ApplyNsHosts();
                HostConfiguration.SetNsNetworks(new[] {
                "loopback 127.0.0.0",
                "link-local 169.254.0.0",
                $"{localDomain} {localNetwork}"

            });
                HostConfiguration.ApplyNsNetworks();
                HostConfiguration.SetNsResolv(new[] {
                $"nameserver {localIp}",
                $"search {localDomain}",
                $"domain {localDomain}"
            });
                HostConfiguration.ApplyNsResolv();
                HostConfiguration.SetNsSwitch(new[] {
                "passwd: compat db files nis",
                "shadow: compat db files nis",
                "group: compat db files nis",
                "hosts: files dns",
                "networks: files dns",
                "services: db files",
                "protocols: db files",
                "rpc: db files",
                "ethers: db files",
                "netmasks: files",
                "netgroup: files",
                "bootparams: files",
                "automount: files",
                "aliases: files"
            });
                HostConfiguration.ApplyNsSwitch();
                Logger.Info("name service ready");
            });
            #endregion

            #region [    Network    ]
            actions.Add(() => {
                var npi = NetworkConfiguration.InterfacePhysical;
                var nifs = NetworkConfiguration.Get().Interfaces;
                const string nifName = "br0";
                var tryget = nifs?.FirstOrDefault(_ => _.Interface == nifName);
                if(tryget == null) {
                    NetworkConfiguration.AddInterfaceSetting(new NetworkInterfaceConfigurationModel {
                        Interface = nifName,
                        Mode = NetworkInterfaceMode.Static,
                        Status = NetworkInterfaceStatus.Up,
                        StaticAddress = localIp,
                        StaticRange = localRange,
                        Type = NetworkInterfaceType.Bridge,
                        InterfaceList = npi.ToList()
                    });
                }
                NetworkConfiguration.ApplyDefaultInterfaceSetting();
            });
            #endregion

            #region [    Dhcpd    ]
            actions.Add(() => {
                DhcpdConfiguration.Save(new DhcpdConfigurationModel {
                    ZoneName = localDomain,
                    ZonePrimaryAddress = localIp,
                    DdnsDomainName = $"{localDomain}.",
                    Option = new List<string> { $"domain-name \"{localDomain}\"", "routers eth0", "local-proxy-config code 252 = text" },
                    KeySecret = "ND991KFHCCA9tUrafsf29uxDM3ZKfnrVR4f1I2J27Ow=",
                    SubnetNtpServers = localIp,
                    SubnetTimeServers = localIp,
                    SubnetOptionRouters = localIp,
                    SubnetDomainNameServers = localIp,
                    SubnetIpMask = "255.255.0.0",
                    SubnetMask = "255.255.0.0",
                    SubnetBroadcastAddress = "10.11.255.255",
                    SubnetIpFamily = localNetwork
                });
                DhcpdConfiguration.Set();
            });
            #endregion

            #region [    Bind    ]
            actions.Add(() => {
                BindConfiguration.Save(new BindConfigurationModel {
                    ControlIp = localIp,
                    AclInternalInterfaces = new List<string> { localIp },
                    AclInternalNetworks = new List<string> { $"{localNetwork}/{localRange}" },
                    Zones = new List<BindConfigurationZoneModel> {
                    new BindConfigurationZoneModel {
                        Name = "11.10.in-addr.arpa",
                        Type = "master",
                        File = "" //todo crea e gestisci file della zona
                    },
                    new BindConfigurationZoneModel {
                        Name = localDomain,
                        Type = "master",
                        File = "" //todo crea e gestisci file della zona
                    },
                }
                });
                BindConfiguration.Set();
            });
            #endregion

            return actions;
        }
        #endregion


        #region [    Post Procedures    ]
        private static void PostProcedures() {
            Logger.Info("[config] post procedures");
            if(!Parameter.IsUnix)
                return;

            #region [    Apply Setup Configuration    ]
            SetupConfiguration.Set();
            Logger.Info("machine configured (apply setup.conf)");
            #endregion

            #region [    Services    ]
            HostConfiguration.ApplyHostServices();
            Logger.Info("services ready");
            #endregion

            #region [    Ssh    ]
            if(SshdConfiguration.IsActive()) {
                SshdConfiguration.Set();
            }
            if(!Directory.Exists(Parameter.RootSsh)) {
                Directory.CreateDirectory(Parameter.RootSsh);
            }
            if(!Directory.Exists(Parameter.RootSshMntCdrom)) {
                Directory.CreateDirectory(Parameter.RootSshMntCdrom);
            }
            if(!MountHelper.IsAlreadyMounted(Parameter.RootSsh)) {
                var mnt = new MountManagement();
                mnt.Dir(Parameter.RootSsh);
            }
            var rk = new RootKeys();
            if(rk.Exists == false) {
                rk.Create();
            }
            var authorizedKeysConfiguration = new AuthorizedKeysConfiguration();
            var storedKeys = authorizedKeysConfiguration.Get().Keys;
            foreach(var storedKey in storedKeys) {
                var home = storedKey.User == "root" ? "/root/.ssh" : $"/home/{storedKey.User}/.ssh";
                var authorizedKeysPath = $"{home}/authorized_keys";
                if(!File.Exists(authorizedKeysPath)) {
                    File.Create(authorizedKeysPath);
                }
                File.AppendAllLines(authorizedKeysPath, new List<string> { $"{storedKey.KeyValue} {storedKey.RemoteUser}" });
                Bash.Execute($"chmod 600 {authorizedKeysPath}");
                Bash.Execute($"chown {storedKey.User}:{storedKey.User} {authorizedKeysPath}");
            }
            Logger.Info("ssh ready");
            #endregion

            #region [    Avahi    ]
            const string avahiServicePath = "/etc/avahi/services/antd.service";
            if(File.Exists(avahiServicePath)) {
                File.Delete(avahiServicePath);
            }
            var appConfiguration = new AppConfiguration().Get();
            File.WriteAllLines(avahiServicePath, AvahiCustomXml.Generate(appConfiguration.AntdUiPort.ToString()));
            Bash.Execute("chmod 755 /etc/avahi/services", false);
            Bash.Execute($"chmod 644 {avahiServicePath}", false);
            Systemctl.Restart("avahi-daemon.service");
            Systemctl.Restart("avahi-daemon.socket");
            Logger.Info("avahi ready");
            #endregion

            #region [    AntdUI    ]
            UiService.Setup();
            Logger.Info("antduisetup");
            #endregion
        }

        private static List<Action> StoredPostProcedures() {
            var actions = new List<Action>();

            #region [    Apply Setup Configuration    ]
            actions.Add(() => {
                SetupConfiguration.Set();
                Logger.Info("machine configured (apply setup.conf)");
            });
            #endregion

            #region [    Services    ]
            actions.Add(() => {
                HostConfiguration.ApplyHostServices();
                Logger.Info("services ready");
            });
            #endregion

            #region [    Ssh    ]
            actions.Add(() => {
                if(SshdConfiguration.IsActive()) {
                    SshdConfiguration.Set();
                }
                if(!Directory.Exists(Parameter.RootSsh)) {
                    Directory.CreateDirectory(Parameter.RootSsh);
                }
                if(!Directory.Exists(Parameter.RootSshMntCdrom)) {
                    Directory.CreateDirectory(Parameter.RootSshMntCdrom);
                }
                if(!MountHelper.IsAlreadyMounted(Parameter.RootSsh)) {
                    var mnt = new MountManagement();
                    mnt.Dir(Parameter.RootSsh);
                }
                var rk = new RootKeys();
                if(rk.Exists == false) {
                    rk.Create();
                }
                var authorizedKeysConfiguration = new AuthorizedKeysConfiguration();
                var storedKeys = authorizedKeysConfiguration.Get().Keys;
                foreach(var storedKey in storedKeys) {
                    var home = storedKey.User == "root" ? "/root/.ssh" : $"/home/{storedKey.User}/.ssh";
                    var authorizedKeysPath = $"{home}/authorized_keys";
                    if(!File.Exists(authorizedKeysPath)) {
                        File.Create(authorizedKeysPath);
                    }
                    File.AppendAllLines(authorizedKeysPath, new List<string> { $"{storedKey.KeyValue} {storedKey.RemoteUser}" });
                    Bash.Execute($"chmod 600 {authorizedKeysPath}");
                    Bash.Execute($"chown {storedKey.User}:{storedKey.User} {authorizedKeysPath}");
                }
                Logger.Info("ssh ready");
            });
            #endregion

            #region [    Avahi    ]
            actions.Add(() => {
                const string avahiServicePath = "/etc/avahi/services/antd.service";
                if(File.Exists(avahiServicePath)) {
                    File.Delete(avahiServicePath);
                }
                var appConfiguration = new AppConfiguration().Get();
                File.WriteAllLines(avahiServicePath, AvahiCustomXml.Generate(appConfiguration.AntdUiPort.ToString()));
                Bash.Execute("chmod 755 /etc/avahi/services", false);
                Bash.Execute($"chmod 644 {avahiServicePath}", false);
                Systemctl.Restart("avahi-daemon.service");
                Systemctl.Restart("avahi-daemon.socket");
                Logger.Info("avahi ready");
            });
            #endregion

            #region [    AntdUI    ]
            actions.Add(() => {
                UiService.Setup();
                Logger.Info("antduisetup");
            });
            #endregion

            return actions;
        }
        #endregion


        private static void WorkingProcedures() {
            Logger.Info("[config] working procedures");
            #region [    Cloud Send Uptime    ]
            var csuTimer = new UpdateCloudInfo();
            csuTimer.Start(1000 * 60 * 5);
            #endregion

            #region [    Cloud Fetch Commands    ]
            var cfcTimer = new FetchRemoteCommand();
            cfcTimer.Start((1000 * 60 * 2) + 330);
            #endregion
        }

        #region [    Shutdown Management    ]
        private static void KeepAlive() {
            var r = Console.ReadLine();
            while(r != "quit") {
                r = Console.ReadLine();
            }
        }
        #endregion
    }
}