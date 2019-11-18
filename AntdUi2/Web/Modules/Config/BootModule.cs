using antd.core;
using Nancy;
using System;
using System.Text;

namespace AntdUi2.Modules {

    public class BootModule : NancyModule {

        public BootModule() : base("/boot/config") {

            Get("/{context}", x => ApiGet());

            Post("/sysctl/save", x => Application.RestConsumer.Redirect(Request, Guid.NewGuid().ToString()));
            Post("/modules/save", x => Application.RestConsumer.Redirect(Request, Guid.NewGuid().ToString()));
            Post("/services/save", x => Application.RestConsumer.Redirect(Request, Guid.NewGuid().ToString()));

            Post("/sysctl/apply", x => Application.RestConsumer.Redirect(Request, Guid.NewGuid().ToString()));
            Post("/modules/apply", x => Application.RestConsumer.Redirect(Request, Guid.NewGuid().ToString()));
            Post("/services/apply", x => Application.RestConsumer.Redirect(Request, Guid.NewGuid().ToString()));
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