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
using Antd.Cloud;
using Antd.License;
using Antd.Overlay;
using Antd.Storage;
using Antd.SystemdTimer;
using Antd.Timer;
using Antd.Ui;
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
using anthilla.core;
using anthilla.core.Helpers;
using Antd.Info;
using EnumerableExtensions = anthilla.core.EnumerableExtensions;
using HostConfiguration = antdlib.config.HostConfiguration;
using Parameter = antdlib.common.Parameter;
using Random = anthilla.core.Random;
using System.Threading;
using Kvpbase;

namespace Antd {
    internal class Application {

        public static string KeyName = "antd";

        private static void Main() {
            ConsoleLogger.Log("[boot step] starting antd");
            var startTime = DateTime.Now;

            ConsoleLogger.Log("[boot step] core procedures");

            #region [    os Rw    ]
            Bash.Execute("mount -o remount,rw /", false);
            Bash.Execute("mount -o remount,rw /mnt/cdrom", false);
            #endregion

            #region [    Remove Limits    ]
            const string limitsFile = "/etc/security/limits.conf";
            if(File.Exists(limitsFile)) {
                if(!File.ReadAllText(limitsFile).Contains("root - nofile 1024000")) {
                    FileWithAcl.AppendAllLines(limitsFile, new[] { "root - nofile 1024000" }, "644", "root", "wheel");
                }
            }
            Bash.Execute("ulimit -n 1024000", false);
            #endregion

            #region [    Overlay Watcher    ]
            if(Directory.Exists(Parameter.Overlay)) {
                new OverlayWatcher().StartWatching();
                ConsoleLogger.Log("overlay watcher ready");
            }
            #endregion

            #region [    Working Directories    ]
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
                DirectoryWithAcl.CreateDirectory(moduleDir);
                Bash.Execute($"mount /mnt/cdrom/Kernel/active-modules {moduleDir}", false);
            }
            Bash.Execute("systemctl restart systemd-modules-load.service", false);
            MountManagement.AllDirectories();
            ConsoleLogger.Log("mounts ready");
            #endregion

            #region [    Application Keys    ]
            var ak = new AsymmetricKeys(Parameter.AntdCfgKeys, KeyName);
            var pub = ak.PublicKey;
            #endregion

            #region [    License Management    ]
            var appconfig = new AppConfiguration().Get();
            ConsoleLogger.Log($"[cloud] {appconfig.CloudAddress}");
            try {
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
            catch(Exception ex) {
                ConsoleLogger.Warn(ex.Message);
            }
            #endregion

            #region [    Secret    ]
            if(!File.Exists(Parameter.AntdCfgSecret)) {
                FileWithAcl.WriteAllText(Parameter.AntdCfgSecret, Secret.Gen(), "644", "root", "wheel");
            }

            if(string.IsNullOrEmpty(File.ReadAllText(Parameter.AntdCfgSecret))) {
                FileWithAcl.WriteAllText(Parameter.AntdCfgSecret, Secret.Gen(), "644", "root", "wheel");
            }
            #endregion

            #region [    JournalD    ]
            if(JournaldConfiguration.IsActive()) {
                JournaldConfiguration.Set();
            }
            #endregion

            #region [    Manage Networkd    ]
            const string networkdService = "systemd-networkd.service";
            if(Systemctl.IsActive(networkdService)) {
                Systemctl.Stop(networkdService);
            }
            if(Systemctl.IsEnabled(networkdService)) {
                Systemctl.Disable(networkdService);
            }
            #endregion

            #region [    Import Existing Configuration    ]

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
                var broadcast = "";
                try {
                    broadcast = Cidr.CalcNetwork(cif.StaticAddress, cif.StaticRange).Broadcast.ToString();
                }
                catch(Exception ex) {
                    ConsoleLogger.Error($"[data import] {ex.Message}");
                }
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
                    Id = Random.ShortGuid(),
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

            #endregion

            #region [    Adjustments    ]
            new Do().ParametersChangesPre();
            ConsoleLogger.Log("modules, services and os parameters ready");
            #endregion

            ConsoleLogger.Log("[boot step] procedures");

            #region [    Users    ]
            var manageMaster = new ManageMaster();
            manageMaster.Setup();
            UserConfiguration.Import();
            UserConfiguration.Set();
            ConsoleLogger.Log("users config ready");
            #endregion

            #region [    Host Configuration & Name Service    ]
            new Do().HostChanges();
            ConsoleLogger.Log("host configured");
            ConsoleLogger.Log("name service ready");
            #endregion

            #region [    Network    ]
            new Do().NetworkChanges();
            if(File.Exists("/cfg/antd/services/network.conf")) {
                File.Delete("/cfg/antd/services/network.conf");
            }
            if(File.Exists("/cfg/antd/services/network.conf.bck")) {
                File.Delete("/cfg/antd/services/network.conf.bck");
            }
            ConsoleLogger.Log("network ready");
            #endregion

            #region [    Firewall    ]
            if(FirewallConfiguration.IsActive()) {
                FirewallConfiguration.Set();
            }
            #endregion

            #region [    Dhcpd    ]
            DhcpdConfiguration.TryImport();
            if(DhcpdConfiguration.IsActive()) {
                DhcpdConfiguration.Set();
            }
            #endregion

            #region [    Bind    ]
            BindConfiguration.TryImport();
            BindConfiguration.DownloadRootServerHits();
            if(BindConfiguration.IsActive()) {
                BindConfiguration.Set();
            }
            #endregion

            ConsoleLogger.Log("[boot step] post procedures");

            #region [    Apply Setup Configuration    ]
            new Do().ParametersChangesPost();
            ConsoleLogger.Log("machine configured (apply setup.conf)");
            #endregion

            #region [    Nginx    ]
            NginxConfiguration.TryImport();
            if(NginxConfiguration.IsActive()) {
                NginxConfiguration.Set();
            }
            #endregion

            #region [    Ssh    ]
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
            #endregion

            #region [    Service Discovery    ]
            try {
                ServiceDiscovery.Rssdp.PublishThisDevice();
                //var rssdp = new ServiceDiscovery.Rssdp();
                //rssdp.BeginSearch();
                ConsoleLogger.Log("[rssdp] published device");
            }
            catch(Exception ex) {
                ConsoleLogger.Log($"[rssdp] {ex.Message}");
            }
            #endregion

            #region [    AntdUI    ]
            UiService.Setup();
            ConsoleLogger.Log("antduisetup");
            #endregion

            ConsoleLogger.Log("[boot step] managed procedures");

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
                ConsoleLogger.Log($"pool {pool} imported");
                Zpool.Import(pool);
            }
            ConsoleLogger.Log("storage ready");
            #endregion

            #region [    Scheduler    ]
            Timers.MoveExistingTimers();
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

            ConsoleLogger.Log("scheduled events ready");
            #endregion

            #region [    Acl    ]
            if(AclConfiguration.IsActive()) {
                AclConfiguration.Set();
                AclConfiguration.ScriptSetup();
            }
            #endregion

            #region [    Sync    ]
            if(GlusterConfiguration.IsActive()) {
                GlusterConfiguration.Launch();
            }
            if(RsyncConfiguration.IsActive()) {
                RsyncConfiguration.Set();
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
            foreach(var mapp in apps) {
                var units = mapp.UnitLauncher;
                foreach(var unit in units) {
                    if(Systemctl.IsActive(unit) == false) {
                        Systemctl.Restart(unit);
                    }
                }
            }
            //AppTarget.StartAll();
            ConsoleLogger.Log("apps ready");
            #endregion

            #region [    Host Init    ]
            var app = new AppConfiguration().Get();
            var port = app.AntdPort;
            var uri = $"http://localhost:{app.AntdPort}/";
            var host = new NancyHost(new Uri(uri));
            host.Start();
            ConsoleLogger.Log("host ready");
            StaticConfiguration.DisableErrorTraces = false;
            ConsoleLogger.Log($"http port: {port}");
            ConsoleLogger.Log("[boot step] antd is running");
            var startupTime = DateTime.Now - startTime;
            ConsoleLogger.Log($"loaded in: {startupTime}");
            if(!File.Exists("/cfg/antd/stats.txt")) {
                FileWithAcl.WriteAllText("/cfg/antd/stats.txt", "");
            }
            File.AppendAllLines("/cfg/antd/stats.txt", new[] { $"{startTime:yyyy MM dd HH:mm} - {startupTime}" });
            #endregion

            ConsoleLogger.Log("[boot step] working procedures");

            #region [    Tor    ]
            if(TorConfiguration.IsActive()) {
                TorConfiguration.Start();
            }
            #endregion

            #region [    Cluster    ]
            ClusterConfiguration.Prepare();
            new Do().ClusterChanges();
            ConsoleLogger.Log("[cluster] active");
            #endregion

            #region [    Directory Watchers    ]
            DirectoryWatcherCluster.Start();
            DirectoryWatcherRsync.Start();
            #endregion

            //#region [    Storage Server    ]
            //VfsConfiguration.SetDefaults();
            //new Thread(() => {
            //    try {
            //        var srv = new StorageServer(VfsConfiguration.GetSystemConfiguration());
            //        srv.Start();
            //    }
            //    catch(Exception ex) {
            //        ConsoleLogger.Error(ex.Message);
            //    }
            //}).Start();
            //#endregion

            //#region [    Cloud Send Uptime    ]
            //var csuTimer = new UpdateCloudInfo();
            //csuTimer.Start(1000 * 60 * 5);
            //#endregion

            //#region [    Cloud Fetch Commands    ]
            //var cfcTimer = new FetchRemoteCommand();
            //cfcTimer.Start(1000 * 60 * 2 + 330);
            //#endregion

            //#region [    Check System Components    ]
            //MachineInfo.CheckSystemComponents();
            //ConsoleLogger.Log("[check] system components health");
            //#endregion

            #region [    Check Units Location    ]
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
                var appsUnits = Directory.EnumerateFiles(Parameter.AppsUnits, "*.*", SearchOption.TopDirectoryOnly);
                foreach(var unit in appsUnits) {
                    var trueUnit = unit.Replace(Parameter.AntdUnits, Parameter.AnthillaUnits);
                    if(!File.Exists(trueUnit)) {
                        File.Copy(unit, trueUnit);
                    }
                    File.Delete(unit);
                    Bash.Execute($"ln -s {trueUnit} {unit}");
                }
                var applicativeUnits = Directory.EnumerateFiles(Parameter.ApplicativeUnits, "*.*", SearchOption.TopDirectoryOnly);
                foreach(var unit in applicativeUnits) {
                    var trueUnit = unit.Replace(Parameter.AntdUnits, Parameter.AnthillaUnits);
                    if(!File.Exists(trueUnit)) {
                        File.Copy(unit, trueUnit);
                    }
                    File.Delete(unit);
                    Bash.Execute($"ln -s {trueUnit} {unit}");
                }
            }
            anthillaUnits = Directory.EnumerateFiles(Parameter.AnthillaUnits, "*.*", SearchOption.TopDirectoryOnly).ToList();
            if(!anthillaUnits.Any()) {
                foreach(var unit in anthillaUnits) {
                    Bash.Execute($"chown root:wheel {unit}");
                    Bash.Execute($"chmod 644 {unit}");
                }
            }
            ConsoleLogger.Log("[check] units integrity");
            #endregion

            #region [    Check Application File Acls    ]
            var files = Directory.EnumerateFiles(Parameter.RepoApps, "*.squashfs.xz", SearchOption.AllDirectories);
            foreach(var file in files) {
                Bash.Execute($"chmod 644 {file}");
                Bash.Execute($"chown root:wheel {file}");
            }
            ConsoleLogger.Log("[check] app-file acl");
            #endregion

            //#region [    Cloud Send Uptime    ]
            //var ncc = new ConfigurationCheck();
            //ncc.Start(1000 * 60 * 2);
            //#endregion

            #region [    Test    ]
#if DEBUG
            Test();
#endif
            #endregion

            KeepAlive();
            ConsoleLogger.Log("antd is closing");
            host.Stop();
            Console.WriteLine("host shutdown");
        }

        private static void Test() {
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