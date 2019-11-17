using anthilla.core;
using Nancy;
using System;
using System.Text;

namespace AntdUi2.Modules {
    public class NetworkModule : NancyModule {

        public NetworkModule() : base("/network/config") {

            Get("/{context}", x => ApiGet());

            Post("/routingtables/save", x => Application.RestConsumer.Redirect(Request, Guid.NewGuid().ToString()));
            Post("/interfaces/save", x => Application.RestConsumer.Redirect(Request, Guid.NewGuid().ToString()));
            Post("/routing/save", x => Application.RestConsumer.Redirect(Request, Guid.NewGuid().ToString()));

            Post("/routingtables/apply", x => Application.RestConsumer.Redirect(Request, Guid.NewGuid().ToString()));
            Post("/interfaces/apply", x => Application.RestConsumer.Redirect(Request, Guid.NewGuid().ToString()));
            Post("/routing/apply", x => Application.RestConsumer.Redirect(Request, Guid.NewGuid().ToString()));
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