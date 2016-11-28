using System.Collections.Generic;
using System.IO;
using System.Linq;
using antd.commands;
using antdlib.common;
using antdlib.common.Tool;

namespace Antd.Network {
    public class NetworkConfiguration {


        private readonly IEnumerable<string> _networkInterfaces;
        private readonly IEnumerable<string> _physicalNetworkInterfaces;
        private readonly CommandLauncher _launcher = new CommandLauncher();

        public NetworkConfiguration() {
            _networkInterfaces = new NetworkInterfaces().GetAll().Select(_ => _.Key).OrderBy(_ => _);
            _physicalNetworkInterfaces = new NetworkInterfaces().GetAll().Where(_ => _.Value == NetworkInterfaces.NetworkInterfaceType.Physical).Select(_ => _.Key).OrderBy(_ => _);
        }

        public void Start() {
            if(!_physicalNetworkInterfaces.Any()) {
                ConsoleLogger.Log("network config: no interface to set");
                StartFallback();
                return;
            }
            var netIf = _physicalNetworkInterfaces.FirstOrDefault();
            var tryStart = _launcher.Launch("dhclient4", new Dictionary<string, string> { { "$net_if", netIf } }).ToList();
            if(!tryStart.Any()) {
                ConsoleLogger.Log("network config: dhclient started");
                ConsoleLogger.Log($"network config: {netIf} is configured");
                return;
            }
            var tryGateway = _launcher.Launch("ip4-show-routes", new Dictionary<string, string> { { "$net_if", netIf } }).ToList();
            if(!tryGateway.Any()) {
                ConsoleLogger.Log("network config: no gateway available");
                StartFallback();
                return;
            }
            var gateway = tryGateway.FirstOrDefault(_ => _.Contains("default"));
            if(string.IsNullOrEmpty(gateway)) {
                ConsoleLogger.Log("network config: no gateway available");
                StartFallback();
                return;
            }
            var gatewayAddress = gateway.Replace("default", "").Trim();
            ConsoleLogger.Log($"network config: available gateway at {gatewayAddress}");

            var tryDns = _launcher.Launch("cat-etc-resolv").ToList();
            if(!tryGateway.Any()) {
                ConsoleLogger.Log("network config: no dns configured");
                SetupResolv();
            }
            var dns = tryDns.FirstOrDefault(_ => _.Contains("nameserver"));
            if(string.IsNullOrEmpty(dns)) {
                ConsoleLogger.Log("network config: no dns configured");
                SetupResolv();
            }
            var dnsAddress = gateway.Replace("nameserver", "").Trim();
            ConsoleLogger.Log($"network config: configured dns is {dnsAddress}");
            var pingDns = _launcher.Launch("ping-c", new Dictionary<string, string> { { "$net_if", dnsAddress } }).ToList();
            if(pingDns.Any(_ => _.ToLower().Contains("unreachable"))) {
                ConsoleLogger.Log("network config: dns is unreachable");
                return;
            }
            ConsoleLogger.Log($"network config: available dns at {dnsAddress}");
        }

        public void StartFallback() {
            ConsoleLogger.Log("network config: setting up a default configuration");
            const string bridge = "br0";
            if(_networkInterfaces.All(_ => _ != bridge)) {
                _launcher.Launch("brctl-add", new Dictionary<string, string> { { "$bridge", bridge } });
                ConsoleLogger.Log($"network config: {bridge} configured");
            }
            foreach(var phy in _physicalNetworkInterfaces) {
                _launcher.Launch("brctl-addif", new Dictionary<string, string> { { "$bridge", bridge }, { "$net_if", phy } });
                ConsoleLogger.Log($"network config: {phy} add to {bridge}");
            }
            foreach(var phy in _physicalNetworkInterfaces) {
                _launcher.Launch("ip4-enable-if", new Dictionary<string, string> { { "$net_if", phy } });
            }
            _launcher.Launch("ip4-enable-if", new Dictionary<string, string> { { "$net_if", bridge } });
            ConsoleLogger.Log("network config: interfaces up");
            const string address = "192.168.1.1";
            const string range = "24";
            _launcher.Launch("ip4-add-addr", new Dictionary<string, string> { { "$address", address }, { "$range", range }, { "$net_if", bridge } });

            var tryBridgeAddress = _launcher.Launch("ifconfig-if", new Dictionary<string, string> { { "$net_if", bridge } }).ToList();
            if(tryBridgeAddress.FirstOrDefault(_ => _.Contains("inet")) != null) {
                var bridgeAddress = tryBridgeAddress.Print(2, " ");
                ConsoleLogger.Log($"network config: {bridge} is now reachable at {bridgeAddress}");
                return;
            }
            ConsoleLogger.Log($"network config: {bridge} is unreachable");
        }

        public void SetupResolv() {
            if(!File.Exists("/etc/resolv.conf") || string.IsNullOrEmpty(File.ReadAllText("/etc/resolv.conf"))) {
                var lines = new List<string> {
                    "nameserver 8.8.8.8",
                    "search antd.local",
                    "domain antd.local"
                };
                File.WriteAllLines("/etc/resolv.conf", lines);
            }
            _launcher.Launch("link-s", new Dictionary<string, string> { { "$link", "/run/systemd/resolve/resolv.conf" }, { "$file", "/etc/resolv.conf" } });
        }

        public void SetupResolvD() {
            if(!File.Exists("/etc/resolv.conf") || string.IsNullOrEmpty(File.ReadAllText("/etc/resolv.conf"))) {
                var lines = new List<string> {
                    "[Resolve]",
                    "DNS=10.1.19.1 10.99.19.1",
                    "FallbackDNS=8.8.8.8 8.8.4.4 2001:4860:4860::8888 2001:4860:4860::8844",
                    "Domains=antd.local",
                    "LLMNR=yes"
                };
                File.WriteAllLines("/etc/systemd/resolved.conf", lines);
            }
            _launcher.Launch("systemctl-restart", new Dictionary<string, string> { { "$service", "systemd-resolved" } });
            _launcher.Launch("systemctl-daemonreload");
        }
    }
}
