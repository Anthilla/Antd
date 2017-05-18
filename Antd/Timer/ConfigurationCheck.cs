using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Timers;
using antdlib.common;
using antdlib.config;
using anthilla.commands;

namespace Antd.Timer {
    public class ConfigurationCheck {

        public System.Timers.Timer Timer { get; private set; }

        public void Start(int milliseconds) {
            Timer = new System.Timers.Timer(milliseconds);
            Timer.Elapsed += Action;
            Timer.Enabled = true;
        }

        public void Stop() {
            Timer?.Dispose();
        }

        private static void Action(object sender, ElapsedEventArgs e) {
            //ConsoleLogger.Log("[confcheck] check modules status");
            //var doo = new Do();
            //doo.ModulesChanges();

            ConsoleLogger.Log("[confcheck] check internet status");
            var p = new Ping();
            var pingReply = p.Send("8.8.8.8");
            if(pingReply?.Status == IPStatus.Success) {
                ConsoleLogger.Log("[confcheck] internet status: ok");
                return;
            }
            ConsoleLogger.Log("[confcheck] internet status: not available");
            ConsoleLogger.Log("[confcheck] check routing status");

            var routes = CommandLauncher.Launch("ip4-show-routes", new Dictionary<string, string> { { "$net_if", "" } }).ToList();
            foreach(var r in routes) {
                ConsoleLogger.Log(r);
            }
            if(!routes.Any(_ => _.Contains("[confcheck] routing failed"))) {
                ConsoleLogger.Warn("no route found");
                var host = Host2Configuration.Host;
                var gatewayConfigurations = Network2Configuration.GatewayConfigurationList;
                var deviceName = Network2Configuration.InterfacePhysical.FirstOrDefault();
                if(string.IsNullOrEmpty(deviceName)) {
                    ConsoleLogger.Log("[confcheck] no physical interface available");
                }
                else {
                    var routingOk = false;
                    var gwConfig = gatewayConfigurations.FirstOrDefault();
                    if(gwConfig == null) {

                        pingReply = p.Send(host.InternalHostIpPrimary);
                        if(pingReply?.Status != IPStatus.Success) {
                            ConsoleLogger.Log($"[confcheck] gateway {host.InternalHostIpPrimary} unreachable");
                        }
                        ConsoleLogger.Log($"[confcheck] applying default route at {deviceName} via {host.InternalHostIpPrimary}");
                        CommandLauncher.Launch("ip4-add-route", new Dictionary<string, string> { { "$net_if", deviceName }, { "$ip_address", "default" }, { "$gateway", host.InternalHostIpPrimary } });
                        routingOk = true;
                    }

                    foreach(var gwConf in gatewayConfigurations) {
                        if(routingOk)
                            continue;
                        pingReply = p.Send(gwConf.GatewayAddress);
                        if(pingReply?.Status != IPStatus.Success) {
                            ConsoleLogger.Log($"[confcheck] gateway {gwConf.GatewayAddress} unreachable");
                            continue;
                        }
                        ConsoleLogger.Log($"[confcheck] applying {gwConf.Route} route at {deviceName} via {gwConf.GatewayAddress}");
                        CommandLauncher.Launch("ip4-add-route", new Dictionary<string, string> { { "$net_if", deviceName }, { "$ip_address", gwConf.Route }, { "$gateway", gwConf.GatewayAddress } });
                        routingOk = true;
                    }
                }
            }

            pingReply = p.Send("8.8.8.8");
            if(pingReply?.Status == IPStatus.Success) {
                ConsoleLogger.Log("[confcheck] internet status: ok");
                return;
            }

            ConsoleLogger.Log("[confcheck] check interfaces status");
            var pifs = Network2Configuration.InterfacePhysical;
            foreach(var nif in pifs) {
                CommandLauncher.Launch("ip4-set-mtu", new Dictionary<string, string> { { "$net_if", nif }, { "$mtu", "6000" } });
                CommandLauncher.Launch("ip4-set-txqueuelen", new Dictionary<string, string> { { "$net_if", nif }, { "$txqueuelen", "10000" } });
                CommandLauncher.Launch("ip4-promisc-on", new Dictionary<string, string> { { "$net_if", nif } });
            }

            pingReply = p.Send("8.8.8.8");
            if(pingReply?.Status == IPStatus.Success) {
                ConsoleLogger.Log("[confcheck] internet status: ok");
            }

            var dhclientFirst = Network2Configuration.InterfacePhysical.FirstOrDefault();
            ConsoleLogger.Log($"[confcheck] configuring {dhclientFirst} via dhclient");
            CommandLauncher.Launch("dhclient4", new Dictionary<string, string> { { "$net_if", dhclientFirst } });

            pingReply = p.Send("8.8.8.8");
            if(pingReply?.Status == IPStatus.Success) {
                ConsoleLogger.Log("[confcheck] internet status: ok");
            }
            else {
                ConsoleLogger.Log("[confcheck] internet status: not available");
            }
        }
    }
}
