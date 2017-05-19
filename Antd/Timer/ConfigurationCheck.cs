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
            Action();
            Timer = new System.Timers.Timer(milliseconds);
            Timer.Elapsed += Elapsed;
            Timer.Enabled = true;
        }

        public void Stop() {
            Timer?.Dispose();
        }

        private static void Elapsed(object sender, ElapsedEventArgs e) {
            Action();
        }

        private static void Action() {
            //ConsoleLogger.Log("[check] check modules status");
            //var doo = new Do();
            //doo.ModulesChanges();

            ConsoleLogger.Log("[check] check internet status");
            var p = new Ping();
            var pingReply = p.Send("8.8.8.8");
            if(pingReply?.Status == IPStatus.Success) {
                ConsoleLogger.Log("[check] internet status: ok");
                return;
            }
            ConsoleLogger.Log("[check] internet status: not available");
            ConsoleLogger.Log("[check] check routing status");

            var routes = CommandLauncher.Launch("ip4-show-routes", new Dictionary<string, string> { { "$net_if", "" } }).ToList();
            foreach(var r in routes) {
                ConsoleLogger.Log(r);
            }
            if(!routes.Any(_ => _.Contains("[check] routing failed"))) {
                ConsoleLogger.Warn("no route found");
                var host = Host2Configuration.Host;
                var gatewayConfigurations = Network2Configuration.GatewayConfigurationList;
                var deviceName = Network2Configuration.InterfacePhysical.FirstOrDefault();
                if(string.IsNullOrEmpty(deviceName)) {
                    ConsoleLogger.Log("[check] no physical interface available");
                }
                else {
                    var routingOk = false;
                    var gwConfig = gatewayConfigurations.FirstOrDefault();
                    if(gwConfig == null) {

                        pingReply = p.Send(host.InternalHostIpPrimary);
                        if(pingReply?.Status != IPStatus.Success) {
                            ConsoleLogger.Log($"[check] gateway {host.InternalHostIpPrimary} unreachable");
                        }
                        ConsoleLogger.Log($"[check] applying default route at {deviceName} via {host.InternalHostIpPrimary}");
                        CommandLauncher.Launch("ip4-add-route", new Dictionary<string, string> { { "$net_if", deviceName }, { "$ip_address", "default" }, { "$gateway", host.InternalHostIpPrimary } });
                        routingOk = true;
                    }

                    foreach(var gwConf in gatewayConfigurations) {
                        if(routingOk)
                            continue;
                        pingReply = p.Send(gwConf.GatewayAddress);
                        if(pingReply?.Status != IPStatus.Success) {
                            ConsoleLogger.Log($"[check] gateway {gwConf.GatewayAddress} unreachable");
                            continue;
                        }
                        ConsoleLogger.Log($"[check] applying {gwConf.Route} route at {deviceName} via {gwConf.GatewayAddress}");
                        CommandLauncher.Launch("ip4-add-route", new Dictionary<string, string> { { "$net_if", deviceName }, { "$ip_address", gwConf.Route }, { "$gateway", gwConf.GatewayAddress } });
                        routingOk = true;
                    }
                }
            }

            pingReply = p.Send("8.8.8.8");
            if(pingReply?.Status == IPStatus.Success) {
                ConsoleLogger.Log("[check] internet status: ok");
                return;
            }

            ConsoleLogger.Log("[check] check interfaces status");
            var pifs = Network2Configuration.InterfacePhysical;
            foreach(var nif in pifs) {
                CommandLauncher.Launch("ip4-set-mtu", new Dictionary<string, string> { { "$net_if", nif }, { "$mtu", "6000" } });
                CommandLauncher.Launch("ip4-set-txqueuelen", new Dictionary<string, string> { { "$net_if", nif }, { "$txqueuelen", "10000" } });
                CommandLauncher.Launch("ip4-promisc-on", new Dictionary<string, string> { { "$net_if", nif } });
            }

            //pingReply = p.Send("8.8.8.8");
            //if(pingReply?.Status == IPStatus.Success) {
            //    ConsoleLogger.Log("[check] internet status: ok");
            //}
            //var dhclientFirst = Network2Configuration.InterfacePhysical.FirstOrDefault();
            //ConsoleLogger.Log($"[check] configuring {dhclientFirst} via dhclient");
            //CommandLauncher.Launch("dhclient4", new Dictionary<string, string> { { "$net_if", dhclientFirst } });
            //CommandLauncher.Launch("ip4-set-mtu", new Dictionary<string, string> { { "$net_if", dhclientFirst }, { "$mtu", "6000" } });
            //CommandLauncher.Launch("ip4-set-txqueuelen", new Dictionary<string, string> { { "$net_if", dhclientFirst }, { "$txqueuelen", "10000" } });
            //CommandLauncher.Launch("ip4-promisc-on", new Dictionary<string, string> { { "$net_if", dhclientFirst } });
            //foreach(var br in Network2Configuration.InterfaceBridge) {
            //    CommandLauncher.Launch("brctl-del-if", new Dictionary<string, string> { { "$bridge", br }, { "$net_if", dhclientFirst } });
            //}
            //foreach(var bond in Network2Configuration.InterfaceBond) {
            //    CommandLauncher.Launch("bond-del-if", new Dictionary<string, string> { { "$bond", bond }, { "$net_if", dhclientFirst } });
            //}

            pingReply = p.Send("8.8.8.8");
            if(pingReply?.Status == IPStatus.Success) {
                ConsoleLogger.Log("[check] internet status: ok");
            }
            else {
                ConsoleLogger.Log("[check] internet status: not available");
            }
        }
    }
}
