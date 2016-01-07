using System;
using System.Diagnostics;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using antdlib.Websocket.Connections;
using antdlib.Websocket.Exceptions;

namespace antdlib.Websocket.WebSocketProtocol {
    public abstract class WebSocketConnection : IConnection {
        private readonly NetworkStream _networkStream;
        private readonly string _header;

        private bool _isDisposed;
        private WebSocketOpCode _multiFrameOpcode;

        protected WebSocketConnection(NetworkStream networkStream, string header) {
            _networkStream = networkStream;
            _header = header;
            Writer = new WebSocketFrameWriter(networkStream);
        }

        public void Respond() {
            PerformHandshake();
            MainReadLoop();
        }

        protected WebSocketFrameWriter Writer { get; }

        protected virtual void OnPing(byte[] payload) {
            Writer.Write(WebSocketOpCode.Pong, payload);
        }

        protected virtual void OnConnectionClosed(byte[] payload) {
            Writer.Write(WebSocketOpCode.ConnectionClose, payload);
            Trace.TraceInformation("Client requested connection close");
        }

        protected virtual void OnTextFrame(string text) {
        }

        protected virtual void OnTextMultiFrame(string text, bool isLastFrame) {
        }

        protected virtual void OnBinaryFrame(byte[] payload) {
        }

        protected virtual void OnBinaryMultiFrame(byte[] payload, bool isLastFrame) {
        }

        private void PerformHandshake() {
            var networkStream = _networkStream;
            var header = _header;

            try {
                var webSocketKeyRegex = new Regex("Sec-WebSocket-Key: (.*)");
                var webSocketVersionRegex = new Regex("Sec-WebSocket-Version: (.*)");
                const int webSocketVersion = 13;
                var secWebSocketVersion = Convert.ToInt32(webSocketVersionRegex.Match(header).Groups[1].Value.Trim());
                if (secWebSocketVersion < webSocketVersion) {
                    throw new WebSocketVersionNotSupportedException($"WebSocket Version {secWebSocketVersion} not suported. Must be {webSocketVersion} or above");
                }
                var secWebSocketKey = webSocketKeyRegex.Match(header).Groups[1].Value.Trim();
                var setWebSocketAccept = WebSocketHandshakeHelper.ComputeSocketAcceptString(secWebSocketKey);
                var response = ("HTTP/1.1 101 Switching Protocols" + Environment.NewLine
                                           + "Connection: Upgrade" + Environment.NewLine
                                           + "Upgrade: websocket" + Environment.NewLine
                                           + "Sec-WebSocket-Accept: " + setWebSocketAccept);

                HttpHelper.WriteHttpHeader(response, networkStream);
                Trace.TraceInformation("Web Socket handshake sent");
            }
            catch (WebSocketVersionNotSupportedException) {
                var response = "HTTP/1.1 426 Upgrade Required" + Environment.NewLine + "Sec-WebSocket-Version: 13";
                HttpHelper.WriteHttpHeader(response, networkStream);
                throw;
            }
            catch (Exception) {
                HttpHelper.WriteHttpHeader("HTTP/1.1 400 Bad Request", networkStream);
                throw;
            }
        }

        private void MainReadLoop() {
            var networkStream = _networkStream;
            var reader = new WebSocketFrameReader();
            while (true) {
                var frame = reader.Read(networkStream);
                if (!frame.IsValid) {
                    return;
                }
                if (frame.OpCode == WebSocketOpCode.ContinuationFrame) {
                    switch (_multiFrameOpcode) {
                        case WebSocketOpCode.TextFrame:
                            var data = Encoding.UTF8.GetString(frame.DecodedPayload, 0, frame.DecodedPayload.Length);
                            OnTextMultiFrame(data, frame.IsFinBitSet);
                            break;
                        case WebSocketOpCode.BinaryFrame:
                            OnBinaryMultiFrame(frame.DecodedPayload, frame.IsFinBitSet);
                            break;
                    }
                }
                else {
                    switch (frame.OpCode) {
                        case WebSocketOpCode.ConnectionClose:
                            OnConnectionClosed(frame.DecodedPayload);
                            return;
                        case WebSocketOpCode.Ping:
                            OnPing(frame.DecodedPayload);
                            break;
                        case WebSocketOpCode.TextFrame:
                            var data = Encoding.UTF8.GetString(frame.DecodedPayload, 0, frame.DecodedPayload.Length);
                            if (frame.IsFinBitSet) {
                                OnTextFrame(data);
                            }
                            else {
                                _multiFrameOpcode = frame.OpCode;
                                OnTextMultiFrame(data, frame.IsFinBitSet);
                            }
                            break;
                        case WebSocketOpCode.BinaryFrame:
                            if (frame.IsFinBitSet) {
                                OnBinaryFrame(frame.DecodedPayload);
                            }
                            else {
                                _multiFrameOpcode = frame.OpCode;
                                OnBinaryMultiFrame(frame.DecodedPayload, frame.IsFinBitSet);
                            }
                            break;
                    }
                }
            }
        }

        public void Dispose() {
            if (!_networkStream.CanWrite || _isDisposed) return;
            _isDisposed = true;
            Writer.Write(WebSocketOpCode.ConnectionClose, new byte[0]);
            Trace.TraceInformation("Server requested connection close");
        }
    }
}
