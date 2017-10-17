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

            Get["/clusterchecklist"] = x => {
                return JsonConvert.SerializeObject(Application.ClusterChecklist);
            };

            Get["/vm"] = x => {
                return JsonConvert.SerializeObject(Application.RunningConfiguration.Services.Virsh);
            };

            Get["/antduptime"] = x => {
                return JsonConvert.SerializeObject(Application.STOPWATCH.ElapsedMilliseconds);
            };
        }
    }
}