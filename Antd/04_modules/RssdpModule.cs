using Nancy;
using Newtonsoft.Json;

namespace Antd.Modules {
    public class RssdpModule : NancyModule {

        public RssdpModule() : base("/rssdp") {

            Get["/discover"] = x => {
                return JsonConvert.SerializeObject(cmds.Rssdp.Discover().Result);
            };
        }
    }
}