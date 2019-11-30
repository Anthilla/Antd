using antd.core;
using Nancy;
using System;
using System.Text;

namespace AntdUi2.Modules {
    public class FileManagerModule : NancyModule {

        public FileManagerModule() : base("/fm") {

            Get("/{path*}", x => ApiGet());

            Post("/folder/create", x => Application.RestConsumer.Redirect(Request, Guid.NewGuid().ToString()));

            Post("/folder/move", x => Application.RestConsumer.Redirect(Request, Guid.NewGuid().ToString()));

            Post("/folder/delete", x => Application.RestConsumer.Redirect(Request, Guid.NewGuid().ToString()));

            Post("/folder/sync", x => Application.RestConsumer.Redirect(Request, Guid.NewGuid().ToString()));

            Post("/file/move", x => Application.RestConsumer.Redirect(Request, Guid.NewGuid().ToString()));

            Post("/file/delete", x => Application.RestConsumer.Redirect(Request, Guid.NewGuid().ToString()));

            Post("/file/sync", x => Application.RestConsumer.Redirect(Request, Guid.NewGuid().ToString()));

            Get("/file/download/{path*}", x => Application.RestConsumer.Redirect(Request, Guid.NewGuid().ToString()));

            Post("/file/upload", x => Application.RestConsumer.Redirect(Request, Guid.NewGuid().ToString()));
        }

        private dynamic ApiGet() {
            var jsonString = ApiConsumer.GetString($"{Application.ServerUrl}{Request.Path}");
            var jsonBytes = Encoding.UTF8.GetBytes(jsonString);
            return new Response {
                ContentType = "application/json",
                Contents = s => s.Write(jsonBytes, 0, jsonBytes.Length)
            };
        }

        private dynamic ApiPostFileDownload(dynamic x) {
            string path = x.path;
            throw new System.NotImplementedException();
        }

        private dynamic ApiPostFileUpload() {
            throw new System.NotImplementedException();
        }
    }
}