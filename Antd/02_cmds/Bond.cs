using anthilla.core;
using System.IO;
using System.Linq;

namespace Antd.cmds {
    public class Bond {

        private const string ifenslaveFileLocation = "/sbin/ifenslave";
        private const string networkAdapterType= "bond";
        private const string bondTxqueuelen = "10000";

        public static NetBond[] Get() {
            var interfaces = Network.Get().Where(_ => _.Type == models.NetworkAdapterType.Bond).ToArray();
            var brs = new NetBond[interfaces.Length];
            for(var i = 0; i < brs.Length; i++) {
                brs[i] = new NetBond() {
                    Id = interfaces[i].Id,
                    Lower = interfaces[i].LowerInterfaces
                };
            }
            return brs;
        }

        public static bool Apply() {
            var current = Application.CurrentConfiguration.Network.Bonds;
            var running = Application.RunningConfiguration.Network.Bonds;
            for(var i = 0; i < current.Length; i++) {
                var br = current[i];
                var run = running.FirstOrDefault(_ => _.Id == br.Id)?.ToString();
                if(CommonString.AreEquals(run, br.ToString()) == false) {
                    Set(br.Id);
                    for(var l = 0; l < br.Lower.Length; l++) {
                        AddNetworkAdapter(br.Id, br.Lower[l]);
                    }
                }
            }
            return true;
        }

        public static bool Set(string bondName) {
            Ip.DisableNetworkAdapter(bondName);
            Ip.DeleteNetworkAdapted(bondName);
            Ip.AddNetworkAdapted(bondName, networkAdapterType);
            Ip.SetNetworkAdapterTxqueuelen(bondName, bondTxqueuelen);
            Ip.DisableNetworkAdapter(bondName);
            var bondDirectory = $"/sys/class/net/{bondName}";
            if(Directory.Exists(bondDirectory)) {
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
            CommonProcess.Do(ifenslaveFileLocation, args);
            return true;
        }

        public static bool DeleteNetworkAdapter(string bondName, string networkAdapter) {
            var args = CommonString.Append("-d ", bondName, " ", networkAdapter);
            CommonProcess.Do(ifenslaveFileLocation, args);
            return true;
        }
    }
}
