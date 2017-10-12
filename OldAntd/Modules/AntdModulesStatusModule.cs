using Antd.Info;
using antdlib.models;
using Nancy;
using Newtonsoft.Json;

namespace Antd.Modules {
    public class AntdModulesStatusModule : NancyModule {

        public AntdModulesStatusModule() {
            Get["/modulesstatus"] = x => {
                var model = new PageModulesStatusModel { Modules = MachineInfo.GetModules() };
                return JsonConvert.SerializeObject(model);
            };
        }
    }
}
