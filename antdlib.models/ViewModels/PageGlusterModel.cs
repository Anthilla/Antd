using System.Collections.Generic;

namespace antdlib.models {
    public class PageGlusterModel {
        public bool GlusterIsActive { get; set; }
        public IEnumerable<string> Nodes { get; set; }
        public IEnumerable<GlusterVolume> Volumes { get; set; }
    }
}
