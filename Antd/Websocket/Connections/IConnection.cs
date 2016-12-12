using System;

namespace Antd.Websocket.Connections {
    public interface IConnection : IDisposable {
        /// <summary>
        /// Sends data back to the client. This is built using the IConnectionFactory
        /// </summary>
        void Respond();
    }
}
