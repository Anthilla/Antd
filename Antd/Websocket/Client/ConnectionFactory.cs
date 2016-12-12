using antdlib.common;
using Antd.Websocket.Connections;

namespace Antd.Websocket.Client {
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
                    if (connectionDetails.Path == "/chat") {
                        return new ChatWebSocketConnection(connectionDetails.NetworkStream, connectionDetails.TcpClient, connectionDetails.Header);
                    }
                    if (connectionDetails.Path == "/cmd") {
                        return new CommandWebSocketConnection(connectionDetails.NetworkStream, connectionDetails.TcpClient, connectionDetails.Header);
                    }
                    break;
                case ConnectionType.Http:
                    return new HttpConnection(connectionDetails.NetworkStream, connectionDetails.Path, _webRoot);
            }
            return new BadRequestConnection(connectionDetails.NetworkStream, connectionDetails.Header);
        }
    }

    public class CustomConnectionFactory : IConnectionFactory {
        private readonly string _webRoot;

        public CustomConnectionFactory(string webRoot) {
            _webRoot = string.IsNullOrWhiteSpace(webRoot) ? "/framework/antd" : webRoot;
            ConsoleLogger.Log($"websocket web root: {_webRoot}");
        }

        public CustomConnectionFactory() : this(null) {

        }

        public IConnection CreateInstance(ConnectionDetails connectionDetails) {
            switch (connectionDetails.ConnectionType) {
                case ConnectionType.WebSocket:
                    if (connectionDetails.Path == "/cmd") {
                        return new CommandWebSocketConnection(connectionDetails.NetworkStream, connectionDetails.TcpClient, connectionDetails.Header);
                    }
                    break;
                case ConnectionType.Http:
                    return new HttpConnection(connectionDetails.NetworkStream, connectionDetails.Path, _webRoot);
            }
            return new BadRequestConnection(connectionDetails.NetworkStream, connectionDetails.Header);
        }
    }
}
