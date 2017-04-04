using Antd.Info;
using antdlib.models;
using Nancy;
using Newtonsoft.Json;

namespace Antd.Modules {
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
