using System.Collections.Generic;

namespace Antd.Rsync {
    public class RsyncConfigurationModel {
        public bool IsActive { get; set; }
        public List<RsyncObjectModel> Directories { get; set; } = new List<RsyncObjectModel>();
    }

    public class RsyncObjectModel {
        public string Guid { get; set; } = System.Guid.NewGuid().ToString();
        public string Type { get; set; }
        public string Source { get; set; }
        public string Destination { get; set; }
    }
}