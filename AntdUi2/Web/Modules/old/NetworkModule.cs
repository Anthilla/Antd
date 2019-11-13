//using anthilla.core;
//using Nancy;
//using System.Collections.Generic;

//namespace AntdUi2.Modules {
//    public class NetworkModule : NancyModule {

//        public NetworkModule() : base("/network") {

//            Get["/primarydomain"] = x => {
//                return ApiConsumer.GetJson(CommonString.Append(Application.ServerUrl, Request.Path));
//            };

//            Get["/knowndns"] = x => {
//                return ApiConsumer.GetJson(CommonString.Append(Application.ServerUrl, Request.Path));
//            };

//            Get["/knownhosts"] = x => {
//                return ApiConsumer.GetJson(CommonString.Append(Application.ServerUrl, Request.Path));
//            };

//            Get["/knownnetworks"] = x => {
//                return ApiConsumer.GetJson(CommonString.Append(Application.ServerUrl, Request.Path));
//            };

//            Get["/internalnetwork"] = x => {
//                return ApiConsumer.GetJson(CommonString.Append(Application.ServerUrl, Request.Path));
//            };

//            Get["/externalnetwork"] = x => {
//                return ApiConsumer.GetJson(CommonString.Append(Application.ServerUrl, Request.Path));
//            };

//            Get["/networkinterfaces"] = x => {
//                return ApiConsumer.GetJson(CommonString.Append(Application.ServerUrl, Request.Path));
//            };

//            Get["/routingtables"] = x => {
//                return ApiConsumer.GetJson(CommonString.Append(Application.ServerUrl, Request.Path));
//            };

//            Get["/routing"] = x => {
//                return ApiConsumer.GetJson(CommonString.Append(Application.ServerUrl, Request.Path));
//            };

//            Get["/devices"] = x => {
//                return ApiConsumer.GetJson(CommonString.Append(Application.ServerUrl, Request.Path));
//            };

//            Get["/devices/addr"] = x => {
//                return ApiConsumer.GetJson(CommonString.Append(Application.ServerUrl, Request.Path));
//            };

//            Get["/interfaces"] = x => {
//                return ApiConsumer.GetJson(CommonString.Append(Application.ServerUrl, Request.Path));
//            };

//            Get["/default/hosts"] = x => {
//                return ApiConsumer.GetJson(CommonString.Append(Application.ServerUrl, Request.Path));
//            };

//            Get["/default/networks"] = x => {
//                return ApiConsumer.GetJson(CommonString.Append(Application.ServerUrl, Request.Path));
//            };

//            Get["/tuns"] = x => {
//                return ApiConsumer.GetJson(CommonString.Append(Application.ServerUrl, Request.Path));
//            };

//            Get["/taps"] = x => {
//                return ApiConsumer.GetJson(CommonString.Append(Application.ServerUrl, Request.Path));
//            };

//            Post["/save/knowndns"] = x => {
//                string data = Request.Form.Data;
//                var dict = new Dictionary<string, string> {
//                    { "Data", data }
//                };
//                return ApiConsumer.Post(CommonString.Append(Application.ServerUrl, Request.Path), dict);
//            };

//            Post["/save/knownhosts"] = x => {
//                string data = Request.Form.Data;
//                var dict = new Dictionary<string, string> {
//                    { "Data", data }
//                };
//                return ApiConsumer.Post(CommonString.Append(Application.ServerUrl, Request.Path), dict);
//            };

//            Post["/save/knownnetworks"] = x => {
//                string data = Request.Form.Data;
//                var dict = new Dictionary<string, string> {
//                    { "Data", data }
//                };
//                return ApiConsumer.Post(CommonString.Append(Application.ServerUrl, Request.Path), dict);
//            };

//            Post["/save/internalnetwork"] = x => {
//                string data = Request.Form.Data;
//                var dict = new Dictionary<string, string> {
//                    { "Data", data }
//                };
//                return ApiConsumer.Post(CommonString.Append(Application.ServerUrl, Request.Path), dict);
//            };

//            Post["/save/externalnetwork"] = x => {
//                string data = Request.Form.Data;
//                var dict = new Dictionary<string, string> {
//                    { "Data", data }
//                };
//                return ApiConsumer.Post(CommonString.Append(Application.ServerUrl, Request.Path), dict);
//            };

//            Post["/save/networkinterfaces"] = x => {
//                string data = Request.Form.Data;
//                var dict = new Dictionary<string, string> {
//                    { "Data", data }
//                };
//                return ApiConsumer.Post(CommonString.Append(Application.ServerUrl, Request.Path), dict);
//            };

//            Post["/save/routingtables"] = x => {
//                string data = Request.Form.Data;
//                var dict = new Dictionary<string, string> {
//                    { "Data", data }
//                };
//                return ApiConsumer.Post(CommonString.Append(Application.ServerUrl, Request.Path), dict);
//            };

//            Post["/save/routing"] = x => {
//                string data = Request.Form.Data;
//                var dict = new Dictionary<string, string> {
//                    { "Data", data }
//                };
//                return ApiConsumer.Post(CommonString.Append(Application.ServerUrl, Request.Path), dict);
//            };

//            Post["/save/interfaces"] = x => {
//                string data = Request.Form.Data;
//                var dict = new Dictionary<string, string> {
//                    { "Data", data }
//                };
//                return ApiConsumer.Post(CommonString.Append(Application.ServerUrl, Request.Path), dict);
//            };

//            Post["/save/tuns"] = x => {
//                string data = Request.Form.Data;
//                var dict = new Dictionary<string, string> {
//                    { "Data", data }
//                };
//                return ApiConsumer.Post(CommonString.Append(Application.ServerUrl, Request.Path), dict);
//            };

//            Post["/save/taps"] = x => {
//                string data = Request.Form.Data;
//                var dict = new Dictionary<string, string> {
//                    { "Data", data }
//                };
//                return ApiConsumer.Post(CommonString.Append(Application.ServerUrl, Request.Path), dict);
//            };

//            Post["/apply/knowndns"] = x => {
//                return ApiConsumer.Post(CommonString.Append(Application.ServerUrl, Request.Path));
//            };

//            Post["/apply/knownhosts"] = x => {
//                return ApiConsumer.Post(CommonString.Append(Application.ServerUrl, Request.Path));
//            };

//            Post["/apply/knownnetworks"] = x => {
//                return ApiConsumer.Post(CommonString.Append(Application.ServerUrl, Request.Path));
//            };

//            Post["/apply/internalnetwork"] = x => {
//                return ApiConsumer.Post(CommonString.Append(Application.ServerUrl, Request.Path));
//            };

//            Post["/apply/externalnetwork"] = x => {
//                return ApiConsumer.Post(CommonString.Append(Application.ServerUrl, Request.Path));
//            };

//            Post["/apply/networkinterfaces"] = x => {
//                return ApiConsumer.Post(CommonString.Append(Application.ServerUrl, Request.Path));
//            };

//            Post["/apply/routingtables"] = x => {
//                return ApiConsumer.Post(CommonString.Append(Application.ServerUrl, Request.Path));
//            };

//            Post["/apply/routing"] = x => {
//                return ApiConsumer.Post(CommonString.Append(Application.ServerUrl, Request.Path));
//            };

//            Post["/apply/interfaces"] = x => {
//                return ApiConsumer.Post(CommonString.Append(Application.ServerUrl, Request.Path));
//            };

//            Post["/apply/tuns"] = x => {
//                return ApiConsumer.Post(CommonString.Append(Application.ServerUrl, Request.Path));
//            };

//            Post["/apply/taps"] = x => {
//                return ApiConsumer.Post(CommonString.Append(Application.ServerUrl, Request.Path));
//            };
//        }
//    }
//}