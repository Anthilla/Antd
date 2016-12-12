using System.Diagnostics;
using System.Net.Sockets;

namespace Antd.Websocket.Connections {
    public class BadRequestConnection : IConnection {
        private readonly NetworkStream _networkStream;
        private readonly string _header;

        public BadRequestConnection(NetworkStream networkStream, string header) {
            _networkStream = networkStream;
            _header = header;
        }

        public void Respond() {
            HttpHelper.WriteHttpHeader("HTTP/1.1 400 Bad Request", _networkStream);
            var header = _header.Length > 255 ? _header.Substring(0, 255) + "..." : _header;
            Trace.TraceInformation("Bad request: '" + header + "'");
        }

        public void Dispose() {
        }
    }
}
