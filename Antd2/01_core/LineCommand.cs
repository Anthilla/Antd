using Antd.Jobs;
using Antd2.cmds;
using Antd2.Configuration;
using anthilla.core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using static Antd.Help;
using Bash = Antd2.cmds.Bash;
using Systemctl = Antd2.cmds.Systemctl;

namespace Antd {

    public class LineCommand {

        public static readonly Dictionary<string, Action<string[]>> Options = new Dictionary<string, Action<string[]>> {
            { "help", HelpFunc },
            { "exit", ExitFunc },
            { "quit", ExitFunc },
            { "start", StartFunc },
            { "conf", ConfFunc },
            { "default", DefaultFunc },
            { "packages", PackagesFunc },
            { "user", UserFunc },
            { "sysctl", SysctlFunc },
            { "nsswitch", NsswitchFunc },
            { "proc", ProcFunc },
            { "fd", OpenFilesFunc },
            { "netstat", NetstatFunc },
        };

        private static void HelpFunc(string[] args) {
            Console.WriteLine("  TOML:");
            Console.WriteLine("  -----");
            Console.WriteLine("  La documentazione sul formato si trova al seguente link:");
            Console.WriteLine("  https://github.com/toml-lang/toml");
            Console.WriteLine();

            Console.WriteLine("  Comandi:");
            Console.WriteLine("  --------");
            PrintHelp("help", new[] { "Mostra questo help" });
            PrintHelp("exit, quit", new[] { "Esce dall'applicazione" });
            Console.WriteLine();

            PrintHelp("start", new[] { "Avvia la procedura di configurazione di Antd utilizzando i parametri configurati",
                                        "Il path di default del file di configurazione è /cfg/antd/antd.toml"});
            PrintHelp("start <file>", new[] { "Può accettare come argomento il path del file di configurazione" });
            Console.WriteLine();

            PrintHelp("conf test", new[] { "Esegue un test di scrittura E lettura del file di configurazione" });
            PrintHelp("conf write", new[] { "Esegue un test di scrittura del file di configurazione" });
            PrintHelp("conf read", new[] { "Esegue un test di lettura del file di configurazione" });
            Console.WriteLine();

            PrintHelp("user check", new[] { "Verifica se utenti e gruppi di sistema di default sono presenti" });
            PrintHelp("user add", new[] { "Crea e configura utenti e gruppi di sistema di default" });
            Console.WriteLine();

            PrintHelp("sysctl check", new[] { "Verifica se i parametri di sysctl corrispondono a quelli di default" });
            PrintHelp("sysctl set", new[] { "Applica i parametri di sysctl di default" });
            PrintHelp("sysctl write", new[] { "Scrive su /etc/sysctl.conf i parametri di default" });
            Console.WriteLine();
        }

        private static void PrintHelp(string command, IEnumerable<string> description) {
            Console.WriteLine($"  {command}");
            foreach (var line in description) {
                Console.WriteLine($"    {line}");
            }
        }

        private static void ExitFunc(string[] args) {
            ConsoleLogger.Log("Exiting antd2.");
            Thread.Sleep(500);
            Environment.Exit(0);
        }

        private static void StartFunc(string[] args) {
            ConsoleLogger.Log("Start antd2.");
            StartCommand.Start(args);
        }

        private static void ConfFunc(string[] args) {
            var line = args.Length > 0 ? args : ReadLine();
            if (ConfCommand.Options.TryGetValue(line[0], out Action<string[]> functionToExecute)) {
                functionToExecute?.Invoke(line.Skip(1).ToArray());
            }
            else {
                ConsoleLogger.Log("Command '" + line[0] + "' not found");
            }
        }

        private static void DefaultFunc(string[] args) {
            var line = args.Length > 0 ? args : ReadLine();
            if (DefaultCommand.Options.TryGetValue(line[0], out Action<string[]> functionToExecute)) {
                functionToExecute?.Invoke(line.Skip(1).ToArray());
            }
            else {
                ConsoleLogger.Log("Command '" + line[0] + "' not found");
            }
        }

        private static void PackagesFunc(string[] args) {
            var line = args.Length > 0 ? args : ReadLine();
            if (PackagesCommand.Options.TryGetValue(line[0], out Action<string[]> functionToExecute)) {
                functionToExecute?.Invoke(line.Skip(1).ToArray());
            }
            else {
                ConsoleLogger.Log("Command '" + line[0] + "' not found");
            }
        }

        private static void UserFunc(string[] args) {
            var line = args.Length > 0 ? args : ReadLine();
            if (UserCommand.Options.TryGetValue(line[0], out Action<string[]> functionToExecute)) {
                functionToExecute?.Invoke(line.Skip(1).ToArray());
            }
            else {
                ConsoleLogger.Log("Command '" + line[0] + "' not found");
            }
        }

        private static void SysctlFunc(string[] args) {
            var line = args.Length > 0 ? args : ReadLine();
            if (SysctlCommand.Options.TryGetValue(line[0], out Action<string[]> functionToExecute)) {
                functionToExecute?.Invoke(line.Skip(1).ToArray());
            }
            else {
                ConsoleLogger.Log("Command '" + line[0] + "' not found");
            }
        }

        private static void NsswitchFunc(string[] args) {
            var line = args.Length > 0 ? args : ReadLine();
            if (NsswitchCommand.Options.TryGetValue(line[0], out Action<string[]> functionToExecute)) {
                functionToExecute?.Invoke(line.Skip(1).ToArray());
            }
            else {
                ConsoleLogger.Log("Command '" + line[0] + "' not found");
            }
        }

        private static void ProcFunc(string[] args) {
            foreach (var proc in Antd.ProcFs.ProcFs.Processes()) {
                Console.WriteLine($"{proc.Pid} {proc.Name} {proc.CommandLine}");
            }
        }
        private static void OpenFilesFunc(string[] args) {
            foreach (var file in new Antd.ProcFs.Process(1).OpenFiles) {
                Console.WriteLine(file);
            }
        }
        private static void NetstatFunc(string[] args) {
            foreach (var svc in Antd.ProcFs.ProcFs.Net.Services.Unix().Where(svc => svc.State == Antd.ProcFs.NetServiceState.Established)) {
                Console.WriteLine(svc);
            }
        }
    }

    public class ConfCommand {
        public static readonly Dictionary<string, Action<string[]>> Options =
            new Dictionary<string, Action<string[]>> {
                { "test", TestFunc },
                { "write", WriteFunc },
                { "read", ReadFunc },
            };

        private const string ConfFile = "/cfg/antd/antd-test.toml";

        private static void TestFunc(string[] args) {
            WriteFunc(args);
            ReadFunc(args);
        }

        private static void TestFunc_PrintOk(string msg) {
            Console.Write(msg);
            Console.ForegroundColor = ConsoleColor.Green;
            ConsoleLogger.Log("  ok");
            Console.ForegroundColor = ConsoleColor.White;
        }

        private static void TestFunc_PrintKo(string msg, string exception) {
            Console.Write(msg);
            Console.ForegroundColor = ConsoleColor.Red;
            ConsoleLogger.Log("  ko");
            Console.ForegroundColor = ConsoleColor.White;
            ConsoleLogger.Log(exception);
        }

        private static void WriteFunc(string[] args) {
            try {
                var conf = new MachineConfiguration();
                conf.Boot.ActiveModules = new string[] {
                    "tun",
                    "br_netfilter",
                };
                conf.Boot.InactiveModules = new string[] {
                    "ip_tables",
                };
                conf.Boot.Sysctl = new string[] {
                    "fs.file-max=1024000",
                    "net.bridge.bridge-nf-call-arptables=0",
                    "net.bridge.bridge-nf-call-ip6tables=0",
                    "net.bridge.bridge-nf-call-iptables=0",
                    "net.bridge.bridge-nf-filter-pppoe-tagged=0",
                    "net.bridge.bridge-nf-filter-vlan-tagged=0",
                    "net.core.netdev_max_backlog=300000",
                    "net.core.optmem_max=40960",
                    "net.core.rmem_max=268435456",
                    "net.core.somaxconn=65536",
                    "net.core.wmem_max=268435456",
                    "net.ipv4.conf.all.accept_local=1",
                    "net.ipv4.conf.all.accept_redirects=1",
                    "net.ipv4.conf.all.accept_source_route=1",
                    "net.ipv4.conf.all.rp_filter=0",
                    "net.ipv4.conf.all.forwarding=1",
                    "net.ipv4.conf.default.rp_filter=0",
                    "net.ipv4.ip_forward=1",
                    "net.ipv4.ip_local_port_range=1024 65000",
                    "net.ipv4.ip_no_pmtu_disc=1",
                    "net.ipv4.tcp_congestion_control=htcp",
                    "net.ipv4.tcp_fin_timeout=40",
                    "net.ipv4.tcp_max_syn_backlog=3240000",
                    "net.ipv4.tcp_max_tw_buckets=1440000",
                    "net.ipv4.tcp_moderate_rcvbuf=1",
                    "net.ipv4.tcp_mtu_probing=1",
                    "net.ipv4.tcp_rmem=4096 87380 134217728",
                    "net.ipv4.tcp_slow_start_after_idle=1",
                    "net.ipv4.tcp_tw_recycle=0",
                    "net.ipv4.tcp_tw_reuse=1",
                    "net.ipv4.tcp_window_scaling=1",
                    "net.ipv4.tcp_wmem=4096 65536 134217728",
                    "net.ipv6.conf.all.disable_ipv6=1",
                    "vm.swappiness=0"
                };
                conf.Boot.ActiveServices = new string[] {
                    "systemd-journald.service"
                };
                conf.Boot.InactiveServices = new string[] {
                    "systemd-networkd.service"
                };

                conf.Time.Timezone = "Europe/Rome";
                conf.Time.EnableNtpSync = true;
                conf.Time.NtpServer = "ntp1.ien.it";

                conf.Host.Name = "box01";
                conf.Host.Chassis = "server";
                conf.Host.Deployment = "developement";
                conf.Host.Location = "onEarth";

                conf.Network.Dns.Domain = "domain";
                conf.Network.Dns.Search = "search";
                conf.Network.Dns.Nameserver = new string[] {
                    "8.8.8.8",
                    "8.8.4.4",
                };

                conf.Network.Interfaces = new Antd2.Configuration.NetInterface[] {
                    new Antd2.Configuration.NetInterface() { Name = "eth1", Ip ="123.456.78.9", Range = "24"}
                };
                conf.Network.Routing = new Antd2.Configuration.NetRoute[] {
                    new Antd2.Configuration.NetRoute() { Gateway = "123.456.78.99", Destination = "default", Device = "eth1" }
                };

                Nett.Toml.WriteFile<MachineConfiguration>(conf, ConfFile);
                TestFunc_PrintOk("  write: ");
            }
            catch (Exception ex) {
                TestFunc_PrintKo("  write: ", ex.ToString());
            }
        }

        private static void ReadFunc(string[] args) {
            try {
                var conf = Nett.Toml.ReadFile<MachineConfiguration>(ConfFile);
                TestFunc_PrintOk("  read");
            }
            catch (Exception ex) {
                TestFunc_PrintKo("  read", ex.ToString());
            }
        }
    }

    public class PackagesCommand {
        public static readonly Dictionary<string, Action<string[]>> Options =
            new Dictionary<string, Action<string[]>> {
                { "check", CheckFunc },
                { "install", InstallFunc },
            };

        private static string[] RequiredPackages = new string[] {
            "apt-get",
            "brctl",
            "date",
            "dhclient",
            "killall",
            "dhcpcd",
            "df",
            "dmesg",
            "bash",
            "getent",
            "gluster",
            "glusterd",
            "groupadd",
            "haproxy",
            "hostnamectl",
            "ip",
            "ifenslave",
            "journalctl",
            "keepalived",
            "kill",
            "losetup",
            "lsmod",
            "modprobe",
            "rmmod",
            "mount",
            "umount",
            "nsupdate",
            "ntpdate",
            "useradd",
            "mkpasswd",
            "usermod",
            "qemu-img",
            "rndc",
            "rsync",
            "ssh",
            "ssh-keygen",
            "sysctl",
            "systemctl",
            "timedatectl",
            "hwclock",
            "uname",
            "uptime",
            "zfs",
            "virsh",
            "zpool",
        };

        private static void CheckFunc(string[] args) {
            foreach (var package in RequiredPackages) {
                var isInstalled = Whereis.IsInstalled(package);
                if (isInstalled) {
                    CheckFunc_PrintInstalled(package);
                }
                else {
                    CheckFunc_PrintNotInstalled(package);
                }
            }
        }

        private static void CheckFunc_PrintInstalled(string package) {
            Console.Write($"  {package}: ");
            Console.ForegroundColor = ConsoleColor.Green;
            ConsoleLogger.Log("installed");
            Console.ForegroundColor = ConsoleColor.White;
        }

        private static void CheckFunc_PrintNotInstalled(string package) {
            Console.Write($"  {package}: ");
            Console.ForegroundColor = ConsoleColor.Red;
            ConsoleLogger.Log("not installed");
            Console.ForegroundColor = ConsoleColor.White;
        }

        private static void InstallFunc(string[] args) {
            foreach (var package in args) {
                ConsoleLogger.Log($"  installing {package}");
                AptGet.Install(package);
            }
        }
    }

    public class DefaultCommand {
        public static readonly Dictionary<string, Action<string[]>> Options =
            new Dictionary<string, Action<string[]>> {
                { "apply", ApplyFunc },
            };

        private static void ApplyFunc(string[] args) {
            ConsoleLogger.Log("  Apply global default settings!");
            UserCommand.AddFunc(args);
            UserCommand.CheckFunc(args);
            SysctlCommand.SetFunc(args);
            SysctlCommand.WriteFunc(args);
            SysctlCommand.CheckFunc(args);
            NsswitchCommand.ApplyFunc(args);
            NsswitchCommand.CheckFunc(args);
        }
    }

    public class UserCommand {
        public static readonly Dictionary<string, Action<string[]>> Options =
            new Dictionary<string, Action<string[]>> {
                { "check", CheckFunc },
                { "add", AddFunc },
            };

        private static string[] RequiredGroups = new string[] {
            "wheel",
        };

        private static string[] RequiredUsers = new string[] {
            "deus",
            "obse",
            "obsi",
            "visor",
        };

        public static void CheckFunc(string[] args) {
            ConsoleLogger.Log("  Check groups");
            ConsoleLogger.Log("--------------");
            var existingGroups = Getent.GetGroups();
            foreach (var group in RequiredGroups) {
                var isInstalled = existingGroups.Contains(group);
                if (isInstalled) {
                    CheckFunc_PrintInstalled(group);
                }
                else {
                    CheckFunc_PrintNotInstalled(group);
                }
            }
            ConsoleLogger.Log("");
            ConsoleLogger.Log("  Check users");
            ConsoleLogger.Log("-------------");
            var existingUsers = Getent.GetUsers();
            foreach (var user in RequiredUsers) {
                var isInstalled = existingUsers.Contains(user);
                if (isInstalled) {
                    CheckFunc_PrintInstalled(user);
                }
                else {
                    CheckFunc_PrintNotInstalled(user);
                }
            }
        }

        private static void CheckFunc_PrintInstalled(string package) {
            Console.Write($"  {package}: ");
            Console.ForegroundColor = ConsoleColor.Green;
            ConsoleLogger.Log("exists");
            Console.ForegroundColor = ConsoleColor.White;
        }

        private static void CheckFunc_PrintNotInstalled(string package) {
            Console.Write($"  {package}: ");
            Console.ForegroundColor = ConsoleColor.Red;
            ConsoleLogger.Log("does not exist");
            Console.ForegroundColor = ConsoleColor.White;
        }

        public static void AddFunc(string[] args) {
            ConsoleLogger.Log("  Create groups");
            ConsoleLogger.Log("---------------");
            foreach (var group in RequiredGroups) {
                ConsoleLogger.Log($"  add group {group}");
                Getent.AddGroup(group);
            }
            ConsoleLogger.Log("");
            ConsoleLogger.Log("  Create users");
            ConsoleLogger.Log("--------------");
            foreach (var user in RequiredUsers) {
                ConsoleLogger.Log($"  add user {user}");
                Getent.AddUser(user);
                foreach (var group in RequiredGroups) {
                    ConsoleLogger.Log($"    assign group {group} to user {user}");
                    Getent.AssignGroup(user, group);
                }
            }
        }
    }

    public class SysctlCommand {
        public static readonly Dictionary<string, Action<string[]>> Options =
            new Dictionary<string, Action<string[]>> {
                { "check", CheckFunc },
                { "set", SetFunc },
                { "write", WriteFunc },
            };

        private static (string Key, string Value)[] RequiredSysctl = new (string Key, string Value)[] {
            ("fs.file-max", "1024000"),
            ("net.bridge.bridge-nf-call-arptables", "0"),
            ("net.bridge.bridge-nf-call-ip6tables", "0"),
            ("net.bridge.bridge-nf-call-iptables", "0"),
            ("net.bridge.bridge-nf-filter-pppoe-tagged", "0"),
            ("net.bridge.bridge-nf-filter-vlan-tagged", "0"),
            ("net.core.netdev_max_backlog", "300000"),
            ("net.core.optmem_max", "40960"),
            ("net.core.rmem_max", "268435456"),
            ("net.core.somaxconn", "65536"),
            ("net.core.wmem_max", "268435456"),
            ("net.ipv4.conf.all.accept_local", "1"),
            ("net.ipv4.conf.all.accept_redirects", "1"),
            ("net.ipv4.conf.all.accept_source_route", "1"),
            ("net.ipv4.conf.all.rp_filter", "0"),
            ("net.ipv4.conf.all.forwarding", "1"),
            ("net.ipv4.conf.default.rp_filter", "0"),
            ("net.ipv4.ip_forward", "1"),
            ("net.ipv4.ip_local_port_range", "1024 65000"),
            ("net.ipv4.ip_no_pmtu_disc", "1"),
            ("net.ipv4.tcp_congestion_control", "htcp"),
            ("net.ipv4.tcp_fin_timeout", "40"),
            ("net.ipv4.tcp_max_syn_backlog", "3240000"),
            ("net.ipv4.tcp_max_tw_buckets", "1440000"),
            ("net.ipv4.tcp_moderate_rcvbuf", "1"),
            ("net.ipv4.tcp_mtu_probing", "1"),
            ("net.ipv4.tcp_rmem", "4096 87380 134217728"),
            ("net.ipv4.tcp_slow_start_after_idle", "1"),
            ("net.ipv4.tcp_tw_recycle", "0"),
            ("net.ipv4.tcp_tw_reuse", "1"),
            ("net.ipv4.tcp_window_scaling", "1"),
            ("net.ipv4.tcp_wmem", "4096 65536 134217728"),
            ("net.ipv6.conf.all.disable_ipv6", "1"),
            ("vm.swappiness", "0"),
        };

        public static void CheckFunc(string[] args) {
            var currentSysctl = Sysctl.Get();
            foreach (var sysctl in RequiredSysctl) {
                var current = currentSysctl.FirstOrDefault(_ => _.Key == sysctl.Key);
                var isConfigured = current.Value == sysctl.Value;
                if (isConfigured) {
                    CheckFunc_PrintInstalled(sysctl.Key);
                }
                else {
                    CheckFunc_PrintNotInstalled(sysctl.Key);
                }
            }
        }

        private static void CheckFunc_PrintInstalled(string param) {
            Console.Write($"  {param}: ");
            Console.ForegroundColor = ConsoleColor.Green;
            ConsoleLogger.Log("configured");
            Console.ForegroundColor = ConsoleColor.White;
        }

        private static void CheckFunc_PrintNotInstalled(string param) {
            Console.Write($"  {param}: ");
            Console.ForegroundColor = ConsoleColor.Red;
            ConsoleLogger.Log("not configured");
            Console.ForegroundColor = ConsoleColor.White;
        }

        public static void SetFunc(string[] args) {
            foreach (var param in RequiredSysctl) {
                ConsoleLogger.Log($"  {param.Key}={param.Value}");
                Sysctl.Set(param.Key, param.Value);
            }
        }

        public static void WriteFunc(string[] args) {
            ConsoleLogger.Log("  Write sysctl file");
            Sysctl.Write(RequiredSysctl);
            ConsoleLogger.Log("  Apply sysctl file");
            Sysctl.Apply();
        }
    }

    public class NsswitchCommand {
        public static readonly Dictionary<string, Action<string[]>> Options =
            new Dictionary<string, Action<string[]>> {
                { "check", CheckFunc },
                { "apply", ApplyFunc },
            };

        private static (string Key, string Value)[] RequiredSysctl = new (string Key, string Value)[] {
            ("passwd", "files winbind"),
            ("group", "files winbind"),
            ("shadow", "compat"),
            ("hosts", "files mdns_minimal [NOTFOUND=return] resolve dns"),
            ("networks", "files dns"),
            ("services", "db files"),
            ("protocols", "db files"),
            ("rpc", "db files"),
            ("ethers", "db files"),
            ("netmasks", "files"),
            ("netgroup", "files"),
            ("bootparams", "files"),
            ("automount", "files"),
            ("aliases", "files"),
        };

        public static void CheckFunc(string[] args) {
            var currentNsswitch = Nsswitch.Get();
            foreach (var nsswitch in RequiredSysctl) {
                var current = currentNsswitch.FirstOrDefault(_ => _.Key == nsswitch.Key);
                var isConfigured = current.Value == nsswitch.Value;
                if (isConfigured) {
                    CheckFunc_PrintInstalled(nsswitch.Key);
                }
                else {
                    CheckFunc_PrintNotInstalled(nsswitch.Key);
                }
            }
        }

        private static void CheckFunc_PrintInstalled(string param) {
            Console.Write($"  {param}: ");
            Console.ForegroundColor = ConsoleColor.Green;
            ConsoleLogger.Log("configured");
            Console.ForegroundColor = ConsoleColor.White;
        }

        private static void CheckFunc_PrintNotInstalled(string param) {
            Console.Write($"  {param}: ");
            Console.ForegroundColor = ConsoleColor.Red;
            ConsoleLogger.Log("not configured");
            Console.ForegroundColor = ConsoleColor.White;
        }

        public static void ApplyFunc(string[] args) {
            ConsoleLogger.Log("  Write nsswitch file");
            Nsswitch.Write(RequiredSysctl);
        }
    }

    public class StartCommand {

        private const string ConfFile = "/cfg/antd/antd.toml";
        public static JobManager Scheduler;
        public static Stopwatch STOPWATCH;
        public static MachineConfiguration CONF;

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

            ConsoleLogger.Log($"antd started in: {STOPWATCH.ElapsedMilliseconds} ms");
        }


        private static void OsReadAndWrite() {
            Bash.Do("mount -o remount,rw,noatime /");
            Bash.Do("mount -o remount,rw,discard,noatime /mnt/cdrom");
        }

        private static void RemoveLimits() {
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
            Mount.WorkingDirectories();
        }

        private static void Time() {
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
            Mount.Set();
            ConsoleLogger.Log("[mnt] ready");
        }

        private static void CheckUnitsLocation() {
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
            if (!File.Exists(Const.AntdCfgSecret)) {
                File.WriteAllText(Const.AntdCfgSecret, Secret.Gen());
            }
            if (string.IsNullOrEmpty(File.ReadAllText(Const.AntdCfgSecret))) {
                File.WriteAllText(Const.AntdCfgSecret, Secret.Gen());
            }
        }

        private static void SetParameters() {
            foreach (var sysctl in CONF.Boot.Sysctl) {
                Sysctl.Set(sysctl);
            }
        }

        private static void SetServices() {
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
            foreach (var module in CONF.Boot.ActiveModules) {
                Mod.Add(module);
            }
            foreach (var module in CONF.Boot.InactiveModules) {
                Mod.Remove(module);
            }
        }

        private static void Users() {
            foreach (var user in CONF.Users) {
                Getent.AddUser(user.Name);
                Getent.AddGroup(user.Group);
                Getent.AssignGroup(user.Name, user.Group);
            }
            ConsoleLogger.Log("[usr] ready");
        }

        private static void SetNetwork() {
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
            foreach (var command in CONF.Commands.Run) {
                ConsoleLogger.Log($"[cmd] {command}");
                Bash.Do(command);
            }
            ConsoleLogger.Log("[cmd] ready");
        }

        private static void ManageSsh() {
            //ConsoleLogger.Log("[ssh] ready");
        }

        private static void StorageZfs() {
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
            Applicative.Setup();
            Applicative.Start();
            ConsoleLogger.Log("[app] ready");
        }

        private static void LaunchJobs() {
            Scheduler.ExecuteJob<ModulesRemoverJob>();
        }
    }
}