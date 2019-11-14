using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace antdlib.ProcFs {
    public static partial class ProcFs {
        internal const string RootPath = "/proc";
        private const string StatPath = RootPath + "/stat";

        public static readonly int TicksPerSecond = Native.SystemConfig(Native.SystemConfigName.TicksPerSecond);

        private static readonly TimeSpan BootTimeCacheInterval = TimeSpan.FromSeconds(0.5);
        private static DateTime? _bootTimeUtc;
        private static readonly Stopwatch BootTimeCacheTimer = new Stopwatch();
        private static readonly ReadOnlyMemory<byte> BtimeStr = "btime ".ToUtf8();
        public static DateTime BootTimeUtc {
            get {
                lock (BootTimeCacheTimer) {
                    if (_bootTimeUtc == null || BootTimeCacheTimer.Elapsed > BootTimeCacheInterval) {
                        var statReader = new Utf8FileReader<X4096>(StatPath);
                        try {
                            statReader.SkipFragment(BtimeStr.Span, true);
                            if (statReader.EndOfStream)
                                throw new NotSupportedException();

                            var bootTimeSeconds = statReader.ReadInt64();
                            _bootTimeUtc = DateTime.UnixEpoch + TimeSpan.FromSeconds(bootTimeSeconds);
                            BootTimeCacheTimer.Restart();
                        }
                        finally {
                            statReader.Dispose();
                        }
                    }

                    return _bootTimeUtc.Value;
                }
            }
        }

        public static IEnumerable<Process> Processes() {
            foreach (var pidPath in Directory.EnumerateDirectories(RootPath)) {
                var pidDir = Path.GetFileName(pidPath);
                if (Int32.TryParse(pidDir, out var pid))
                    yield return new Process(pid);
            }
        }
    }
}
