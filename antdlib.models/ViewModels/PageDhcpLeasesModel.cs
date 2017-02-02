using System.Collections.Generic;

namespace antdlib.models {
    public class PageDhcpLeasesModel {
        public bool EmptyList { get; set; }
        public IEnumerable<DhcpdLeaseModel> DhcpdLeases { get; set; }
    }
}
