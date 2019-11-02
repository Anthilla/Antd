using Nancy;

namespace Antd2.Web {

    public class HomeModule : NancyModule {
        public HomeModule() {

            Get("/", args => "Hello from Nancy running on CoreCLR");

        }
    }
}
