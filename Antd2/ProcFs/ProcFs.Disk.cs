using System.Collections.Generic;

namespace Antd.ProcFs {
    public static partial class ProcFs {
        public static class Disk {
            public static IEnumerable<DiskStatistics> Statistics() => DiskStatistics.GetAll();
        }
    }
}