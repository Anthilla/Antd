using Antd.cmds;
using Nancy;
using Newtonsoft.Json;

namespace Antd.Modules {
    public class BondModule : NancyModule {

        public BondModule() : base("/bond") {

            Get["/"] = x => {
                return JsonConvert.SerializeObject(Application.CurrentConfiguration.Network.Bonds);
            };

            Get["/running"] = x => {
                return JsonConvert.SerializeObject(Application.RunningConfiguration.Network.Bonds);
            };

            Post["/save"] = x => {
                string data = Request.Form.Data;
                var objects = JsonConvert.DeserializeObject<NetBond[]>(data);
                Application.CurrentConfiguration.Network.Bonds = objects;
                ConfigRepo.Save();
                return HttpStatusCode.OK;
            };

            Post["/apply"] = x => {
                Bond.Apply();
                return HttpStatusCode.OK;
            };
        }
    }
}