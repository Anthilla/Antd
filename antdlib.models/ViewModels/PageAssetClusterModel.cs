using System.Collections.Generic;

namespace antdlib.models {
    public class PageAssetClusterModel {
        public Cluster.Configuration Info { get; set; }
        public List<NodeModel> ClusterNodes { get; set; }
        public List<string> NetworkAdapters { get; set; }
    }
}
