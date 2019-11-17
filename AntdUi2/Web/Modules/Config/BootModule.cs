using anthilla.core;
using Nancy;
using System;
using System.Text;

namespace AntdUi2.Modules {

    public class BootModule : NancyModule {

        public BootModule() : base("/boot/config") {

            Get("/{context}", x => ApiGet());

            Post("/sysctl/save", x => Application.RestConsumer.Redirect(Request, Guid.NewGuid().ToString()));
            Post("/modules/active/save", x => Application.RestConsumer.Redirect(Request, Guid.NewGuid().ToString()));
            Post("/modules/inactive/save", x => Application.RestConsumer.Redirect(Request, Guid.NewGuid().ToString()));
            Post("/services/active/save", x => Application.RestConsumer.Redirect(Request, Guid.NewGuid().ToString()));
            Post("/services/inactive/save", x => Application.RestConsumer.Redirect(Request, Guid.NewGuid().ToString()));
            Post("/services/disabled/save", x => Application.RestConsumer.Redirect(Request, Guid.NewGuid().ToString()));
            Post("/services/blocked/save", x => Application.RestConsumer.Redirect(Request, Guid.NewGuid().ToString()));

            Post("/sysctl/apply", x => Application.RestConsumer.Redirect(Request, Guid.NewGuid().ToString()));
            Post("/modules/active/apply", x => Application.RestConsumer.Redirect(Request, Guid.NewGuid().ToString()));
            Post("/modules/inactive/apply", x => Application.RestConsumer.Redirect(Request, Guid.NewGuid().ToString()));
            Post("/services/active/apply", x => Application.RestConsumer.Redirect(Request, Guid.NewGuid().ToString()));
            Post("/services/inactive/apply", x => Application.RestConsumer.Redirect(Request, Guid.NewGuid().ToString()));
            Post("/services/disabled/apply", x => Application.RestConsumer.Redirect(Request, Guid.NewGuid().ToString()));
            Post("/services/blocked/apply", x => Application.RestConsumer.Redirect(Request, Guid.NewGuid().ToString()));
        }

        private dynamic ApiGet() {
            var jsonString = ApiConsumer.GetString($"{Application.ServerUrl}{Request.Path}");
            var jsonBytes = Encoding.UTF8.GetBytes(jsonString);
            return new Response {
                ContentType = "application/json",
                Contents = s => s.Write(jsonBytes, 0, jsonBytes.Length)
            };
        }
    }
}