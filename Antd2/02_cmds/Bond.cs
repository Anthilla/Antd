using antd.core;
using Antd2.Configuration;
using System.IO;

namespace Antd2.cmds {
    public class Bond {

        private const string ifenslaveCommand = "ifenslave";
        private const string networkAdapterType = "bond";
        private const string bondTxqueuelen = "10000";

        public static NetBond[] Get() {
            throw new System.NotImplementedException();
        }

        public static bool Create(string bondName) {
            Ip.DisableNetworkAdapter(bondName);
            Ip.DeleteNetworkAdapted(bondName);
            Ip.AddNetworkAdapted(bondName, networkAdapterType);
            Ip.SetNetworkAdapterTxqueuelen(bondName, bondTxqueuelen);
            Ip.DisableNetworkAdapter(bondName);
            var bondDirectory = $"/sys/class/net/{bondName}";
            if (Directory.Exists(bondDirectory)) {
                Echo.PipeToFile("4", $"{bondDirectory}/bonding/mode");
                Echo.PipeToFile("1", $"{bondDirectory}/bonding/lacp_rate");
                Echo.PipeToFile("1", $"{bondDirectory}/lacp_rate");
                Echo.PipeToFile("100", $"{bondDirectory}/bonding/miimon");
            }
            Ip.EnableNetworkAdapter(bondName);
            return true;
        }

        public static bool AddNetworkAdapter(string bondName, string networkAdapter) {
            var args = CommonString.Append(bondName, " ", networkAdapter);
            var cmd = $"{ifenslaveCommand} {args}";
            Bash.Do(cmd);
            return true;
        }

        public static bool DeleteNetworkAdapter(string bondName, string networkAdapter) {
            var args = CommonString.Append("-d ", bondName, " ", networkAdapter);
            var cmd = $"{ifenslaveCommand} {args}";
            Bash.Do(cmd);
            return true;
        }
    }
}
