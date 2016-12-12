using System;
using System.IO;

namespace Antd.Websocket.WebSocketProtocol {
    public class BinaryReaderWriter {
        public static byte[] ReadExactly(int length, Stream stream) {
            var buffer = new byte[length];
            if (length == 0) {
                return buffer;
            }

            var offset = 0;
            do {
                var bytesRead = stream.Read(buffer, offset, length - offset);
                if (bytesRead == 0) {
                    throw new EndOfStreamException($"Unexpected end of stream encountered whilst attempting to read {length:#,##0} bytes");
                }

                offset += bytesRead;
            } while (offset < length);

            return buffer;
        }

        public static void WriteULong(ulong value, Stream stream) {
            var buffer = BitConverter.GetBytes(value);
            stream.Write(buffer, 0, buffer.Length);
        }

        public static void WriteUShort(ushort value, Stream stream) {
            var buffer = BitConverter.GetBytes(value);
            stream.Write(buffer, 0, buffer.Length);
        }
    }
}
