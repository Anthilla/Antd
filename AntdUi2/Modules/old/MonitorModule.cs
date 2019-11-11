//using anthilla.core;
//using Nancy;
//using Nancy.Security;

//namespace AntdUi2.Modules {
//    public class MonitorModule : NancyModule {

//        public MonitorModule() : base("/monitor") {
//            this.RequiresAuthentication();

//            Get["/"] = x => {
//                return ApiConsumer.GetJson(CommonString.Append(Application.ServerUrl, Request.Path));
//            };
//        }
//    }
//}