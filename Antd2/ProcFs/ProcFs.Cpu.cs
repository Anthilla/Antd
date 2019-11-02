using System.Collections.Generic;

namespace ProcFsCore
{
    public static partial class ProcFs
    {
        public static class Cpu
        {
            public static IEnumerable<CpuStatistics> Statistics() => CpuStatistics.GetAll();
        }
    }
}