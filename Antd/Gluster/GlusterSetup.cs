using System.Collections.Generic;

namespace Antd.Gluster {
    public class GlusterSetup {
        public string Name { get; set; }
        public string Path { get; set; }
        public bool IsConfigured { get; set; }
        /// <summary>
        /// Lista dei server/nodi
        /// es: srv01, srv02, srv03...
        /// </summary>
        public List<string> Nodes { get; set; }

        /// <summary>
        /// Lista delle informazioni dei volumi da avviare/configurare sulla macchina
        /// </summary>
        public List<GfsVolume> Volumes { get; set; }
    }

    public class GfsVolume {
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
        /// es: /Data/gv01
        /// </summary>
        public string MountPoint { get; set; }
    }
}
