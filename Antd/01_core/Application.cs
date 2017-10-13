using Antd.cmds;
using Antd.models;
using anthilla.core;
using anthilla.crypto;
using anthilla.scheduler;
using Nancy;
using Nancy.Hosting.Self;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;

namespace Antd {

    /// <summary>
    /// TODO
    /// - conf (current & running)
    ///     - aggiornare i valori in maniera parziale/contestuale per ridurre il carico
    ///     - aggiungere al Modulo Nancy il get parziale/contestuale
    /// - aggiungere configurazione Storage > Retention/Gestione Spazio Libero
    /// - consulta log -> journalctl syslog
    /// </summary>
    internal class Application {

        public static string KeyName = "antd";

        /// <summary>
        /// NB: aggiorna questo parametro ad ogni modifica
        /// </summary>
        public static MachineConfig CurrentConfiguration;
        /// <summary>
        /// Questo parametro viene aggiornato ogni minuto, sulla base dell'azione schedulata indicata in ImportRunningConfigurationJob
        /// </summary>
        public static MachineStatus RunningConfiguration;
        /// <summary>
        /// Questo parametro va aggiornato periodicamente con un Job e sarà esposto da un api
        /// </summary>
        public static MachineStatusChecklistModel Checklist;
        public static ClusterNodeChecklistModel[] ClusterChecklist;
        public static AsymmetricKeys Keys;
        public static JobManager Scheduler;
        public static Stopwatch STOPWATCH;
        public static string Agent;

        private static void Main() {
            var resetEvent = new AutoResetEvent(initialState: false);
            Console.CancelKeyPress += (s, e) => { e.Cancel = true; resetEvent.Set(); };
            STOPWATCH = new Stopwatch();
            STOPWATCH.Start();
            ConsoleLogger.Log($"[{KeyName}] start");
            Scheduler = new JobManager();

            OsReadAndWrite();
            RemoveLimits();
            CreateWorkingDirectories();
            MountWorkingDirectories();
            OverlayWatcher();

            CurrentConfiguration = ConfigRepo.Read();
            if(CurrentConfiguration == null) {
                CurrentConfiguration = new MachineConfig();
                CurrentConfiguration.Host.MachineUid = Guid.NewGuid();
                CurrentConfiguration.Host.SerialNumber = Guid.NewGuid();
                CurrentConfiguration.Host.PartNumber = Guid.NewGuid();

                CurrentConfiguration.Users.ApplicativeUsers = new ApplicativeUser[] { new ApplicativeUser() { Active = true, Type = AuthenticationType.simple, Id = "master", Claims = new[] { SHA.Generate("master") } } };

                CurrentConfiguration.Network = Default.Network();

                CurrentConfiguration.NsSwitch = new NsSwitch() {
                    Aliases = "files",
                    Ethers = "db files",
                    Group = "files winbind",
                    Hosts = "files mdns_minimal [NOTFOUND=return] resolve dns",
                    Netgroup = "files",
                    Networks = "files dns",
                    Passwd = "files winbind",
                    Protocols = "db files",
                    Rpc = "db files",
                    Services = "db files",
                    Shadow = "compat",
                    Netmasks = "files",
                    Bootparams = "files",
                    Automount = "files"
                };

                CurrentConfiguration.Services.Ssh.AuthorizedKey = Ssh.GetAuthorizedKey();

                ConfigRepo.Save();
            }
            if(RunningConfiguration == null) {
                ConsoleLogger.Log("[conf] get running");
                //RunningConfiguration = ConfigRepo.GetRunning();
                RunningConfiguration = new MachineStatus();
            }
            if(Checklist == null) {
                Checklist = new MachineStatusChecklistModel();
            }

            Time();
            CheckUnitsLocation();
            Mounts();
            Hostname();
            GenerateSecret();
            License();

            SetServices();
            SetModules();
            SetParameters();

            Users();
            Dns();
            Network();

            Ntpd();
            Firewall();
            Dhcpd();
            Bind();
            ApplySetupConfiguration();
            Nginx();
            ManageSsh();
            Samba();
            Syslog();
            StorageZfs();
            Ca();
            Apps();
            Rsync();
            VfsServer();
            Tor();
            ManageCluster();
            Gluster();
            DirectoryWatchers();
            CheckApplicationFileAcls();

            var port = CurrentConfiguration.WebService.Port;
            var uri = $"http://localhost:{port}/";
            var webService = new NancyHost(new Uri(uri));
            webService.Start();
            StaticConfiguration.DisableErrorTraces = false;
            ConsoleLogger.Log($"[{KeyName}] web service is listening on port {port}");

            #region [    Working    ]
            PrepareGuiService();
            StartRssdp();
            LaunchJobs();
            Test();
            #endregion

            ConsoleLogger.Log($"[{KeyName}] loaded in: {STOPWATCH.ElapsedMilliseconds} ms");
            resetEvent.WaitOne();
            webService.Stop();
            STOPWATCH.Stop();
            ConsoleLogger.Log($"[{KeyName}] stop");
            Environment.Exit(0);
        }

        private static void OsReadAndWrite() {
            Bash.Execute("mount -o remount,rw,noatime /", false);
            Bash.Execute("mount -o remount,rw,discard,noatime /mnt/cdrom", false);
        }

        private static void RemoveLimits() {
            const string limitsFile = "/etc/security/limits.conf";
            if(File.Exists(limitsFile)) {
                if(!File.ReadAllText(limitsFile).Contains("root - nofile 1024000")) {
                    FileWithAcl.AppendAllLines(limitsFile, new[] { "root - nofile 1024000" }, "644", "root", "wheel");
                }
            }
            Bash.Execute("ulimit -n 1024000", false);
        }

        private static void CreateWorkingDirectories() {
            Directory.CreateDirectory(Parameter.RepoDirs);
            Directory.CreateDirectory(Parameter.TimerUnits);
            Directory.CreateDirectory(CommonString.Append(Parameter.AntdCfg, "/antd_conf_repo"));
            Directory.CreateDirectory(Parameter.AntdCfg);
            Directory.CreateDirectory(Parameter.AntdCfgConf);
            Directory.CreateDirectory(Parameter.AnthillaUnits);
            Directory.CreateDirectory(Parameter.AntdCfgVfs);
            Directory.CreateDirectory(Parameter.AntdCfgSetup);
            if(!File.Exists($"{Parameter.AntdCfgSetup}/setup.conf")) {
                File.WriteAllText($"{Parameter.AntdCfgSetup}/setup.conf", "echo Hello World!");
            }
        }

        private static void MountWorkingDirectories() {
            Mount.WorkingDirectories();
        }

        private static void Time() {
            Scheduler.ExecuteJob<SyncLocalClockJob>();
            Timedatectl.Apply();
            Ntp.Prepare();
            ConsoleLogger.Log("[time] ready");
        }

        private static void OverlayWatcher() {
            //if(Directory.Exists(Parameter.Overlay)) {
            //    new OverlayWatcher().StartWatching();
            //    ConsoleLogger.Log("overlay watcher ready");
            //}
        }

        private static void Mounts() {
            Mount.Set();
            ConsoleLogger.Log("mounts ready");
        }

        private static void CheckUnitsLocation() {
            var anthillaUnits = Directory.EnumerateFiles(Parameter.AnthillaUnits, "*.*", SearchOption.TopDirectoryOnly);
            if(!anthillaUnits.Any()) {
                var antdUnits = Directory.EnumerateFiles(Parameter.AntdUnits, "*.*", SearchOption.TopDirectoryOnly);
                foreach(var unit in antdUnits) {
                    var trueUnit = unit.Replace(Parameter.AntdUnits, Parameter.AnthillaUnits);
                    if(!File.Exists(trueUnit)) {
                        File.Copy(unit, trueUnit);
                    }
                    File.Delete(unit);
                    Bash.Execute($"ln -s {trueUnit} {unit}");
                }
                var kernelUnits = Directory.EnumerateFiles(Parameter.KernelUnits, "*.*", SearchOption.TopDirectoryOnly);
                foreach(var unit in kernelUnits) {
                    var trueUnit = unit.Replace(Parameter.KernelUnits, Parameter.AnthillaUnits);
                    if(!File.Exists(trueUnit)) {
                        File.Copy(unit, trueUnit);
                    }
                    File.Delete(unit);
                    Bash.Execute($"ln -s {trueUnit} {unit}");
                }
                var applicativeUnits = Directory.EnumerateFiles(Parameter.ApplicativeUnits, "*.*", SearchOption.TopDirectoryOnly);
                foreach(var unit in applicativeUnits) {
                    var trueUnit = unit.Replace(Parameter.ApplicativeUnits, Parameter.AnthillaUnits);
                    if(!File.Exists(trueUnit)) {
                        File.Copy(unit, trueUnit);
                    }
                    File.Delete(unit);
                    Bash.Execute($"ln -s {trueUnit} {unit}");
                }
            }
            //anthillaUnits = Directory.EnumerateFiles(Parameter.AnthillaUnits, "*.*", SearchOption.TopDirectoryOnly).ToList();
            if(!anthillaUnits.Any()) {
                foreach(var unit in anthillaUnits) {
                    Bash.Execute($"chown root:wheel {unit}");
                    Bash.Execute($"chmod 644 {unit}");
                }
            }
            ConsoleLogger.Log("[check] units integrity");
        }

        private static void Hostname() {
            Hostnamectl.Apply();
            ConsoleLogger.Log("[hostname] ready");
        }

        private static void GenerateSecret() {
            if(!File.Exists(Parameter.AntdCfgSecret)) {
                FileWithAcl.WriteAllText(Parameter.AntdCfgSecret, Secret.Gen(), "644", "root", "wheel");
            }
            if(string.IsNullOrEmpty(File.ReadAllText(Parameter.AntdCfgSecret))) {
                FileWithAcl.WriteAllText(Parameter.AntdCfgSecret, Secret.Gen(), "644", "root", "wheel");
            }
        }

        private static void License() {
            Keys = new AsymmetricKeys(Parameter.AntdCfgKeys, KeyName);
            ConsoleLogger.Log($"[part_number] {CurrentConfiguration.Host.PartNumber}");
            ConsoleLogger.Log($"[serial_number] {CurrentConfiguration.Host.SerialNumber}");
            ConsoleLogger.Log($"[machine_id] {CurrentConfiguration.Host.MachineUid}");
            LicenseManagement.Download("Antd", Keys.PublicKey);
            var licenseStatus = LicenseManagement.Check("Antd", Keys.PublicKey);
            ConsoleLogger.Log(licenseStatus == null
                ? "[license] license results null"
                : $"[license] {licenseStatus.Status} - {licenseStatus.Message}");
        }

        private static void SetParameters() {
            Sysctl.SaveDefaultValues();
            RunningConfiguration.Boot.Parameters = Sysctl.Get();
            Sysctl.Set();
        }

        private static void SetServices() {
            cmds.Systemctl.Set();
        }

        private static void SetModules() {
            var modules = Mod.Get();
            var bootModules = new SystemModule[modules.Length];
            for(var i = 0; i < modules.Length; i++) {
                bootModules[i] = new SystemModule() { Module = modules[i].Module, Active = true };
            }
            RunningConfiguration.Boot.Modules = bootModules;
            Mod.Set();
        }

        private static void Users() {
            Passwd.Set();
            ConsoleLogger.Log("[users] ready");
        }

        private static void Dns() {
            cmds.Dns.Set();
            ConsoleLogger.Log("[name_service] ready");
        }

        private static void Network() {
            RunningConfiguration.Network.NetworkInterfaces = cmds.Network.Get();
            cmds.Network.SetTuns();
            cmds.Network.SetTaps();
            Brctl.Apply();
            Bond.Apply();
            RunningConfiguration.Network.NetworkInterfaces = cmds.Network.Get();
            cmds.Network.Prepare();
            WiFi.Apply();
            cmds.Network.Set();
            cmds.Network.ApplyNetwork(CurrentConfiguration.Network.InternalNetwork);
            cmds.Network.ApplyNetwork(CurrentConfiguration.Network.ExternalNetwork);
            Route.SetRoutingTable();
            Route.Set();
            ConsoleLogger.Log("[network] ready");
        }

        private static void Ntpd() {
            Ntp.Set();
            ConsoleLogger.Log("[ntp] ready");
        }

        private static void Firewall() {
            if(CurrentConfiguration.Services.Firewall != null && CurrentConfiguration.Services.Firewall.Active) {
                cmds.Firewall.Apply();
            }
        }

        private static void Dhcpd() {
            if(CurrentConfiguration.Services.Dhcpd.Active) {
                cmds.Dhcpd.Apply();
            }
        }

        private static void Bind() {
            if(CurrentConfiguration.Services.Bind.Active) {
                cmds.Bind.Apply();
            }
        }

        private static void ApplySetupConfiguration() {
            SetupCommands.Set();
        }

        private static void Nginx() {
            if(CurrentConfiguration.Services.Nginx.Active) {
                cmds.Nginx.Apply();
            }
        }

        private static void ManageSsh() {
            if(RunningConfiguration.Services.Sshd.Active) {
                Sshd.Set();
            }
            if(string.IsNullOrEmpty(RunningConfiguration.Services.Ssh.PublicKey)) {
                Ssh.CreateRootKeys();
            }
            CurrentConfiguration.Services.Ssh.PublicKey = Ssh.GetRootPublicKey();
            CurrentConfiguration.Services.Ssh.PrivateKey = Ssh.GetRootPrivateKey();
            RunningConfiguration.Services.Ssh.PublicKey = Ssh.GetRootPublicKey();
            RunningConfiguration.Services.Ssh.PrivateKey = Ssh.GetRootPrivateKey();
            ConsoleLogger.Log("[ssh] ready");
        }

        private static void Samba() {
            if(CurrentConfiguration.Services.Samba.Active) {
                cmds.Samba.Apply();
            }
        }

        private static void Syslog() {
            if(CurrentConfiguration.Services.SyslogNg.Active) {
                cmds.SyslogNg.Apply();
            }
        }

        private static void StorageZfs() {
            var pools = Zpool.GetImportPools();
            for(var i = 0; i < pools.Length; i++) {
                var currentPool = pools[i];
                Zpool.Import(currentPool);
                ConsoleLogger.Log($"[zpool] pool {currentPool} imported");
            }
            if(RunningConfiguration.Storage.Zpools.Length > 1 && RunningConfiguration.Storage.ZfsSnapshots.Length > 1) {
                Scheduler.ExecuteJob<ZfsSnapshotLaunchJob>();
                Scheduler.ExecuteJob<ZfsSnapshotCleanupJob>();
            }
        }

        private static void Ca() {
            if(CurrentConfiguration.Services.Ca.Active) {
                cmds.Ca.Apply();
            }
        }

        private static void Apps() {
            Applicative.Setup();
            Applicative.Start();
            ConsoleLogger.Log("[apps] ready");
        }

        private static void Rsync() {
            if(CurrentConfiguration.Services.Rsync.Active) {
                RsyncWatcher.Start();
            }
        }

        private static void VfsServer() {
            if(CurrentConfiguration.Storage.Server.Active) {
                Vfs.SetDefaults();
                ThreadPool.QueueUserWorkItem(new WaitCallback((state) => {
                    var srv = new Kvpbase.StorageServer(Vfs.GetSystemConfiguration());
                    try {
                        srv.Start();
                    }
                    catch(Exception) {

                    }
                }));
            }
        }

        private static void Tor() {
            if(CurrentConfiguration.Services.Tor.Active) {
                cmds.Tor.Apply();
            }
        }

        private static void ManageCluster() {
            if(CurrentConfiguration.Cluster.Active) {
                cmds.Cluster.Apply();
                ConsoleLogger.Log("[cluster] ready");
            }
        }

        private static void Gluster() {
            if(CurrentConfiguration.Services.Gluster.Active) {
                cmds.Gluster.Apply();
            }
        }

        private static void DirectoryWatchers() {
            SetupWatcher.Start();
        }

        private static void CheckApplicationFileAcls() {
            //var files = Directory.EnumerateFiles(Parameter.RepoApps, "*.squashfs.xz", SearchOption.AllDirectories);
            //foreach(var file in files) {
            //    Bash.Execute($"chmod 644 {file}");
            //    Bash.Execute($"chown root:wheel {file}");
            //}
            //ConsoleLogger.Log("[check] app-file acl");
        }

        private static void PrepareGuiService() {
            var hostReferenceFile = $"{Parameter.AntdCfg}/host_reference";
            var url = CommonString.Append(CurrentConfiguration.WebService.Protocol, "://", CurrentConfiguration.WebService.Host, ":", CurrentConfiguration.WebService.Port.ToString());
            File.WriteAllText(hostReferenceFile, url);
        }

        private static void StartRssdp() {
            RunningConfiguration.Network.Routing = Route.Get();
            if(RunningConfiguration.Network.Routing.Any()) {
                cmds.Rssdp.PublishThisDevice();
                ConsoleLogger.Log("[rssdp] published device");
            }
            else {
                ConsoleLogger.Log("[rssdp] cannot publish device: missing some route");
            }
        }

        private static void LaunchJobs() {
            Scheduler.ExecuteJob<ImportRunningConfigurationJob>();
            Scheduler.ExecuteJob<FetchRemoteCommandsJob>();
            Scheduler.ExecuteJob<UpdateCloudInfoJob>();
            Scheduler.ExecuteJob<UpdateRestAgentJob>();
            Scheduler.ExecuteJob<ClusterCheckHeartbeatJob>();
            Scheduler.ExecuteJob<MachineChecklistJob>();
        }

        private static void Test() {

        }
    }
}