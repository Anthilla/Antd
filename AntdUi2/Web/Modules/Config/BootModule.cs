using Nancy;
using System;

namespace AntdUi2.Modules {

    public class BootModule : NancyModule {

        public BootModule() : base("/boot/config") {

            Get("/", x => Application.RestConsumer.Redirect(Request, Guid.NewGuid().ToString()));

            Get("/sysctl/save", x => Application.RestConsumer.Redirect(Request, Guid.NewGuid().ToString()));
            Get("/modules/active/save", x => Application.RestConsumer.Redirect(Request, Guid.NewGuid().ToString()));
            Get("/modules/inactive/save", x => Application.RestConsumer.Redirect(Request, Guid.NewGuid().ToString()));
            Get("/services/active/save", x => Application.RestConsumer.Redirect(Request, Guid.NewGuid().ToString()));
            Get("/services/inactive/save", x => Application.RestConsumer.Redirect(Request, Guid.NewGuid().ToString()));
            Get("/services/disabled/save", x => Application.RestConsumer.Redirect(Request, Guid.NewGuid().ToString()));
            Get("/services/blocked/save", x => Application.RestConsumer.Redirect(Request, Guid.NewGuid().ToString()));

            Get("/sysctl/apply", x => Application.RestConsumer.Redirect(Request, Guid.NewGuid().ToString()));
            Get("/modules/active/apply", x => Application.RestConsumer.Redirect(Request, Guid.NewGuid().ToString()));
            Get("/modules/inactive/apply", x => Application.RestConsumer.Redirect(Request, Guid.NewGuid().ToString()));
            Get("/services/active/apply", x => Application.RestConsumer.Redirect(Request, Guid.NewGuid().ToString()));
            Get("/services/inactive/apply", x => Application.RestConsumer.Redirect(Request, Guid.NewGuid().ToString()));
            Get("/services/disabled/apply", x => Application.RestConsumer.Redirect(Request, Guid.NewGuid().ToString()));
            Get("/services/blocked/apply", x => Application.RestConsumer.Redirect(Request, Guid.NewGuid().ToString()));
        }
    }
}