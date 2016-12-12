using System;
using System.IO;
using System.Net.Sockets;

namespace Antd.Websocket.WebSocketProtocol {
    public class WebSocketFrameReader {
        public WebSocketFrame Read(NetworkStream stream) {
            var byte1 = (byte)stream.ReadByte();
            if (!stream.DataAvailable && byte1 == 0xFF) {
                return new WebSocketFrame(true, WebSocketOpCode.ConnectionClose, new byte[0], false);
            }
            const byte finBitFlag = 0x80;
            const byte opCodeFlag = 0x0F;
            var isFinBitSet = (byte1 & finBitFlag) == finBitFlag;
            var opCode = (WebSocketOpCode)(byte1 & opCodeFlag);
            var byte2 = (byte)stream.ReadByte();
            const byte maskFlag = 0x80;
            var isMaskBitSet = (byte2 & maskFlag) == maskFlag;
            var len = ReadLength(byte2, stream);
            byte[] decodedPayload;
            if (isMaskBitSet) {
                const int maskKeyLen = 4;
                var maskKey = BinaryReaderWriter.ReadExactly(maskKeyLen, stream);
                var encodedPayload = BinaryReaderWriter.ReadExactly((int)len, stream);
                decodedPayload = new byte[len];
                for (var i = 0; i < encodedPayload.Length; i++) {
                    decodedPayload[i] = (byte)(encodedPayload[i] ^ maskKey[i % maskKeyLen]);
                }
            }
            else {
                decodedPayload = BinaryReaderWriter.ReadExactly((int)len, stream);
            }

            var frame = new WebSocketFrame(isFinBitSet, opCode, decodedPayload, true);
            return frame;
        }

        private static uint ReadLength(byte byte2, Stream stream) {
            const byte payloadLenFlag = 0x7F;
            var lenght = (uint)(byte2 & payloadLenFlag);
            const uint maxLen = 2147483648;
            if (lenght > maxLen) {
                throw new ArgumentOutOfRangeException($"Payload length out of range. Min 0 max 2GB. Actual {lenght:#,##0} bytes.");

            }
            if (lenght == 126) {
                var lenBuffer = BinaryReaderWriter.ReadExactly(2, stream);
                lenght = BitConverter.ToUInt16(lenBuffer, 0);
            }
            else if (lenght == 127) {
                var lenBuffer = BinaryReaderWriter.ReadExactly(8, stream);
                lenght = (uint)BitConverter.ToUInt64(lenBuffer, 0);
            }
            return lenght;
        }
    }
}
