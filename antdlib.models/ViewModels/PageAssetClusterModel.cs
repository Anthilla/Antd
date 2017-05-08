using System.Collections.Generic;

namespace antdlib.models {
    public class PageAssetClusterModel {
        public Cluster.Configuration Info { get; set; }
        public List<Cluster.Node> ClusterNodes { get; set; }
    }
}
