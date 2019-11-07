using anthilla.core;
using Nancy;
using Newtonsoft.Json;

namespace Antd2.Modules {
    public class HomeModule : NancyModule {

        public HomeModule() {

            Get("/agent", x => ApiGetAgent());

            Get("/conf", x => ApiGetConf());
            
            Get("/running", x => ApiGetRunning());

        }

        private dynamic ApiGetAgent() {
            var agent = Encryption.XHash(Application.Agent);
            return Response.AsText((string)agent);
        }

        private dynamic ApiGetConf() {
            return Response.AsJson((object)Application.CurrentConfiguration);
        }

        private dynamic ApiGetRunning() {
            return Response.AsJson((object)Application.CurrentConfiguration);
        }
    }
}