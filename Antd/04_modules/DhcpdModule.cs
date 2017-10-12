using Antd.cmds;
using Antd.models;
using Nancy;
using Newtonsoft.Json;

namespace Antd.Modules {
    public class DhcpdModule : NancyModule {

        public DhcpdModule() : base("/dhcpd") {

            Post["/apply"] = x => {
                Dhcpd.Apply();
                return HttpStatusCode.OK;
            };

            Post["/start"] = x => {
                Dhcpd.Start();
                return HttpStatusCode.OK;
            };

            Post["/set"] = x => {
                string data = Request.Form.Data;
                var objects = JsonConvert.DeserializeObject<DhcpdModel>(data);
                Application.CurrentConfiguration.Services.Dhcpd = objects;
                ConfigRepo.Save();
                return HttpStatusCode.OK;
            };
        }
    }
}