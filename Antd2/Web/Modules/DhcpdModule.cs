using Antd.cmds;
using Antd.models;
using Nancy;
using Newtonsoft.Json;

namespace Antd2.Modules {
    public class DhcpdModule : NancyModule {

        public DhcpdModule() : base("/dhcpd") {

            Post("/save", x => ApiPostSave());

            Post("/start", x => ApiPostStart());

            Post("/apply", x => ApiPostApply());

        }

        private dynamic ApiPostSave() {
            //string data = Request.Form.Data;
            //var objects = JsonConvert.DeserializeObject<DhcpdModel>(data);
            //Application.CurrentConfiguration.Services.Dhcpd = objects;
            //ConfigRepo.Save();
            return HttpStatusCode.OK;
        }

        private dynamic ApiPostApply() {
            //Dhcpd.Apply();
            return HttpStatusCode.OK;
        }

        private dynamic ApiPostStart() {
            //Dhcpd.Start();
            return HttpStatusCode.OK;
        }
    }
}