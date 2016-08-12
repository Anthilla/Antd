using System.Data.SqlTypes;
using System.IO;
using System.Linq;
using antdlib;
using antdlib.Apps;
using antdlib.common;
using antdlib.common.Helpers;
using antdlib.Systemd;
using antdlib.views;
using Antd.Configuration;
using Antd.Database;
using Antd.MountPoint;
using Antd.Storage;
using Antd.SystemdTimer;
using RaptorDB;

namespace Antd {
    public class AntdBoot {

        public void RemoveLimits() {
            const int openFileLimit = 1024000;
            var checkLimit = Terminal.Execute("ulimit -a | grep 'open files' | awk '{print $4}'");
            if (checkLimit != openFileLimit.ToString()) {
                Terminal.Execute("ulimit -Hn 1024000");
                Terminal.Execute("ulimit -Sn 1024000");
                ConsoleLogger.Log("removed open files limit");
            }
        }

        public void StartOverlayWatcher() {
            new OverlayWatcher().StartWatching();
            ConsoleLogger.Log("overlay watcher ready");
        }

        public void CheckOsIsRw() {
            Terminal.Execute($"{Parameter.Aossvc} reporemountrw");
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
            database.RegisterView(new SshKeyView());
            database.RegisterView(new MacAddressView());
            database.RegisterView(new SambaConfigView());
            database.RegisterView(new DhcpConfigView());
            database.RegisterView(new BindConfigView());
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
                Terminal.Execute($"mount {"/mnt/cdrom/Kernel/active-firmware"} {"/lib64/firmware"}");
            }
            const string module = "/mnt/cdrom/Kernel/active-modules";
            var kernelRelease = Terminal.Execute("uname -r").Trim();
            var linkedRelease = Terminal.Execute($"file {module}").Trim();
            if (Mounts.IsAlreadyMounted(module) == false && linkedRelease.Contains(kernelRelease)) {
                var moduleDir = $"/lib64/modules/{kernelRelease}/";
                ConsoleLogger.Log($"creating {moduleDir} to mount OS-modules");
                Directory.CreateDirectory(moduleDir);
                Terminal.Execute($"mount {module} {moduleDir}");
            }
            Terminal.Execute("systemctl restart systemd-modules-load.service");
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
                    File.WriteAllText(file, value);
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
                    Terminal.Execute($"modprobe {module}");
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

        public void ReloadUsers() {
            if (!Parameter.IsUnix)
                return;
            //SystemUser.Config.ResetPasswordForUserStoredInDb();
            ConsoleLogger.Log("users config ready");
        }

        public void CommandExecuteLocal() {
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
            if (!Parameter.IsUnix)
                return;
            new SshKeyRepository().Import();
            ConsoleLogger.Log("ssh keys imported");

            Terminal.Execute("mkdir -p /root/.ssh");
            if (!File.Exists(Parameter.AuthKeys)) {
                Terminal.Execute($"touch {Parameter.AuthKeys}");
            }
            var mntDir = Mounts.SetDirsPath(Parameter.EtcSsh);
            if (!Directory.Exists(mntDir)) {
                Terminal.Execute($"cp -fR {Parameter.EtcSsh} {mntDir}");
            }
            if (Mounts.IsAlreadyMounted(Parameter.EtcSsh)) {
                Terminal.Execute("ssh-keygen -A");
                Terminal.Execute("systemctl restart sshd.service");
                ConsoleLogger.Log("ssh config ready");
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

        public void StartScheduler() {
            if (!Parameter.IsUnix)
                return;
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

        public void StartDirectoryWatcher() {
            new DirectoryWatcher().StartWatching();
            ConsoleLogger.Log("directory watcher ready");
        }

        public void CheckCertificate() {
            var certificate = ApplicationSetting.CertificatePath();
            if (!File.Exists(certificate)) {
                File.Copy($"{Parameter.Resources}/certificate.pfx", certificate, true);
            }
            ConsoleLogger.Log("certificates ready");
        }

        public void LaunchApps() {
            if (!Parameter.IsUnix)
                return;
            var apps = Management.DetectApps();
            foreach (var app in apps) {
                var dirs = Management.GetWantedDirectories(app);
                foreach (var dir in dirs) {
                    Mount.Dir(dir);
                }
            }
            AnthillaSp.SetApp();
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
            Terminal.Execute($"chmod 777 {filePath}");
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
            Terminal.Execute("systemctl restart systemd-journald.service");
            ConsoleLogger.Log("journald config ready");
        }

        public void SetFirewall() {
            if (!Parameter.IsUnix)
                return;
            ConsoleLogger.Log("firewall ready");
        }

        public void LoadCollectd() {
            var file = $"{Parameter.RepoDirs}/{"FILE_etc_collectd.conf"}";
            File.Copy($"{Parameter.Resources}/FILE_etc_collectd.conf", file);
            var realFileName = Mounts.GetFilesPath("FILE_etc_collectd.conf");
            if (Mounts.IsAlreadyMounted(file, realFileName) == false) {
                Mount.File(realFileName);
            }
            Terminal.Execute("systemctl restart collectd.service");
        }

        public void LoadWpaSupplicant() {
            var file = $"{Parameter.RepoDirs}/{"FILE_etc_wpa_supplicant_wpa_suplicant.conf"}";
            File.Copy($"{Parameter.Resources}/FILE_etc_wpa_supplicant_wpa_suplicant.conf", file);
            var realFileName = Mounts.GetFilesPath("FILE_etc_wpa_supplicant_wpa__suplicant.conf");
            if (Mounts.IsAlreadyMounted(file, realFileName) == false) {
                Mount.File(realFileName);
            }
            Terminal.Execute("systemctl restart wpa_supplicant.service");
        }
        #endregion
    }
}