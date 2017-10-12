using Nancy;
using Newtonsoft.Json;

namespace Antd.Modules {
    public class GatewayModule : NancyModule {

        public GatewayModule() : base("/gateway") {

            Get["/"] = x => {
                return JsonConvert.SerializeObject(Application.CurrentConfiguration.Network.Gateways);
            };

            Post["/save"] = x => {
                string data = Request.Form.Data;
                var objects = JsonConvert.DeserializeObject<NetGateway[]>(data);
                Application.CurrentConfiguration.Network.Gateways = objects;
                ConfigRepo.Save();
                return HttpStatusCode.OK;
            };
        }
    }
}