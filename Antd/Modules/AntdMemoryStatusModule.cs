using Antd.Info;
using antdlib.models;
using Nancy;
using Newtonsoft.Json;

namespace Antd.Modules {
    public class AntdMemoryStatusModule : NancyModule {

        public AntdMemoryStatusModule() {
            Get["/memorystatus"] = x => {
                var model = new PageMemoryStatusModel {
                    Meminfo = MachineInfo.GetMeminfo(),
                    Free = MachineInfo.GetFree()
                };
                return JsonConvert.SerializeObject(model);
            };
        }
    }
}
