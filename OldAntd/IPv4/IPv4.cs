using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace Antd {
    public class IPv4 {
        public static List<string> GetAllLocalDescription() {
            return NetworkInterface.GetAllNetworkInterfaces()
             .Where(_ => _.OperationalStatus == OperationalStatus.Up)
             .Select(_ => _.Description)
             .ToList();
        }

        public static List<string> GetAllLocalAddress() {
            return NetworkInterface.GetAllNetworkInterfaces()
                .Where(_ => _.OperationalStatus == OperationalStatus.Up)
                .SelectMany(_ => _.GetIPProperties().UnicastAddresses)
                .Where(_ => _.Address.AddressFamily == AddressFamily.InterNetwork)
                .Select(_ => _.Address.ToString())
                .ToList();
        }

        public static List<string> GetIpFromAdapter(string networkAdapter) {
            return NetworkInterface.GetAllNetworkInterfaces()
                .FirstOrDefault(_ => _.Description == networkAdapter).GetIPProperties().UnicastAddresses
                .Where(_ => _.Address.AddressFamily == AddressFamily.InterNetwork).Select(_ => _.Address.ToString()).ToList();
        }

        public static string GetAdapterFromIp(string ipAddress) {
            return NetworkInterface.GetAllNetworkInterfaces()
                .FirstOrDefault(_ => _.GetIPProperties().UnicastAddresses.Select(__ => __.Address.ToString()).Contains(ipAddress))
                .Description;
        }
    }
}
