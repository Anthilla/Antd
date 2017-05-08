using System.Collections.Generic;

namespace antdlib.models {

    public class Cluster {

        public class Configuration {
            public string Guid { get; set; }
            public string Password { get; set; }
            public string VirtualIpAddress { get; set; }
        }

        public class Node {
            public string Hostname { get; set; }
            public string IpAddress { get; set; }
            public List<Service> Services { get; set; }


            public class Service {
                public string Name { get; set; }
                public string Port { get; set; }
            }
        }
    }
}