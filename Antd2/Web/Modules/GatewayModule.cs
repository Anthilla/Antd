using Nancy;

namespace Antd2.Modules {
    public class GatewayModule : NancyModule {

        public GatewayModule() : base("/gateway") {

            Get("/", x => ApiGet());

            Post("/save", x => ApiPostSave());

        }

        private dynamic ApiGet() {
            return Response.AsJson((object)Application.CurrentConfiguration.Network.Gateways);
        }

        private dynamic ApiPostSave() {
            //string data = Request.Form.Data;
            //var objects = JsonConvert.DeserializeObject<NetGateway[]>(data);
            //Application.CurrentConfiguration.Network.Gateways = objects;
            //ConfigRepo.Save();
            return HttpStatusCode.OK;
        }
    }
}