using Nancy;
using Nancy.Security;

namespace AntdUi.Modules {
    public class AppModule : NancyModule {
        public AppModule() {
            this.RequiresAuthentication();

            //Before += ctx => {
            //    var req = this.Request.Headers.UserAgent;
            //    antdlib.common.ConsoleLogger.Log(req);
            //    return null;
            //};

            Get["/"] = _ => {
                return Response.AsRedirect("/antd");
            };

            Get["/antd"] = _ => {
                return View["home"];
            };
        }
    }
}