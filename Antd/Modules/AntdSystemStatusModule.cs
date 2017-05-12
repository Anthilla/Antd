using Antd.Info;
using antdlib.models;
using Nancy;
using Newtonsoft.Json;

namespace Antd.Modules {
    public class AntdSystemStatusModule : NancyModule {

        public AntdSystemStatusModule() {
            Get["/systemstatus"] = x => {
                var model = new PageSystemStatusModel {
                    Components = MachineInfo.GetSystemComponentModels()
                };
                return JsonConvert.SerializeObject(model);
            };
        }
    }
}
