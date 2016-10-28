using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using antdlib;
using antdlib.common;
using antdlib.common.Helpers;
using antdlib.Systemd;
using antdlib.views;
using Antd.Apps;
using Antd.Avahi;
using Antd.Configuration;
using Antd.Database;
using Antd.Firewall;
using Antd.Gluster;
using Antd.MountPoint;
using Antd.Storage;
using Antd.SystemdTimer;
using Antd.Timer;
using Antd.Users;
using RaptorDB;

namespace Antd {
    public class AntdBoot {

        public void RemoveLimits() {
            if (!Parameter.IsUnix)
                return;
            const string limitsFile = "/etc/security/limits.conf";
            if (File.Exists(limitsFile)) {
                var t = File.ReadAllText(limitsFile);
                if (!t.Contains("root - nofile 1024000")) {
                    File.AppendAllLines(limitsFile, new[] { "root - nofile 1024000" });
                }
            }

            Bash.Execute("ulimit -n 1024000");
            ConsoleLogger.Log("removed open files limit");
        }

        public void StartOverlayWatcher() {
            new OverlayWatcher().StartWatching();
            ConsoleLogger.Log("overlay watcher ready");
        }

        public void CheckOsIsRw() {
            Bash.Execute($"{Parameter.Aossvc} reporemountrw");
        }

        public void SetWorkingDirectories() {
            if (!Parameter.IsUnix)
                return;
            Mount.WorkingDirectories();
            ConsoleLogger.Log("working directories ready");
        }

        public void SetCoreParameters() {
            ApplicationSetting.WriteDefaults();
            ConsoleLogger.Log("antd core parameters ready");
        }

        public RaptorDB.RaptorDB StartDatabase() {
            var path = ApplicationSetting.DatabasePath();
            var database = RaptorDB.RaptorDB.Open(path);
            Global.RequirePrimaryView = false;
            Global.BackupCronSchedule = null;
            Global.EnableWebStudio = true;
            Global.WebStudioPort = 9999;
            Global.LocalOnlyWebStudio = false;
            Global.FlushStorageFileImmediately = true;
            database.RegisterView(new ApplicationView());
            database.RegisterView(new AuthorizedKeysView());
            database.RegisterView(new BootModuleLoadView());
            database.RegisterView(new BootServiceLoadView());
            database.RegisterView(new BootOsParametersLoadView());
            database.RegisterView(new CommandView());
            database.RegisterView(new CommandValuesView());
            database.RegisterView(new CustomTableView());
            database.RegisterView(new FirewallListView());
            database.RegisterView(new TimerView());
            database.RegisterView(new LogView());
            database.RegisterView(new MountView());
            database.RegisterView(new NetworkInterfaceView());
            database.RegisterView(new ObjectView());
            database.RegisterView(new RsyncView());
            database.RegisterView(new UserClaimView());
            database.RegisterView(new UserView());
            database.RegisterView(new MacAddressView());
            database.RegisterView(new SambaConfigView());
            database.RegisterView(new SyslogView());
            database.RegisterView(new DhcpConfigView());
            database.RegisterView(new DhcpServerOptionsView());
            database.RegisterView(new DhcpServerClassView());
            database.RegisterView(new DhcpServerSubnetView());
            database.RegisterView(new DhcpServerPoolView());
            database.RegisterView(new DhcpServerReservationView());
            database.RegisterView(new BindConfigView());
            database.RegisterView(new NftView());
            ConsoleLogger.Log("database ready");
            return database;
        }

        public void PrepareConfiguration() {
            MachineConfiguration.Set();
            ConsoleLogger.Log("configuration prepared");
        }

        public void SetOsMount() {
            if (!Parameter.IsUnix)
                return;
            if (Mounts.IsAlreadyMounted("/mnt/cdrom/Kernel/active-firmware", "/lib64/firmware") == false) {
                Bash.Execute($"mount {"/mnt/cdrom/Kernel/active-firmware"} {"/lib64/firmware"}");
            }
            const string module = "/mnt/cdrom/Kernel/active-modules";
            var kernelRelease = Bash.Execute("uname -r").Trim();
            var linkedRelease = Bash.Execute($"file {module}").Trim();
            if (Mounts.IsAlreadyMounted(module) == false && linkedRelease.Contains(kernelRelease)) {
                var moduleDir = $"/lib64/modules/{kernelRelease}/";
                ConsoleLogger.Log($"creating {moduleDir} to mount OS-modules");
                Directory.CreateDirectory(moduleDir);
                Bash.Execute($"mount {module} {moduleDir}");
            }
            Bash.Execute("systemctl restart systemd-modules-load.service");
            ConsoleLogger.Log("os mounts ready");
        }

        public void SetOsParametersLocal() {
            if (!Parameter.IsUnix)
                return;
            var kvps = new BootOsParametersLoadRepository().Retrieve();
            if (kvps != null) {
                foreach (var kvp in kvps) {
                    var file = kvp.Key;
                    var value = kvp.Value;
                    if (!string.IsNullOrEmpty(file) && !string.IsNullOrEmpty(value)) {
                        File.WriteAllText(file, value);
                    }
                }
            }
            ConsoleLogger.Log("os local parameters ready");
        }

        public void LoadModules() {
            if (!Parameter.IsUnix)
                return;
            var modules = new BootModuleLoadRepository().Retrieve();
            if (modules != null) {
                foreach (var module in modules) {
                    Bash.Execute($"modprobe {module}");
                }
            }
            ConsoleLogger.Log("modules ready");
        }

        public void SetMounts() {
            if (!Parameter.IsUnix)
                return;
            Mount.AllDirectories();
            ConsoleLogger.Log("mounts ready");
        }

        public void ImportCommands() {
            Directory.CreateDirectory(Parameter.AntdCfgCommands);
            new CommandRepository().Import();
            new CommandValuesRepository().Import();
            ConsoleLogger.Log("commands imported");
        }

        private readonly UserRepository _userRepository = new UserRepository();
        public void ReloadUsers() {
            if (!Parameter.IsUnix)
                return;
            var sysUser = _userRepository.Import();
            foreach (var user in _userRepository.GetAll()) {
                if (!sysUser.ContainsKey(user.Alias)) {
                    SystemUser.Create(user.Alias);
                }
                if (!string.IsNullOrEmpty(user.Password)) {
                    SystemUser.SetPassword(user.Alias, user.Password);
                }
            }
            ConsoleLogger.Log("users config ready");
        }

        public void CommandExecuteLocal() {
            if (!Parameter.IsUnix)
                return;
            SetupConfiguration.Set();
            ConsoleLogger.Log("machine configured");
        }

        public void ImportNetworkConfiguration() {
            if (!Parameter.IsUnix)
                return;
            if (!new NetworkInterfaceRepository().GetAll().Any()) {
                new NetworkInterfaceRepository().Import();
            }
            ConsoleLogger.Log("network interfaces imported");
        }

        public void Ssh() {
            if (!Parameter.IsUnix) {
                return;
            }
            var storedKeyRepo = new AuthorizedKeysRepository();
            var storedKeys = storedKeyRepo.GetAll();
            foreach (var storedKey in storedKeys) {
                var home = storedKey.User == "root" ? "/root/.ssh" : $"/home/{storedKey.User}/.ssh";
                var authorizedKeysPath = $"{home}/authorized_keys";
                if (!File.Exists(authorizedKeysPath)) {
                    File.Create(authorizedKeysPath);
                }
                var line = $"{storedKey.KeyValue} {storedKey.RemoteUser}";
                File.AppendAllLines(authorizedKeysPath, new List<string> { line });
                Bash.Execute($"chmod 600 {authorizedKeysPath}");
                Bash.Execute($"chown {storedKey.User}:{storedKey.User} {authorizedKeysPath}");
            }
        }

        public void CommandExecuteNetwork() {
            if (!Parameter.IsUnix)
                return;
            ConsoleLogger.Log("network configuration ready");
        }

        //todo
        public void SetOsParametersNetwork() {
            if (!Parameter.IsUnix)
                return;
            ConsoleLogger.Log("os parameters ready");
        }

        public void SetFirewall() {
            if (!Parameter.IsUnix)
                return;
            NfTables.Setup();
            NfTables.ReloadConfiguration();
            NfTables.Import();
            NfTables.Export();
            NfTables.ReloadConfiguration();
            ConsoleLogger.Log("firewall ready");
        }

        public void LoadServices() {
            if (!Parameter.IsUnix)
                return;
            var services = new BootServiceLoadRepository().Retrieve();
            if (services != null) {
                foreach (var service in services) {
                    if (Systemctl.IsActive(service) == false) {
                        Systemctl.Restart(service);
                    }
                }
            }
            ConsoleLogger.Log("services ready");
        }

        public void SetSyslogNg() {
            if (!Parameter.IsUnix)
                return;
            var s = SyslogConfiguration.Set();
            if (s) {
                ConsoleLogger.Log("syslog ready");
            }
        }

        public void InitAvahi() {
            if (!Parameter.IsUnix)
                return;
            const string avahiServicePath = "/etc/avahi/services/antd.service";
            var xml = AvahiCustomXml.Generate(ApplicationSetting.HttpPort());
            if (File.Exists(avahiServicePath)) {
                File.Delete(avahiServicePath);
            }
            File.WriteAllLines(avahiServicePath, xml);
            Bash.Execute("chmod 755 /etc/avahi/services");
            Bash.Execute($"chmod 644 {avahiServicePath}");
            Systemctl.Restart("avahi-daemon.service");
            Systemctl.Restart("avahi-daemon.socket");
            ConsoleLogger.Log("avahi ready");
        }

        public void ImportPools() {
            if (!Parameter.IsUnix)
                return;
            var pools = Zpool.ImportList().ToList();
            foreach (var pool in pools) {
                if (!string.IsNullOrEmpty(pool)) {
                    ConsoleLogger.Log($"pool {pool} imported");
                    Zpool.Import(pool);
                }
            }
            if (pools.Count > 0) {
                ConsoleLogger.Log("pools imported");
            }
        }

        public void StartScheduler() {
            if (!Parameter.IsUnix)
                return;
            Timers.CleanUp();
            Timers.Setup();
            Timers.Import();
            Timers.Export();

            var pools = Zpool.List();
            foreach (var zp in pools) {

                Timers.Create(zp.Name.ToLower() + "snap", "hourly", $"/sbin/zfs snap -r {zp.Name}@${{TTDATE}}");
            }

            Timers.StartAll();
            ConsoleLogger.Log("scheduler ready");
        }

        public void InitGlusterfs() {
            if (!Parameter.IsUnix)
                return;
            GlusterConfiguration.Set();
            if (GlusterConfiguration.IsConfigured) {
                GlusterConfiguration.Start();
                GlusterConfiguration.Launch();
            }
            ConsoleLogger.Log("glusterfs ready");
        }

        public void StartDirectoryWatcher() {
            new DirectoryWatcher().StartWatching();
            ConsoleLogger.Log("directory watcher ready");
        }

        public void LaunchInternalTimers() {
            if (!Parameter.IsUnix)
                return;
            SnapshotCleanup.Start(new TimeSpan(2, 00, 00));
            ConsoleLogger.Log("internal timers ready");
        }

        private readonly ApplicationRepository _applicationRepository = new ApplicationRepository();

        public void LaunchApps() {
            if (!Parameter.IsUnix)
                return;
            AppTarget.Setup();
            var apps = _applicationRepository.GetAll().Select(_ => new ApplicationModel(_)).ToList();
            foreach (var app in apps) {
                var units = app.UnitLauncher;
                foreach (var unit in units) {
                    if (Systemctl.IsActive(unit) == false) {
                        Systemctl.Restart(unit);
                    }
                }
            }
            //AppTarget.StartAll();
            ConsoleLogger.Log("apps ready");
        }

        #region Unused Configuration
        public void SetWebsocketd() {
            if (!Parameter.IsUnix)
                return;
            var filePath = $"{Parameter.AntdCfg}/websocketd";
            if (File.Exists(filePath))
                return;
            File.Copy($"{Parameter.Resources}/websocketd", filePath);
            Bash.Execute($"chmod 777 {filePath}");
            ConsoleLogger.Log("websocketd ready");
        }

        public void SetSystemdJournald() {
            if (!Parameter.IsUnix)
                return;
            var file = $"{Parameter.RepoDirs}/{"FILE_etc_systemd_journald.conf"}";
            if (File.Exists(file)) {
                return;
            }
            File.Copy($"{Parameter.Resources}/FILE_etc_systemd_journald.conf", file);
            var realFileName = Mounts.GetFilesPath("FILE_etc_systemd_journald.conf");
            if (Mounts.IsAlreadyMounted(file, realFileName) == false) {
                Mount.File(realFileName);
            }
            Bash.Execute("systemctl restart systemd-journald.service");
            ConsoleLogger.Log("journald config ready");
        }

        public void LoadCollectd() {
            var file = $"{Parameter.RepoDirs}/{"FILE_etc_collectd.conf"}";
            File.Copy($"{Parameter.Resources}/FILE_etc_collectd.conf", file);
            var realFileName = Mounts.GetFilesPath("FILE_etc_collectd.conf");
            if (Mounts.IsAlreadyMounted(file, realFileName) == false) {
                Mount.File(realFileName);
            }
            Bash.Execute("systemctl restart collectd.service");
        }

        public void LoadWpaSupplicant() {
            var file = $"{Parameter.RepoDirs}/{"FILE_etc_wpa_supplicant_wpa_suplicant.conf"}";
            File.Copy($"{Parameter.Resources}/FILE_etc_wpa_supplicant_wpa_suplicant.conf", file);
            var realFileName = Mounts.GetFilesPath("FILE_etc_wpa_supplicant_wpa__suplicant.conf");
            if (Mounts.IsAlreadyMounted(file, realFileName) == false) {
                Mount.File(realFileName);
            }
            Bash.Execute("systemctl restart wpa_supplicant.service");
        }
        #endregion
    }
}