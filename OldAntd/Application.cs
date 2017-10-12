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
using Antd.Info;
using Antd.License;
using Antd.Overlay;
using Antd.Storage;
using Antd.SystemdTimer;
using Antd.Timer;
using Antd.Ui;
using Antd.VFS;
using antdlib.config;
using antdlib.config.shared;
using antdlib.models;
using anthilla.commands;
using anthilla.core;
using anthilla.core.Helpers;
using anthilla.crypto;
using anthilla.scheduler;
using Kvpbase;
using Nancy;
using Nancy.Hosting.Self;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using EnumerableExtensions = anthilla.core.EnumerableExtensions;
using HostConfiguration = antdlib.config.HostConfiguration;

namespace Antd {
    internal class Application {

        public static string KeyName = "antd";
        public static VfsWatcher VfsWatcher;
        public static Stopwatch STOPWATCH;

        private static void Main() {
            var resetEvent = new AutoResetEvent(initialState: false);
            Console.CancelKeyPress += (s, e) => { e.Cancel = true; resetEvent.Set(); };

            STOPWATCH = new Stopwatch();
            ConsoleLogger.Log("[antd] start");
            STOPWATCH.Start();

            OsReadAndWrite();
            RemoveLimits();
            Time();
            StopNetworkd();
            OverlayWatcher();
            WorkingDirectories();
            Mounts();
            CheckUnitsLocation();
            GenerateSecret();
            LicenseManagement();
            JournalD();
            ImportConfiguration();
            Adjustments();
            Users();
            HostConfiguration_NameService();
            Network();
            Firewall();
            Dhcpd();
            Bind();
            ApplySetupConfiguration();
            Nginx();
            Ssh();
            StartRssdp();
            Samba();
            Syslog();
            StorageZfs();
            Acl();
            Ca();
            HostInit();
            Apps();
            Sync_Gluster();
            StorageServer();
            Tor();
            Cluster();
            Gluster();
            DirectoryWatchers();
            CheckApplicationFileAcls();
            CheckSystemComponents();
            Scheduler();
            Test();

            ConsoleLogger.Log($"loaded in: {STOPWATCH.ElapsedMilliseconds} ms");
            resetEvent.WaitOne();
            HOST.Stop();
            STOPWATCH.Stop();
            Console.WriteLine("[antd] stop");
        }

        private static NancyHost HOST;

        private static void OsReadAndWrite() {
            Bash.Execute("mount -o remount,rw,noatime /", false);
            Bash.Execute("mount -o remount,rw,discard,noatime /mnt/cdrom", false);
        }

        private static void RemoveLimits() {
            const string limitsFile = "/etc/security/limits.conf";
            if(File.Exists(limitsFile)) {
                if(!File.ReadAllText(limitsFile).Contains("root - nofile 1024000")) {
                    FileWithAcl.AppendAllLines(limitsFile, new[] { "root - nofile 1024000" }, "644", "root", "wheel");
                }
            }
            Bash.Execute("ulimit -n 1024000", false);
        }

        private static void Time() {
            CommandLauncher.Launch("sync-clock");
        }

        private static void StopNetworkd() {
            Systemctl.Stop("systemd-networkd.service");
            Systemctl.Disable("systemd-networkd.service");
            Systemctl.Mask("systemd-networkd.service");
            Systemctl.Stop("systemd-resolved.service");
            Systemctl.Disable("systemd-resolved.service");
            Systemctl.Mask("systemd-resolved.service");
        }

        private static void OverlayWatcher() {
            if(Directory.Exists(Parameter.Overlay)) {
                new OverlayWatcher().StartWatching();
                ConsoleLogger.Log("overlay watcher ready");
            }
        }

        private static void WorkingDirectories() {
            Directory.CreateDirectory(Parameter.AntdCfg);
            Directory.CreateDirectory(Parameter.AntdCfgServices);
            Directory.CreateDirectory(Parameter.AntdCfgNetwork);
            Network2Configuration.CreateWorkingDirectories();
            Directory.CreateDirectory(Parameter.AntdCfgParameters);
            Directory.CreateDirectory(Parameter.AntdCfgCluster);
            Directory.CreateDirectory($"{Parameter.AntdCfgServices}/acls");
            Directory.CreateDirectory($"{Parameter.AntdCfgServices}/acls/template");
            Directory.CreateDirectory(Parameter.RepoConfig);
            Directory.CreateDirectory(Parameter.RepoDirs);
            Directory.CreateDirectory(Parameter.AnthillaUnits);
            Directory.CreateDirectory(Parameter.TimerUnits);
            Directory.CreateDirectory(Parameter.AntdCfgVfs);
            Directory.CreateDirectory(Parameter.AntdCfgRssdp);
            ConsoleLogger.Log("working directories created");
            MountManagement.WorkingDirectories();
            ConsoleLogger.Log("working directories mounted");
        }

        private static void Mounts() {
            if(MountHelper.IsAlreadyMounted("/mnt/cdrom/Kernel/active-firmware", "/lib64/firmware") == false) {
                Bash.Execute("mount /mnt/cdrom/Kernel/active-firmware /lib64/firmware", false);
            }
            var kernelRelease = Bash.Execute("uname -r").Trim();
            var linkedRelease = Bash.Execute("file /mnt/cdrom/Kernel/active-modules").Trim();
            if(MountHelper.IsAlreadyMounted("/mnt/cdrom/Kernel/active-modules") == false &&
                linkedRelease.Contains(kernelRelease)) {
                var moduleDir = $"/lib64/modules/{kernelRelease}/";
                DirectoryWithAcl.CreateDirectory(moduleDir);
                Bash.Execute($"mount /mnt/cdrom/Kernel/active-modules {moduleDir}", false);
            }
            Bash.Execute("systemctl restart systemd-modules-load.service", false);
            MountManagement.AllDirectories();
            ConsoleLogger.Log("mounts ready");
        }

        private static void CheckUnitsLocation() {
            var anthillaUnits = Directory.EnumerateFiles(Parameter.AnthillaUnits, "*.*", SearchOption.TopDirectoryOnly);
            if(!anthillaUnits.Any()) {
                var antdUnits = Directory.EnumerateFiles(Parameter.AntdUnits, "*.*", SearchOption.TopDirectoryOnly);
                foreach(var unit in antdUnits) {
                    var trueUnit = unit.Replace(Parameter.AntdUnits, Parameter.AnthillaUnits);
                    if(!File.Exists(trueUnit)) {
                        File.Copy(unit, trueUnit);
                    }
                    File.Delete(unit);
                    Bash.Execute($"ln -s {trueUnit} {unit}");
                }
                var kernelUnits = Directory.EnumerateFiles(Parameter.KernelUnits, "*.*", SearchOption.TopDirectoryOnly);
                foreach(var unit in kernelUnits) {
                    var trueUnit = unit.Replace(Parameter.KernelUnits, Parameter.AnthillaUnits);
                    if(!File.Exists(trueUnit)) {
                        File.Copy(unit, trueUnit);
                    }
                    File.Delete(unit);
                    Bash.Execute($"ln -s {trueUnit} {unit}");
                }
                var applicativeUnits = Directory.EnumerateFiles(Parameter.ApplicativeUnits, "*.*", SearchOption.TopDirectoryOnly);
                foreach(var unit in applicativeUnits) {
                    var trueUnit = unit.Replace(Parameter.ApplicativeUnits, Parameter.AnthillaUnits);
                    if(!File.Exists(trueUnit)) {
                        File.Copy(unit, trueUnit);
                    }
                    File.Delete(unit);
                    Bash.Execute($"ln -s {trueUnit} {unit}");
                }
            }
            //anthillaUnits = Directory.EnumerateFiles(Parameter.AnthillaUnits, "*.*", SearchOption.TopDirectoryOnly).ToList();
            if(!anthillaUnits.Any()) {
                foreach(var unit in anthillaUnits) {
                    Bash.Execute($"chown root:wheel {unit}");
                    Bash.Execute($"chmod 644 {unit}");
                }
            }
            ConsoleLogger.Log("[check] units integrity");
        }

        private static void GenerateSecret() {
            if(!File.Exists(Parameter.AntdCfgSecret)) {
                FileWithAcl.WriteAllText(Parameter.AntdCfgSecret, Secret.Gen(), "644", "root", "wheel");
            }
            if(string.IsNullOrEmpty(File.ReadAllText(Parameter.AntdCfgSecret))) {
                FileWithAcl.WriteAllText(Parameter.AntdCfgSecret, Secret.Gen(), "644", "root", "wheel");
            }
        }

        private static void LicenseManagement() {
            var ak = new AsymmetricKeys(Parameter.AntdCfgKeys, KeyName);
            var pub = ak.PublicKey;
            var appconfig = new AppConfiguration().Get();
            ConsoleLogger.Log($"[cloud] {appconfig.CloudAddress}");
            var machineIds = Machine.MachineIds.Get;
            ConsoleLogger.Log($"[machineid] {machineIds.PartNumber}");
            ConsoleLogger.Log($"[machineid] {machineIds.SerialNumber}");
            ConsoleLogger.Log($"[machineid] {machineIds.MachineUid}");
            var licenseManagement = new LicenseManagement();
            licenseManagement.Download("Antd", machineIds, pub);
            var licenseStatus = licenseManagement.Check("Antd", machineIds, pub);
            ConsoleLogger.Log(licenseStatus == null
                ? "[license] license results null"
                : $"[license] {licenseStatus.Status} - {licenseStatus.Message}");

        }

        private static void JournalD() {
            if(JournaldConfiguration.IsActive()) {
                JournaldConfiguration.Set();
            }
        }

        private static void ImportConfiguration() {
            Network2Configuration.SetWorkingDirectories();

            #region import host2model
            var tmpHost = HostConfiguration.Host;
            var varsFile = Host2Configuration.FilePath;
            var vars = new Host2Model {
                HostName = tmpHost.HostName.StoredValues.FirstOrDefault().Value,
                HostChassis = tmpHost.HostChassis.StoredValues.FirstOrDefault().Value,
                HostDeployment = tmpHost.HostDeployment.StoredValues.FirstOrDefault().Value,
                HostLocation = tmpHost.HostLocation.StoredValues.FirstOrDefault().Value,
                InternalDomainPrimary = tmpHost.InternalDomain,
                ExternalDomainPrimary = tmpHost.ExternalDomain,
                InternalHostIpPrimary = "",
                ExternalHostIpPrimary = "",
                Timezone = tmpHost.Timezone.StoredValues.FirstOrDefault().Value,
                NtpdateServer = tmpHost.NtpdateServer.StoredValues.FirstOrDefault().Value,
                MachineUid = Machine.MachineIds.Get.MachineUid,
                Cloud = Parameter.Cloud
            };

            if(File.Exists(HostConfiguration.FilePath)) {

                if(!File.Exists(varsFile)) {
                    ConsoleLogger.Log("[data import] host configuration");
                    Host2Configuration.Export(vars);
                }
                else {
                    if(string.IsNullOrEmpty(File.ReadAllText(varsFile))) {
                        ConsoleLogger.Log("[data import] host configuration");
                        Host2Configuration.Export(vars);
                    }
                }
            }
            #endregion

            #region import network2model
            var tmpNet = NetworkConfiguration.Get();
            var tmpHost2 = Host2Configuration.Host;
            var niflist = new List<NetworkInterface>();
            foreach(var cif in tmpNet.Interfaces) {
                ConsoleLogger.Log($"[data import] network configuration for '{cif.Interface}'");
                var broadcast = Cidr.CalcNetwork(cif.StaticAddress, cif.StaticRange).Broadcast.ToString();
                var hostname = $"{vars.HostName}{NetworkInterfaceType.Internal}.{vars.InternalDomainPrimary}";
                var subnet = tmpHost2.InternalNetPrimaryBits;
                var index = Network2Configuration.InterfaceConfigurationList.Count(_ => _.Type == NetworkInterfaceType.Internal);
                var networkConfiguration = new NetworkInterfaceConfiguration {
                    Id = cif.Interface + cif.Guid.Substring(0, 8),
                    Adapter = cif.Type,
                    Alias = "import " + cif.Interface,
                    Ip = cif.StaticAddress,
                    Subnet = subnet,
                    Mode = cif.Mode,
                    ChildrenIf = cif.InterfaceList,
                    Broadcast = broadcast,
                    Type = NetworkInterfaceType.Internal,
                    Hostname = hostname,
                    Index = index,
                    Description = "import " + cif.Interface,
                    RoleVerb = NetworkRoleVerb.iif
                };

                var tryget = Network2Configuration.InterfaceConfigurationList.FirstOrDefault(_ => _.Id == networkConfiguration.Id);
                if(tryget == null) {
                    Network2Configuration.AddInterfaceConfiguration(networkConfiguration);
                }

                var ifConfig = new NetworkInterface {
                    Device = cif.Interface,
                    Configuration = networkConfiguration.Id,
                    AdditionalConfigurations = new List<string>(),
                    GatewayConfiguration = ""
                };

                var tryget2 = Network2Configuration.Conf.Interfaces.FirstOrDefault(_ => _.Device == cif.Interface);
                if(tryget2 == null) {
                    niflist.Add(ifConfig);
                }
            }
            if(niflist.Any()) {
                Network2Configuration.SaveInterfaceSetting(niflist);
            }
            if(!Network2Configuration.GatewayConfigurationList.Any()) {
                var defaultGatewayConfiguration = new NetworkGatewayConfiguration {
                    Id = CommonRandom.ShortGuid(),
                    IsDefault = true,
                    GatewayAddress = vars.InternalHostIpPrimary,
                    Description = "DFGW"
                };
                Network2Configuration.AddGatewayConfiguration(defaultGatewayConfiguration);
            }
            #endregion

            #region import parameters
            if(!File.Exists($"{Parameter.AntdCfgParameters}/endcommands.conf")) {
                var tmpsetup = SetupConfiguration.Get();
                HostParametersConfiguration.SetEndCommandsList(tmpsetup);
            }
            if(!File.Exists($"{Parameter.AntdCfgParameters}/modprobes.conf")) {
                var ddd = EnumerableExtensions.Merge(tmpHost.Modprobes.Select(_ => _.StoredValues.Select(___ => ___.Value)));
                HostParametersConfiguration.SetModprobesList(ddd.ToList());
            }
            if(!File.Exists($"{Parameter.AntdCfgParameters}/rmmod.conf")) {
                var ddd = tmpHost.RemoveModules.StoredValues.FirstOrDefault().Value.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                HostParametersConfiguration.SetRmmodList(ddd.ToList());
            }
            if(!File.Exists($"{Parameter.AntdCfgParameters}/modulesblacklist.conf")) {
                var ddd = tmpHost.ModulesBlacklist;
                HostParametersConfiguration.SetModulesBlacklistList(ddd.ToList());
            }
            if(!File.Exists($"{Parameter.AntdCfgParameters}/osparameters.conf")) {
                var list = new List<string> {
                                "/proc/sys/fs/file-max 1024000",
                                "/proc/sys/net/bridge/bridge-nf-call-arptables 0",
                                "/proc/sys/net/bridge/bridge-nf-call-ip6tables 0",
                                "/proc/sys/net/bridge/bridge-nf-call-iptables 0",
                                "/proc/sys/net/bridge/bridge-nf-filter-pppoe-tagged 0",
                                "/proc/sys/net/bridge/bridge-nf-filter-vlan-tagged 0",
                                "/proc/sys/net/core/netdev_max_backlog 300000",
                                "/proc/sys/net/core/optmem_max 40960",
                                "/proc/sys/net/core/rmem_max 268435456",
                                "/proc/sys/net/core/somaxconn 65536",
                                "/proc/sys/net/core/wmem_max 268435456",
                                "/proc/sys/net/ipv4/conf/all/accept_local 1",
                                "/proc/sys/net/ipv4/conf/all/accept_redirects 1",
                                "/proc/sys/net/ipv4/conf/all/accept_source_route 1",
                                "/proc/sys/net/ipv4/conf/all/rp_filter 0",
                                "/proc/sys/net/ipv4/conf/all/forwarding 1",
                                "/proc/sys/net/ipv4/conf/default/rp_filter 0",
                                "/proc/sys/net/ipv4/ip_forward 1",
                                "/proc/sys/net/ipv4/ip_local_port_range 1024 65000",
                                "/proc/sys/net/ipv4/ip_no_pmtu_disc 1",
                                "/proc/sys/net/ipv4/tcp_congestion_control htcp",
                                "/proc/sys/net/ipv4/tcp_fin_timeout 40",
                                "/proc/sys/net/ipv4/tcp_max_syn_backlog 3240000",
                                "/proc/sys/net/ipv4/tcp_max_tw_buckets 1440000",
                                "/proc/sys/net/ipv4/tcp_moderate_rcvbuf 1",
                                "/proc/sys/net/ipv4/tcp_mtu_probing 1",
                                "/proc/sys/net/ipv4/tcp_rmem 4096 87380 134217728",
                                "/proc/sys/net/ipv4/tcp_slow_start_after_idle 1",
                                "/proc/sys/net/ipv4/tcp_tw_recycle 0",
                                "/proc/sys/net/ipv4/tcp_tw_reuse 1",
                                "/proc/sys/net/ipv4/tcp_window_scaling 1",
                                "/proc/sys/net/ipv4/tcp_wmem 4096 65536 134217728",
                                "/proc/sys/net/ipv6/conf/br0/disable_ipv6 1",
                                "/proc/sys/net/ipv6/conf/eth0/disable_ipv6 1",
                                "/proc/sys/net/ipv6/conf/wlan0/disable_ipv6 1",
                                "/proc/sys/vm/swappiness 0"
                            };
                HostParametersConfiguration.SetOsParametersList(list);

                ConsoleLogger.Log("[data import] parameters");
            }
            #endregion
        }

        private static void Adjustments() {
            Do.ParametersChangesPre();
            ConsoleLogger.Log("modules, services and os parameters ready");
        }

        private static void Users() {
            var manageMaster = new ManageMaster();
            manageMaster.Setup();
            UserConfiguration.Import();
            UserConfiguration.Set();
            ConsoleLogger.Log("users config ready");
        }

        private static void HostConfiguration_NameService() {
            Do.HostChanges();
            ConsoleLogger.Log("host configured");
            ConsoleLogger.Log("name service ready");
        }

        private static void Network() {
            Do.NetworkChanges();
            if(File.Exists("/cfg/antd/services/network.conf")) {
                File.Delete("/cfg/antd/services/network.conf");
            }
            if(File.Exists("/cfg/antd/services/network.conf.bck")) {
                File.Delete("/cfg/antd/services/network.conf.bck");
            }
            ConsoleLogger.Log("network ready");
        }

        private static void Firewall() {
            if(FirewallConfiguration.IsActive()) {
                FirewallConfiguration.Set();
            }
        }

        private static void Dhcpd() {
            DhcpdConfiguration.TryImport();
            if(DhcpdConfiguration.IsActive()) {
                DhcpdConfiguration.Set();
            }
        }

        private static void Bind() {
            BindConfiguration.TryImport();
            BindConfiguration.DownloadRootServerHits();
            if(BindConfiguration.IsActive()) {
                BindConfiguration.Set();
            }
        }

        private static void ApplySetupConfiguration() {
            Do.ParametersChangesPost();
            ConsoleLogger.Log("machine configured (apply setup.conf)");
        }

        private static void Nginx() {
            NginxConfiguration.TryImport();
            if(NginxConfiguration.IsActive()) {
                NginxConfiguration.Set();
            }
        }

        private static void Ssh() {
            if(SshdConfiguration.IsActive()) {
                SshdConfiguration.Set();
            }
            DirectoryWithAcl.CreateDirectory(Parameter.RootSsh, "755", "root", "wheel");
            DirectoryWithAcl.CreateDirectory(Parameter.RootSshMntCdrom, "755", "root", "wheel");
            if(!MountHelper.IsAlreadyMounted(Parameter.RootSsh)) {
                MountManagement.Dir(Parameter.RootSsh);
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
                FileWithAcl.AppendAllLines(authorizedKeysPath, new List<string> { $"{storedKey.KeyValue} {storedKey.RemoteUser}" }, "644", "root", "wheel");
                Bash.Execute($"chmod 600 {authorizedKeysPath}");
                Bash.Execute($"chown {storedKey.User}:{storedKey.User} {authorizedKeysPath}");
            }
            ConsoleLogger.Log("ssh ready");
        }

        private static void StartRssdp() {
            try {
                ServiceDiscovery.Rssdp.PublishThisDevice();
                ConsoleLogger.Log("[rssdp] published device");
            }
            catch(Exception) {
                //aaa
            }
        }

        private static void AntdUI() {
            UiService.Setup();
            ConsoleLogger.Log("antduisetup");
        }

        private static void Samba() {
            if(SambaConfiguration.IsActive()) {
                SambaConfiguration.Set();
            }
        }

        private static void Syslog() {
            if(SyslogNgConfiguration.IsActive()) {
                SyslogNgConfiguration.Set();
            }
        }

        private static void StorageZfs() {
            foreach(var pool in Zpool.ImportList().ToList()) {
                if(string.IsNullOrEmpty(pool))
                    continue;
                ConsoleLogger.Log($"pool {pool} imported");
                Zpool.Import(pool);
            }
            ConsoleLogger.Log("storage ready");
        }

        private static void Scheduler() {
            Timers.MoveExistingTimers();
            Timers.Setup();
            Timers.Import();
            Timers.Export();
            foreach(var zp in Zpool.List()) {
                Timers.Create(zp.Name.ToLower() + "snap", "hourly", $"/sbin/zfs snap -r {zp.Name}@${{TTDATE}}");
            }
            Timers.StartAll();
            new SnapshotCleanup().Start(new TimeSpan(6, 00, 00));
            new SyncTime().Start(new TimeSpan(0, 42, 00));
            new RemoveUnusedModules().Start(new TimeSpan(2, 15, 00));
            JobManager jobManager = new JobManager();
            jobManager.ExecuteAllJobs();
            ConsoleLogger.Log("scheduled events ready");
        }

        private static void Acl() {
            if(AclConfiguration.IsActive()) {
                AclConfiguration.Set();
                AclConfiguration.ScriptSetup();
            }
        }

        private static void Ca() {
            if(CaConfiguration.IsActive()) {
                CaConfiguration.Set();
            }
        }

        private static void HostInit() {
            var app = new AppConfiguration().Get();
            var port = app.AntdPort;
            var uri = $"http://localhost:{app.AntdPort}/";
            HOST = new NancyHost(new Uri(uri));
            HOST.Start();
            ConsoleLogger.Log("host ready");
            StaticConfiguration.DisableErrorTraces = false;
            ConsoleLogger.Log($"http port: {port}");
            ConsoleLogger.Log("[boot step] antd is running");
        }

        private static void Apps() {
            AppTarget.Setup();
            var apps = AppsConfiguration.Get().Apps;
            foreach(var mapp in apps) {
                var units = mapp.UnitLauncher;
                foreach(var unit in units) {
                    if(Systemctl.IsActive(unit) == false) {
                        Systemctl.Restart(unit);
                    }
                }
            }
            AppTarget.StartAll();
            ConsoleLogger.Log("apps ready");
        }

        private static void Sync_Gluster() {
            if(RsyncConfiguration.IsActive()) {
                RsyncConfiguration.Set();
            }
        }

        private static void StorageServer() {
            VfsConfiguration.SetDefaults();
            ThreadPool.QueueUserWorkItem(new WaitCallback((state) => {
                var srv = new StorageServer(VfsConfiguration.GetSystemConfiguration());
                try {
                    srv.Start();
                }
                catch(Exception) {
                      
                }
            }));
        }

        private static void Tor() {
            if(TorConfiguration.IsActive()) {
                TorConfiguration.Start();
            }
        }

        private static void Cluster() {
            VfsWatcher = new VfsWatcher();
            ClusterConfiguration.Prepare();
            Do.ClusterChanges();
            ConsoleLogger.Log("[cluster] active");
        }

        private static void Gluster() {
            if(GlusterConfiguration.IsActive()) {
                GlusterConfiguration.Start();
            }
        }

        private static void DirectoryWatchers() {
            DirectoryWatcherCluster.Start();
            DirectoryWatcherRsync.Start();
        }

        private static void CheckApplicationFileAcls() {
            var files = Directory.EnumerateFiles(Parameter.RepoApps, "*.squashfs.xz", SearchOption.AllDirectories);
            foreach(var file in files) {
                Bash.Execute($"chmod 644 {file}");
                Bash.Execute($"chown root:wheel {file}");
            }
            ConsoleLogger.Log("[check] app-file acl");
        }

        private static void CheckSystemComponents() {
            MachineInfo.CheckSystemComponents();
            ConsoleLogger.Log("[check] system components health");
        }

        private static void Test() {
        }
    }
}