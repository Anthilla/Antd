//using anthilla.core;
//using Nancy;
//using Nancy.Security;
//using System.Collections.Generic;

//namespace AntdUi2.Modules {
//    public class VirshModule : NancyModule {

//        public VirshModule() : base("/virsh") {
//            this.RequiresAuthentication();

//            Get["/"] = x => {
//                return ApiConsumer.GetJson(CommonString.Append(Application.ServerUrl, Request.Path));
//            };

//            Post["/save"] = x => {
//                string data = Request.Form.Data;
//                var dict = new Dictionary<string, string> {
//                    { "Data", data }
//                };
//                return ApiConsumer.Post(CommonString.Append(Application.ServerUrl, Request.Path), dict);
//            };

//            Post["/destroy"] = x => {
//                string data = Request.Form.Data;
//                var dict = new Dictionary<string, string> {
//                    { "Data", data }
//                };
//                return ApiConsumer.Post(CommonString.Append(Application.ServerUrl, Request.Path), dict);
//            };

//            Post["/reboot"] = x => {
//                string data = Request.Form.Data;
//                var dict = new Dictionary<string, string> {
//                    { "Data", data }
//                };
//                return ApiConsumer.Post(CommonString.Append(Application.ServerUrl, Request.Path), dict);
//            };

//            Post["/reset"] = x => {
//                string data = Request.Form.Data;
//                var dict = new Dictionary<string, string> {
//                    { "Data", data }
//                };
//                return ApiConsumer.Post(CommonString.Append(Application.ServerUrl, Request.Path), dict);
//            };

//            Post["/restore"] = x => {
//                string data = Request.Form.Data;
//                var dict = new Dictionary<string, string> {
//                    { "Data", data }
//                };
//                return ApiConsumer.Post(CommonString.Append(Application.ServerUrl, Request.Path), dict);
//            };

//            Post["/resume"] = x => {
//                string data = Request.Form.Data;
//                var dict = new Dictionary<string, string> {
//                    { "Data", data }
//                };
//                return ApiConsumer.Post(CommonString.Append(Application.ServerUrl, Request.Path), dict);
//            };

//            Post["/shutdown"] = x => {
//                string data = Request.Form.Data;
//                var dict = new Dictionary<string, string> {
//                    { "Data", data }
//                };
//                return ApiConsumer.Post(CommonString.Append(Application.ServerUrl, Request.Path), dict);
//            };

//            Post["/start"] = x => {
//                string data = Request.Form.Data;
//                var dict = new Dictionary<string, string> {
//                    { "Data", data }
//                };
//                return ApiConsumer.Post(CommonString.Append(Application.ServerUrl, Request.Path), dict);
//            };

//            Post["/suspend"] = x => {
//                string data = Request.Form.Data;
//                var dict = new Dictionary<string, string> {
//                    { "Data", data }
//                };
//                return ApiConsumer.Post(CommonString.Append(Application.ServerUrl, Request.Path), dict);
//            };

//            Post["/dompmsuspend"] = x => {
//                string data = Request.Form.Data;
//                var dict = new Dictionary<string, string> {
//                    { "Data", data }
//                };
//                return ApiConsumer.Post(CommonString.Append(Application.ServerUrl, Request.Path), dict);
//            };

//            Post["/dompmwakeup"] = x => {
//                string data = Request.Form.Data;
//                var dict = new Dictionary<string, string> {
//                    { "Data", data }
//                };
//                return ApiConsumer.Post(CommonString.Append(Application.ServerUrl, Request.Path), dict);
//            };
//        }
//    }
//}