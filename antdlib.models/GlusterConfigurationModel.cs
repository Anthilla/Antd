namespace antdlib.models {
    public class GlusterConfigurationModel {
        public bool IsActive { get; set; }
        /// <summary>
        /// Lista dei server/nodi
        /// es: srv01, srv02, srv03...
        /// </summary>
        public string[] Nodes { get; set; }
        /// <summary>
        /// Lista delle informazioni dei volumi da avviare/configurare sulla macchina
        /// </summary>
        public GlusterVolume[] Volumes { get; set; }
    }

    public class GlusterVolume {
        public string Guid { get; set; } = System.Guid.NewGuid().ToString();
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
