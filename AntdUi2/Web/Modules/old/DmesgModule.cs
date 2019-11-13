//using anthilla.core;
//using Nancy;
//using Nancy.Security;

//namespace AntdUi2.Modules {
//    public class DmesgModule : NancyModule {

//        public DmesgModule() : base("/dmesg") {
//            this.RequiresAuthentication();

//            Get["/"] = x => {
//                return ApiConsumer.GetJson(CommonString.Append(Application.ServerUrl, Request.Path));
//            };
//        }
//    }
//}