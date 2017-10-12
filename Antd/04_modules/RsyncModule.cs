using Antd.cmds;
using Antd.models;
using Nancy;
using Newtonsoft.Json;

namespace Antd.Modules {
    public class RsyncModule : NancyModule {

        public RsyncModule() : base("/rsync") {

            Post["/apply"] = x => {
                RsyncWatcher.Start();
                return HttpStatusCode.OK;
            };

            Post["/set"] = x => {
                string data = Request.Form.Data;
                var objects = JsonConvert.DeserializeObject<RsyncModel>(data);
                Application.CurrentConfiguration.Services.Rsync = objects;
                ConfigRepo.Save();
                return HttpStatusCode.OK;
            };
        }
    }
}