using System.Collections.Generic;

namespace antdlib.models {
    public class PageOverlayModel {
        public IEnumerable<PageOverlayDirectoryModel> Directories { get; set; }
    }

    public class PageOverlayDirectoryModel {
        public string Path { get; set; }
        public string Dimension { get; set; }
    }
}
