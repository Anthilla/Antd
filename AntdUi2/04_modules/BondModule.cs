using anthilla.core;
using Nancy;
using Nancy.Security;
using System.Collections.Generic;

namespace AntdUi.Modules {
    public class BondModule : NancyModule {

        public BondModule() : base("/bond") {
            this.RequiresAuthentication();

            Get["/"] = x => {
                return ApiConsumer.GetJson(CommonString.Append(Application.ServerUrl, Request.Path));
            };

            Get["/running"] = x => {
                return ApiConsumer.GetJson(CommonString.Append(Application.ServerUrl, Request.Path));
            };

            Post["/save"] = x => {
                string data = Request.Form.Data;
                var dict = new Dictionary<string, string> {
                    { "Data", data }
                };
                return ApiConsumer.Post(CommonString.Append(Application.ServerUrl, Request.Path), dict);
            };

            Post["/apply"] = x => {
                return ApiConsumer.Post(CommonString.Append(Application.ServerUrl, Request.Path));
            };
        }
    }
}