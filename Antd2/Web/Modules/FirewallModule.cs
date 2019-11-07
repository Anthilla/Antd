using Nancy;

namespace Antd2.Modules {
    public class FirewallModule : NancyModule {

        public FirewallModule() : base("/firewall") {

            Get("/", x => ApiGet());

            Post("/save", x => ApiPostSave());

            Post("/apply", x => ApiPostApply());

            Post("/start", x => ApiPostStart());
        }

        private dynamic ApiGet() {
            return Response.AsJson((object)Application.CurrentConfiguration.Services.Firewall);
        }

        private dynamic ApiPostSave() {
            //string data = Request.Form.Data;
            //var objects = JsonConvert.DeserializeObject<FirewallModel>(data);
            //Application.CurrentConfiguration.Services.Firewall = objects;
            //ConfigRepo.Save();
            return HttpStatusCode.OK;
        }

        private dynamic ApiPostApply() {
            //Firewall.Apply();
            return HttpStatusCode.OK;
        }
        private dynamic ApiPostStart() {
            //Firewall.Start();
            return HttpStatusCode.OK;
        }
    }
}