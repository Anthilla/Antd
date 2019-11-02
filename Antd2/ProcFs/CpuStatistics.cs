using System;
using System.Buffers.Text;
using System.Collections.Generic;

namespace ProcFsCore
{
    public struct CpuStatistics
    {
        private const string StatPath = ProcFs.RootPath + "/stat";
        
        public short? CpuNumber { get; private set; }
        public double UserTime { get; private set; }
        public double NiceTime { get; private set; }
        public double KernelTime { get; private set; }
        public double IdleTime { get; private set; }
        public double IrqTime { get; private set; }
        public double SoftIrqTime { get; private set; }

        private static readonly ReadOnlyMemory<byte> CpuNumberStart = "cpu".ToUtf8();
        internal static IEnumerable<CpuStatistics> GetAll()
        {
            var statReader = new Utf8FileReader<X4096>(StatPath);
            try
            {
                while (!statReader.EndOfStream)
                {
                    var cpuStr = statReader.ReadWord();
                    if (!cpuStr.StartsWith(CpuNumberStart.Span))
                        yield break;

                    var cpuNumberStr = cpuStr.Slice(CpuNumberStart.Length);
                    short? cpuNumber;
                    if (cpuNumberStr.Length == 0)
                        cpuNumber = null;
                    else
                    {
                        Utf8Parser.TryParse(cpuNumberStr, out short num, out _);
                        cpuNumber = num;
                    }

                    var ticksPerSecond = (double) ProcFs.TicksPerSecond;
                    var userTime = statReader.ReadInt64() / ticksPerSecond;
                    var niceTime = statReader.ReadInt64() / ticksPerSecond;
                    var kernelTime = statReader.ReadInt64() / ticksPerSecond;
                    var idleTime = statReader.ReadInt64() / ticksPerSecond;
                    statReader.SkipWord();
                    var irqTime = statReader.ReadInt64() / ticksPerSecond;
                    var softIrqTime = statReader.ReadInt64() / ticksPerSecond;

                    statReader.SkipLine();

                    yield return new CpuStatistics
                    {
                        CpuNumber = cpuNumber,
                        UserTime = userTime,
                        NiceTime = niceTime,
                        KernelTime = kernelTime,
                        IdleTime = idleTime,
                        IrqTime = irqTime,
                        SoftIrqTime = softIrqTime
                    };
                }
            }
            finally
            {
                statReader.Dispose();
            }
        }
    }
}