using System.Collections.Generic;

namespace antdlib.models {
    public class PageRsyncModel {
        public bool RsyncIsActive { get; set; }
        public IEnumerable<RsyncObjectModel> RsyncDirectories { get; set; }
    }
}
