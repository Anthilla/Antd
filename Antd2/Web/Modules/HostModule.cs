using Antd.cmds;
using Nancy;
using Newtonsoft.Json;

namespace Antd2.Modules {
    public class HostModule : NancyModule {

        public HostModule() : base("/host") {

            Get["/"] = x => {
                return JsonConvert.SerializeObject(Application.CurrentConfiguration.Host);
            };

            Get["/running"] = x => {
                return JsonConvert.SerializeObject(Application.RunningConfiguration.Host);
            };

            Post["/save"] = x => {
                string data = Request.Form.Data;
                var objects = JsonConvert.DeserializeObject<Host>(data);
                Application.CurrentConfiguration.Host = objects;
                ConfigRepo.Save();
                return HttpStatusCode.OK;
            };

            Post["/apply"] = x => {
                Hostnamectl.Apply();
                Application.RunningConfiguration.Host = Hostnamectl.Get();
                return HttpStatusCode.OK;
            };
        }
    }
}