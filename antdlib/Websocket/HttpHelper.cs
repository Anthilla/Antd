using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using antdlib.Websocket.Exceptions;

namespace antdlib.Websocket {
    public class HttpHelper {
        public static string ReadHttpHeader(NetworkStream networkStream) {
            const int length = 1024 * 16;
            var buffer = new byte[length];
            var offset = 0;
            int bytesRead;
            do {
                if (offset >= length) {
                    throw new EntityTooLargeException("Http header message too large to fit in buffer (16KB)");
                }
                bytesRead = networkStream.Read(buffer, offset, length - offset);
                offset += bytesRead;
                var header = Encoding.UTF8.GetString(buffer, 0, offset);
                if (header.Contains("\r\n\r\n")) {
                    return header;
                }
            } while (bytesRead > 0);
            return string.Empty;
        }

        public static void WriteHttpHeader(string response, Stream stream) {
            response = response.Trim() + Environment.NewLine + Environment.NewLine;
            var bytes = Encoding.UTF8.GetBytes(response);
            stream.Write(bytes, 0, bytes.Length);
        }
    }
}
