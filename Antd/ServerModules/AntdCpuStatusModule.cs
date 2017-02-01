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
                model.Cpuinfo = machineInfo.GetCpuinfo();
                return JsonConvert.SerializeObject(model);
            };
        }
    }
}
