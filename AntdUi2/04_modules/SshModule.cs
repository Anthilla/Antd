using anthilla.core;
using Nancy;
using Nancy.Security;
using System.Collections.Generic;

namespace AntdUi.Modules {
    public class SshModule : NancyModule {

        private static string Agent;

        public SshModule() : base("/ssh") {
            this.RequiresAuthentication();

            Before += ctx => {
                Agent = ApiConsumer.GetString(CommonString.Append(Application.ServerUrl, "/agent"));
                return null;
            };

            Get["/authorizedkeys"] = x => {
                return ApiConsumer.GetJson(CommonString.Append(Application.ServerUrl, Request.Path));
            };

            Get["/publickey"] = x => {
                return ApiConsumer.GetJson(CommonString.Append(Application.ServerUrl, Request.Path));
            };

            Post["/save/authorizedkeys"] = x => {
                string data = Request.Form.Data;
                var dict = new Dictionary<string, string> {
                    { "Data", data }
                };
                return ApiConsumer.Post(CommonString.Append(Application.ServerUrl, Request.Path), dict);
            };

            Post["/apply/authorizedkeys"] = x => {
                return ApiConsumer.Post(CommonString.Append(Application.ServerUrl, Request.Path));
            };
        }
    }
}