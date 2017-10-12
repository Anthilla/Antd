using Antd.cmds;
using Antd.models;
using Nancy;
using Newtonsoft.Json;

namespace Antd.Modules {
    public class NginxModule : NancyModule {

        public NginxModule() : base("/nginx") {

            Post["/apply"] = x => {
                Nginx.Apply();
                return HttpStatusCode.OK;
            };

            Post["/start"] = x => {
                Nginx.Start();
                return HttpStatusCode.OK;
            };

            Post["/set"] = x => {
                string data = Request.Form.Data;
                var objects = JsonConvert.DeserializeObject<NginxModel>(data);
                Application.CurrentConfiguration.Services.Nginx = objects;
                ConfigRepo.Save();
                return HttpStatusCode.OK;
            };
        }
    }
}