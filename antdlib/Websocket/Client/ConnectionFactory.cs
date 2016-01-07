using System.Diagnostics;
using System.IO;
using System.Reflection;
using antdlib.Log;
using antdlib.Websocket.Connections;

namespace antdlib.Websocket.Client {
    public class ConnectionFactory : IConnectionFactory {
        private readonly string _webRoot;

        public ConnectionFactory(string webRoot) {
            _webRoot = string.IsNullOrWhiteSpace(webRoot) ? "/framework/antd" : webRoot;
            ConsoleLogger.Log($"websocket web root: {_webRoot}");
        }

        public ConnectionFactory() : this(null) {

        }

        public IConnection CreateInstance(ConnectionDetails connectionDetails) {
            switch (connectionDetails.ConnectionType) {
                case ConnectionType.WebSocket:
                    // you can support different kinds of web socket connections using a different path
                    if (connectionDetails.Path == "/chat") {
                        return new ChatWebSocketConnection(connectionDetails.NetworkStream, connectionDetails.TcpClient, connectionDetails.Header);
                    }
                    break;
                case ConnectionType.Http:
                    return new HttpConnection(connectionDetails.NetworkStream, connectionDetails.Path, _webRoot);
            }
            return new BadRequestConnection(connectionDetails.NetworkStream, connectionDetails.Header);
        }
    }
}
