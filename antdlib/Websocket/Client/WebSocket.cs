using System;
using System.Threading.Tasks;
using antdlib.Log;

namespace antdlib.Websocket.Client {
    public class WebSocket {

        public static void Start(int port) {
            Task.Run(() => Launch(port));
        }

        private static void Launch(int port) {
            try {
                using (var server = new WebServer(new ConnectionFactory(""))) {
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
