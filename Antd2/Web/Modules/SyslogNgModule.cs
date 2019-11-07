using Antd.cmds;
using Antd.models;
using Nancy;
using Newtonsoft.Json;

namespace Antd2.Modules {
    public class SyslogNgModule : NancyModule {

        public SyslogNgModule() : base("/syslogng") {

            Get["/"] = x => {
                return JsonConvert.SerializeObject(Application.CurrentConfiguration.Services.SyslogNg);
            };

            Post["/save"] = x => {
                string data = Request.Form.Data;
                var objects = JsonConvert.DeserializeObject<SyslogNgModel>(data);
                Application.CurrentConfiguration.Services.SyslogNg = objects;
                ConfigRepo.Save();
                return HttpStatusCode.OK;
            };

            Post["/apply"] = x => {
                SyslogNg.Apply();
                return HttpStatusCode.OK;
            };
        }
    }
}