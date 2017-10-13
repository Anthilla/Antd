using Antd.cmds;
using Antd.models;
using anthilla.core;
using anthilla.crypto;
using Newtonsoft.Json;
using System;
using System.IO;

namespace Antd {
    public class ConfigRepo {

        private const string fileName = "antd";
        private const string fileExtension = ".json";

        /// <summary>
        /// Salva su disco la configurazione
        /// </summary>
        /// <param name="data">Configurazione presa da Application.CurrentConfiguration</param>
        public static void Save() {
            var settings = new JsonSerializerSettings {
                ObjectCreationHandling = ObjectCreationHandling.Replace,
                Formatting = Formatting.Indented
            };
            var text = JsonConvert.SerializeObject(Application.CurrentConfiguration, settings);
            var filePath = CommonString.Append(Parameter.AntdCfg, "/", fileName, fileExtension);
            Backup();
            //if(File.Exists(filePath)) {
            //    var backupFilePath = CommonString.Append(Parameter.AntdCfg, "/", fileName, fileExtension, ".bck");
            //    File.Copy(filePath, backupFilePath, true);
            //}
            using(var file = File.CreateText(filePath)) {
                file.Write(text);
            }
        }

        /// <summary>
        /// Crea una copia di backup dell'ultima configurazione salvata
        /// </summary>
        public static void Backup() {
            Directory.CreateDirectory(Parameter.AntdCfgRestore);
            var filePath = CommonString.Append(Parameter.AntdCfg, "/", fileName, fileExtension);
            if(!File.Exists(filePath)) {
                return;
            }
            var version = DateTime.Now.ToString("yyyyMMddHHmmss");
            var backupFilePath = CommonString.Append(Parameter.AntdCfgRestore, "/", fileName, version, fileExtension);
            File.Copy(filePath, backupFilePath, true);
        }

        /// <summary>
        /// Importa la configurazione salvata su disco
        /// </summary>
        /// <returns></returns>
        public static MachineConfig Read() {
            var filePath = CommonString.Append(Parameter.AntdCfg, "/", fileName, fileExtension);
            if(!File.Exists(filePath)) {
                return null;
            }
            var text = File.ReadAllText(filePath);
            return JsonConvert.DeserializeObject<MachineConfig>(text);
        }

        /// <summary>
        /// Carica una configurazione precedentemente salvata tramite Backup()
        /// </summary>
        /// <param name="version">Versione della configurazione: data con formato 'yyyyMMddHHmmss'</param>
        /// <returns></returns>
        public static MachineConfig Read(string version) {
            var filePath = CommonString.Append(Parameter.AntdCfgRestore, "/", fileName, version, fileExtension);
            if(!File.Exists(filePath)) {
                return null;
            }
            var text = File.ReadAllText(filePath);
            return JsonConvert.DeserializeObject<MachineConfig>(text);
        }

        /// <summary>
        /// Ottiene un Master valorizzato coi valori ottenuti direttamente dal sistema operativo
        /// Potrebbe non corrispondere al Master salvato
        /// Viene utilizzato per ottenere le differenze tra i parametri salvati e quelli effettivamente applicati
        /// </summary>
        /// <returns></returns>
        public static MachineStatus GetRunning() {
            var STOPWATCH = new System.Diagnostics.Stopwatch();
            STOPWATCH.Start();
            var master = new MachineStatus();

            master.Info.Uptime = Uptime.Get();
            master.Info.CpuInfo = CpuInfo.Get();
            master.Info.MemInfo = MemInfo.Get();
            master.Info.Free = Free.Get();
            master.Info.Losetup = Losetup.Get();
            master.Info.DiskUsage = DiskUsage.Get();
            master.Info.Versions = Versioning.Get();

            master.Host = Hostnamectl.Get();
            master.TimeDate = new TimeDate() {
                Timezone = Timedatectl.Get().Timezone
            };
            master.Boot = new Boot();
            var modules = Mod.Get();
            var bootModules = new SystemModule[modules.Length];
            for(var i = 0; i < modules.Length; i++) {
                bootModules[i] = new SystemModule() { Module = modules[i].Module, Active = true };
            }
            master.Boot.Modules = bootModules;
            master.Boot.Services = cmds.Systemctl.GetAll();
            master.Boot.Parameters = Sysctl.Get();
            master.Users.SystemUsers = Passwd.Get();
            master.Users.ApplicativeUsers = new ApplicativeUser[] { new ApplicativeUser() { Active = true, Type = AuthenticationType.simple, Id = "master", Claims = new[] { SHA.Generate("master") } } };

            master.Network.KnownDns = Dns.GetResolv();
            master.Network.KnownHosts = Dns.GetHosts();
            master.Network.KnownNetworks = Dns.GetNetworks();
            master.Network.Bridges = Brctl.Get();
            master.Network.Bonds = Bond.Get();
            master.Network.NetworkInterfaces = cmds.Network.Get();
            master.Network.RoutingTables = Route.GetRoutingTable();
            master.Network.Routing = Route.Get();

            master.NsSwitch = NS.Switch.Get();

            master.Storage.Mounts = Mount.Get();
            master.Storage.Zpools = Zpool.GetPools();
            master.Storage.ZfsDatasets = Zfs.GetDatasets();
            master.Storage.ZfsSnapshots = Zfs.GetSnapshots();

            master.Services.Ssh.PublicKey = Ssh.GetRootPublicKey();
            master.Services.Ssh.PrivateKey = Ssh.GetRootPrivateKey();
            master.Services.Ssh.AuthorizedKey = Ssh.GetAuthorizedKey();

            master.Services.Virsh.Domains = Virsh.GetDomains();

            //ConsoleLogger.Log($"[conf] loaded running conf ({STOPWATCH.ElapsedMilliseconds})");
            return master;
        }
    }
}

