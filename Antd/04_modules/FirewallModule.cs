using Antd.cmds;
using Antd.models;
using Nancy;
using Newtonsoft.Json;

namespace Antd.Modules {
    public class FirewallModule : NancyModule {

        public FirewallModule() : base("/firewall") {

            Get["/"] = x => {
                return JsonConvert.SerializeObject(Application.CurrentConfiguration.Services.Firewall);
            };

            Post["/save"] = x => {
                string data = Request.Form.Data;
                var objects = JsonConvert.DeserializeObject<FirewallModel>(data);
                Application.CurrentConfiguration.Services.Firewall = objects;
                ConfigRepo.Save();
                return HttpStatusCode.OK;
            };

            Post["/apply"] = x => {
                Firewall.Apply();
                return HttpStatusCode.OK;
            };

            Post["/start"] = x => {
                Firewall.Start();
                return HttpStatusCode.OK;
            };
        }
    }
}