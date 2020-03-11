using Antd2.cmds;
using Antd2.Configuration;
using Nancy;
using Newtonsoft.Json;
using System;
using System.Text;

namespace Antd2.Modules {
    public class NetworkVirtualModule : NancyModule {

        public NetworkVirtualModule() : base("/network/config") {

            Get("/bridge", x => ApiGetBridge());
            Get("/bond", x => ApiGetBond());
            Get("/tun", x => ApiGetTun());
            Get("/tap", x => ApiGetTap());

            Post("/bridge/save", x => ApiPostSaveBridge());
            Post("/bond/save", x => ApiPostSaveBond());
            Post("/tun/save", x => ApiPostSaveTun());
            Post("/tap/save", x => ApiPostSaveTap());

            Post("/bridge/apply", x => ApiPostApplyBridge());
            Post("/bond/apply", x => ApiPostApplyBond());
            Post("/tun/apply", x => ApiPostApplyTun());
            Post("/tap/apply", x => ApiPostApplyTap());
        }

        private dynamic ApiGetBridge() {
            var bridges = ConfigManager.Config.Saved.Network.Bridges;
            foreach (var bridge in bridges) {
                bridge.LowerTxt = string.Join(";", bridge.Lower);
            }
            var jsonString = JsonConvert.SerializeObject(bridges);
            var jsonBytes = Encoding.UTF8.GetBytes(jsonString);
            return new Response {
                ContentType = "application/json",
                Contents = s => s.Write(jsonBytes, 0, jsonBytes.Length)
            };
        }
        private dynamic ApiGetBond() {
            var bonds = ConfigManager.Config.Saved.Network.Bonds;
            foreach (var bond in bonds) {
                bond.LowerTxt = string.Join(";", bond.Lower);
            }
            var jsonString = JsonConvert.SerializeObject(bonds);
            var jsonBytes = Encoding.UTF8.GetBytes(jsonString);
            return new Response {
                ContentType = "application/json",
                Contents = s => s.Write(jsonBytes, 0, jsonBytes.Length)
            };
        }
        private dynamic ApiGetTun() {
            var routingtables = ConfigManager.Config.Saved.Network.Tuns;
            var jsonString = JsonConvert.SerializeObject(routingtables);
            var jsonBytes = Encoding.UTF8.GetBytes(jsonString);
            return new Response {
                ContentType = "application/json",
                Contents = s => s.Write(jsonBytes, 0, jsonBytes.Length)
            };
        }
        private dynamic ApiGetTap() {
            var routing = ConfigManager.Config.Saved.Network.Taps;
            var jsonString = JsonConvert.SerializeObject(routing);
            var jsonBytes = Encoding.UTF8.GetBytes(jsonString);
            return new Response {
                ContentType = "application/json",
                Contents = s => s.Write(jsonBytes, 0, jsonBytes.Length)
            };
        }

        private dynamic ApiPostSaveBridge() {
            string json = Request.Form.Data;
            var data = JsonConvert.DeserializeObject<NetBridge[]>(json);
            foreach(var d in data) 
                d.Lower = d.LowerTxt.Split(';');
            ConfigManager.Config.Saved.Network.Bridges = data;
            ConfigManager.Config.Dump();
            return HttpStatusCode.OK;
        }
        private dynamic ApiPostSaveBond() {
            string json = Request.Form.Data;
            var data = JsonConvert.DeserializeObject<NetBond[]>(json);
            foreach (var d in data)
                d.Lower = d.LowerTxt.Split(';');
            ConfigManager.Config.Saved.Network.Bonds = data;
            ConfigManager.Config.Dump();
            return HttpStatusCode.OK;
        }
        private dynamic ApiPostSaveTun() {
            string json = Request.Form.Data;
            var data = JsonConvert.DeserializeObject<NetTun[]>(json);
            ConfigManager.Config.Saved.Network.Tuns = data;
            ConfigManager.Config.Dump();
            return HttpStatusCode.OK;
        }
        private dynamic ApiPostSaveTap() {
            string json = Request.Form.Data;
            var data = JsonConvert.DeserializeObject<NetTap[]>(json);
            ConfigManager.Config.Saved.Network.Taps = data;
            ConfigManager.Config.Dump();
            return HttpStatusCode.OK;
        }

        private dynamic ApiPostApplyBridge() {
            foreach (var i in ConfigManager.Config.Saved.Network.Bridges) {
                foreach (var na in i.Lower)
                    Ip.EnableNetworkAdapter(na);
                Console.WriteLine($"[net] brctl add {i.Name}");
                Brctl.Create(i.Name);
                foreach (var na in i.Lower) {
                    Console.WriteLine($"[net] brctl add {i.Name} - {na}");
                    Brctl.AddNetworkAdapter(i.Name, na);
                    Ip.EnableNetworkAdapter(na);
                }
                Ip.EnableNetworkAdapter(i.Name);
            }
            return HttpStatusCode.OK;
        }
        private dynamic ApiPostApplyBond() {
            foreach (var i in ConfigManager.Config.Saved.Network.Bonds) {
                foreach (var na in i.Lower)
                    Ip.EnableNetworkAdapter(na);
                Bond.Create(i.Name);
                foreach (var na in i.Lower) {
                    Bond.AddNetworkAdapter(i.Name, na);
                    Ip.EnableNetworkAdapter(na);
                }
                Ip.EnableNetworkAdapter(i.Name);
            }
            return HttpStatusCode.OK;
        }
        private dynamic ApiPostApplyTun() {
            foreach (var i in ConfigManager.Config.Saved.Network.Tuns)
                Ip.CreateTun(i);
            return HttpStatusCode.OK;
        }
        private dynamic ApiPostApplyTap() {
            foreach (var i in ConfigManager.Config.Saved.Network.Taps)
                Ip.CreateTap(i);
            return HttpStatusCode.OK;
        }

    }
}