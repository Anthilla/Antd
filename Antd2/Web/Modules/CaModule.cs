using Antd.cmds;
using Antd.models;
using Nancy;
using Newtonsoft.Json;

namespace Antd2.Modules {
    public class CaModule : NancyModule {

        public CaModule() : base("/ca") {

            Post["/apply"] = x => {
                Ca.Apply();
                return HttpStatusCode.OK;
            };

            Post["/set"] = x => {
                string data = Request.Form.Data;
                var objects = JsonConvert.DeserializeObject<CaModel>(data);
                Application.CurrentConfiguration.Services.Ca = objects;
                ConfigRepo.Save();
                return HttpStatusCode.OK;
            };
        }
    }
}