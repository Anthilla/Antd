using Antd.cmds;
using Antd.models;
using Nancy;
using Newtonsoft.Json;

namespace Antd2.Modules {
    public class BindModule : NancyModule {

        public BindModule() : base("/bind") {

            Get["/"] = x => {
                return JsonConvert.SerializeObject(Application.CurrentConfiguration.Services.Bind);
            };

            Post["/apply"] = x => {
                Bind.Apply();
                return HttpStatusCode.OK;
            };

            Post["/save"] = x => {
                string data = Request.Form.Data;
                var objects = JsonConvert.DeserializeObject<BindModel>(data);
                Application.CurrentConfiguration.Services.Bind = objects;
                ConfigRepo.Save();
                return HttpStatusCode.OK;
            };
        }
    }
}