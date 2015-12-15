using System;
using System.IO;
using System.Linq;
using System.Threading;
using antdlib;
using antdlib.Antdsh;
using antdlib.Apps;
using antdlib.Boot;
using antdlib.Common;
using antdlib.Config;
using antdlib.Directories;
using antdlib.Firewall;
using antdlib.Info;
using antdlib.Log;
using antdlib.MountPoint;
using antdlib.Network;
using antdlib.Scheduler;
using antdlib.Terminal;
using antdlib.Users;

namespace Antd {
    public class AntdBoot {

        public static void CheckOsIsRw() {
            Execute.RemounwRwOs();
        }

        public static void SetWorkingDirectories() {
            if (!AssemblyInfo.IsUnix)
                return;
            Mount.WorkingDirectories();
            ConsoleLogger.Log("working directories ready");
        }

        public static void SetCoreParameters() {
            CoreParametersConfig.WriteDefaults();
            ConsoleLogger.Log("antd core parameters ready");
        }

        public static void StartDatabase() {
            var databaseName = Parameter.AntdCfgDatabaseName;
            var databasePaths = new[] { CoreParametersConfig.GetDb() };
            foreach (var dbPath in databasePaths) {
                Terminal.Execute($"mkdir -p {dbPath}");
                Terminal.Execute($"mkdir -p {dbPath}/{databaseName}");
                Directory.CreateDirectory(dbPath);
                Directory.CreateDirectory($"{dbPath}/{databaseName}");
            }
            DeNSo.Configuration.BasePath = databasePaths;
            DeNSo.Configuration.EnableJournaling = true;
            DeNSo.Configuration.EnableDataCompression = false;
            DeNSo.Configuration.ReindexCheck = new TimeSpan(0, 1, 0);
            DeNSo.Configuration.EnableOperationsLog = false;
            DeNSo.Session.DefaultDataBase = databaseName;
            DeNSo.Session.Start();
            var y = databasePaths.Length > 1 ? "ies" : "y";
            ConsoleLogger.Log($"database director{y}: {string.Join(", ", databasePaths)}");
            ConsoleLogger.Log("database ready");
        }

        public static void CheckCertificate() {
            var certificate = CoreParametersConfig.GetCertificatePath();
            if (!File.Exists(certificate)) {
                File.Copy($"{Parameter.Resources}/certificate.pfx", certificate, true);
            }
            ConsoleLogger.Log("certificates ready");
        }

        public static void ReloadUsers() {
            if (!AssemblyInfo.IsUnix)
                return;
            SystemUser.Config.ResetPasswordForUserStoredInDb();
            ConsoleLogger.Log("users config ready");
        }

        public static void ReloadSsh() {
            if (!AssemblyInfo.IsUnix)
                return;
            Terminal.Execute("mkdir -p /root/.ssh");
            if (!File.Exists(Parameter.AuthKeys)) {
                Terminal.Execute($"touch {Parameter.AuthKeys}");
            }
            const string dir = "/etc/ssh";
            var mntDir = Mount.SetDirsPath(dir);
            if (!Directory.Exists(mntDir)) {
                Terminal.Execute($"cp -fR {dir} {mntDir}");
            }
            Mount.Umount(dir);
            Mount.Dir(dir);
            Terminal.Execute("ssh-keygen -A");
            Terminal.Execute("systemctl restart sshd.service");
            ConsoleLogger.Log("ssh config ready");
            //SshConfig.Keys.PropagateKeys(new[] { "" }, new[] { "" });
        }

        public static void SetOverlayDirectories() {
            if (!AssemblyInfo.IsUnix)
                return;
            Mount.OverlayDirectories();
            ConsoleLogger.Log("overlay ready");
        }

        public static void SetMounts() {
            if (!AssemblyInfo.IsUnix)
                return;
            Mount.AllDirectories();
            ConsoleLogger.Log("mounts ready");
        }

        public static void SetOsMount() {
            if (!AssemblyInfo.IsUnix)
                return;
            if (Mount.IsAlreadyMounted("/mnt/cdrom/Kernel/active-firmware", "/lib64/firmware") == false) {
                Terminal.Execute($"mount {"/mnt/cdrom/Kernel/active-firmware"} {"/lib64/firmware"}");
            }
            const string module = "/mnt/cdrom/Kernel/active-modules";
            var kernelRelease = Terminal.Execute("uname -r").Trim();
            var linkedRelease = Terminal.Execute($"file {module}").Trim();
            if (Mount.IsAlreadyMounted(module) == false && linkedRelease.Contains(kernelRelease)) {
                var moduleDir = $"/lib64/modules/{kernelRelease}/";
                ConsoleLogger.Log($"Creating {moduleDir} to mount OS-modules");
                Directory.CreateDirectory(moduleDir);
                Terminal.Execute($"mount {module} {moduleDir}");
            }
            Terminal.Execute("systemctl restart systemd-modules-load.service");
            ConsoleLogger.Log("os mounts ready");
        }

        public static void LaunchDefaultOsConfiguration() {
            if (!AssemblyInfo.IsUnix)
                return;
            ConfigManagement.FromFile.ApplyForAll();
            ConsoleLogger.Log("default os configuration ready");
        }

        public static void SetWebsocketd() {
            if (!AssemblyInfo.IsUnix)
                return;
            var filePath = $"{Parameter.AntdCfg}/websocketd";
            if (File.Exists(filePath))
                return;
            File.Copy($"{Parameter.Resources}/websocketd", filePath);
            Terminal.Execute($"chmod 777 {filePath}");
            ConsoleLogger.Log("websocketd ready");
        }

        public static void SetSystemdJournald() {
            if (!AssemblyInfo.IsUnix)
                return;
            var file = $"{Parameter.RepoDirs}/{"FILE_etc_systemd_journald.conf"}";
            if (File.Exists(file)) {
                return;
            }
            File.Copy($"{Parameter.Resources}/FILE_etc_systemd_journald.conf", file);
            var realFileName = Mount.GetFilesPath("FILE_etc_systemd_journald.conf");
            if (Mount.IsAlreadyMounted(file, realFileName) == false) {
                Mount.File(realFileName);
            }
            Terminal.Execute("systemctl restart systemd-journald.service");
            ConsoleLogger.Log("journald config ready");
        }

        public static void CheckResolv() {
            if (!AssemblyInfo.IsUnix)
                return;
            if (File.Exists("/etc/resolv.conf"))
                return;
            Terminal.Execute("touch /etc/resolv.conf");
            ConsoleLogger.Log("resolv ready");
        }

        public static void SetFirewall() {
            if (!AssemblyInfo.IsUnix)
                return;
            FirewallLists.SetDefaultLists();
            NfTables.Export.ExportTemplate();
            ConsoleLogger.Log("firewall ready");
        }

        public static void ImportSystemInformation() {
            if (!AssemblyInfo.IsUnix)
                return;
            if (!NetworkInterface.GetAll().Any()) {
                NetworkInterface.ImportNetworkInterface();
            }
            if (!SystemInfo.GetAll().Any()) {
                SystemInfo.Import();
            }
            ConsoleLogger.Log("network interfaces imported");
        }

        public static void StartScheduler(bool loadFromDatabase) {
            JobScheduler.Start(loadFromDatabase);
            ConsoleLogger.Log("scheduler ready");
        }

        public static void StartDirectoryWatcher() {
            new DirectoryWatcher().StartWatching();
            ConsoleLogger.Log("directory watcher ready");
        }

        public static void LaunchApps() {
            if (!AssemblyInfo.IsUnix)
                return;
            var apps = Management.DetectApps();
            if (apps.Length > 0) {
                foreach (
                    var dir in
                        from app in apps
                        select Management.GetWantedDirectories(app)
                        into dirs
                        where dirs.Length > 0
                        from dir in dirs
                        select dir) {
                    Mount.Dir(dir);
                }
            }
            Thread.Sleep(10);
            AnthillaSp.SetApp();
            ConsoleLogger.Log("apps ready");
        }

        public static void DownloadDefaultRepoFiles() {
            if (!AssemblyInfo.IsUnix)
                return;
            var dir = $"{Parameter.RepoConfig}/database";
            Directory.CreateDirectory(dir);
            FileSystem.Download("http://www.internic.net/domain/named.root", $"{dir}/named.root");
            FileSystem.Download("http://www.internic.net/domain/root.zone", $"{dir}/root.zone");
            FileSystem.Download("http://standards-oui.ieee.org/oui.txt", $"{dir}/oui.txt");
        }

        public static void LoadCollectd() {
            var file = $"{Parameter.RepoDirs}/{"FILE_etc_collectd.conf"}";
            File.Copy($"{Parameter.Resources}/FILE_etc_collectd.conf", file);
            var realFileName = Mount.GetFilesPath("FILE_etc_collectd.conf");
            if (Mount.IsAlreadyMounted(file, realFileName) == false) {
                Mount.File(realFileName);
            }
            Terminal.Execute("systemctl restart collectd.service");
        }

        public static void LoadWpaSupplicant() {
            var file = $"{Parameter.RepoDirs}/{"FILE_etc_wpa_supplicant_wpa_suplicant.conf"}";
            File.Copy($"{Parameter.Resources}/FILE_etc_wpa_supplicant_wpa_suplicant.conf", file);
            var realFileName = Mount.GetFilesPath("FILE_etc_wpa_supplicant_wpa__suplicant.conf");
            if (Mount.IsAlreadyMounted(file, realFileName) == false) {
                Mount.File(realFileName);
            }
            Terminal.Execute("systemctl restart wpa_supplicant.service");
        }
    }
}