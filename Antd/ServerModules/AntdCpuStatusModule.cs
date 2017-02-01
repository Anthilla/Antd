using System.Linq;
using antdlib.models;
using Antd.Info;
using Nancy;
using Newtonsoft.Json;

namespace Antd.ServerModules {
    public class AntdCpuStatusModule : NancyModule {

        public AntdCpuStatusModule() {
            Get["/cpustatus"] = x => {
                var model = new PageCpuStatusModel();
                var machineInfo = new MachineInfo();
                model.Cpuinfo = machineInfo.GetCpuinfo().Where(_ => _.Key.Length > 1 && _.Value.Length > 1);
                return JsonConvert.SerializeObject(model);
            };
        }
    }
}
