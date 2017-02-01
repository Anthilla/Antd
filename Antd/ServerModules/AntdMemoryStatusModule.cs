using antdlib.models;
using Antd.Info;
using Nancy;
using Newtonsoft.Json;

namespace Antd.ServerModules {
    public class AntdMemoryStatusModule : NancyModule {

        public AntdMemoryStatusModule() {
            Get["/memorystatus"] = x => {
                var model = new PageMemoryStatusModel();
                var machineInfo = new MachineInfo();
                model.Meminfo = machineInfo.GetMeminfo();
                model.Free = machineInfo.GetFree();
                return JsonConvert.SerializeObject(model);
            };
        }
    }
}
