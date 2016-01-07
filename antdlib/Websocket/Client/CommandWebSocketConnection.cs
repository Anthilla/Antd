using System.Net.Sockets;
using antdlib.Log;
using antdlib.Websocket.WebSocketProtocol;

namespace antdlib.Websocket.Client {
    public class CommandWebSocketConnection : WebSocketConnection {
        public CommandWebSocketConnection(NetworkStream networkStream, TcpClient tcpClient, string header)
            : base(networkStream, header) {
            tcpClient.NoDelay = true;
        }

        protected override void OnTextFrame(string message) {
            ConsoleLogger.Log($"executing {message}");
            var response = Terminal.Terminal.Execute(message);
            Writer.WriteText(response);
        }
    }
}
