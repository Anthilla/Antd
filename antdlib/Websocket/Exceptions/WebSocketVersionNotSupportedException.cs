using System;

namespace antdlib.Websocket.Exceptions {
    public class WebSocketVersionNotSupportedException : Exception {
        public WebSocketVersionNotSupportedException(string message) : base(message) {
        }
    }
}
