using System;

namespace Antd.Websocket.Exceptions {
    public class WebSocketVersionNotSupportedException : Exception {
        public WebSocketVersionNotSupportedException(string message) : base(message) {
        }
    }
}
