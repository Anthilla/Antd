using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
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
using antdlib.Status;
using antdlib.Terminal;
using antdlib.Users;
using Microsoft.AspNet.SignalR;
using Nancy;
using Nancy.Owin;
using Owin;

namespace Antd {
    public class AntdBoot {

        /// <summary>
        /// 01
        /// </summary>
        public static void CheckOsIsRw() {
            Execute.RemounwRwOs();
        }

        /// <summary>
        /// 02
        /// </summary>
        public static void SetWorkingDirectories() {
            if (!AssemblyInfo.IsUnix)
                return;
            Mount.WorkingDirectories();
            ConsoleLogger.Log("working directories ready");
        }

        /// <summary>
        /// 03
        /// </summary>
        public static void SetCoreParameters() {
            CoreParametersConfig.WriteDefaults();
            ConsoleLogger.Log("antd core parameters ready");
        }

        /// <summary>
        /// 04
        /// </summary>
        public static void StartDatabase() {
            var databasePaths = new[] { CoreParametersConfig.GetDb() };
            foreach (var dbPath in databasePaths) {
                Directory.CreateDirectory(dbPath);
            }
            DeNSo.Configuration.BasePath = databasePaths;
            DeNSo.Configuration.EnableJournaling = true;
            DeNSo.Configuration.EnableDataCompression = false;
            DeNSo.Configuration.ReindexCheck = new TimeSpan(0, 1, 0);
            DeNSo.Configuration.EnableOperationsLog = false;
            DeNSo.Session.DefaultDataBase = Parameter.AntdCfgDatabaseName;
            DeNSo.Session.Start();
            ConsoleLogger.Log($"database directory: {string.Join(", ", databasePaths)}");
            ConsoleLogger.Log("database ready");
        }

        /// <summary>
        /// 05
        /// </summary>
        public static void CheckCertificate() {
            var certificate = CoreParametersConfig.GetCertificatePath();
            if (!File.Exists(certificate)) {
                File.Copy($"{Parameter.Resources}/certificate.pfx", certificate, true);
            }
            ConsoleLogger.Log("certificates ready");
        }

        /// <summary>
        /// 06
        /// </summary>
        public static void ReloadUsers() {
            if (!AssemblyInfo.IsUnix)
                return;
            SystemUser.Config.ResetPasswordForUserStoredInDb();
            ConsoleLogger.Log("users config ready");
        }

        /// <summary>
        /// 07
        /// </summary>
        public static void ReloadSsh() {
            if (!AssemblyInfo.IsUnix)
                return;
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
        }

        /// <summary>
        /// 08
        /// </summary>
        public static void SetMounts() {
            if (!AssemblyInfo.IsUnix)
                return;
            Mount.AllDirectories();
            ConsoleLogger.Log("mounts ready");
        }

        /// <summary>
        /// 09
        /// </summary>
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
                //todo rimuovere poi la riga qui sotto
                Directory.CreateDirectory($"/mnt/cdrom/DIRS/prova-{kernelRelease}");
                ConsoleLogger.Log($"Creating {moduleDir} to mount OS-modules");
                Directory.CreateDirectory(moduleDir);
                Terminal.Execute($"mount {module} {moduleDir}");
            }
            Terminal.Execute("systemctl restart systemd-modules-load.service");
            ConsoleLogger.Log("os mounts ready");
        }

        /// <summary>
        /// 10
        /// </summary>
        public static void LaunchDefaultOsConfiguration() {
            if (!AssemblyInfo.IsUnix)
                return;
            if (ConfigManagement.Exists) {
                ConfigManagement.ExecuteAll();
            }
            ConfigManagement.FromFile.ApplyForAll();
            ConsoleLogger.Log("default os configuration ready");
        }

        /// <summary>
        /// 11
        /// </summary>
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

        /// <summary>
        /// 12
        /// </summary>
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

        /// <summary>
        /// 13
        /// </summary>
        public static void CheckResolv() {
            if (!AssemblyInfo.IsUnix)
                return;
            if (File.Exists("/etc/resolv.conf"))
                return;
            Terminal.Execute("touch /etc/resolv.conf");
            ConsoleLogger.Log("resolv ready");
        }

        /// <summary>
        /// 14
        /// </summary>
        public static void SetFirewall() {
            if (!AssemblyInfo.IsUnix)
                return;
            FirewallLists.SetDefaultLists();
            NfTables.Export.ExportTemplate();
            ConsoleLogger.Log("firewall ready");
        }

        /// <summary>
        /// 15
        /// </summary>
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

        /// <summary>
        /// 16
        /// </summary>
        /// <param name="loadFromDatabase"></param>
        public static void StartScheduler(bool loadFromDatabase) {
            JobScheduler.Start(loadFromDatabase);
            ConsoleLogger.Log("scheduler ready");
        }

        /// <summary>
        /// 17
        /// </summary>
        /// <param name="watchDirectories"></param>
        /// <param name="isActive"></param>
        public static void StartDirectoryWatcher() {
            new DirectoryWatcher().StartWatching();
            ConsoleLogger.Log("directory watcher ready");
        }

        /// <summary>
        /// 18
        /// </summary>
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

        /// <summary>
        /// 19
        /// </summary>
        public static void DownloadDefaultRepoFiles() {
            if (!AssemblyInfo.IsUnix)
                return;
            var dir = $"{Parameter.RepoConfig}/database";
            Directory.CreateDirectory(dir);
            FileSystem.Download("http://www.internic.net/domain/named.root", $"{dir}/named.root");
            FileSystem.Download("http://www.internic.net/domain/root.zone", $"{dir}/root.zone");
            FileSystem.Download("http://standards-oui.ieee.org/oui.txt", $"{dir}/oui.txt");
        }

        public static void StartSignalR(IAppBuilder app, bool detailedErrors, bool isActive) {
            if (isActive) {
                var hubConfiguration = new HubConfiguration { EnableDetailedErrors = detailedErrors };
                app.MapSignalR(hubConfiguration);
                ConsoleLogger.Log("signalR ready");
            }
            else {
                ConsoleLogger.Log("signalR skipped");
            }
        }

        public static void StartNancy(IAppBuilder app) {
            StaticConfiguration.DisableErrorTraces = false;
            var options = new NancyOptions { EnableClientCertificates = true };
            app.UseNancy(options);
            ConsoleLogger.Log("nancyfx ready");
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