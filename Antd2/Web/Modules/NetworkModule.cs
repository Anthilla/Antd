using Antd2.cmds;
using Antd2.Configuration;
using Nancy;
using Nancy.ModelBinding;
using System;
using System.Linq;

namespace Antd2.Modules {
    public class NetworkModule : NancyModule {

        public NetworkModule() : base("/network") {

            Get("/", x => Response.AsJson((object)ConfigManager.Config.Saved.Network));

            Get("/routingtables/save", x => ApiPostSaveRoutingTables());
            Get("/interfaces/save", x => ApiPostSaveInterfaces());
            Get("/routing/save", x => ApiPostSaveRouting());

            Get("/routingtables/apply", x => ApiPostApplyRoutingTables());
            Get("/interfaces/apply", x => ApiPostApplyInterfaces());
            Get("/routing/apply", x => ApiPostApplyRouting());
        }

        private dynamic ApiPostSaveRoutingTables() {
            var model = this.Bind<NetRoutingTable[]>();
            ConfigManager.Config.Saved.Network.RoutingTables = model;
            ConfigManager.Config.Dump();
            return HttpStatusCode.OK;
        }
        private dynamic ApiPostSaveInterfaces() {
            var model = this.Bind<NetInterface[]>();
            ConfigManager.Config.Saved.Network.Interfaces = model;
            ConfigManager.Config.Dump();
            return HttpStatusCode.OK;
        }
        private dynamic ApiPostSaveRouting() {
            var model = this.Bind<NetRoute[]>();
            ConfigManager.Config.Saved.Network.Routing = model;
            ConfigManager.Config.Dump();
            return HttpStatusCode.OK;
        }

        private dynamic ApiPostApplyRoutingTables() {
            if (ConfigManager.Config.Saved.Network.RoutingTables.Length > 0) {
                RoutingTables.Write(ConfigManager.Config.Saved.Network.RoutingTables.Select(_ => (_.Id, _.Name)));
            }
            return HttpStatusCode.OK;
        }
        private dynamic ApiPostApplyInterfaces() {
            Dns.SetResolv(ConfigManager.Config.Saved.Network.Dns);
            foreach (var i in ConfigManager.Config.Saved.Network.Interfaces) {
                Console.WriteLine($"[net] configuring {i.Iface} {i.Address}");
                if (i.Auto == "up") {
                    foreach (var cmd in i.PreUp) {
                        Console.WriteLine($"[net] {i.Iface} - {cmd}");
                        Bash.Do(cmd);
                    }
                    Ip.EnableNetworkAdapter(i.Iface);
                    if (!string.IsNullOrEmpty(i.Address)) {
                        var address = Help.SplitAddressAndRange(i.Address);
                        if (string.IsNullOrEmpty(address.Range)) {
                            address.Range = "24";
                            Console.WriteLine($"[net] missing range definition, assigning 24 by default");
                        }
                        Ip.AddAddress(i.Iface, address.Address, address.Range);
                    }
                    foreach (var cmd in i.PostUp) {
                        Console.WriteLine($"[net] {i.Iface} - {cmd}");
                        Bash.Do(cmd);
                    }
                }
                else if (i.Auto == "down") {
                    foreach (var cmd in i.PreDown) {
                        Console.WriteLine($"[net] {i.Iface} - {cmd}");
                        Bash.Do(cmd);
                    }
                    Ip.DisableNetworkAdapter(i.Iface);
                    foreach (var cmd in i.PostDown) {
                        Console.WriteLine($"[net] {i.Iface} - {cmd}");
                        Bash.Do(cmd);
                    }
                }
            }
            foreach (var r in ConfigManager.Config.Saved.Network.Routing) {
                Console.WriteLine($"[net] routing {r.Gateway} {r.Destination} {r.Device}");
                Ip.AddRoute(r.Device, r.Gateway, r.Destination);
            }
            return HttpStatusCode.OK;
        }
        private dynamic ApiPostApplyRouting() {
            if (ConfigManager.Config.Saved.Network.RoutingTables.Length > 0) {
                foreach (var rt in ConfigManager.Config.Saved.Network.RoutingTables) {
                    foreach (var rule in rt.Rules) {
                        Bash.Do(rule);
                    }
                }
            }
            return HttpStatusCode.OK;
        }
    }
}