using Nancy;
using Nancy.Security;

namespace AntdUi2.Modules {
    public class MainModule : NancyModule {

        public MainModule() {
            Before += ctx => {
                // (this.Context.CurrentUser == null) ? HttpStatusCode.Unauthorized :
                if (this.Context.CurrentUser == null) {
                    return HttpStatusCode.Unauthorized;
                }
                return null;
            };

            Get("/", x => View["home.min.html"]);
        }
    }
}