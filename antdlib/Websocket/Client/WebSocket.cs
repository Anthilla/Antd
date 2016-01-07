using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace antdlib.Websocket.Client {
    public class WebSocket {
        public static void Start(int port, string webroot = "") {
            Task.Run(() => Launch(webroot, port));
        }

        private static void Launch(string webroot, int port) {
            try {
                var connectionFactory = new ConnectionFactory(webroot);
                using (var server = new WebServer(connectionFactory)) {
                    server.Listen(port);
                    Console.ReadKey();
                }
            }
            catch (Exception ex) {
                Trace.TraceError(ex.ToString());
                Console.ReadKey();
            }
        }
    }
}
