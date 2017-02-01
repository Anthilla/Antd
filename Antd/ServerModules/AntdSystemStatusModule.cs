using antdlib.models;
using Antd.Info;
using Nancy;
using Newtonsoft.Json;

namespace Antd.ServerModules {
    public class AntdSystemStatusModule : NancyModule {

        public AntdSystemStatusModule() {
            Get["/systemstatus"] = x => {
                var machineInfo = new MachineInfo();
                var model = new PageSystemStatusModel {
                    Components = machineInfo.GetSystemComponentModels()
                };
                return JsonConvert.SerializeObject(model);
            };
        }
    }
}
