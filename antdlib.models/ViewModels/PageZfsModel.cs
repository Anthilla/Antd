using System.Collections.Generic;

namespace antdlib.models {
    public class PageZfsModel {
        public IEnumerable<ZpoolModel> ZpoolList { get; set; }
        public IEnumerable<ZfsModel> ZfsList { get; set; }
        public IEnumerable<ZfsSnapModel> ZfsSnap { get; set; }
        public IEnumerable<string> ZpoolHistory { get; set; }
        public IEnumerable<DiskModel> DisksList { get; set; }
        public IEnumerable<string> DisksListById { get; set; }
    }
}
