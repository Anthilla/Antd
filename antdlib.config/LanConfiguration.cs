using antdlib.common;
using anthilla.commands;
using System.Collections.Generic;
using System.Linq;

namespace antdlib.config {
    public class LanConfiguration {
        private readonly string[] _interfaces;

        public LanConfiguration() {
            _interfaces = GetPhysicalInterfaces();
        }

        private static string[] GetPhysicalInterfaces() {
            var ifList = new List<string>();
            var list = Bash.Execute("ls -la /sys/class/net").SplitBash().Where(_ => _.Contains("->"));
            foreach(var f in list) {
                if(f.Contains("bond")) { }
                else if(f.Contains("br")) { }
                else if(f.Contains("virtual/net") || f.Contains("platform")) { }
                else if(!f.Contains("virtual/net")) {
                    var name = f.Print(9, " ");
                    ifList.Add(name.Trim());
                }
            }
            return ifList.ToArray();
        }

        public bool NothingIsConfigured() {
            return _interfaces.Select(t => CommandLauncher.Launch("ifconfig-if", new Dictionary<string, string> { { "$if", t } })).Select(result => result.FirstOrDefault(_ => _.Contains("inet"))).All(ip => ip == null);
        }

        public string GetFirstInterfaceNotConfigured() {
            var netIf = string.Empty;
            foreach(var t in _interfaces) {
                var result = CommandLauncher.Launch("ifconfig-if", new Dictionary<string, string> { { "$if", t } });
                var ip = result.FirstOrDefault(_ => _.Contains("inet"));
                if(ip == null) {
                    continue;
                }
                netIf = t;
                break;
            }
            return netIf;
        }

        public string GetInterfaceWithCarrier() {
            var netIf = string.Empty;
            foreach(var t in _interfaces) {
                var result = CommandLauncher.Launch("net-carrier", new Dictionary<string, string> { { "$if", t } }).ToList();
                if(!result.Any()) {
                    continue;
                }
                var carrierValue = result.FirstOrDefault();
                if(carrierValue == "1") {
                    netIf = t;
                    break;
                }
            }
            return netIf;
        }

        public string ConfigureInterface() {
            var netIf = GetInterfaceWithCarrier();
            if(string.IsNullOrEmpty(netIf)) {
                return string.Empty;
            }
            CommandLauncher.Launch("ip4-add-addr", new Dictionary<string, string> {
                { "$address", "192.168.1.1" },
                { "$range", "24" },
                { "$net_if", netIf }
            });
            return netIf;
        }
    }
}
