namespace antdlib.models {

    public class Cluster {

        public class Configuration {
            public string Guid { get; set; }
            public string Password { get; set; }
            public string NetworkInterface { get; set; } // /etc/keepalived/keepalived.conf
            public string VirtualIpAddress { get; set; }
        }

        public class Node {
            public string Hostname { get; set; }
            public string IpAddress { get; set; }
        }
    }
}