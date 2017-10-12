using Antd.cmds;
using Antd.models;
using Nancy;
using Newtonsoft.Json;

namespace Antd.Modules {
    public class SambaModule : NancyModule {

        public SambaModule() : base("/samba") {

            Post["/apply"] = x => {
                Samba.Apply();
                return HttpStatusCode.OK;
            };

            Post["/start"] = x => {
                Samba.Start();
                return HttpStatusCode.OK;
            };

            Post["/set"] = x => {
                string data = Request.Form.Data;
                var objects = JsonConvert.DeserializeObject<SambaModel>(data);
                Application.CurrentConfiguration.Services.Samba = objects;
                ConfigRepo.Save();
                return HttpStatusCode.OK;
            };
        }
    }
}