using System.Collections.Generic;

namespace antdlib.ProcFs {
    public static partial class ProcFs {
        public static class Cpu {
            public static IEnumerable<CpuStatistics> Statistics() => CpuStatistics.GetAll();
        }
    }
}