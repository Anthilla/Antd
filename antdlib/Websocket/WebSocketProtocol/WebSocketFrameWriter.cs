using System.IO;
using System.Text;

namespace antdlib.Websocket.WebSocketProtocol {
    public class WebSocketFrameWriter {
        private readonly Stream _stream;

        public WebSocketFrameWriter(Stream stream) {
            _stream = stream;
        }

        public void Write(WebSocketOpCode opCode, byte[] payload, bool isLastFrame) {
            using (var memoryStream = new MemoryStream()) {
                var finBitSetAsByte = isLastFrame ? (byte)0x80 : (byte)0x00;
                var byte1 = (byte)(finBitSetAsByte | (byte)opCode);
                memoryStream.WriteByte(byte1);
                if (payload.Length < 126) {
                    var byte2 = (byte)payload.Length;
                    memoryStream.WriteByte(byte2);
                }
                else if (payload.Length <= ushort.MaxValue) {
                    byte byte2 = 126;
                    memoryStream.WriteByte(byte2);
                    BinaryReaderWriter.WriteUShort((ushort)payload.Length, memoryStream);
                }
                else {
                    byte byte2 = 127;
                    memoryStream.WriteByte(byte2);
                    BinaryReaderWriter.WriteULong((ulong)payload.Length, memoryStream);
                }
                memoryStream.Write(payload, 0, payload.Length);
                var buffer = memoryStream.ToArray();
                _stream.Write(buffer, 0, buffer.Length);
            }
        }

        public void Write(WebSocketOpCode opCode, byte[] payload) {
            Write(opCode, payload, true);
        }

        public void WriteText(string text) {
            var responseBytes = Encoding.UTF8.GetBytes(text);
            Write(WebSocketOpCode.TextFrame, responseBytes);
        }
    }
}
