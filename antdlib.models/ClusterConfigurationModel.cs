using System.Collections.Generic;

namespace antdlib.models {
    public class Cluster {
        public class Configuration {
            public string Guid { get; set; }
            public string NetworkInterface { get; set; } // /etc/keepalived/keepalived.conf
            public string VirtualIpAddress { get; set; }
            public string Priority { get; set; } = "100";
            public List<PortMapping> PortMapping { get; set; } = new List<PortMapping>();
        }

        public class PortMapping {
            public string VirtualPort { get; set; }
            public string ServicePort { get; set; }
        }

        public class Node {
            public string Hostname { get; set; }
            public string IpAddress { get; set; }
        }
    }
}