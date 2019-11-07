using Antd.cmds;
using Nancy;
using Newtonsoft.Json;

namespace Antd2.Modules {
    public class RouteModule : NancyModule {

        public RouteModule() : base("/route") {

            Post["/apply"] = x => {
                Route.Set();
                return HttpStatusCode.OK;
            };

            Post["/set"] = x => {
                string data = Request.Form.Data;
                var objects = JsonConvert.DeserializeObject<NetRoute[]>(data);
                Application.CurrentConfiguration.Network.Routing = objects;
                ConfigRepo.Save();
                return HttpStatusCode.OK;
            };
        }
    }
}