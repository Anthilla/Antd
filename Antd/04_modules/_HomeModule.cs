using anthilla.core;
using Nancy;
using Newtonsoft.Json;

namespace Antd.Modules {
    public class HomeModule : NancyModule {

        public HomeModule() {

            Get["/agent"] = x => {
                var agent = Encryption.XHash(Application.Agent);
                return Response.AsText(agent);
            };

            Get["/conf"] = x => {
                return JsonConvert.SerializeObject(Application.CurrentConfiguration, Formatting.Indented);
            };

            Get["/running"] = x => {
                return JsonConvert.SerializeObject(Application.RunningConfiguration, Formatting.Indented);
            };
        }
    }
}