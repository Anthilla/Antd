using Antd.cmds;
using Nancy;

namespace Antd2.Modules {
    public class MountModule : NancyModule {

        public MountModule() : base("/mount") {

            Post("/bind", x => ApiPostBind());

            Post("/apply", x => ApiPostApply());

        }

        private dynamic ApiPostBind() {
            //string source = Request.Form.Source;
            //string destination = Request.Form.Destination;
            //Mount.MountWithBind(source, destination);
            return HttpStatusCode.OK;
        }

        private dynamic ApiPostApply() {
            //Mount.Set();
            return HttpStatusCode.OK;
        }
    }
}