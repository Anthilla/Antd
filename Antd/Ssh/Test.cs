using Renci.SshNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Antd.Ssh {
    public  class Test {
        static SshClient client;
        static ForwardedPortDynamic port;

        public static void Start(string server, string user, string password, int serverport = 22) {
            client = new SshClient(server, user, password);
            client.KeepAliveInterval = new TimeSpan(0, 0, 5);
            client.ConnectionInfo.Timeout = new TimeSpan(0, 0, 20);
            //client.Connect();
            //if (client.IsConnected) {
                try {
                    port = new ForwardedPortDynamic("127.0.0.1", 1080);
                    client.AddForwardedPort(port);
                    port.Start();
                    Console.WriteLine(client.CreateCommand("echo johnny").Result);
                }
                catch (Exception ex) {
                    Console.WriteLine(ex.ToString());
                }
            //}
        }

        public static void Stop() {
            port.Stop();
            port.Dispose();
            client.Disconnect();
            client.Dispose();
        }
    }
}
