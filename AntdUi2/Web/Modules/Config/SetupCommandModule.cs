using Nancy;
using System;

namespace AntdUi2.Modules {
    public class SetupCommandModule : NancyModule {

        public SetupCommandModule() : base("/setupcmd/config") {

            Get("/", x => Application.RestConsumer.Redirect(Request, Guid.NewGuid().ToString()));

            Get("/save", x => Application.RestConsumer.Redirect(Request, Guid.NewGuid().ToString()));

            Get("/apply", x => Application.RestConsumer.Redirect(Request, Guid.NewGuid().ToString()));
        }
    }
}