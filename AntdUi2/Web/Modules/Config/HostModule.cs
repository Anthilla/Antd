using antd.core;
using Nancy;
using System;
using System.Text;

namespace AntdUi2.Modules {
    public class HostModule : NancyModule {

        public HostModule() : base("/host/config") {

            Get("/", x => ApiGet());

            Post("/save", x => Application.RestConsumer.Redirect(Request, Guid.NewGuid().ToString()));

            Post("/apply", x => Application.RestConsumer.Redirect(Request, Guid.NewGuid().ToString()));
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