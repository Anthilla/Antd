using antdlib.models;
using Antd.Info;
using Nancy;
using Newtonsoft.Json;

namespace Antd.ServerModules {
    public class AntdLosetupStatusModule : NancyModule {

        public AntdLosetupStatusModule() {
            Get["/losetupstatus"] = x => {
                var machineInfo = new MachineInfo();
                var model = new PageLosetupStatusModel {
                    Components = machineInfo.GetLosetup()
                };
                return JsonConvert.SerializeObject(model);
            };
        }
    }
}
