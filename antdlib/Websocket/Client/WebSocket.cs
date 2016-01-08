using System;
using System.Threading.Tasks;
using antdlib.Log;

namespace antdlib.Websocket.Client {
    public class WebSocket {
        private static int _port;

        public static int Start(int port) {
            Task.Run(() => Launch(port));
            return _port;
        }

        private static void Launch(int port) {
            try {
                using (var server = new WebServer(new ConnectionFactory(""))) {
                    _port = port;
                    server.Listen(port);
                    Console.ReadKey();
                }
            }
            catch (Exception ex) {
                ConsoleLogger.Warn(ex.Message);
            }
        }
    }
}
