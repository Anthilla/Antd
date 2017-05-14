using System.Collections.Generic;

namespace antdlib.models {
    public class PageTorModel {
        public bool TorIsActive { get; set; }
        public List<TorService> Services { get; set; }
    }
}
