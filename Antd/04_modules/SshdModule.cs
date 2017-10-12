using Antd.cmds;
using Antd.models;
using Nancy;
using Newtonsoft.Json;

namespace Antd.Modules {
    public class SshdModule : NancyModule {

        public SshdModule() : base("/sshd") {

            Post["/apply"] = x => {
                Sshd.Set();
                return HttpStatusCode.OK;
            };

            Post["/set"] = x => {
                string data = Request.Form.Data;
                var objects = JsonConvert.DeserializeObject<SshdModel>(data);
                Application.CurrentConfiguration.Services.Sshd = objects;
                ConfigRepo.Save();
                return HttpStatusCode.OK;
            };
        }
    }
}