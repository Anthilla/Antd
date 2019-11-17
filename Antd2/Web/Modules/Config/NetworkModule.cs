using Antd2.cmds;
using Antd2.Configuration;
using Nancy;
using Nancy.ModelBinding;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Text;

namespace Antd2.Modules {
    public class NetworkModule : NancyModule {

        public NetworkModule() : base("/network/config") {

            Get("/routingtables", x => ApiGetRoutingTables());
            Get("/interfaces", x => ApiGetInterfaces());
            Get("/routing", x => ApiGetRouting());

            Post("/routingtables/save", x => ApiPostSaveRoutingTables());
            Post("/interfaces/save", x => ApiPostSaveInterfaces());
            Post("/routing/save", x => ApiPostSaveRouting());

            Post("/routingtables/apply", x => ApiPostApplyRoutingTables());
            Post("/interfaces/apply", x => ApiPostApplyInterfaces());
            Post("/routing/apply", x => ApiPostApplyRouting());
        }

        private dynamic ApiGetInterfaces() {
            var interfaces = ConfigManager.Config.Saved.Network.Interfaces;
            foreach (var inf in interfaces) {
                if (inf.Auto == "up" || inf.Auto == "on")
                    inf.AutoBool = true;
                if (inf.Auto == "down" || inf.Auto == "off")
                    inf.AutoBool = false;
                inf.PreUpTxt = string.Join("\n", inf.PreUp);
                inf.PostUpTxt = string.Join("\n", inf.PostUp);
                inf.PreDownTxt = string.Join("\n", inf.PreDown);
                inf.PostDownTxt = string.Join("\n", inf.PostDown);
            }
            var jsonString = JsonConvert.SerializeObject(interfaces);
            var jsonBytes = Encoding.UTF8.GetBytes(jsonString);
            return new Response {
                ContentType = "application/json",
                Contents = s => s.Write(jsonBytes, 0, jsonBytes.Length)
            };
        }

        private dynamic ApiGetRoutingTables() {
            var routingtables = ConfigManager.Config.Saved.Network.RoutingTables;
            foreach (var inf in routingtables) {
                inf.RulesTxt = string.Join("\n", inf.Rules);
            }
            var jsonString = JsonConvert.SerializeObject(routingtables);
            var jsonBytes = Encoding.UTF8.GetBytes(jsonString);
            return new Response {
                ContentType = "application/json",
                Contents = s => s.Write(jsonBytes, 0, jsonBytes.Length)
            };
        }

        private dynamic ApiGetRouting() {
            var routing = ConfigManager.Config.Saved.Network.Routing;
            var jsonString = JsonConvert.SerializeObject(routing);
            var jsonBytes = Encoding.UTF8.GetBytes(jsonString);
            return new Response {
                ContentType = "application/json",
                Contents = s => s.Write(jsonBytes, 0, jsonBytes.Length)
            };
        }
        private dynamic ApiPostSaveRoutingTables() {
            string json = Request.Form.Data;
            var model = JsonConvert.DeserializeObject<NetRoutingTable[]>(json);
            foreach (var inf in model) {
                inf.Rules = inf.RulesTxt.Split('\n');
            }
            ConfigManager.Config.Saved.Network.RoutingTables = model;
            ConfigManager.Config.Dump();
            return HttpStatusCode.OK;
        }
        private dynamic ApiPostSaveInterfaces() {
            string json = Request.Form.Data;
            var interfaces = JsonConvert.DeserializeObject<NetInterface[]>(json);
            foreach (var inf in interfaces) {
                if (inf.AutoBool == true)
                    inf.Auto = "up";
                if (inf.AutoBool == false)
                    inf.Auto = "down";
                inf.PreUp = inf.PreUpTxt.Split('\n');
                inf.PostUp = inf.PostUpTxt.Split('\n');
                inf.PreDown = inf.PreDownTxt.Split('\n');
                inf.PostDown = inf.PostDownTxt.Split('\n');
            }
            ConfigManager.Config.Saved.Network.Interfaces = interfaces;
            ConfigManager.Config.Dump();
            return HttpStatusCode.OK;
        }
        private dynamic ApiPostSaveRouting() {
            string json = Request.Form.Data;
            var model = JsonConvert.DeserializeObject<NetRoute[]>(json);
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