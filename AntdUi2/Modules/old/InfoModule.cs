//using anthilla.core;
//using Nancy;
//using Nancy.Security;

//namespace AntdUi2.Modules {
//    public class InfoModule : NancyModule {

//        public InfoModule() : base("/info") {
//            this.RequiresAuthentication();

//            Get["/"] = x => {
//                return ApiConsumer.GetJson(CommonString.Append(Application.ServerUrl, Request.Path));
//            };

//            Get["/memory"] = x => {
//                return ApiConsumer.GetJson(CommonString.Append(Application.ServerUrl, Request.Path));
//            };

//            Get["/free"] = x => {
//                return ApiConsumer.GetJson(CommonString.Append(Application.ServerUrl, Request.Path));
//            };

//            Get["/cpu"] = x => {
//                return ApiConsumer.GetJson(CommonString.Append(Application.ServerUrl, Request.Path));
//            };
//        }
//    }
//}