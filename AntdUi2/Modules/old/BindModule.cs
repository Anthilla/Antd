//using anthilla.core;
//using Nancy;
//using Nancy.Security;
//using System.Collections.Generic;

//namespace AntdUi2.Modules {
//    public class BindModule : NancyModule {

//        public BindModule() : base("/bind") {
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

//            Post["/apply"] = x => {
//                return ApiConsumer.Post(CommonString.Append(Application.ServerUrl, Request.Path));
//            };
//        }
//    }
//}