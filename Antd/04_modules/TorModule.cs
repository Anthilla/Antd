using Antd.cmds;
using Antd.models;
using Nancy;
using Newtonsoft.Json;

namespace Antd.Modules {
    public class TorModule : NancyModule {

        public TorModule() : base("/tor") {

            Post["/apply"] = x => {
                Tor.Apply();
                return HttpStatusCode.OK;
            };

            Post["/start"] = x => {
                Tor.Start();
                return HttpStatusCode.OK;
            };

            Post["/set"] = x => {
                string data = Request.Form.Data;
                var objects = JsonConvert.DeserializeObject<TorModel>(data);
                Application.CurrentConfiguration.Services.Tor = objects;
                ConfigRepo.Save();
                return HttpStatusCode.OK;
            };
        }
    }
}