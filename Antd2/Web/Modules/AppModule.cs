using Antd2.cmds;
using Nancy;

namespace Antd2.Modules {
    public class AppModule : NancyModule {

        public AppModule() : base("/app") {

            Get("/", x => ApiGet());

            Post("/restart", x => ApiPostRestart());

        }

        private dynamic ApiGet() {
            return Response.AsJson((object)null);
        }

        private dynamic ApiPostRestart() {
            string data = Request.Form.Data;
            Systemctl.Restart(data);
            return HttpStatusCode.OK;
        }
    }
}