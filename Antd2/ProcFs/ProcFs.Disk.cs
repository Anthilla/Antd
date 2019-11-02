using System.Collections.Generic;

namespace ProcFsCore
{
    public static partial class ProcFs
    {
        public static class Disk
        {
            public static IEnumerable<DiskStatistics> Statistics() => DiskStatistics.GetAll();
        }
    }
}