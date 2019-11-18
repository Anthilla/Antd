using antd.core;
using Nancy;
using System;
using System.Text;

namespace AntdUi2.Modules {
    public class VolumesModule : NancyModule {

        public VolumesModule() : base("/volumes") {

            Get("/", x => ApiGet());

            Post("/mount", x => Application.RestConsumer.Redirect(Request, Guid.NewGuid().ToString()));
            Post("/umount", x => Application.RestConsumer.Redirect(Request, Guid.NewGuid().ToString()));
            Post("/webdav/start", x => Application.RestConsumer.Redirect(Request, Guid.NewGuid().ToString()));
            Post("/webdav/stop", x => Application.RestConsumer.Redirect(Request, Guid.NewGuid().ToString()));
            Post("/sync", x => Application.RestConsumer.Redirect(Request, Guid.NewGuid().ToString()));

        }

        private Response ApiGet() {
            var myJsonString = ApiConsumer.GetString($"{Application.ServerUrl}{Request.Path}");
            var jsonBytes = Encoding.UTF8.GetBytes(myJsonString);
            return new Response {
                ContentType = "application/json",
                Contents = s => s.Write(jsonBytes, 0, jsonBytes.Length)
            };
        }
    }
}