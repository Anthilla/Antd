using System.Collections.Generic;

namespace Antd.ProcFs {
    public static partial class ProcFs {
        public static class Cpu {
            public static IEnumerable<CpuStatistics> Statistics() => CpuStatistics.GetAll();
        }
    }
}