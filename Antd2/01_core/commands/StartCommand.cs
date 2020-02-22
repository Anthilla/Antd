using Antd2.cmds;
using Antd2.Configuration;
using Antd2.Jobs;
using Antd2.Web;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Bash = Antd2.cmds.Bash;
using Systemctl = Antd2.cmds.Systemctl;
using antd.core;
using System.Collections.Generic;
using Antd2.Storage;

#if NETCOREAPP
using Antd2.Init;
#endif

namespace Antd2 {
    public class StartCommand {

        public static JobManager Scheduler;
        public static Stopwatch STOPWATCH;
#if NETCOREAPP
        public static ServiceInit ServiceInit;
#endif

        public static readonly IDictionary<string, WebDavServer> WebdavInstances = new Dictionary<string, WebDavServer>();
        public static readonly IDictionary<string, bool> WebdavStatus = new Dictionary<string, bool>();


        public static void Start(string[] args) {
            STOPWATCH = new Stopwatch();
            STOPWATCH.Start();
            Scheduler = new JobManager();

            OsReadAndWrite();
            RemoveLimits();
            MountWorkingDirectories();
            CreateWorkingDirectories();

            Console.WriteLine("[conf] load antd conf");
            ConfigManager.Config.Reload();
            if (ConfigManager.Config.Saved == null) {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"[conf] missing file configuration at {ConfigManager.Config.TomlPath}!");
                Console.ForegroundColor = ConsoleColor.White;
                Environment.Exit(1);
                return;
            }

            Console.WriteLine($"[uid] {ConfigManager.Config.Saved.Host.Uid}");

            Init();

            CheckDefaultStaticFiles();

            Users();

            Time();
            CheckUnitsLocation();
            Mounts();
            Hostname();
            GenerateSecret();
            //License();

            SetServices();
            SetModules();
            SetParameters();

            SetRoutingTables();
            SetNetwork();
            SetRoutingRules();

            ManageSsh();

            SetDnsServer();

            StorageZfs();
            Apps();
            ApplySetupCommands();

            LaunchJobs();

            StartWebdavInstances();

            StartWebserver();

            Console.WriteLine($"antd started in: {STOPWATCH.ElapsedMilliseconds} ms");
        }

#if NETCOREAPP
        private static void Init() {
            //if (ServiceInit == null) {
            //    ServiceInit = new ServiceInit();
            //}
            //Directory.CreateDirectory("/var/run/sharpinit");
            //if (File.Exists("/var/run/sharpinit/sharpinit.sock")) {
            //    File.Delete("/var/run/sharpinit/sharpinit.sock");
            //}
            //ServiceInit.Start();
        }
#endif
#if NETFRAMEWORK
        private static void Init() {
            Console.WriteLine("Init not supported by .net framework");
        }
#endif

        private static void CheckDefaultStaticFiles() {
            if (Application.IsUnix == false) { return; }

#if NETCORE
            var etcIssueFile = "./Templates/TPL_etc_issue";
            if (File.Exists(etcIssueFile)) {
                Console.WriteLine("copy /etc/issue");
                File.Copy(etcIssueFile, "/etc/issue");
                Bash.Do("dos2unix /etc/issue");
            }

            var etcBashrcFile = "./Templates/TPL_etc_bashrc";
            if (File.Exists(etcBashrcFile)) {
                Console.WriteLine("copy .bashrc");
                File.Copy(etcBashrcFile, "/root/.bashrc");
                Bash.Do("dos2unix /root/.bashrc");
                File.Copy(etcBashrcFile, "/home/visor/.bashrc");
                Bash.Do("dos2unix /home/visor/.bashrc");
            }

            var etcMotdFile = "./Templates/TPL_etc_motd";
            if (File.Exists(etcMotdFile)) {
                Console.WriteLine("copy /etc/motd");
                File.Copy(etcMotdFile, "/etc/motd");
                Bash.Do("dos2unix /etc/motd");
            }
#endif

        }

        private static void OsReadAndWrite() {
            if (Application.IsUnix == false) { return; }
#if NETFRAMEWORK
            Bash.Do("mount -o remount,rw,noatime /");
            Bash.Do("mount -o remount,rw,discard,noatime /mnt/cdrom");
#endif
        }

        private static void RemoveLimits() {
            if (Application.IsUnix == false) { return; }
            const string limitsFile = "/etc/security/limits.conf";
            if (File.Exists(limitsFile)) {
                if (!File.ReadAllText(limitsFile).Contains("root - nofile 1024000")) {
                    File.AppendAllLines(limitsFile, new[] { "root - nofile 1024000" });
                }
            }
            Bash.Do("ulimit -n 1024000");
        }

        private static void CreateWorkingDirectories() {
            Directory.CreateDirectory("/Antd");
            Directory.CreateDirectory("/Data");
            Directory.CreateDirectory(Const.AntdCfg);
            Directory.CreateDirectory(Const.AntdCfgRestore);
            Directory.CreateDirectory(Const.AntdCfgConf);
            Directory.CreateDirectory(Const.AntdCfgKeys);
            Directory.CreateDirectory(Const.AntdCfgVfs);
            Directory.CreateDirectory(Const.AntdCfgLog);
            Directory.CreateDirectory(Const.AntdCfgSetup);
            if (!File.Exists($"{Const.AntdCfgSetup}/setup.conf")) {
                File.WriteAllText($"{Const.AntdCfgSetup}/setup.conf", "echo Hello World!");
            }
        }

        private static void MountWorkingDirectories() {
            if (Application.IsUnix == false) { return; }
            Mount.WorkingDirectories();
        }

        private static void Time() {
            if (Application.IsUnix == false) { return; }
            //Scheduler.ExecuteJob<SyncLocalClockJob>();
            if (ConfigManager.Config.Saved.Time.EnableNtpSync &&
                ConfigManager.Config.Saved.Time.NtpServer.Length > 0 &&
                !string.IsNullOrEmpty(ConfigManager.Config.Saved.Time.NtpServer[0])) {

                Ntpdate.SyncFromRemoteServer(ConfigManager.Config.Saved.Time.NtpServer[0]);
            }
            if (!string.IsNullOrEmpty(ConfigManager.Config.Saved.Time.Timezone)) {
                Timedatectl.SetTimezone(ConfigManager.Config.Saved.Time.Timezone);
            }
            Console.WriteLine("[time] ready");
        }

        private static void Mounts() {
            if (Application.IsUnix == false) { return; }
            Mount.Set();
            Console.WriteLine("[mnt] ready");
        }

        private static void CheckUnitsLocation() {
            if (Application.IsUnix == false) { return; }
            if (!Directory.Exists(Const.AnthillaUnits)) { return; }
            if (!Directory.Exists(Const.AntdUnits)) { return; }
            var anthillaUnits = Directory.EnumerateFiles(Const.AnthillaUnits, "*.*", SearchOption.TopDirectoryOnly);
            if (!anthillaUnits.Any()) {
                var antdUnits = Directory.EnumerateFiles(Const.AntdUnits, "*.*", SearchOption.TopDirectoryOnly);
                foreach (var unit in antdUnits) {
                    var trueUnit = unit.Replace(Const.AntdUnits, Const.AnthillaUnits);
                    if (!File.Exists(trueUnit)) {
                        File.Copy(unit, trueUnit);
                    }
                    File.Delete(unit);
                    Bash.Do($"ln -s {trueUnit} {unit}");
                }
                var kernelUnits = Directory.EnumerateFiles(Const.KernelUnits, "*.*", SearchOption.TopDirectoryOnly);
                foreach (var unit in kernelUnits) {
                    var trueUnit = unit.Replace(Const.KernelUnits, Const.AnthillaUnits);
                    if (!File.Exists(trueUnit)) {
                        File.Copy(unit, trueUnit);
                    }
                    File.Delete(unit);
                    Bash.Do($"ln -s {trueUnit} {unit}");
                }
                var applicativeUnits = Directory.EnumerateFiles(Const.ApplicativeUnits, "*.*", SearchOption.TopDirectoryOnly);
                foreach (var unit in applicativeUnits) {
                    var trueUnit = unit.Replace(Const.ApplicativeUnits, Const.AnthillaUnits);
                    if (!File.Exists(trueUnit)) {
                        File.Copy(unit, trueUnit);
                    }
                    File.Delete(unit);
                    Bash.Do($"ln -s {trueUnit} {unit}");
                }
            }
            //anthillaUnits = Directory.EnumerateFiles(Const.AnthillaUnits, "*.*", SearchOption.TopDirectoryOnly).ToList();
            if (!anthillaUnits.Any()) {
                foreach (var unit in anthillaUnits) {
                    Bash.Do($"chown root:wheel {unit}");
                    Bash.Do($"chmod 644 {unit}");
                }
            }
            Console.WriteLine("[check] units integrity");
        }

        private static void Hostname() {
            if (Application.IsUnix == false) { return; }
            if (!string.IsNullOrEmpty(ConfigManager.Config.Saved.Host.Name)) {
                Hostnamectl.SetHostname(ConfigManager.Config.Saved.Host.Name);
            }
            if (!string.IsNullOrEmpty(ConfigManager.Config.Saved.Host.Chassis)) {
                Hostnamectl.SetChassis(ConfigManager.Config.Saved.Host.Chassis);
            }
            if (!string.IsNullOrEmpty(ConfigManager.Config.Saved.Host.Deployment)) {
                Hostnamectl.SetDeployment(ConfigManager.Config.Saved.Host.Deployment);
            }
            if (!string.IsNullOrEmpty(ConfigManager.Config.Saved.Host.Location)) {
                Hostnamectl.SetLocation(ConfigManager.Config.Saved.Host.Location);
            }
            Console.WriteLine("[hostname] ready");
        }

        private static void GenerateSecret() {
            if (Application.IsUnix == false) { return; }
            if (!File.Exists(Const.AntdCfgSecret)) {
                File.WriteAllText(Const.AntdCfgSecret, Secret.Gen());
            }
            if (string.IsNullOrEmpty(File.ReadAllText(Const.AntdCfgSecret))) {
                File.WriteAllText(Const.AntdCfgSecret, Secret.Gen());
            }
        }

        private static void SetParameters() {
            if (Application.IsUnix == false) { return; }
            foreach (var sysctl in ConfigManager.Config.Saved.Boot.Sysctl) {
                Sysctl.Set(sysctl);
            }
        }

        private static void SetServices() {
            if (Application.IsUnix == false) { return; }
            foreach (var service in ConfigManager.Config.Saved.Boot.ActiveServices) {
                if (Systemctl.IsEnabled(service) == false)
                    Systemctl.Enable(service);
                if (Systemctl.IsActive(service) == false)
                    Systemctl.Start(service);
            }
            foreach (var service in ConfigManager.Config.Saved.Boot.InactiveServices) {
                if (Systemctl.IsActive(service))
                    Systemctl.Stop(service);
            }
            foreach (var service in ConfigManager.Config.Saved.Boot.DisabledServices) {
                if (Systemctl.IsActive(service))
                    Systemctl.Stop(service);
                if (Systemctl.IsEnabled(service))
                    Systemctl.Disable(service);
            }
            foreach (var service in ConfigManager.Config.Saved.Boot.BlockedServices) {
                if (Systemctl.IsActive(service))
                    Systemctl.Stop(service);
                if (Systemctl.IsEnabled(service))
                    Systemctl.Disable(service);
                Systemctl.Mask(service);
            }
        }

        private static void SetModules() {
            if (Application.IsUnix == false) { return; }
            var loadedModules = Mod.Get();
            foreach (var module in ConfigManager.Config.Saved.Boot.InactiveModules) {
                var loadedModule = loadedModules.FirstOrDefault(_ => _.Module == module);
                if (string.IsNullOrEmpty(loadedModule.Module)) {
                    continue;
                }

                Console.Write($"[mod] removing module '{module}'");
                if (loadedModule.UsedBy.Length > 0)
                    Console.WriteLine($" and its {loadedModule.UsedBy.Length} dependecies");
                else
                    Console.WriteLine();

                Mod.Remove(loadedModule);
            }
            foreach (var module in ConfigManager.Config.Saved.Boot.ActiveModules) {
                var (Module, UsedBy) = loadedModules.FirstOrDefault(_ => _.Module == module);
                if (!string.IsNullOrEmpty(Module)) {
                    continue;
                }
                Mod.Add(module);
            }
        }

        private static void Users() {
            if (Application.IsUnix == false) { return; }
            Getent.AddAOSDefaults();
            Console.WriteLine("[usr] check default");
            foreach (var user in ConfigManager.Config.Saved.Users) {
                Getent.AddUser(user.Name);
                Getent.AddGroup(user.Group);
                Getent.AssignGroup(user.Name, user.Group);
            }
            Console.WriteLine("[usr] ready");
        }

        private static void SetRoutingTables() {
            if (Application.IsUnix == false) { return; }
            if (ConfigManager.Config.Saved.Network.RoutingTables.Length > 0) {
                RoutingTables.Write(ConfigManager.Config.Saved.Network.RoutingTables.Select(_ => (_.Id, _.Name)));
            }
            Console.WriteLine("[rt] tables ready");
        }

        private static void SetNetwork() {
            if (Application.IsUnix == false) { return; }
            Dns.SetResolv(ConfigManager.Config.Saved.Network.Dns);
            foreach (var i in ConfigManager.Config.Saved.Network.Interfaces) {
                Console.WriteLine($"[net] configuring {i.Iface} {i.Address}");
                if (i.Auto == "up" || i.Auto == "on") {
                    foreach (var cmd in i.PreUp) {
                        Console.WriteLine($"[net] {i.Iface} - {cmd}");
                        Bash.Do(cmd);
                    }
                    Ip.EnableNetworkAdapter(i.Iface);
                    if (!string.IsNullOrEmpty(i.Address)) {
                        var address = Help.SplitAddressAndRange(i.Address);
                        if (string.IsNullOrEmpty(address.Range)) {
                            address.Range = "24";
                            Console.WriteLine($"[net] missing range definition, assigning 24 by default");
                        }
                        Ip.AddAddress(i.Iface, address.Address, address.Range);
                    }
                    foreach (var cmd in i.PostUp) {
                        Console.WriteLine($"[net] {i.Iface} - {cmd}");
                        Bash.Do(cmd);
                    }
                }
                else if (i.Auto == "down" || i.Auto == "off") {
                    foreach (var cmd in i.PreDown) {
                        Console.WriteLine($"[net] {i.Iface} - {cmd}");
                        Bash.Do(cmd);
                    }
                    Ip.DisableNetworkAdapter(i.Iface);
                    foreach (var cmd in i.PostDown) {
                        Console.WriteLine($"[net] {i.Iface} - {cmd}");
                        Bash.Do(cmd);
                    }
                }
            }
            foreach (var r in ConfigManager.Config.Saved.Network.Routing) {
                Console.WriteLine($"[net] routing {r.Gateway} {r.Destination} {r.Device}");
                Ip.AddRoute(r.Device, r.Gateway, r.Destination);
            }
            Console.WriteLine("[net] ready");
        }

        private static void SetRoutingRules() {
            if (Application.IsUnix == false) { return; }
            if (ConfigManager.Config.Saved.Network.RoutingTables.Length > 0) {
                foreach (var rt in ConfigManager.Config.Saved.Network.RoutingTables) {
                    foreach (var rule in rt.Rules) {
                        Bash.Do(rule);
                    }
                }
            }
            Console.WriteLine("[rt] rules ready");
        }

        private static void ApplySetupCommands() {
            if (Application.IsUnix == false) { return; }
            foreach (var command in ConfigManager.Config.Saved.Commands.Run) {
                Console.WriteLine($"[cmd] {command}");
                Bash.Do(command);
            }
            Console.WriteLine("[cmd] ready");
        }

        private static void ManageSsh() {
            if (Application.IsUnix == false) { return; }
            var service = "sshd";
            if (Systemctl.IsEnabled(service) == false) {
                Systemctl.Enable(service);
                Console.WriteLine("[ssh] enable sshd");
            }
            if (Systemctl.IsActive(service) == false) {
                Systemctl.Start(service);
                Console.WriteLine("[ssh] start sshd");
            }
        }

        private static void SetDnsServer() {
            //ADNS.Start();
            Console.WriteLine("[adns] ready");
        }

        private static void StorageZfs() {
            if (Application.IsUnix == false) { return; }
            Console.WriteLine("[zpl] start");
            var pools = Zpool.GetImportPools();
            if (pools.Length == 0) {
                Console.WriteLine("[zpl] no pools to import");
            }
            for (var i = 0; i < pools.Length; i++) {
                var currentPool = pools[i];
                Console.WriteLine($"[zpl] pool {currentPool} importing");
                Zpool.Import(currentPool);
                Console.WriteLine($"[zpl] pool {currentPool} imported");
            }
            if (Zpool.GetPools().Length > 1) {
                Console.WriteLine("[zpl] launch zpool scheduled jobs");
                Scheduler.ExecuteJob<ZfsSnapshotLaunchJob>();
            }
        }

        private static void Apps() {
            if (Application.IsUnix == false) { return; }
            Applicative.Setup();
            Applicative.Start();
            Console.WriteLine("[app] ready");
        }

        private static void LaunchJobs() {
            Scheduler.ExecuteJob<LogRotateJob>();
            Scheduler.ExecuteJob<CheckNewDiskJob>();
            //Scheduler.ExecuteJob<SyncTimeJob>();
            //Scheduler.ExecuteJob<ModulesControllerJob>();
            //Scheduler.ExecuteJob<ServicesControllerJob>();
            LaunchCronJobs();
        }

        private static void LaunchCronJobs() {
            foreach (var job in ConfigManager.Config.Saved.Cron)
                Cron.Jobs.Add(job.Name, job.Command, job.Time);
        }

        private static void StartWebdavInstances() {
            foreach (var webdavConf in ConfigManager.Config.Saved.Webdav) {
                if (webdavConf.Enable == false)
                    continue;

                var activeUsers = ConfigManager.Config.Saved.Users
                    .Where(_ => webdavConf.Users.Contains(_.Name))
                    .ToList();

                if (!activeUsers.Any())
                    continue;

                webdavConf.MappedUsers = activeUsers;

                System.Threading.Tasks.Task.Factory.StartNew(() => {
                    var wd = new WebDavServer(webdavConf);
                    WebdavStatus[webdavConf.Target] = true;
                    WebdavInstances[webdavConf.Target] = wd;
                    wd.Start();
                });
            }
        }

        private static void StartWebserver() {
            System.Threading.Tasks.Task.Factory.StartNew(() => {
                var url = $"http://{ConfigManager.Config.Saved.App.Address}:{ConfigManager.Config.Saved.App.HttpPort}";
                var host = new WebHostBuilder()
                  .UseContentRoot(Directory.GetCurrentDirectory())
                  .UseKestrel()
                  .UseStartup<Startup>()
                  .UseUrls(url)
                  .Build();
                host.Run();
            });
        }
    }
}