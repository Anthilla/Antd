using System.IO;
using System.Linq;
using antdlib;
using antdlib.Apps;
using antdlib.common;
using antdlib.common.Helpers;
using Antd.Configuration;
using Antd.Database;
using Antd.MountPoint;
using Antd.Storage;
using Antd.SystemdTimer;

namespace Antd {
    public class AntdBoot {

        public void CheckOsIsRw() {
            Terminal.Execute($"{Parameter.Aossvc} reporemountrw");
        }

        public void ImportCommands() {
            Directory.CreateDirectory(Parameter.AntdCfgCommands);
            new CommandRepository().Import();
            new CommandValuesRepository().Import();
            ConsoleLogger.Log("commands imported");
        }

        public void ConfigureMachine() {
            MachineConfiguration.Set();
            ConsoleLogger.Log("machine configured");
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

        public void CheckCertificate() {
            var certificate = ApplicationSetting.CertificatePath();
            if (!File.Exists(certificate)) {
                File.Copy($"{Parameter.Resources}/certificate.pfx", certificate, true);
            }
            ConsoleLogger.Log("certificates ready");
        }

        public void ReloadUsers() {
            if (!Parameter.IsUnix)
                return;
            //SystemUser.Config.ResetPasswordForUserStoredInDb();
            ConsoleLogger.Log("users config ready");
        }

        public void ReloadSsh() {
            if (!Parameter.IsUnix)
                return;
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

        public void StartOverlayWatcher() {
            new OverlayWatcher().StartWatching();
            ConsoleLogger.Log("overlay watcher ready");
        }

        public void SetMounts() {
            if (!Parameter.IsUnix)
                return;
            Mount.AllDirectories();
            ConsoleLogger.Log("mounts ready");
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
                ConsoleLogger.Log($"Creating {moduleDir} to mount OS-modules");
                Directory.CreateDirectory(moduleDir);
                Terminal.Execute($"mount {module} {moduleDir}");
            }
            Terminal.Execute("systemctl restart systemd-modules-load.service");
            ConsoleLogger.Log("os mounts ready");
        }

        //public  void LaunchDefaultOsConfiguration() {
        //    if (!Parameter.IsUnix)
        //        return;
        //    ConsoleLogger.Log("default os configuration ready");
        //}

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

        //public void CheckResolv() {
        //    if (!Parameter.IsUnix)
        //        return;
        //    if (File.Exists("/etc/resolv.conf"))
        //        return;
        //    Terminal.Execute("touch /etc/resolv.conf");
        //    ConsoleLogger.Log("resolv ready");
        //}

        public void SetFirewall() {
            if (!Parameter.IsUnix)
                return;
            ConsoleLogger.Log("firewall ready");
        }

        public void ImportSystemInformation() {
            if (!Parameter.IsUnix)
                return;
            if (!new NetworkInterfaceRepository().GetAll().Any()) {
                new NetworkInterfaceRepository().Import();
            }
            ConsoleLogger.Log("network interfaces imported");
        }

        public void StartScheduler() {
            if (!Parameter.IsUnix)
                return;
            Timers.Setup();
            Timers.Import();
            Timers.Export();
            StartZpoolSnapshot();
            Timers.StartAll();
            ConsoleLogger.Log("scheduler ready");
        }

        private static void StartZpoolSnapshot() {
            var pools = Zpool.List();
            foreach (var zp in pools) {
                Timers.Create(zp.Name.ToLower() + "snap", "1", $"/sbin/zfs snap -r {zp.Name}@${{TTDATE}}");
            }
        }

        public void StartDirectoryWatcher() {
            new DirectoryWatcher().StartWatching();
            ConsoleLogger.Log("directory watcher ready");
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

        //public  void StartWebsocketServer() {
        //    var port = PortManagement.GetFirstAvailable(1234);
        //    ApplicationSetting.SetWebsocketPort(port.ToString());
        //    WebSocket.Start(Convert.ToInt32(ApplicationSetting.WebsocketPort()));
        //}
    }
}