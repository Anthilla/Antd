using Nancy;
using Nancy.Security;

namespace AntdUi.Modules {
    public class MainModule : NancyModule {

        public MainModule() {
            this.RequiresAuthentication();

            Get["/"] = x => {
                return View["home"];
            };

            Get["/404"] = x => {
                return HttpStatusCode.NotFound;
            };

            Get["/403"] = x => {
                return HttpStatusCode.Forbidden;
            };

            Get["/500"] = x => {
                return HttpStatusCode.InternalServerError;
            };

            Get["/{page}"] = x => {
                string page = x.page;
                return View[page];
            };
        }
    }
}