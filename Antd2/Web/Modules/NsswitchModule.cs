using Antd.cmds;
using Nancy;
using Newtonsoft.Json;

namespace Antd2.Modules {
    public class NsswitchModule : NancyModule {

        public NsswitchModule() : base("/nsswitch") {

            Post["/apply"] = x => {
                NS.Switch.Set();
                return HttpStatusCode.OK;
            };

            Post["/set"] = x => {
                string data = Request.Form.Data;
                var objects = JsonConvert.DeserializeObject<NsSwitch>(data);
                Application.CurrentConfiguration.NsSwitch = objects;
                ConfigRepo.Save();
                return HttpStatusCode.OK;
            };
        }
    }
}