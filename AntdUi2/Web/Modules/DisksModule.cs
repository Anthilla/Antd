using anthilla.core;
using Nancy;
using System;
using System.Text;

namespace AntdUi2.Modules {
    public class DisksModule : NancyModule {

        public DisksModule() : base("/disks") {

            Get("/", x => ApiGet());

            Post("/create/partition/table", x => Application.RestConsumer.Redirect(Request, Guid.NewGuid().ToString()));
            Post("/create/partition", x => Application.RestConsumer.Redirect(Request, Guid.NewGuid().ToString()));
            Post("/create/fs/ext4", x => Application.RestConsumer.Redirect(Request, Guid.NewGuid().ToString()));
            Post("/create/fs/zfs", x => Application.RestConsumer.Redirect(Request, Guid.NewGuid().ToString()));

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