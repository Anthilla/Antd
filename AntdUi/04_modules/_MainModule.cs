using Nancy;
using Nancy.Security;

namespace AntdUi.Modules {
    public class MainModule : NancyModule {

        public MainModule() {
            this.RequiresAuthentication();

            Get["/"] = x => {
                return View["home"];
            };
        }
    }
}