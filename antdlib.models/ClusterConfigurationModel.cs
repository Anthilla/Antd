using System.Collections.Generic;

namespace antdlib.models {
    public class Cluster {
        public class Configuration {
            public string Guid { get; set; }
            public string NetworkInterface { get; set; } // /etc/keepalived/keepalived.conf
            public string VirtualIpAddress { get; set; }
            public string Priority { get; set; } = "100";
            public List<PortMapping> PortMapping { get; set; } = new List<PortMapping>();
            public List<FileSystemMapping> FileSystemMapping { get; set; } = new List<FileSystemMapping>();
        }

        /// <summary>
        /// Virtual Port è la porta su cui viene pubblicato il servizio sull'ip virtuale
        /// ServicePort è la porta su cio viene pubblicato il servizio localmente
        /// Quindi quando verrà scritto il file di configurazione vado a estrarre la ServicePort dai servizi pubblicati dall'host
        /// es: da Antd api.Get("/device/services") => lista dei servizi
        /// poi estraggo il dato partendo dal ServiceName
        /// </summary>
        public class PortMapping {
            public string ServiceName { get; set; }
            public string VirtualPort { get; set; }
            public string ServicePort { get; set; }
        }

        /// <summary>
        /// 
        /// </summary>
        public class FileSystemMapping {
            public string ContentName { get; set; }
            public string LocalPath { get; set; }
        }

        public class DeployConf {
            public Configuration Configuration { get; set; }
            public List<NodeModel> Nodes { get; set; }
        }
    }
}