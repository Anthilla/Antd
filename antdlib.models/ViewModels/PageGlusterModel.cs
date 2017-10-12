namespace antdlib.models {
    public class PageGlusterModel {
        public bool GlusterIsActive { get; set; }
        public GlusterConfigurationModel Gluster { get; set; }

        public string GlusterPeerStatus { get; set; }
        public string GlusterVolumeStatus { get; set; }
    }
}
