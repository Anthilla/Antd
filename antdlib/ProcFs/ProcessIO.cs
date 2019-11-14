using System;

namespace antdlib.ProcFs {
    public struct ProcessIO {
        public readonly Direction Read;
        public readonly Direction Write;

        private ProcessIO(in Direction read, in Direction write) {
            Read = read;
            Write = write;
        }

        private static readonly ReadOnlyMemory<byte> StatNameSeparators = ": ".ToUtf8();
        public static ProcessIO Get(int pid) {
            var statReader = new Utf8FileReader<X256>($"{ProcFs.RootPath}/{pid}/io");
            try {
                statReader.SkipFragment(StatNameSeparators.Span);
                var readCharacters = statReader.ReadInt64();
                statReader.SkipFragment(StatNameSeparators.Span);
                var writeCharacters = statReader.ReadInt64();
                statReader.SkipFragment(StatNameSeparators.Span);
                var readSysCalls = statReader.ReadInt64();
                statReader.SkipFragment(StatNameSeparators.Span);
                var writeSysCalls = statReader.ReadInt64();
                statReader.SkipFragment(StatNameSeparators.Span);
                var readBytes = statReader.ReadInt64();
                statReader.SkipFragment(StatNameSeparators.Span);
                var writeBytes = statReader.ReadInt64();

                return new ProcessIO(new Direction(readCharacters, readBytes, readSysCalls),
                    new Direction(writeCharacters, writeBytes, writeSysCalls));
            }
            finally {
                statReader.Dispose();
            }
        }

        public struct Direction {
            public long Characters { get; }
            public long Bytes { get; }
            public long SysCalls { get; }

            public Direction(long characters, long bytes, long sysCalls) {
                Characters = characters;
                Bytes = bytes;
                SysCalls = sysCalls;
            }
        }
    }
}