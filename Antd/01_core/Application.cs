using Antd.cmds;
using Antd.models;
using Antd.Mqtt;
using anthilla.core;
using anthilla.crypto;
using anthilla.scheduler;
using MQTTnet;
using MQTTnet.Core;
using MQTTnet.Core.Client;
using MQTTnet.Core.Server;
using Nancy;
using Nancy.Hosting.Self;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Antd {

    /// <summary>
    /// TODO
    /// - conf (current & running)
    ///     - aggiornare i valori in maniera parziale/contestuale per ridurre il carico
    ///     - aggiungere al Modulo Nancy il get parziale/contestuale
    /// - aggiungere configurazione Storage > Retention/Gestione Spazio Libero
    /// - consulta log -> journalctl / syslog
    /// - re-implementa "rsync"
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

        public static LibvirtWatcher LIBVIRT_WATCHER;
        public static IMqttClient MQTTCLIENT;
        public static MachineIdStatus MACHINE_ID;
        private static bool _connected_to_cloud = false;

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
            Tor();
            ManageVirsh();
            ManageMQTT().GetAwaiter().GetResult();
            ManageCluster();
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
            //ConnectToCloudViaMqttAsync().GetAwaiter().GetResult();
            StartCloudUpdateJob();
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
                    File.AppendAllLines(limitsFile, new[] { "root - nofile 1024000" });
                }
            }
            Bash.Execute("ulimit -n 1024000", false);
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
            if(!File.Exists($"{Const.AntdCfgSetup}/setup.conf")) {
                File.WriteAllText($"{Const.AntdCfgSetup}/setup.conf", "echo Hello World!");
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
            //if(Directory.Exists(Const.Overlay)) {
            //    new OverlayWatcher().StartWatching();
            //    ConsoleLogger.Log("overlay watcher ready");
            //}
        }

        private static void Mounts() {
            Mount.Set();
            ConsoleLogger.Log("[mounts] ready");
        }

        private static void CheckUnitsLocation() {
            if(!Directory.Exists(Const.AnthillaUnits)) { return; }
            if(!Directory.Exists(Const.AntdUnits)) { return; }
            var anthillaUnits = Directory.EnumerateFiles(Const.AnthillaUnits, "*.*", SearchOption.TopDirectoryOnly);
            if(!anthillaUnits.Any()) {
                var antdUnits = Directory.EnumerateFiles(Const.AntdUnits, "*.*", SearchOption.TopDirectoryOnly);
                foreach(var unit in antdUnits) {
                    var trueUnit = unit.Replace(Const.AntdUnits, Const.AnthillaUnits);
                    if(!File.Exists(trueUnit)) {
                        File.Copy(unit, trueUnit);
                    }
                    File.Delete(unit);
                    Bash.Execute($"ln -s {trueUnit} {unit}");
                }
                var kernelUnits = Directory.EnumerateFiles(Const.KernelUnits, "*.*", SearchOption.TopDirectoryOnly);
                foreach(var unit in kernelUnits) {
                    var trueUnit = unit.Replace(Const.KernelUnits, Const.AnthillaUnits);
                    if(!File.Exists(trueUnit)) {
                        File.Copy(unit, trueUnit);
                    }
                    File.Delete(unit);
                    Bash.Execute($"ln -s {trueUnit} {unit}");
                }
                var applicativeUnits = Directory.EnumerateFiles(Const.ApplicativeUnits, "*.*", SearchOption.TopDirectoryOnly);
                foreach(var unit in applicativeUnits) {
                    var trueUnit = unit.Replace(Const.ApplicativeUnits, Const.AnthillaUnits);
                    if(!File.Exists(trueUnit)) {
                        File.Copy(unit, trueUnit);
                    }
                    File.Delete(unit);
                    Bash.Execute($"ln -s {trueUnit} {unit}");
                }
            }
            //anthillaUnits = Directory.EnumerateFiles(Const.AnthillaUnits, "*.*", SearchOption.TopDirectoryOnly).ToList();
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
            if(!File.Exists(Const.AntdCfgSecret)) {
                File.WriteAllText(Const.AntdCfgSecret, Secret.Gen());
            }
            if(string.IsNullOrEmpty(File.ReadAllText(Const.AntdCfgSecret))) {
                File.WriteAllText(Const.AntdCfgSecret, Secret.Gen());
            }
        }

        private static void License() {
            Keys = new AsymmetricKeys(Const.AntdCfgKeys, KeyName);
            ConsoleLogger.Log($"[part_number] {CurrentConfiguration.Host.PartNumber}");
            ConsoleLogger.Log($"[serial_number] {CurrentConfiguration.Host.SerialNumber}");
            ConsoleLogger.Log($"[machine_id] {CurrentConfiguration.Host.MachineUid}");
            LicenseManagement.Download("Antd", Keys.PublicKey);
            var licenseStatus = LicenseManagement.Check("Antd", Keys.PublicKey);
            ConsoleLogger.Log(licenseStatus == null
                ? "[license] license results null"
                : $"[license] {licenseStatus.Status} - {licenseStatus.Message}");

            MACHINE_ID = new MachineIdStatus() {
                MachineUid = CurrentConfiguration.Host.MachineUid,
                PartNumber = CurrentConfiguration.Host.PartNumber,
                SerialNumber = CurrentConfiguration.Host.SerialNumber
            };
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
                SyslogNg.Apply();
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

        private static void Tor() {
            if(CurrentConfiguration.Services.Tor.Active) {
                cmds.Tor.Apply();
            }
        }

        private static void ManageVirsh() {
            Virsh.PrepareDirectory();
            if(CurrentConfiguration.Services.Virsh.Active) {
                Virsh.StartAll();
            }
        }

        public static IMqttServer MQTT;
        public static ClusterNode[] CLUSTER_NODES;
        public static MqttSyncClient[] CLUSTER_MQTT_CLIENTS;
        public const int MQTT_DEFAULT_PORT = 31883;

        private static async Task ManageMQTT() {
            //await MqttHandler.MqttServerSetupForCluster();
        }

        public const int STORAGESERVER_DEFAULT_PORT = 38008;

        private static void ManageCluster() {
            if(CurrentConfiguration.Cluster.Active) {
                cmds.ClusterSetup.ApplyNetwork();
                cmds.ClusterSetup.ApplyServices();
                cmds.ClusterSetup.ApplyFs();

                if(File.Exists("/cfg/antd/vfs/system.json")) {
                    var settings = Kvpbase.Settings.FromFile("/cfg/antd/vfs/system.json");
                    var ss = new Kvpbase.StorageServer(settings);
                    ss.Start();
                }

                ConsoleLogger.Log("[cluster] ready");
            }
        }

        private static void DirectoryWatchers() {
            SetupWatcher.Start();
        }

        private static void CheckApplicationFileAcls() {
            //var files = Directory.EnumerateFiles(Const.RepoApps, "*.squashfs.xz", SearchOption.AllDirectories);
            //foreach(var file in files) {
            //    Bash.Execute($"chmod 644 {file}");
            //    Bash.Execute($"chown root:wheel {file}");
            //}
            //ConsoleLogger.Log("[check] app-file acl");
        }

        private static void PrepareGuiService() {
            var hostReferenceFile = $"{Const.AntdCfg}/host_reference";
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
            Scheduler.ExecuteJob<ScanRssdpServices>();
        }

        private static void LaunchJobs() {
            Scheduler.ExecuteJob<ImportRunningConfigurationJob>();
            Scheduler.ExecuteJob<UpdateRestAgentJob>();
            Scheduler.ExecuteJob<ClusterCheckHeartbeatJob>();
            Scheduler.ExecuteJob<ClusterCheckHostnameJob>();
            Scheduler.ExecuteJob<MachineChecklistJob>();
        }

        private static async Task ConnectToCloudViaMqttAsync() {
            if(Const.IsUnix == false) {
                return;
            }
            var factory = new MqttFactory();
            MQTTCLIENT = factory.CreateMqttClient();
            var clientOptions = new MqttClientOptionsBuilder()
                .WithClientId(CurrentConfiguration.Host.MachineUid.ToString())
                .WithTcpServer(CurrentConfiguration.WebService.Cloud, CurrentConfiguration.WebService.CloudPort)
                .WithCredentials(CurrentConfiguration.WebService.CloudUser, CurrentConfiguration.WebService.CloudPassword)
                .WithCleanSession()
                .Build();
            MQTTCLIENT.Connected += async (s, e) => {
                await MQTTCLIENT.SubscribeAsync(new TopicFilterBuilder().WithTopic("/status").Build());
                await MQTTCLIENT.SubscribeAsync(
                    new TopicFilterBuilder()
                    .WithTopic($"/control/{CurrentConfiguration.Host.MachineUid}/{CurrentConfiguration.Host.PartNumber}/{CurrentConfiguration.Host.SerialNumber}")
                    .Build()
                    );
                ConsoleLogger.Log($"[mqtt] connected to {CurrentConfiguration.WebService.Cloud}");
                _connected_to_cloud = true;
            };
            MQTTCLIENT.Disconnected += async (s, e) => {
                _connected_to_cloud = false;
                await Task.Delay(TimeSpan.FromSeconds(5));
                try {
                    await MQTTCLIENT.ConnectAsync(clientOptions);
                }
                catch {
                    ConsoleLogger.Error("[mqtt] unable to reconnect");
                }
            };
            MQTTCLIENT.ApplicationMessageReceived += (s, e) => {
                Cloud.ParsePayload(e.ApplicationMessage.Topic, e.ApplicationMessage.Payload);
            };
            await MQTTCLIENT.ConnectAsync(clientOptions);

            Scheduler = new JobManager();
            Scheduler.ExecuteJob<SendInfoToCloudJob>();
        }

        private static void StartCloudUpdateJob() {
            if(Const.IsUnix == false) {
                return;
            }
            if(_connected_to_cloud) {
                Scheduler.ExecuteJob<SendInfoToCloudJob>();
            }
        }

        private static void Test() {
            //var testNode = new ClusterNode() { EntryPoint = "http://127.0.0.1:8084/" };
            //StorageClient.GetFolder(testNode, "/");
            //Thread thread = new Thread(() => {
            //    Ip.EnableNetworkAdapter("eth0");
            //});
            //thread.Start();
        }
    }
}