using Nancy;

namespace Antd2.Modules {
    public class BrctlModule : NancyModule {

        public BrctlModule() : base("/brctl") {

            Get("/", x => ApiGet());

            Get("/running", x => ApiGetRunning());

            Post("/save", x => ApiPostSave());

            Post("/apply", x => ApiPostApply());

        }

        private dynamic ApiGet() {
            return Response.AsJson((object)Application.CurrentConfiguration.Network.Bridges);
        }

        private dynamic ApiGetRunning() {
            return Response.AsJson((object)Application.RunningConfiguration.Network.Bridges);
        }

        private dynamic ApiPostSave() {
            //string data = Request.Form.Data;
            //var objects = JsonConvert.DeserializeObject<NetBridge[]>(data);
            //Application.CurrentConfiguration.Network.Bridges = objects;
            //ConfigRepo.Save();
            return HttpStatusCode.OK;
        }

        private dynamic ApiPostApply() {
            //Brctl.Apply();
            return HttpStatusCode.OK;
        }
    }
}