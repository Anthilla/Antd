using System.Collections.Generic;

namespace antdlib.models {
    public class GlusterConfigurationModel {
        public bool IsActive { get; set; }
        /// <summary>
        /// Lista dei server/nodi
        /// es: srv01, srv02, srv03...
        /// </summary>
        public List<GlusterNode> Nodes { get; set; } = new List<GlusterNode>();

        /// <summary>
        /// Lista delle informazioni dei volumi da avviare/configurare sulla macchina
        /// </summary>
        public List<GlusterVolume> Volumes { get; set; } = new List<GlusterVolume>();
    }

    public class GlusterNode {
        public string Hostname { get; set; }
        public string Ip { get; set; }
    }

    public class GlusterVolume {
        /// <summary>
        /// Nome del volume di gluster
        /// es: gv01
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Nome del brick
        /// es: /Data/Storage02/Brick01
        /// </summary>
        public string Brick { get; set; }

        /// <summary>
        /// Mountpoint del volume sul fs locale
        /// es: /Data/gv01 o /cfg/sync
        /// </summary>
        public string MountPoint { get; set; }
    }
}
