using System.Collections.Generic;
using System.Linq;
using antd.commands;

namespace Antd.Network {
    public class LanConfiguration {
        private readonly string[] _interfaces;
        private readonly CommandLauncher _launcher;

        public LanConfiguration() {
            _interfaces = new NetworkInterfaces().GetAll().Where(_ => _.Value == NetworkInterfaces.NetworkInterfaceType.Physical).Select(_ => _.Key).ToArray();
            _launcher = new CommandLauncher();
        }

        public bool NothingIsConfigured() {
            var b = true;
            for(var i = 0; i < _interfaces.Length; i++) {
                var result = _launcher.Launch("ifconfig-if", new Dictionary<string, string> { { "$if", _interfaces[i] } });
                var ip = result.FirstOrDefault(_ => _.Contains("inet"));
                if(ip == null) {
                    continue;
                }
                b = false;
                break;
            }
            return b;
        }

        public string GetFirstInterfaceNotConfigured() {
            var netIf = string.Empty;
            for(var i = 0; i < _interfaces.Length; i++) {
                var result = _launcher.Launch("ifconfig-if", new Dictionary<string, string> { { "$if", _interfaces[i] } });
                var ip = result.FirstOrDefault(_ => _.Contains("inet"));
                if(ip == null) {
                    continue;
                }
                netIf = _interfaces[i];
                break;
            }
            return netIf;
        }

        public string GetInterfaceWithCarrier() {
            var netIf = string.Empty;
            for(var i = 0; i < _interfaces.Length; i++) {
                var result = _launcher.Launch("net-carrier", new Dictionary<string, string> { { "$if", _interfaces[i] } }).ToList();
                if(!result.Any()) {
                    continue;
                }
                var carrierValue = result.FirstOrDefault();
                if(carrierValue == "1") {
                    netIf = _interfaces[i];
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
            _launcher.Launch("ip4-add-addr", new Dictionary<string, string> {
                { "$address", "192.168.1.1" },
                { "$range", "24" },
                { "$net_if", netIf }
            });
            return netIf;
        }
    }
}
