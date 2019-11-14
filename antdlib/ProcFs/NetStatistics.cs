using System;
using System.Collections.Generic;

namespace antdlib.ProcFs {
    public struct NetStatistics {
        private const string NetDevPath = ProcFs.RootPath + "/net/dev";

        private static readonly int ReceiveColumnCount;

        static NetStatistics() {
            var statReader = new Utf8FileReader<X512>(NetDevPath);
            try {
                statReader.SkipLine();
                statReader.SkipFragment('|');
                statReader.SkipWhiteSpaces();
                var receiveColumnCount = 0;
                while (true) {
                    var column = statReader.ReadWord();
                    if (column.IndexOf('|') >= 0) {
                        if (column.Length != 0)
                            ++receiveColumnCount;
                        break;
                    }
                    ++receiveColumnCount;
                }
                ReceiveColumnCount = receiveColumnCount;
            }
            finally {
                statReader.Dispose();
            }
        }

        public string InterfaceName { get; }
        public readonly Direction Receive;
        public readonly Direction Transmit;

        private NetStatistics(string interfaceName, in Direction receive, in Direction transmit) {
            InterfaceName = interfaceName;
            Receive = receive;
            Transmit = transmit;
        }

        private static readonly ReadOnlyMemory<byte> InterfaceNameSeparators = ": ".ToUtf8();

        internal static IEnumerable<NetStatistics> GetAll() {
            var statReader = new Utf8FileReader<X2048>(NetDevPath);
            try {
                statReader.SkipLine();
                statReader.SkipLine();
                while (!statReader.EndOfStream) {
                    statReader.SkipWhiteSpaces();
                    var interfaceName = statReader.ReadFragment(InterfaceNameSeparators.Span).ToUtf8String();

                    var receive = Direction.Parse(ref statReader);

                    for (var i = 0; i < ReceiveColumnCount - 4; ++i)
                        statReader.SkipWord();

                    var transmit = Direction.Parse(ref statReader);

                    statReader.SkipLine();

                    yield return new NetStatistics(interfaceName, receive, transmit);
                }
            }
            finally {
                statReader.Dispose();
            }
        }

        public struct Direction {
            public long Bytes { get; }
            public long Packets { get; }
            public long Errors { get; }
            public long Drops { get; }

            private Direction(long bytes, long packets, long errors, long drops) {
                Bytes = bytes;
                Packets = packets;
                Errors = errors;
                Drops = drops;
            }

            internal static Direction Parse<TReader>(ref TReader reader)
                where TReader : struct, IUtf8Reader {
                var bytes = reader.ReadInt64();
                var packets = reader.ReadInt64();
                var errors = reader.ReadInt64();
                var drops = reader.ReadInt64();
                return new Direction(bytes, packets, errors, drops);
            }
        }
    }
}