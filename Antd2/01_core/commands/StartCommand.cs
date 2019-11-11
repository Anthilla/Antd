using Antd2.cmds;
using Antd2.Configuration;
using Antd2.Init;
using Antd2.Jobs;
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
                Console.WriteLine("  missing file configuration!");
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

            SetDnsServer();

            StorageZfs();
            Apps();
            ApplySetupCommands();

            LaunchJobs();

            StartWebserver();

            Console.WriteLine($"antd started in: {STOPWATCH.ElapsedMilliseconds} ms");
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
            if (CONF.Time.EnableNtpSync && CONF.Time.NtpServer.Length > 0 && !string.IsNullOrEmpty(CONF.Time.NtpServer[0])) {
                Ntpdate.SyncFromRemoteServer(CONF.Time.NtpServer[0]);
            }
            if (!string.IsNullOrEmpty(CONF.Time.Timezone)) {
                Timedatectl.SetTimezone(CONF.Time.Timezone);
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
            foreach (var sysctl in CONF.Boot.Sysctl) {
                Sysctl.Set(sysctl);
            }
        }

        private static void SetServices() {
            if (Application.IsUnix == false) { return; }
            foreach (var service in CONF.Boot.ActiveServices) {
                if (Systemctl.IsEnabled(service) == false)
                    Systemctl.Enable(service);
                if (Systemctl.IsActive(service) == false)
                    Systemctl.Start(service);
            }
            foreach (var service in CONF.Boot.InactiveServices) {
                if (Systemctl.IsActive(service))
                    Systemctl.Stop(service);
            }
            foreach (var service in CONF.Boot.DisabledServices) {
                if (Systemctl.IsActive(service))
                    Systemctl.Stop(service);
                if (Systemctl.IsEnabled(service))
                    Systemctl.Disable(service);
            }
            foreach (var service in CONF.Boot.BlockedServices) {
                if (Systemctl.IsActive(service))
                    Systemctl.Stop(service);
                if (Systemctl.IsEnabled(service))
                    Systemctl.Disable(service);
                Systemctl.Mask(service);
            }
        }

        private static void SetModules() {
            if (Application.IsUnix == false) { return; }
            foreach (var module in CONF.Boot.ActiveModules) {
                Mod.Add(module);
            }
            var loadedModules = Mod.Get();
            foreach (var module in CONF.Boot.InactiveModules) {
                if (loadedModules.Any(_ => _.Module.Trim().ToUpperInvariant() == module.Trim().ToUpperInvariant())) {
                    Mod.Remove(loadedModules.FirstOrDefault(_ => _.Module.Trim().ToUpperInvariant() == module.Trim().ToUpperInvariant()));
                }
            }
        }

        private static void Users() {
            if (Application.IsUnix == false) { return; }
            foreach (var user in CONF.Users) {
                Getent.AddUser(user.Name);
                Getent.AddGroup(user.Group);
                Getent.AssignGroup(user.Name, user.Group);
            }
            Console.WriteLine("[usr] ready");
        }

        private static void SetNetwork() {
            if (Application.IsUnix == false) { return; }
            Dns.SetResolv(CONF.Network.Dns);
            foreach (var i in CONF.Network.Interfaces) {
                Console.WriteLine($"[net] configuring {i.Iface} {i.Address}");
                Ip.EnableNetworkAdapter(i.Iface);
                if (!string.IsNullOrEmpty(i.Address)) {
                    var address = Help.SplitAddressAndRange(i.Address);
                    if (string.IsNullOrEmpty(address.Range)) {
                        address.Range = "24";
                        Console.WriteLine($"[net] missing range definition, assigning 24 by default");
                    }
                    Ip.AddAddress(i.Iface, address.Address, address.Range);
                }
                foreach (var cmd in i.Conf) {
                    Console.WriteLine($"[net] {i.Iface} - {cmd}");
                    Bash.Do(cmd);
                }
            }
            foreach (var r in CONF.Network.Routing) {
                Console.WriteLine($"[net] routing {r.Gateway} {r.Destination} {r.Device}");
                Ip.AddRoute(r.Device, r.Gateway, r.Destination);
            }
            Console.WriteLine("[net] ready");
        }

        private static void ApplySetupCommands() {
            if (Application.IsUnix == false) { return; }
            foreach (var command in CONF.Commands.Run) {
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
            ADNS.Start();
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
            Scheduler.ExecuteJob<ModulesControllerJob>();
            Scheduler.ExecuteJob<ServicesControllerJob>();
        }

        private static void StartWebserver() {
            //Task.Factory.StartNew(() => {
            //    var host = new WebHostBuilder()
            //      .UseContentRoot(Directory.GetCurrentDirectory())
            //      .UseKestrel()
            //      .UseStartup<Startup>()
            //      .UseUrls("http://0.0.0.0:8085")
            //      .Build();
            //    host.Run();
            //});
        }
    }
}