using System;

namespace Antd.Storage {
    public class ZfsSnapModel {
        public string Guid { get; set; }
        public string Name { get; set; }
        public string Used { get; set; }
        public string Available { get; set; }
        public string Refer { get; set; }
        public string Mountpoint { get; set; }
        public bool IsEmpty { get; set; }

        public int Index { get; set; }
        public DateTime Created { get; set; }
        public long Dimension { get; set; }
    }
}
