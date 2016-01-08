using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using antdlib.Log;
using antdlib.Websocket.Connections;

namespace antdlib.Websocket {
    public class WebServer : IDisposable {
        private readonly List<IDisposable> _openConnections;
        private readonly IConnectionFactory _connectionFactory;
        private TcpListener _listener;
        private bool _isDisposed;

        public WebServer(IConnectionFactory connectionFactory) {
            _connectionFactory = connectionFactory;
            _openConnections = new List<IDisposable>();
        }

        /// <summary>
        /// Listens on the port specified
        /// </summary>
        public void Listen(int port) {
            try {
                var localAddress = IPAddress.Any;
                _listener = new TcpListener(localAddress, port);
                _listener.Start();
                ConsoleLogger.Log($"Server started listening on port {port}");
                StartAccept();
            }
            catch (SocketException ex) {
                ConsoleLogger.Warn($"Error listening on port {port}. Make sure IIS or another application is not running and consuming your port: {ex.Message}");
            }
        }

        /// <summary>
        /// Gets the first available port and listens on it. Returns the port
        /// </summary>
        public int Listen() {
            var localAddress = IPAddress.Any;
            _listener = new TcpListener(localAddress, 0);
            _listener.Start();
            StartAccept();
            var port = ((IPEndPoint)_listener.LocalEndpoint).Port;
            ConsoleLogger.Log($"Server started listening on port {port}");
            return port;
        }

        private void StartAccept() {
            _listener.BeginAcceptTcpClient(HandleAsyncConnection, null);
        }

        private static ConnectionDetails GetConnectionDetails(NetworkStream networkStream, TcpClient tcpClient) {
            var header = HttpHelper.ReadHttpHeader(networkStream);
            var getRegex = new Regex(@"^GET(.*)HTTP\/1\.1");

            var getRegexMatch = getRegex.Match(header);
            if (!getRegexMatch.Success)
                return new ConnectionDetails(networkStream, tcpClient, string.Empty, ConnectionType.Unknown, header);
            var path = getRegexMatch.Groups[1].Value.Trim();
            var webSocketUpgradeRegex = new Regex("Upgrade: websocket");
            var webSocketUpgradeRegexMatch = webSocketUpgradeRegex.Match(header);
            return webSocketUpgradeRegexMatch.Success ? new ConnectionDetails(networkStream, tcpClient, path, ConnectionType.WebSocket, header) : new ConnectionDetails(networkStream, tcpClient, path, ConnectionType.Http, header);
        }

        private void HandleAsyncConnection(IAsyncResult res) {
            try {
                using (var tcpClient = _listener.EndAcceptTcpClient(res)) {
                    StartAccept();
                    Trace.TraceInformation("Connection opened");
                    using (var networkStream = tcpClient.GetStream()) {
                        var connectionDetails = GetConnectionDetails(networkStream, tcpClient);
                        using (var connection = _connectionFactory.CreateInstance(connectionDetails)) {
                            try {
                                lock (_openConnections) {
                                    _openConnections.Add(connection);
                                }
                                connection.Respond();
                            }
                            finally {
                                lock (_openConnections) {
                                    _openConnections.Remove(connection);
                                }
                            }
                        }
                    }
                    Trace.TraceInformation("Connection closed");
                }
            }
            catch (ObjectDisposedException) {
            }
            catch (Exception ex) {
                Trace.TraceError(ex.ToString());
            }
        }

        private void CloseAllConnections() {
            lock (_openConnections) {
                foreach (var openConnection in _openConnections) {
                    try {
                        openConnection.Dispose();
                    }
                    catch (Exception ex) {
                        Trace.TraceError(ex.ToString());
                    }
                }
                _openConnections.Clear();
            }
        }

        public void Dispose() {
            if (_isDisposed) return;
            try {
                _listener.Server.Close();
                _listener.Stop();
            }
            catch (Exception ex) {
                Trace.TraceError(ex.ToString());
            }

            CloseAllConnections();
            _isDisposed = true;
            Trace.TraceInformation("Web Server disposed");
        }
    }
}
