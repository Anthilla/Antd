using System.Diagnostics;
using System.Net.Sockets;
using antdlib.Websocket.WebSocketProtocol;

namespace antdlib.Websocket.Client {
    public class ChatWebSocketConnection : WebSocketConnection {
        public ChatWebSocketConnection(NetworkStream networkStream, TcpClient tcpClient, string header)
            : base(networkStream, header) {
            tcpClient.NoDelay = true;
        }

        protected override void OnTextFrame(string text) {
            var response = "Server: " + text;
            Writer.WriteText(response);
            Trace.TraceInformation(response);
        }
    }
}
