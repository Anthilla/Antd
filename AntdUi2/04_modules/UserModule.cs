using anthilla.core;
using Nancy;
using System.Collections.Generic;

namespace AntdUi.Modules {
    public class UserModule : NancyModule {

        public UserModule() : base("/user") {

            Get["/get/system"] = x => {
                return ApiConsumer.GetJson(CommonString.Append(Application.ServerUrl, Request.Path));
            };

            Get["/get/group"] = x => {
                return ApiConsumer.GetJson(CommonString.Append(Application.ServerUrl, Request.Path));
            };

            Get["/get/group/running"] = x => {
                return ApiConsumer.GetJson(CommonString.Append(Application.ServerUrl, Request.Path));
            };

            Get["/get/applicative"] = x => {
                return ApiConsumer.GetJson(CommonString.Append(Application.ServerUrl, Request.Path));
            };

            Post["/save/system"] = x => {
                string data = Request.Form.Data;
                var dict = new Dictionary<string, string> {
                    { "Data", data }
                };
                return ApiConsumer.Post(CommonString.Append(Application.ServerUrl, Request.Path), dict);
            };

            Post["/save/group"] = x => {
                string data = Request.Form.Data;
                var dict = new Dictionary<string, string> {
                    { "Data", data }
                };
                return ApiConsumer.Post(CommonString.Append(Application.ServerUrl, Request.Path), dict);
            };

            Post["/save/applicative"] = x => {
                string data = Request.Form.Data;
                var dict = new Dictionary<string, string> {
                    { "Data", data }
                };
                return ApiConsumer.Post(CommonString.Append(Application.ServerUrl, Request.Path), dict);
            };

            Post["/apply/system"] = x => {
                return ApiConsumer.Post(CommonString.Append(Application.ServerUrl, Request.Path));
            };

            Post["/apply/group"] = x => {
                return ApiConsumer.Post(CommonString.Append(Application.ServerUrl, Request.Path));
            };
        }
    }
}