using Antd.cmds;
using Nancy;
using Newtonsoft.Json;

namespace Antd.Modules {
    public class BrctlModule : NancyModule {

        public BrctlModule() : base("/brctl") {

            Get["/"] = x => {
                return JsonConvert.SerializeObject(Application.CurrentConfiguration.Network.Bridges);
            };

            Get["/running"] = x => {
                return JsonConvert.SerializeObject(Application.RunningConfiguration.Network.Bridges);
            };

            Post["/save"] = x => {
                string data = Request.Form.Data;
                var objects = JsonConvert.DeserializeObject<NetBridge[]>(data);
                Application.CurrentConfiguration.Network.Bridges = objects;
                ConfigRepo.Save();
                return HttpStatusCode.OK;
            };

            Post["/apply"] = x => {
                Brctl.Apply();
                return HttpStatusCode.OK;
            };
        }
    }
}