using Antd.cmds;
using Antd.models;
using Nancy;
using Newtonsoft.Json;

namespace Antd.Modules {
    public class GlusterModule : NancyModule {

        public GlusterModule() : base("/gluster") {

            Post["/apply"] = x => {
                Gluster.Apply();
                return HttpStatusCode.OK;
            };

            Post["/start"] = x => {
                Gluster.Start();
                return HttpStatusCode.OK;
            };

            Post["/set"] = x => {
                string data = Request.Form.Data;
                var objects = JsonConvert.DeserializeObject<GlusterModel>(data);
                Application.CurrentConfiguration.Services.Gluster = objects;
                ConfigRepo.Save();
                return HttpStatusCode.OK;
            };
        }
    }
}