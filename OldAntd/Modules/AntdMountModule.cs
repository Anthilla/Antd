using antdlib.config;
using Nancy;

namespace Antd.Modules {
    public class AntdMountModule : NancyModule {

        public AntdMountModule() {
            Get["/mount/checkdirs"] = x => {
                MountManagement.AllDirectories();
                return HttpStatusCode.OK;
            };
        }
    }
}
