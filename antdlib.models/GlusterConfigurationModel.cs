namespace antdlib.models {

    //systemctl enable glusterd
    //systemctl start glusterd
    //gluster peer probe avm702
    //gluster volume create GlusterE replica 2 transport tcp avm701.local:/Data/DataE avm702.local:/Data/DataE force
    //gluster volume start GlusterE
    //mkdir -p /Data/GData
    //mount -t glusterfs avm702:GlusterE /Data/GData

    public class GlusterConfigurationModel {
        /// <summary>
        /// Lista delle etichette dei volumi configurati
        /// per ognuno di questi valori vado a prendere le informazioni dei volumi configurati in Gluster2ConfigurationModel.Nodes.Volumes
        /// in modo da avere: nome del volume, percorso del brick in ogni nodo e infine il mountpoint
        /// </summary>
        public string[] VolumesLabels { get; set; } = new string[0];

        /// <summary>
        /// Lista dei nodi/peer di Gluster
        /// </summary>
        public GlusterNode[] Nodes { get; set; } = new GlusterNode[0];
    }

    public class GlusterNode {
        /// <summary>
        /// Hostname del nodo Gluster, dovrà essere presente in /etc/hosts
        /// </summary>
        public string Hostname { get; set; }

        public GlusterVolume[] Volumes { get; set; }
    }

    public class GlusterVolume {
        /// <summary>
        /// Nome del volume di gluster, sarà compreso nell'array di Gluster2ConfigurationModel.VolumesLabels
        /// </summary>
        public string Label { get; set; }

        /// <summary>
        /// Percorso del 'brick' di Gluster
        /// es: /Data/Storage02/Brick01
        /// </summary>
        public string Brick { get; set; }

        /// <summary>
        /// Mountpoint del volume sul fs dell'host
        /// es: /Data/gv01 o /cfg/sync
        /// </summary>
        public string MountPoint { get; set; }
    }
}
