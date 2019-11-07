using Antd.cmds;
using Nancy;

namespace Antd2.Modules {
    public class MountModule : NancyModule {

        public MountModule() : base("/mount") {

            Post["/apply"] = x => {
                Mount.Set();
                return HttpStatusCode.OK;
            };

            Post["/bind"] = x => {
                string source = Request.Form.Source;
                string destination = Request.Form.Destination;
                Mount.MountWithBind(source, destination);
                return HttpStatusCode.OK;
            };
        }
    }
}