using System;
using System.Diagnostics;

namespace antdlib.Websocket.Client {
    public class Program {
        private static void Main() {
            try {
                const string webRoot = "";
                const int port = 8888;

                // used to decide what to do with incomming connections
                var connectionFactory = new ConnectionFactory(webRoot);

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
