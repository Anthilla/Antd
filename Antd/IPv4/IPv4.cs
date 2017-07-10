using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace Antd {
    public class IPv4 {

        public static List<string> GetAllLocalDescription() {
            return NetworkInterface.GetAllNetworkInterfaces()
             .Where(x => x.OperationalStatus == OperationalStatus.Up)
             .Select(x => x.Description).ToList();
        }

        public static List<string> GetAllLocalAddress() {
            return NetworkInterface.GetAllNetworkInterfaces()
                .Where(x => x.OperationalStatus == OperationalStatus.Up)
                .SelectMany(x => x.GetIPProperties().UnicastAddresses)
                .Where(x => x.Address.AddressFamily == AddressFamily.InterNetwork).Select(x => x.Address.ToString()).ToList();
        }
    }
}
