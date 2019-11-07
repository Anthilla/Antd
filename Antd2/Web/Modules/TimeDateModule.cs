using Nancy;

namespace Antd2.Modules {
    public class TimeDateModule : NancyModule {

        public TimeDateModule() : base("/timedate") {

            Get("/", x => ApiGet());

            Get("/running", x => ApiGetRunning());

            Post("/save", x => ApiPostSave());

            Post("/apply", x => ApiPostApply());
        }

        private dynamic ApiGet() {
            return Response.AsJson((object)Application.CurrentConfiguration.TimeDate);
        }
        private dynamic ApiGetRunning() {
            return Response.AsJson((object)Application.RunningConfiguration.TimeDate);
        }

        private dynamic ApiPostSave() {
            //string data = Request.Form.Data;
            //var objects = JsonConvert.DeserializeObject<TimeDate>(data);
            //Application.CurrentConfiguration.TimeDate = objects;
            //ConfigRepo.Save();
            return HttpStatusCode.OK;
        }

        private dynamic ApiPostApply() {
            //Timedatectl.Apply();
            //if (Application.CurrentConfiguration.TimeDate.SyncFromRemoteServer) {
            //    Ntpdate.SyncFromRemoteServer(Application.CurrentConfiguration.TimeDate.RemoteNtpServer);
            //}
            return HttpStatusCode.OK;
        }
    }
}