using antd.core;
using System.Linq;

namespace Antd2.cmds {

    public class Brctl {

        private const string brctlCommand = "brctl";
        private const string addbrArg = "addbr";
        private const string addifArg = "addif";
        private const string delbrArg = "delbr";
        private const string delifArg = "delif";
        private const string showArg = "show";
        private const string stpArg = "stp";

        //public static NetBridge[] Get() {
        //    var interfaces = Network.Get().Where(_ => _.Type == NetworkAdapterType.Bridge).ToArray();
        //    var brs = new NetBridge[interfaces.Length];
        //    for (var i = 0; i < brs.Length; i++) {
        //        brs[i] = new NetBridge() {
        //            Id = interfaces[i].Id,
        //            Lower = interfaces[i].LowerInterfaces
        //        };
        //    }
        //    return brs;
        //}

        public static bool Apply() {
            //var current = Application.CurrentConfiguration.Network.Bridges;
            //var running = Application.RunningConfiguration.Network.Bridges;
            //for (var i = 0; i < current.Length; i++) {
            //    var br = current[i];
            //    var run = running.FirstOrDefault(_ => _.Id == br.Id)?.ToString() ?? string.Empty;
            //    if (CommonString.AreEquals(run, br.ToString()) == false) {
            //        Create(br.Id);
            //        for (var l = 0; l < br.Lower.Length; l++) {
            //            AddNetworkAdapter(br.Id, br.Lower[l]);
            //        }
            //    }
            //}
            return true;
        }

        public static bool Create(string bridge) {
            var args = CommonString.Append(addbrArg, " ", bridge);
            Bash.Do($"{brctlCommand} {args}");
            return true;
        }

        public static bool AddNetworkAdapter(string bridge, string networkAdapter) {
            var args = CommonString.Append(addifArg, " ", bridge, " ", networkAdapter);
            Bash.Do($"{brctlCommand} {args}");
            return true;
        }

        public static bool Remove(string bridge) {
            var args = CommonString.Append(delbrArg, " ", bridge);
            Bash.Do($"{brctlCommand} {args}");
            return true;
        }

        public static bool DeleteNetworkAdapter(string bridge, string networkAdapter) {
            var args = CommonString.Append(delifArg, " ", bridge, " ", networkAdapter);
            Bash.Do($"{brctlCommand} {args}");
            return true;
        }

        public static bool SetStpOn(string bridge) {
            var args = CommonString.Append(stpArg, " ", bridge, " on");
            Bash.Do($"{brctlCommand} {args}");
            return true;
        }

        public static bool SetStpOff(string bridge) {
            var args = CommonString.Append(stpArg, " ", bridge, " off");
            Bash.Do($"{brctlCommand} {args}");
            return true;
        }

        public static bool SetPathCost(string bridge, string path, string cost) {
            var args = CommonString.Append("setpathcost ", bridge, " ", path, " ", cost, " set path cost");
            Bash.Do($"{brctlCommand} {args}");
            return true;
        }

        public static bool SetPortPriority(string bridge, string port, string priority) {
            var args = CommonString.Append("setportprio ", bridge, " ", port, " ", priority, " set port priority");
            Bash.Do($"{brctlCommand} {args}");
            return true;
        }
    }
}
