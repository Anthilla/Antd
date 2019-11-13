using Nancy;
using System;

namespace AntdUi2.Modules {
    public class NetworkModule : NancyModule {

        public NetworkModule() : base("/network/config") {

            Get("/", x => Application.RestConsumer.Redirect(Request, Guid.NewGuid().ToString()));

            Get("/routingtables/save", x => Application.RestConsumer.Redirect(Request, Guid.NewGuid().ToString()));
            Get("/interfaces/save", x => Application.RestConsumer.Redirect(Request, Guid.NewGuid().ToString()));
            Get("/routing/save", x => Application.RestConsumer.Redirect(Request, Guid.NewGuid().ToString()));

            Get("/routingtables/apply", x => Application.RestConsumer.Redirect(Request, Guid.NewGuid().ToString()));
            Get("/interfaces/apply", x => Application.RestConsumer.Redirect(Request, Guid.NewGuid().ToString()));
            Get("/routing/apply", x => Application.RestConsumer.Redirect(Request, Guid.NewGuid().ToString()));
        }
    }
}