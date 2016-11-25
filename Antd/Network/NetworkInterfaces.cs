using System.Collections.Generic;
using System.Linq;
using antdlib.common;
using antdlib.common.Tool;

namespace Antd.Network {
    public class NetworkInterfaces {
        public enum NetworkInterfaceType {
            Physical = 1,
            Virtual = 2,
            Bond = 3,
            Bridge = 4,
            Other = 99
        }

        private readonly Bash _bash = new Bash();

        public IEnumerable<string> GetAllNames() {
            if(!Parameter.IsUnix) {
                return new List<string>();
            }
            var list = _bash.Execute("ls -la /sys/class/net").SplitBash().Where(_ => _.Contains("->"));
            return list.Select(f => f.Print(9, " ")).ToList();
        }

        public Dictionary<string, NetworkInterfaceType> GetAll() {
            if(!Parameter.IsUnix) {
                return new Dictionary<string, NetworkInterfaceType>();
            }
            var ifList = new Dictionary<string, NetworkInterfaceType>();
            var list = _bash.Execute("ls -la /sys/class/net").SplitBash().Where(_ => _.Contains("->"));
            foreach(var f in list) {
                NetworkInterfaceType type;
                if(f.Contains("bond")) {
                    type = NetworkInterfaceType.Bond;
                }
                else if(f.Contains("br")) {
                    type = NetworkInterfaceType.Bridge;
                }
                else if(f.Contains("virtual/net") || f.Contains("platform")) {
                    type = NetworkInterfaceType.Virtual;
                }
                else if(!f.Contains("virtual/net")) {
                    type = NetworkInterfaceType.Physical;
                }
                else {
                    type = NetworkInterfaceType.Physical;
                }

                var name = f.Print(9, " ");
                ifList.Add(name, type);
            }
            return ifList;
        }
    }
}
