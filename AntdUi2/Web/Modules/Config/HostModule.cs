using Nancy;
using System;

namespace AntdUi2.Modules {
    public class HostModule : NancyModule {

        public HostModule() : base("/host/config") {

            Get("/", x => Application.RestConsumer.Redirect(Request, Guid.NewGuid().ToString()));

            Get("/save", x => Application.RestConsumer.Redirect(Request, Guid.NewGuid().ToString()));

            Get("/apply", x => Application.RestConsumer.Redirect(Request, Guid.NewGuid().ToString()));
        }
    }
}