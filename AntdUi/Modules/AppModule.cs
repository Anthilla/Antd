using Nancy;
using Nancy.Security;

namespace AntdUi.Modules {
    public class AppModule : NancyModule {
        public AppModule() {
            this.RequiresAuthentication();

            Get["/"] = _ => {
                return Response.AsRedirect("/antd");
            };

            Get["/antd"] = _ => {
                return View["home"];
            };
        }
    }
}