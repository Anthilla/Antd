using System;
using System.Threading.Tasks;
using antdlib.Log;

namespace antdlib.Websocket.Client {
    public class WebSocket {
        private readonly WebServer _webserver;

        public WebSocket() {
            _webserver = new WebServer(new ConnectionFactory(""));
        }

        public void Start(int port) {
            Task.Run(() => Launch(port));
        }

        private void Launch(int port) {
            try {
                using (_webserver) {
                    _webserver.Listen(port);
                    Console.ReadKey();
                }
            }
            catch (Exception ex) {
                ConsoleLogger.Warn(ex.Message);
            }
        }

        public void Close() {
            _webserver.Dispose();
        }
    }
}
