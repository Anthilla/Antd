using Antd.Info;
using antdlib.models;
using Nancy;
using Newtonsoft.Json;

namespace Antd.Modules {
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
