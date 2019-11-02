using System;
using System.Buffers.Text;
using System.Collections.Generic;
using System.Linq;

namespace Antd.ProcFs {
    public struct MemoryStatistics {
        private const string StatPath = ProcFs.RootPath + "/meminfo";

        public long Total { get; private set; }
        public long Available { get; private set; }
        public long Free { get; private set; }
        public long SwapTotal { get; private set; }
        public long SwapFree { get; private set; }

        private static readonly List<ReadOnlyMemory<byte>> Names = Enum.GetNames(typeof(Section)).Select(n => n.ToUtf8()).ToList();

        internal static unsafe MemoryStatistics Get() {
            var statReader = new Utf8FileReader<X2048>(StatPath);
            try {
                var sections = stackalloc long[(int)Section.Max];
                for (var i = 0; i < (int)Section.Max; ++i)
                    sections[i] = -1;

                var sectionsRead = 0;
                while (sectionsRead < (int)Section.Max) {
                    var section = statReader.ReadLine();

                    var nameEnd = section.IndexOf(':');
                    var name = section.Slice(0, nameEnd);


                    var valueStart = nameEnd + 1;
                    while (section[valueStart] == ' ')
                        ++valueStart;
                    var valueEnd = section.IndexOf(' ', valueStart);
                    if (valueEnd < 0)
                        valueEnd = section.Length;
                    Utf8Parser.TryParse(section.Slice(valueStart, valueEnd - valueStart), out long value, out _);
                    value *= 0x400;

                    for (Section sectionType = default; sectionType < Section.Max; ++sectionType)
                        if (Names[(int)sectionType].Span.SequenceEqual(name)) {
                            sections[(int)sectionType] = value;
                            break;
                        }

                    sectionsRead = 0;
                    for (var i = 0; i < (int)Section.Max; ++i)
                        if (sections[i] >= 0)
                            ++sectionsRead;
                }

                return new MemoryStatistics {
                    Total = sections[(int)Section.MemTotal],
                    Free = sections[(int)Section.MemFree],
                    Available = sections[(int)Section.MemAvailable],
                    SwapTotal = sections[(int)Section.SwapTotal],
                    SwapFree = sections[(int)Section.SwapFree]
                };
            }
            finally {
                statReader.Dispose();
            }
        }

        private enum Section {
            MemTotal,
            MemFree,
            MemAvailable,
            SwapTotal,
            SwapFree,
            Max
        }
    }
}