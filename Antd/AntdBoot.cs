using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using antdlib;
using antdlib.Apps;
using antdlib.Boot;
using antdlib.Common;
using antdlib.Config;
using antdlib.Directories;
using antdlib.Firewall;
using antdlib.Log;
using antdlib.MountPoint;
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
        public static void CheckIfGlobalRepositoryIsWriteable() {
            if (!AssemblyInfo.IsUnix)
                return;
            var bootExtData = Terminal.Execute("blkid | grep BootExt");
            if (bootExtData.Length <= 0)
                return;
            var bootExtDevice = new Regex(".*:").Matches(bootExtData)[0].Value.Replace(":", "").Trim();
            var bootExtUid =
                new Regex("[\\s]UUID=\"[\\d\\w\\-]+\"").Matches(bootExtData)[0].Value.Replace("UUID=", "")
                    .Replace("\"", "")
                    .Trim();
            ConsoleLogger.Log($"global repository: checking");
            var mountResult = Terminal.Execute($"cat /proc/mounts | grep '{bootExtDevice} /mnt/cdrom '");
            if (mountResult.Length > 0) {
                if (mountResult.Contains("ro") && !mountResult.Contains("rw")) {
                    ConsoleLogger.Log($"is RO: remounting");
                    Terminal.Execute("Mount -o remount,rw,discard,noatime /mnt/cdrom");
                }
                else if (mountResult.Contains("rw") && !mountResult.Contains("ro")) {
                    ConsoleLogger.Log($"is RW: ok!");
                }
            }
            else {
                ConsoleLogger.Log($"is not mounted: IMPOSSIBLE");
            }
            ConsoleLogger.Log($"global repository: {bootExtDevice} - {bootExtUid}");
            ConsoleLogger.Log($"global repository: checked");
        }

        public static void SetWorkingDirectories() {
            if (!AssemblyInfo.IsUnix)
                return;
            Mount.WorkingDirectories();
            ConsoleLogger.Log($"working directories: checked");
        }

        public static void CheckCertificate() {
            var certificate = CoreParametersConfig.GetCertificatePath();
            if (!File.Exists(certificate)) {
                File.Copy($"{Folder.Resources}/certificate.pfx", certificate, true);
            }
        }

        public static void SetMounts() {
            if (!AssemblyInfo.IsUnix)
                return;
            Mount.AllDirectories();
            ConsoleLogger.Log($"mounts: checked");
        }

        public static void SetUsersMount(bool isActive) {
            if (!isActive || !AssemblyInfo.IsUnix)
                return;
            SystemUser.SetReady();
            SystemGroup.SetReady();
            ConsoleLogger.Log($"users Mount: checked");
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
                //todo rimuovere poi la riga qui sotto
                Directory.CreateDirectory($"/mnt/cdrom/DIRS/prova-{kernelRelease}");
                ConsoleLogger.Log($"Creating {moduleDir} to mount OS-modules");
                Directory.CreateDirectory(moduleDir);
                Terminal.Execute($"mount {module} {moduleDir}");
            }
            ConsoleLogger.Log($"os Mount: checked");
            Terminal.Execute("systemctl restart systemd-modules-load.service");
        }

        public static void SetWebsocketd() {
            if (!AssemblyInfo.IsUnix)
                return;
            ConsoleLogger.Log("installing websocketd");
            LoadOsConfiguration.LoadWebsocketd();
            ConsoleLogger.Log("websocketd installed");
        }

        public static void SetSystemdJournald() {
            if (!AssemblyInfo.IsUnix)
                return;
            ConsoleLogger.Log($"load journald");
            LoadOsConfiguration.LoadSystemdJournald();
        }

        public static void SetCoreParameters() {
            CoreParametersConfig.WriteDefaults();
            ConsoleLogger.Log($"antd core parameters: loaded");
        }

        public static void CheckSysctl(bool isActive) {
            if (!AssemblyInfo.IsUnix)
                return;
            if (isActive) {
                Sysctl.WriteConfig();
                Sysctl.LoadConfig();
                ConsoleLogger.Log($"sysctl: loaded");
            }
            else {
                ConsoleLogger.Log($"sysctl: skipped");
            }
        }

        public static void StartNetworkd() {
            if (AssemblyInfo.IsUnix) {
                Networkd.SetConfiguration();
            }
        }

        public static void StartScheduler(bool loadFromDatabase) {
            JobScheduler.Start(loadFromDatabase);
            ConsoleLogger.Log($"scheduler: loaded");
        }

        public static void StartDirectoryWatcher(string[] watchDirectories, bool isActive) {
            if (isActive && watchDirectories.Length > 0) {
                ConsoleLogger.Log($"directory watcher: enabled");
                foreach (var folder in watchDirectories) {
                    if (Directory.Exists(folder)) {
                        new DirectoryWatcher(folder).Watch();
                        ConsoleLogger.Log($"directory watcher: enabled for {0}", folder);
                    }
                    else {
                        ConsoleLogger.Log($"directory watcher: {0} does not exist", folder);
                    }
                }
            }
            else {
                ConsoleLogger.Log("directory watcher: skipped");
            }
        }

        public static void StartDatabase() {
            var applicationDatabasePath = CoreParametersConfig.GetDb();
            Directory.CreateDirectory(applicationDatabasePath);
            ConsoleLogger.Log("root info: application database path: {0}", applicationDatabasePath);
            if (Directory.Exists(applicationDatabasePath)) {
                var databases = new[] { applicationDatabasePath };
                DatabaseBoot.Start(databases, true);
                ConsoleLogger.Log("database: loaded");
            }
            else {
                ConsoleLogger.Warn("database: failed to load");
                ConsoleLogger.Warn("directory does not exist");
            }
        }

        public static void StartSignalR(IAppBuilder app, bool detailedErrors, bool isActive) {
            if (isActive) {
                var hubConfiguration = new HubConfiguration { EnableDetailedErrors = detailedErrors };
                app.MapSignalR(hubConfiguration);
                ConsoleLogger.Log($"signalR: loaded");
            }
            else {
                ConsoleLogger.Log($"signalR: skipped");
            }
        }

        public static void StartNancy(IAppBuilder app) {
            StaticConfiguration.DisableErrorTraces = false;
            var options = new NancyOptions { EnableClientCertificates = true };
            app.UseNancy(options);
            ConsoleLogger.Log($"nancy: loaded");
        }

        public static void TestWebDav(string uri, string path) {
            //NameValueCollection properties = new NameValueCollection();
            //properties["showDateTime"] = "true";
            //LogManager.Adapter = new ConsoleOutLoggerFactoryAdapter(properties);
            //WebDavServer server = new WebDavServer(new WebDavDiskStore(path));
            //server.Listener.Prefixes.Add(uri);
            //server.Start();
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
        }

        public static void ReloadSsh() {
            if (!AssemblyInfo.IsUnix)
                return;
            const string dir = "/etc/ssh";
            var mntDir = Mount.SetDirsPath(dir);
            ConsoleLogger.Log("ssh> set directories");
            if (!Directory.Exists(mntDir)) {
                Terminal.Execute($"cp -fR {dir} {mntDir}");
            }
            Mount.Umount(dir);
            Mount.Dir(dir);
            Terminal.Execute("ssh-keygen -A");
            Terminal.Execute("systemctl restart sshd.service");
        }

        public static void ReloadUsers() {
            if (!AssemblyInfo.IsUnix)
                return;
            SystemUser.Config.ResetPasswordForUserStoredInDb();
        }

        public static void CheckResolv() {
            if (!AssemblyInfo.IsUnix)
                return;
            if (!File.Exists("/etc/resolv.conf"))
                Terminal.Execute("touch /etc/resolv.conf");
        }

        public static void DownloadDefaultRepoFiles() {
            if (!AssemblyInfo.IsUnix)
                return;
            var dir = $"{Folder.RepoConfig}/database";
            Directory.CreateDirectory(dir);
            FileSystem.Download("http://www.internic.net/domain/named.root", $"{dir}/named.root");
            FileSystem.Download("http://www.internic.net/domain/root.zone", $"{dir}/root.zone");
            FileSystem.Download("http://standards-oui.ieee.org/oui.txt", $"{dir}/oui.txt");
        }

        public static void SetBootConfiguration() {
            if (!AssemblyInfo.IsUnix)
                return;
            if (ConfigManagement.Exists) {
                ConfigManagement.ExecuteAll();
            }
            ConfigManagement.FromFile.ApplyForAll();
        }

        public static void SetFirewall() {
            ConsoleLogger.Log("loading default values for firewall");
            FirewallLists.SetDefaultLists();
        }
    }
}