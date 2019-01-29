using anthilla.core;
using Nancy;
using Nancy.Security;

namespace AntdUi.Modules {
    public class RssdpModule : NancyModule {

        public RssdpModule() : base("/rssdp") {
            this.RequiresAuthentication();

            Get["/discover"] = x => {
                return ApiConsumer.GetJson(CommonString.Append(Application.ServerUrl, Request.Path));
            };

            Get["/clear"] = x => {
                return ApiConsumer.GetJson(CommonString.Append(Application.ServerUrl, Request.Path));
            };
        }
    }
}