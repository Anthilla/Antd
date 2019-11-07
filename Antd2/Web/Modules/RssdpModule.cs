using Nancy;
using Newtonsoft.Json;

namespace Antd2.Modules {
    public class RssdpModule : NancyModule {

        public RssdpModule() : base("/rssdp") {

            Get["/discover"] = x => {
                return JsonConvert.SerializeObject(ScanRssdpServices.ScannedDevices);
            };
        }
    }
}