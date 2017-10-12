using Nancy;
using Newtonsoft.Json;

namespace Antd.Modules {
    public class InfoModule : NancyModule {

        public InfoModule() : base("/info") {

            Get["/memory"] = x => {
                return JsonConvert.SerializeObject(Application.RunningConfiguration.Info.MemInfo);
            };

            Get["/free"] = x => {
                return JsonConvert.SerializeObject(Application.RunningConfiguration.Info.Free);
            };

            Get["/cpu"] = x => {
                return JsonConvert.SerializeObject(Application.RunningConfiguration.Info.CpuInfo);
            };
        }
    }
}