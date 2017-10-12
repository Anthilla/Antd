using antdlib.config;
using antdlib.models;
using Nancy;
using Newtonsoft.Json;

namespace Antd.Modules {
    public class AntdOvermountStatusModule : NancyModule {

        public AntdOvermountStatusModule() {
            Get["/overmountstatus"] = x => {
                var model = new PageOvermountStatusModel {
                    Components = MountManagement.GetAll()
                };
                return JsonConvert.SerializeObject(model);
            };
        }
    }
}
