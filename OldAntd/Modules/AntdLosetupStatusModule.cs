using Antd.Info;
using antdlib.models;
using Nancy;
using Newtonsoft.Json;

namespace Antd.Modules {
    public class AntdLosetupStatusModule : NancyModule {

        public AntdLosetupStatusModule() {
            Get["/losetupstatus"] = x => {
                var model = new PageLosetupStatusModel {
                    Components = MachineInfo.GetLosetup()
                };
                return JsonConvert.SerializeObject(model);
            };
        }
    }
}
