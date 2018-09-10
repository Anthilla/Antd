using anthilla.core;
using Nancy;
using Nancy.Security;
using System.Collections.Generic;

namespace AntdUi.Modules {
    public class AppModule : NancyModule {

        public AppModule() : base("/app") {
            this.RequiresAuthentication();

            Get["/"] = x => {
                return ApiConsumer.GetJson(CommonString.Append(Application.ServerUrl, Request.Path));
            };

            Post["/restart"] = x => {
                string data = Request.Form.Data;
                var dict = new Dictionary<string, string> {
                    { "Data", data }
                };
                return ApiConsumer.Post(CommonString.Append(Application.ServerUrl, Request.Path), dict);
            };
        }
    }
}