//using anthilla.core;
//using Nancy;
//using Nancy.Security;
//using System.Collections.Generic;

//namespace AntdUi2.Modules {
//    public class BootModule : NancyModule {

//        public BootModule() : base("/boot") {
//            this.RequiresAuthentication();

//            Get["/parameters"] = x => {
//                return ApiConsumer.GetJson(CommonString.Append(Application.ServerUrl, Request.Path));
//            };

//            Get["/modules"] = x => {
//                return ApiConsumer.GetJson(CommonString.Append(Application.ServerUrl, Request.Path));
//            };

//            Get["/modules/list"] = x => {
//                return ApiConsumer.GetJson(CommonString.Append(Application.ServerUrl, Request.Path));
//            };

//            Get["/services"] = x => {
//                return ApiConsumer.GetJson(CommonString.Append(Application.ServerUrl, Request.Path));
//            };

//            Get["/services/list"] = x => {
//                return ApiConsumer.GetJson(CommonString.Append(Application.ServerUrl, Request.Path));
//            };

//            Get["/commands"] = x => {
//                return ApiConsumer.GetJson(CommonString.Append(Application.ServerUrl, Request.Path));
//            };

//            Post["/save/parameters"] = x => {
//                string data = Request.Form.Data;
//                var dict = new Dictionary<string, string> {
//                    { "Data", data }
//                };
//                return ApiConsumer.Post(CommonString.Append(Application.ServerUrl, Request.Path), dict);
//            };

//            Post["/save/modules"] = x => {
//                string data = Request.Form.Data;
//                var dict = new Dictionary<string, string> {
//                    { "Data", data }
//                };
//                return ApiConsumer.Post(CommonString.Append(Application.ServerUrl, Request.Path), dict);
//            };

//            Post["/save/services"] = x => {
//                string data = Request.Form.Data;
//                var dict = new Dictionary<string, string> {
//                    { "Data", data }
//                };
//                return ApiConsumer.Post(CommonString.Append(Application.ServerUrl, Request.Path), dict);
//            };

//            Post["/save/commands"] = x => {
//                string data = Request.Form.Data;
//                var dict = new Dictionary<string, string> {
//                    { "Data", data }
//                };
//                return ApiConsumer.Post(CommonString.Append(Application.ServerUrl, Request.Path), dict);
//            };

//            Post["/apply/parameters"] = x => {
//                return ApiConsumer.Post(CommonString.Append(Application.ServerUrl, Request.Path));
//            };

//            Post["/apply/modules"] = x => {
//                return ApiConsumer.Post(CommonString.Append(Application.ServerUrl, Request.Path));
//            };

//            Post["/apply/services"] = x => {
//                return ApiConsumer.Post(CommonString.Append(Application.ServerUrl, Request.Path));
//            };

//            Post["/apply/commands"] = x => {
//                return ApiConsumer.Post(CommonString.Append(Application.ServerUrl, Request.Path));
//            };
//        }
//    }
//}