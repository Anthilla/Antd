using antd.core;
using Nancy;
using System;
using System.Text;

namespace AntdUi2.Modules {
    public class NetworkVirtualModule : NancyModule {

        public NetworkVirtualModule() : base("/network/config") {

            Post("/bridge/save", x => Application.RestConsumer.Redirect(Request, Guid.NewGuid().ToString()));
            Post("/bond/save", x => Application.RestConsumer.Redirect(Request, Guid.NewGuid().ToString()));
            Post("/tun/save", x => Application.RestConsumer.Redirect(Request, Guid.NewGuid().ToString()));
            Post("/tap/save", x => Application.RestConsumer.Redirect(Request, Guid.NewGuid().ToString()));

            Post("/bridge/apply", x => Application.RestConsumer.Redirect(Request, Guid.NewGuid().ToString()));
            Post("/bond/apply", x => Application.RestConsumer.Redirect(Request, Guid.NewGuid().ToString()));
            Post("/tun/apply", x => Application.RestConsumer.Redirect(Request, Guid.NewGuid().ToString()));
            Post("/tap/apply", x => Application.RestConsumer.Redirect(Request, Guid.NewGuid().ToString()));
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