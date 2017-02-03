using Nancy;
using Nancy.Security;

namespace AntdUi.Modules {
    public class MainModule : NancyModule {
        public MainModule() {
            this.RequiresAuthentication();

            //Before += ctx => {
            //    var req = this.Request.Headers.UserAgent;
            //    antdlib.common.ConsoleLogger.Log(req);
            //    return null;
            //};

            Get["/"] = _ => Response.AsRedirect("/antd");

            Get["/antd"] = _ => View["home"];
        }
    }
}