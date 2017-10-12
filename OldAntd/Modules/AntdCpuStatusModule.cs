using Antd.Info;
using antdlib.models;
using Nancy;
using Newtonsoft.Json;
using System.Linq;

namespace Antd.Modules {
    public class AntdCpuStatusModule : NancyModule {

        public AntdCpuStatusModule() {
            Get["/cpustatus"] = x => {
                var model = new PageCpuStatusModel {
                    Cpuinfo = MachineInfo.GetCpuinfo().Where(_ => _.Key.Length > 1 && _.Value.Length > 1)
                };
                return JsonConvert.SerializeObject(model);
            };
        }
    }
}
