using Antd.cmds;
using Nancy;
using Newtonsoft.Json;

namespace Antd.Modules {
    public class AppModule : NancyModule {

        public AppModule() : base("/app") {

            Get["/"] = x => {
                return JsonConvert.SerializeObject(App.GetLocal());
            };

            Post["/restart"] = x => {
                string data = Request.Form.Data;
                Systemctl.Restart(data);
                return HttpStatusCode.OK;
            };
        }
    }
}