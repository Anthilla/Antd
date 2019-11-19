using antd.core;
using Nancy;
using System.Collections.Generic;
using System.Text;

namespace AntdUi2.Modules {
    public class TerminalModule : NancyModule {

        public TerminalModule() : base("/terminal") {

            Post("/", x => ApiPost());
        }

        private Response ApiPost() {
            string cmd = Request.Form["Command[command]"];
            var dict = new Dictionary<string, string>() {
                {"Command", cmd },
            };
            var myJsonString = ApiConsumer.PostAndGetJson($"{Application.ServerUrl}{Request.Path}", dict);
            var jsonBytes = Encoding.UTF8.GetBytes(myJsonString);
            return new Response {
                ContentType = "application/json",
                Contents = s => s.Write(jsonBytes, 0, jsonBytes.Length)
            };
        }
    }
}