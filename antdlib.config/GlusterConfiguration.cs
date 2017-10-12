using antdlib.models;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using anthilla.core;

namespace antdlib.config {
    public static class GlusterConfiguration {

        private static readonly string CfgFile = $"{Parameter.AntdCfgCluster}/gluster.conf";
        private const string ServiceName = "glusterd.service";

        public static GlusterConfigurationModel Conf() {
            if(!File.Exists(CfgFile)) {
                return new GlusterConfigurationModel();
            }
            var text = File.ReadAllText(CfgFile);
            var obj = JsonConvert.DeserializeObject<GlusterConfigurationModel>(text);
            return obj;
        }

        public static void Save(GlusterConfigurationModel model) {
            var text = JsonConvert.SerializeObject(model, Formatting.Indented);
            FileWithAcl.WriteAllText(CfgFile, text, "644", "root", "wheel");
            ConsoleLogger.Log("[gluster] configuration saved");
        }

        public static bool IsActive() {
            if(!File.Exists(CfgFile)) {
                return false;
            }
            return Conf().Nodes.Length > 0 && Conf().VolumesLabels.Length > 0;
        }

        //systemctl enable glusterd
        //systemctl start glusterd
        //gluster peer probe avm702
        //gluster volume create GlusterE replica 2 transport tcp avm701.local:/Data/DataE avm702.local:/Data/DataE force
        //gluster volume start GlusterE
        //mkdir -p /Data/GData
        //mount -t glusterfs avm702:GlusterE /Data/GData

        public static void Start() {
            if(!File.Exists(CfgFile)) {
                return;
            }
            Systemctl.Enable(ServiceName);
            Systemctl.Start(ServiceName);

            var config = Conf();
            for(var i = 0; i < config.Nodes.Length; i++) {
                IncludeNode(config.Nodes[i].Hostname);
            }

            for(var i = 0; i < config.VolumesLabels.Length; i++) {
                var currentLabel = config.VolumesLabels[i];
                //creo e avvio il volume di Gluster sui vari nodi in cui è configurato
                StartVolume(currentLabel, config.Nodes);
            }

            ConsoleLogger.Log("[gluster] check config");
        }

        private static void IncludeNode(string node) {
            Bash.Execute($"gluster peer probe {node}", false);
        }

        private static void StartVolume(string volumeLabel, GlusterNode[] nodes) {
            ConsoleLogger.Log($"[gluster] create {volumeLabel}");
            int volumeCount = 0;
            string replicaString = "";
            List<GlusterNode> activeNodes = new List<GlusterNode>();
            for(var i = 0; i < nodes.Length; i++) {
                var currentNode = nodes[i];
                var currentVolume = currentNode.Volumes.FirstOrDefault(_ => _.Label == volumeLabel);
                if(currentVolume != null) {
                    //qui  ho trovato all'interno della conf del nodo una conf del volume corrispondente all'etichetta presa in considerazione
                    //quindi prendo queste info relative all'host e al suo volume per comporre la stringa di creazione del volume stesso
                    replicaString += $"{currentNode.Hostname}:{currentVolume.Brick} ";
                    //e incremento di 1 il counter, sempre per comporre il comando di creazione del vol
                    volumeCount = volumeCount + 1;
                    activeNodes.Add(currentNode);
                }
            }

            if(volumeCount == 0) {
                //non ci sono volumi configurati... evito possibili errori
                return;
            }

            //creo il volume di gluster e lo avvio
            ConsoleLogger.Log($"[gluster] gluster volume create {volumeLabel} replica {volumeCount} transport tcp {replicaString} force");
            Bash.Execute($"gluster volume create {volumeLabel} replica {volumeCount} transport tcp {replicaString} force", false);
            System.Threading.Thread.Sleep(500);
            ConsoleLogger.Log($"[gluster] gluster volume start {volumeLabel}");
            Bash.Execute($"gluster volume start {volumeLabel}", false);

            //a questo punto posso montare il volume di Gluster sul filesystem, su ogni nodo
            MountVolume(volumeLabel, activeNodes.ToArray());
        }

        private static void MountVolume(string volumeLabel, GlusterNode[] nodes) {
            //ogni nodo monterà sul proprio filesystem il volume di gluster configurato su se stesso
            //i nodi in questo caso so già che conterranno le informazioni del volume

            for(var i = 0; i < nodes.Length; i++) {
                var currentNode = nodes[i];
                var currentVolume = currentNode.Volumes.FirstOrDefault(_ => _.Label == volumeLabel);
                if(currentVolume != null) {
                    //per evitare errori controllo che ci siano le info del volume
                    //poi dovrò lanciare i comandi ssh per creare la cartella e montarla
                    ConsoleLogger.Log($"[gluster] ssh root@{currentNode.Hostname} mkdir -p {currentVolume.MountPoint}");
                    Bash.Execute($"ssh root@{currentNode.Hostname} mkdir -p {currentVolume.MountPoint}", false);
                    ConsoleLogger.Log($"[gluster] ssh root@{currentNode.Hostname} mount -t glusterfs {currentNode.Hostname}:{volumeLabel} {currentVolume.MountPoint}");
                    Bash.Execute($"ssh root@{currentNode.Hostname} mount -t glusterfs {currentNode.Hostname}:{volumeLabel} {currentVolume.MountPoint}", false);
                }
            }
        }
    }
}
