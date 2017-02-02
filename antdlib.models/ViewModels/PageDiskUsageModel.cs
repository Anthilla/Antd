using System.Collections.Generic;

namespace antdlib.models {
    public class PageDiskUsageModel {
        public IEnumerable<DiskUsageModel> DisksUsage { get; set; }
    }
}
