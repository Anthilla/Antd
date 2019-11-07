using Nancy;
using System;

namespace Antd2.Web {

    public class HomeModule : NancyModule {
        public HomeModule() {

            Get("/", args => "Hello from Nancy running on CoreCLR");
            Get("/200", args => HttpStatusCode.OK);
            Get("/404", args => HttpStatusCode.NotFound);

        }
    }
}
