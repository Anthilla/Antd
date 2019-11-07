using Antd.Jobs;
using Antd2.cmds;
using Antd2.Configuration;
using Antd2.Init;
using Antd2.Web;
using anthilla.core;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Bash = Antd2.cmds.Bash;
using Systemctl = Antd2.cmds.Systemctl;

namespace Antd2 {
    public class StartCommand {

        private const string ConfFile = "/cfg/antd/antd.toml";
        public static JobManager Scheduler;
        public static Stopwatch STOPWATCH;
        public static MachineConfiguration CONF;
        public static ServiceInit ServiceInit;

        public static void Start(string[] args) {
            STOPWATCH = new Stopwatch();
            STOPWATCH.Start();
            Scheduler = new JobManager();

            var confFile = args.Length > 0 ? !string.IsNullOrEmpty(args[0]) ? args[0] : ConfFile : ConfFile;
            CONF = Nett.Toml.ReadFile<MachineConfiguration>(confFile);
            if (CONF == null) {
                Console.ForegroundColor = ConsoleColor.Red;
                ConsoleLogger.Log("  missing file configuration!");
                Console.ForegroundColor = ConsoleColor.White;
                return;
            }

            Init();
            OsReadAndWrite();
            RemoveLimits();
            CreateWorkingDirectories();
            MountWorkingDirectories();

            Time();
            CheckUnitsLocation();
            Mounts();
            Hostname();
            GenerateSecret();
            //License();

            SetServices();
            SetModules();
            SetParameters();

            Users();
            SetNetwork();

            ManageSsh();
            StorageZfs();
            Apps();
            ApplySetupCommands();

            LaunchJobs();

            StartWebserver();

            ConsoleLogger.Log($"antd started in: {STOPWATCH.ElapsedMilliseconds} ms");
        }

        private static void Init() {
            if (ServiceInit == null) {
                ServiceInit = new ServiceInit();
            }
            if (File.Exists("/var/run/sharpinit/sharpinit.sock")) {
                File.Delete("/var/run/sharpinit/sharpinit.sock");
            }
            ServiceInit.Start();
        }

        private static void OsReadAndWrite() {
            if (Application.IsUnix == false) { return; }
            Bash.Do("mount -o remount,rw,noatime /");
            Bash.Do("mount -o remount,rw,discard,noatime /mnt/cdrom");
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
            Directory.CreateDirectory(Const.RepoDirs);
            Directory.CreateDirectory(Const.TimerUnits);
            Directory.CreateDirectory(Const.AnthillaUnits);
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
            if (CONF.Time.EnableNtpSync && !string.IsNullOrEmpty(CONF.Time.NtpServer)) {
                Ntpdate.SyncFromRemoteServer(CONF.Time.NtpServer);
            }
            if (!string.IsNullOrEmpty(CONF.Time.Timezone)) {
                Timedatectl.SetTimezone(CONF.Time.Timezone);
            }
            ConsoleLogger.Log("[time] ready");
        }

        private static void Mounts() {
            if (Application.IsUnix == false) { return; }
            Mount.Set();
            ConsoleLogger.Log("[mnt] ready");
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
            ConsoleLogger.Log("[check] units integrity");
        }

        private static void Hostname() {
            if (Application.IsUnix == false) { return; }
            if (!string.IsNullOrEmpty(CONF.Host.Name)) {
                Hostnamectl.SetHostname(CONF.Host.Name);
            }
            if (!string.IsNullOrEmpty(CONF.Host.Chassis)) {
                Hostnamectl.SetChassis(CONF.Host.Chassis);
            }
            if (!string.IsNullOrEmpty(CONF.Host.Deployment)) {
                Hostnamectl.SetDeployment(CONF.Host.Deployment);
            }
            if (!string.IsNullOrEmpty(CONF.Host.Location)) {
                Hostnamectl.SetLocation(CONF.Host.Location);
            }
            ConsoleLogger.Log("[hostname] ready");
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
            foreach (var sysctl in CONF.Boot.Sysctl) {
                Sysctl.Set(sysctl);
            }
        }

        private static void SetServices() {
            if (Application.IsUnix == false) { return; }
            foreach (var service in CONF.Boot.ActiveServices) {
                Systemctl.Enable(service);
                Systemctl.Start(service);
            }
            foreach (var service in CONF.Boot.InactiveServices) {
                Systemctl.Stop(service);
                Systemctl.Disable(service);
            }
        }

        private static void SetModules() {
            if (Application.IsUnix == false) { return; }
            foreach (var module in CONF.Boot.ActiveModules) {
                Mod.Add(module);
            }
            foreach (var module in CONF.Boot.InactiveModules) {
                Mod.Remove(module);
            }
        }

        private static void Users() {
            if (Application.IsUnix == false) { return; }
            foreach (var user in CONF.Users) {
                Getent.AddUser(user.Name);
                Getent.AddGroup(user.Group);
                Getent.AssignGroup(user.Name, user.Group);
            }
            ConsoleLogger.Log("[usr] ready");
        }

        private static void SetNetwork() {
            if (Application.IsUnix == false) { return; }
            Dns.SetResolv(CONF.Network.Dns);
            foreach (var i in CONF.Network.Interfaces) {
                ConsoleLogger.Log($"[net] configuring {i.Name} {i.Ip}/{i.Range}");
                Ip.EnableNetworkAdapter(i.Name);
                if (!string.IsNullOrEmpty(i.Ip) && !string.IsNullOrEmpty(i.Range)) {
                    Ip.AddAddress(i.Name, i.Ip, i.Range);
                }
                foreach (var cmd in i.Conf) {
                    ConsoleLogger.Log($"[net] {i.Name} - {cmd}");
                    Bash.Do(cmd);
                }
            }
            foreach (var r in CONF.Network.Routing) {
                ConsoleLogger.Log($"[net] routing {r.Gateway} {r.Destination} {r.Device}");
                Ip.AddRoute(r.Device, r.Gateway, r.Destination);
            }
            ConsoleLogger.Log("[net] ready");
        }

        private static void ApplySetupCommands() {
            if (Application.IsUnix == false) { return; }
            foreach (var command in CONF.Commands.Run) {
                ConsoleLogger.Log($"[cmd] {command}");
                Bash.Do(command);
            }
            ConsoleLogger.Log("[cmd] ready");
        }

        private static void ManageSsh() {
            if (Application.IsUnix == false) { return; }
            //ConsoleLogger.Log("[ssh] ready");
        }

        private static void StorageZfs() {
            if (Application.IsUnix == false) { return; }
            ConsoleLogger.Log("[zpl] start");
            var pools = Zpool.GetImportPools();
            if (pools.Length == 0) {
                ConsoleLogger.Log("[zpl] no pools to import");
            }
            for (var i = 0; i < pools.Length; i++) {
                var currentPool = pools[i];
                ConsoleLogger.Log($"[zpl] pool {currentPool} importing");
                Zpool.Import(currentPool);
                ConsoleLogger.Log($"[zpl] pool {currentPool} imported");
            }
            if (Zpool.GetPools().Length > 1) {
                ConsoleLogger.Log("[zpl] launch zpool scheduled jobs");
                Scheduler.ExecuteJob<ZfsSnapshotLaunchJob>();
            }
        }

        private static void Apps() {
            if (Application.IsUnix == false) { return; }
            Applicative.Setup();
            Applicative.Start();
            ConsoleLogger.Log("[app] ready");
        }

        private static void LaunchJobs() {
            Scheduler.ExecuteJob<ModulesRemoverJob>();
        }

        private static void StartWebserver() {
            Task.Factory.StartNew(() => {
                var host = new WebHostBuilder()
                  .UseContentRoot(Directory.GetCurrentDirectory())
                  .UseKestrel()
                  .UseStartup<Startup>()
                  .UseUrls("http://0.0.0.0:8085")
                  .Build();
                host.Run();
            });
        }
    }
}