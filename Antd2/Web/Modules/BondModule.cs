using Nancy;
using Newtonsoft.Json;

namespace Antd2.Modules {
    public class BondModule : NancyModule {

        public BondModule() : base("/bond") {

            Get("/", x => ApiGet());

            Get("/running", x => ApiGetRunning());

            Post("/save", x => ApiPostSave());

            Post("/apply", x => ApiPostApply());

        }

        private dynamic ApiGet() {
            return Response.AsJson((object)Application.CurrentConfiguration.Network.Bonds);
        }
        private dynamic ApiGetRunning() {
            return Response.AsJson((object)Application.RunningConfiguration.Network.Bonds);
        }
        private dynamic ApiPostSave() {
            //string data = Request.Form.Data;
            //var objects = JsonConvert.DeserializeObject<NetBond[]>(data);
            //Application.CurrentConfiguration.Network.Bonds = objects;
            //ConfigRepo.Save();
            return HttpStatusCode.OK;
        }

        private dynamic ApiPostApply() {
            //Bond.Apply();
            return HttpStatusCode.OK;
        }

    }
}