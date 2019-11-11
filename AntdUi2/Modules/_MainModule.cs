using Nancy;
using Nancy.Security;

namespace AntdUi2.Modules {
    public class MainModule : NancyModule {

        public MainModule() {
            this.RequiresAuthentication();

            Get("/", x => View["home"]);
        }
    }
}