using Nancy;
using Newtonsoft.Json;

namespace Antd.Modules {
    public class DeviceModule : NancyModule {

        public DeviceModule() : base("/device") {

            Get["/description"] = x => {
                return JsonConvert.SerializeObject(cmds.Rssdp.GetDeviceDescription());
            };

            Get["/services"] = x => {
                return JsonConvert.SerializeObject(cmds.Rssdp.GetServices());
            };

            Post["/ok"] = x => {
                return HttpStatusCode.OK;
            };

            Get["/checklist"] = x => {
                return JsonConvert.SerializeObject(Application.Checklist);
            };
        }
    }
}