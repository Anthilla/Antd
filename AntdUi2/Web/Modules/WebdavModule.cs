using antd.core;
using Nancy;
using System;
using System.Text;

namespace AntdUi2.Modules {
    public class WebdavModule : NancyModule {

        public WebdavModule() : base("/webdav") {

            Get("/", x => ApiGet());

            Post("/save", x => Application.RestConsumer.Redirect(Request, Guid.NewGuid().ToString()));

            Post("/start", x => Application.RestConsumer.Redirect(Request, Guid.NewGuid().ToString()));

            Post("/stop", x => Application.RestConsumer.Redirect(Request, Guid.NewGuid().ToString()));

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