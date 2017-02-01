using antdlib.models;
using Antd.Info;
using Nancy;
using Newtonsoft.Json;

namespace Antd.ServerModules {
    public class AntdModulesStatusModule : NancyModule {

        public AntdModulesStatusModule() {
            Get["/modulesstatus"] = x => {
                var model = new PageModulesStatusModel();
                var machineInfo = new MachineInfo();
                model.Modules = machineInfo.GetModules();
                return JsonConvert.SerializeObject(model);
            };
        }
    }
}
