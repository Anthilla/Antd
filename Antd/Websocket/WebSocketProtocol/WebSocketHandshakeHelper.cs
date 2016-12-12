using System;
using System.Security.Cryptography;
using System.Text;

namespace Antd.Websocket.WebSocketProtocol {
    public class WebSocketHandshakeHelper {
        /// <summary>
        /// Combines the key supplied by the client with a guid and returns the sha1 hash of the combination
        /// </summary>
        public static string ComputeSocketAcceptString(string secWebSocketKey) {
            const string webSocketGuid = "258EAFA5-E914-47DA-95CA-C5AB0DC85B11";
            var concatenated = secWebSocketKey + webSocketGuid;
            var concatenatedAsBytes = Encoding.UTF8.GetBytes(concatenated);
            var sha1Hash = SHA1.Create().ComputeHash(concatenatedAsBytes);
            var secWebSocketAccept = Convert.ToBase64String(sha1Hash);
            return secWebSocketAccept;
        }
    }
}
