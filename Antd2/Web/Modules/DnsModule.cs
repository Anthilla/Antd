using Nancy;

namespace Antd2.Modules {
    public class DnsModule : NancyModule {

        public DnsModule() : base("/dns") {

            Post("/apply", x => ApiPostApply());

            Post("/set/resolv", x => ApiPostSetResolv());

            Post("/set/hosts", x => ApiPostSetHosts());

            Post("/set/networks", x => ApiPostSetNetworks());

        }

        private dynamic ApiPostSave() {
            //Dns.Set();
            return HttpStatusCode.OK;
        }

        private dynamic ApiPostSetResolv() {
            //string data = Request.Form.Data;
            //var objects = JsonConvert.DeserializeObject<DnsClientConfiguration>(data);
            //Application.CurrentConfiguration.Network.KnownDns = objects;
            //ConfigRepo.Save();
            return HttpStatusCode.OK;
        }

        private dynamic ApiPostSetHosts() {
            //string data = Request.Form.Data;
            //var objects = JsonConvert.DeserializeObject<KnownHost[]>(data);
            //Application.CurrentConfiguration.Network.KnownHosts = objects;
            //ConfigRepo.Save();
            return HttpStatusCode.OK;
        }

        private dynamic ApiPostSetNetworks() {
            //string data = Request.Form.Data;
            //var objects = JsonConvert.DeserializeObject<KnownNetwork[]>(data);
            //Application.CurrentConfiguration.Network.KnownNetworks = objects;
            //ConfigRepo.Save();
            return HttpStatusCode.OK;
        }
    }
}