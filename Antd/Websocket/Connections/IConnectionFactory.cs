namespace Antd.Websocket.Connections {
    /// <summary>
    /// Implement this to decide what connection to use based on the http header
    /// </summary>
    public interface IConnectionFactory {
        IConnection CreateInstance(ConnectionDetails connectionDetails);
    }
}
