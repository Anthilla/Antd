using Antd.cmds;
using Nancy;
using Newtonsoft.Json;

namespace Antd.Modules {
    public class TimeDateModule : NancyModule {

        public TimeDateModule() : base("/timedate") {

            Get["/"] = x => {
                return JsonConvert.SerializeObject(Application.CurrentConfiguration.TimeDate);
            };

            Get["/running"] = x => {
                return JsonConvert.SerializeObject(Application.RunningConfiguration.TimeDate);
            };

            Post["/save"] = x => {
                string data = Request.Form.Data;
                var objects = JsonConvert.DeserializeObject<TimeDate>(data);
                Application.CurrentConfiguration.TimeDate = objects;
                ConfigRepo.Save();
                return HttpStatusCode.OK;
            };

            Post["/apply"] = x => {
                Timedatectl.Apply();
                if(Application.CurrentConfiguration.TimeDate.SyncFromRemoteServer) {
                    Ntpdate.SyncFromRemoteServer(Application.CurrentConfiguration.TimeDate.RemoteNtpServer);
                }
                return HttpStatusCode.OK;
            };
        }
    }
}