using System.Collections.Generic;
using anthilla.core;
using System.Linq;

namespace Antd.cmds {

    public class GlusterFs {

        //systemctl enable glusterd
        //systemctl start glusterd
        //gluster peer probe avm702
        //gluster volume create GlusterE replica 2 transport tcp avm701.local:/Data/DataE avm702.local:/Data/DataE force
        //gluster volume start GlusterE
        //mkdir -p /Data/GData
        //mount -t glusterfs avm702:GlusterE /Data/GData

        private const string serviceName = "glusterd.service";
        private const string glusterFileLocation = "/usr/sbin/gluster";
        private const string includeNodeArg = "peer probe";

        public static void Stop() {
            Systemctl.Stop(serviceName);
            ConsoleLogger.Log("[gluster] stop");
        }

        public static void Set() {
            var options = Application.CurrentConfiguration.Cluster.SharedFs;
            if(options == null) {
                return;
            }
            Systemctl.Enable(serviceName);
            Systemctl.Start(serviceName);
            var nodes = Application.CurrentConfiguration.Cluster.Nodes;
            for(var i = 0; i < nodes.Length; i++) {
                IncludeNode(nodes[i].Hostname);
            }

            for(var i = 0; i < options.VolumesLabels.Length; i++) {
                var currentLabel = options.VolumesLabels[i];
                //creo e avvio il volume di Gluster sui vari nodi in cui è configurato
                StartVolume(currentLabel, nodes);
            }
            ConsoleLogger.Log("[gluster] start");
        }

        private static void IncludeNode(string nodeName) {
            var args = CommonString.Append(includeNodeArg, " ", nodeName);
            CommonProcess.Do(glusterFileLocation, args);
        }

        private static void StartVolume(string volumeLabel, ClusterNode[] nodes) {
            ConsoleLogger.Log($"[gluster] create {volumeLabel}");
            int volumeCount = 0;
            string replicaString = "";
            List<ClusterNode> activeNodes = new List<ClusterNode>();
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
            var glusterCreateArg = CommonString.Append("volume create ", volumeLabel, " replica ", volumeCount.ToString(), " transport tcp ", replicaString, " force");
            CommonProcess.Do(glusterFileLocation, glusterCreateArg);
            System.Threading.Thread.Sleep(500);
            ConsoleLogger.Log($"[gluster] gluster volume start {volumeLabel}");
            var glusterStartArg = CommonString.Append("volume start ", volumeLabel);
            CommonProcess.Do(glusterFileLocation, glusterStartArg);
            //a questo punto posso montare il volume di Gluster sul filesystem, su ogni nodo
            MountVolume(volumeLabel, activeNodes.ToArray());
        }

        private static void MountVolume(string volumeLabel, ClusterNode[] nodes) {
            //ogni nodo monterà sul proprio filesystem il volume di gluster configurato su se stesso
            //i nodi in questo caso so già che conterranno le informazioni del volume
            for(var i = 0; i < nodes.Length; i++) {
                var currentVolume = nodes[i].Volumes.FirstOrDefault(_ => _.Label == volumeLabel);
                //per evitare errori controllo che ci siano le info del volume
                if(currentVolume == null) {
                    continue;
                }
                //poi lancio i comandi ssh per creare la cartella e montarla
                ConsoleLogger.Log($"[gluster] ssh root@{nodes[i].Hostname} mkdir -p {currentVolume.MountPoint}");
                var prepareMountPointCommand = CommonString.Append("mkdir -p ", currentVolume.MountPoint);
                Ssh.Do("root", nodes[i].Hostname, prepareMountPointCommand);
                ConsoleLogger.Log($"[gluster] ssh root@{nodes[i].Hostname} mount -t glusterfs {nodes[i].Hostname}:{volumeLabel} {currentVolume.MountPoint}");
                var mountVolumeCommand = CommonString.Append("mount -t glusterfs ", nodes[i].Hostname, ":", volumeLabel, " ", currentVolume.MountPoint);
                Ssh.Do("root", nodes[i].Hostname, mountVolumeCommand);
            }
        }
    }
}