using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace antdlib.Log {
    public class SyslogTest {
        public static void TestSyslog() {
            try {
                const int facility = (int)Syslog.Facility.User; // Local5
                const int level = (int)Syslog.Level.Warning; // Debug;
                new Syslog("192.168.20.65", 2000).Send(facility, level, "Hello, Syslog.");
            }
            catch (Exception ex1) {
                Console.WriteLine("Exception! " + ex1);
            }
        }
    }

    public class Syslog {
        private readonly IPHostEntry _ipHostInfo;
        private readonly string _sysLogServerIp;
        private readonly int _port;

        public Syslog(string sysLogServerIp, int port) {
            _sysLogServerIp = sysLogServerIp;
            _port = port;
            _ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
        }

        public void Send(int facility, int level, string text) {
            var sock = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            var serverAddr = IPAddress.Parse(_sysLogServerIp);
            var endPoint = new IPEndPoint(serverAddr, _port);
            var priority = facility * 8 + level;
            string msg = $"<{priority}>{DateTime.Now.ToString("MMM dd HH:mm:ss")} {_ipHostInfo.HostName} {text}";
            var bytes = Encoding.ASCII.GetBytes(msg);
            sock.SendTo(bytes, endPoint);
            sock.Close();// chiudo il socket
        }

        public enum Level {
            Emergency = 0,
            Alert = 1,
            Critical = 2,
            Error = 3,
            Warning = 4,
            Notice = 5,
            Information = 6,
            Debug = 7,
        }

        public enum Facility {
            Kernel = 0,
            User = 1,
            Mail = 2,
            Daemon = 3,
            Auth = 4,
            Syslog = 5,
            Lpr = 6,
            News = 7,
            UUCP = 8,
            Cron = 9,
            Local0 = 10,
            Local1 = 11,
            Local2 = 12,
            Local3 = 13,
            Local4 = 14,
            Local5 = 15,
            Local6 = 16,
            Local7 = 17,
        }
    }
}
