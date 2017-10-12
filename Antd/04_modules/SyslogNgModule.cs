using Antd.cmds;
using Antd.models;
using Nancy;
using Newtonsoft.Json;

namespace Antd.Modules {
    public class SyslogNgModule : NancyModule {

        public SyslogNgModule() : base("/syslogng") {

            Post["/apply"] = x => {
                SyslogNg.Apply();
                return HttpStatusCode.OK;
            };

            Post["/start"] = x => {
                SyslogNg.Start();
                return HttpStatusCode.OK;
            };

            Post["/set"] = x => {
                string data = Request.Form.Data;
                var objects = JsonConvert.DeserializeObject<SyslogNgModel>(data);
                Application.CurrentConfiguration.Services.SyslogNg = objects;
                ConfigRepo.Save();
                return HttpStatusCode.OK;
            };
        }
    }
}