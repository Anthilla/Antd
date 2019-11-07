using Nancy;

namespace Antd2.Modules {
    public class RouteModule : NancyModule {

        public RouteModule() : base("/route") {

            Post("/save", x => ApiPostSave());

            Post("/apply", x => ApiPostApply());

        }

        private dynamic ApiPostSave() {
            //string data = Request.Form.Data;
            //var objects = JsonConvert.DeserializeObject<NetRoute[]>(data);
            //Application.CurrentConfiguration.Network.Routing = objects;
            //ConfigRepo.Save();
            return HttpStatusCode.OK;
        }

        private dynamic ApiPostApply() {
            //Route.Set();
            return HttpStatusCode.OK;
        }
    }
}