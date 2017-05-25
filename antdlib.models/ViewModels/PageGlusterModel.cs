using System.Collections.Generic;

namespace antdlib.models {
    public class PageGlusterModel {
        public bool GlusterIsActive { get; set; }
        public GlusterConfigurationModel Gluster { get; set; }
        public IEnumerable<GlusterNode> Nodes { get; set; }
        public IEnumerable<GlusterVolume> Volumes { get; set; }
    }
}
