using System.Collections.Generic;

namespace antdlib.models {
    public class TorConfigurationModel {
        public bool IsActive { get; set; }

        public List<TorService> Services { get; set; } = new List<TorService>();
    }

    public class TorService {
        public string Name { get; set; }
        public string IpAddress { get; set; }
        public string TorPort { get; set; }
    }
}