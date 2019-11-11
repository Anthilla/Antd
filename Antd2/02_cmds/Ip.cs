using anthilla.core;
using System.Collections.Generic;
using System.IO;

namespace Antd2.cmds {
    public class Ip {

        private const string ipCommand = "ip";
        private const string ifenslaveFileLocation = "/sbin/ifenslave";
        private const string processName = "haproxy";

        public static bool AddAddress(string networkAdapter, string addressAndRange) {
            var args = CommonString.Append("addr add ", addressAndRange, " dev ", networkAdapter);
            Bash.Do($"{ipCommand} {args}");
            return true;
        }

        public static bool AddAddress(string networkAdapter, string address, string range) {
            var args = CommonString.Append("addr add ", address, "/", range, " dev ", networkAdapter);
            Bash.Do($"{ipCommand} {args}");
            return true;
        }

        public static bool AddBroadcast(string networkAdapter, string address, string range, string broadcast) {
            var args = CommonString.Append("addr add ", address, "/", range, " broadcast ", broadcast, " dev ", networkAdapter);
            Bash.Do($"{ipCommand} {args}");
            return true;
        }

        public static bool AddMultipathRoute(string networkAdapter, string secondNetworkAdapter) {
            var args = CommonString.Append("route add default scope global nexthop dev ", networkAdapter, " dev ", secondNetworkAdapter);
            Bash.Do($"{ipCommand} {args}");
            return true;
        }

        public static bool AddNat(string address, string secondAddress) {
            var args = CommonString.Append("route add nat ", address, " via ", secondAddress);
            Bash.Do($"{ipCommand} {args}");
            return true;
        }

        public static bool AddNetworkAdapted(string networkAdapter, string type) {
            var args = CommonString.Append("link add name ", networkAdapter, " type ", type);
            Bash.Do($"{ipCommand} {args}");
            return true;
        }

        public static bool AddRoute(string networkAdapter, string gateway, string destinationAddress = "default") {
            var args = CommonString.Append("route add ", destinationAddress, " via ", gateway, " dev ", networkAdapter);
            Bash.Do($"{ipCommand} {args}");
            return true;
        }

        public static bool DeleteAddress(string networkAdapter, string address, string range) {
            var args = CommonString.Append("addr del ", address, "/", range, " dev ", networkAdapter);
            Bash.Do($"{ipCommand} {args}");
            return true;
        }

        public static bool DeleteBroadcast(string networkAdapter, string address, string range, string broadcast) {
            var args = CommonString.Append("addr del ", address, "/", range, " broadcast ", broadcast, " dev ", networkAdapter);
            Bash.Do($"{ipCommand} {args}");
            return true;
        }

        public static bool DeleteRoute(string networkAdapter, string gateway, string destinationAddress = "default") {
            var args = CommonString.Append("route del ", destinationAddress, " via ", gateway, " dev ", networkAdapter);
            Bash.Do($"{ipCommand} {args}");
            return true;
        }

        public static bool DeleteNetworkAdapted(string networkAdapter) {
            var args = CommonString.Append("link del ", networkAdapter);
            Bash.Do($"{ipCommand} {args}");
            return true;
        }

        public static bool EnableNetworkAdapter(string networkAdapter) {
            var args = CommonString.Append("link set ", networkAdapter, " up");
            Bash.Do($"{ipCommand} {args}");
            return true;
        }

        public static bool DisableNetworkAdapter(string networkAdapter) {
            var args = CommonString.Append("link set ", networkAdapter, " down");
            Bash.Do($"{ipCommand} {args}");
            return true;
        }

        public static bool FlushNetworkAdapter(string networkAdapter) {
            var args = CommonString.Append("addr flush dev ", networkAdapter);
            Bash.Do($"{ipCommand} {args}");
            return true;
        }

        public static IEnumerable<string> GetNetworkAdapter(string networkAdapter) {
            //todo map to model
            //dict["ip4-show-if-addr"] = new Command {
            //dict["ip4-show-if-link"] = new Command {
            var args = CommonString.Append("addr show ", networkAdapter);
            return Bash.Execute($"{ipCommand} {args}");
        }

        public static string GetNetworkAdapterMacAddresss(string networkAdapter) {
            var file = CommonString.Append("/sys/class/net/", networkAdapter, "/address");
            if (!File.Exists(file)) {
                return string.Empty;
            }
            return File.ReadAllText(file);
        }

        public static long GetNetworkAdapterMTU(string networkAdapter) {
            var file = CommonString.Append("/sys/class/net/", networkAdapter, "/mtu");
            if (!File.Exists(file)) {
                return 0;
            }
            return long.Parse(File.ReadAllText(file));
        }

        public static long GetNetworkAdapterTxqueuelen(string networkAdapter) {
            var file = CommonString.Append("/sys/class/net/", networkAdapter, "/tx_queue_len");
            if (!File.Exists(file)) {
                return 0;
            }
            return long.Parse(File.ReadAllText(file));
        }

        public static bool SetNetworkAdapterPromiscOn(string networkAdapter) {
            var args = CommonString.Append("link set dev ", networkAdapter, " promisc on");
            Bash.Do($"{ipCommand} {args}");
            return true;
        }

        public static bool SetNetworkAdapterPromiscOff(string networkAdapter) {
            var args = CommonString.Append("link set dev ", networkAdapter, " promisc off");
            Bash.Do($"{ipCommand} {args}");
            return true;
        }

        public static bool SetNetworkAdapterMacAddresss(string networkAdapter, string macAddress) {
            DisableNetworkAdapter(networkAdapter);
            var args = CommonString.Append("link set dev ", networkAdapter, " address ", macAddress);
            Bash.Do($"{ipCommand} {args}");
            EnableNetworkAdapter(networkAdapter);
            return true;
        }

        public static bool SetNetworkAdapterMTU(string networkAdapter, string mtuValue) {
            var args = CommonString.Append("link set dev ", networkAdapter, " mtu ", mtuValue);
            Bash.Do($"{ipCommand} {args}");
            return true;
        }

        public static bool SetNetworkAdapterTxqueuelen(string networkAdapter, string txqueuelenValue) {
            var args = CommonString.Append("link set dev ", networkAdapter, " txqueuelen ", txqueuelenValue);
            Bash.Do($"{ipCommand} {args}");
            return true;
        }

        public static IEnumerable<string> GetNetworkAdapterRouting(string networkAdapter) {
            //todo map to model
            var args = CommonString.Append("route show ", networkAdapter);
            return Bash.Execute($"{ipCommand} {args}");
        }
    }
}
