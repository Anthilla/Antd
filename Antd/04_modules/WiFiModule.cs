using Antd.cmds;
using Nancy;
using Newtonsoft.Json;

namespace Antd.Modules {
    public class WiFiModule : NancyModule {

        public WiFiModule() : base("/wifi") {

            Get["/"] = x => {
                return JsonConvert.SerializeObject(Application.CurrentConfiguration.Network.WpaSupplicant);
            };

            Post["/save"] = x => {
                string data = Request.Form.Data;
                var objects = JsonConvert.DeserializeObject<WpaSupplicant>(data);
                Application.CurrentConfiguration.Network.WpaSupplicant = objects;
                ConfigRepo.Save();
                return HttpStatusCode.OK;
            };

            Post["/apply"] = x => {
                WiFi.Apply();
                return HttpStatusCode.OK;
            };
        }
    }
}