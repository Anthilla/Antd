using System.Collections.Generic;

namespace Antd.Rsync {
    public class RsyncConfigurationModel {
        public bool IsActive { get; set; }
        public List<RsyncDirectoriesModel> Directories { get; set; } = new List<RsyncDirectoriesModel>();
    }

    public class RsyncDirectoriesModel {
        public string Guid { get; set; } = System.Guid.NewGuid().ToString();
        public string Source { get; set; }
        public string Destination { get; set; }
    }
}