using Antd.cmds;
using Nancy;
using Newtonsoft.Json;

namespace Antd.Modules {
    public class DnsModule : NancyModule {

        public DnsModule() : base("/dns") {

            Post["/apply"] = x => {
                Dns.Set();
                return HttpStatusCode.OK;
            };

            Post["/set/resolv"] = x => {
                string data = Request.Form.Data;
                var objects = JsonConvert.DeserializeObject<DnsClientConfiguration>(data);
                Application.CurrentConfiguration.Network.KnownDns = objects;
                ConfigRepo.Save();
                return HttpStatusCode.OK;
            };

            Post["/set/hosts"] = x => {
                string data = Request.Form.Data;
                var objects = JsonConvert.DeserializeObject<KnownHost[]>(data);
                Application.CurrentConfiguration.Network.KnownHosts = objects;
                ConfigRepo.Save();
                return HttpStatusCode.OK;
            };

            Post["/set/networks"] = x => {
                string data = Request.Form.Data;
                var objects = JsonConvert.DeserializeObject<KnownNetwork[]>(data);
                Application.CurrentConfiguration.Network.KnownNetworks = objects;
                ConfigRepo.Save();
                return HttpStatusCode.OK;
            };
        }
    }
}