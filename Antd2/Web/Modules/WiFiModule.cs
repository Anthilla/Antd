using Nancy;
using Newtonsoft.Json;

namespace Antd2.Modules {
    public class WiFiModule : NancyModule {

        public WiFiModule() : base("/wifi") {

            Get("/", x => ApiGet());

            Post("/save", x => ApiPostSave());

            Post("/apply", x => ApiPostApply());
        }

        private dynamic ApiGet() {
            return Response.AsJson((object)Application.CurrentConfiguration.Network.WpaSupplicant);
        }

        private dynamic ApiPostSave() {
            //string data = Request.Form.Data;
            //var objects = JsonConvert.DeserializeObject<WpaSupplicant>(data);
            //Application.CurrentConfiguration.Network.WpaSupplicant = objects;
            //ConfigRepo.Save();
            return HttpStatusCode.OK;
        }

        private dynamic ApiPostApply() {
            //WiFi.Apply();
            return HttpStatusCode.OK;
        }
    }
}