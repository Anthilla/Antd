using Antd.models;
using anthilla.core;
using Nancy;

namespace AntdUi.Modules {
    public class DeviceModule : NancyModule {

        public DeviceModule() : base("/device") {
            Get["/description"] = x => {
                var model = ApiConsumer.Get<ServiceDiscoveryModel>(CommonString.Append(Application.ServerUrl, Request.Path));
                return Response.AsXml(model);
            };

            Get["/services"] = x => {
                return ApiConsumer.GetJson(CommonString.Append(Application.ServerUrl, Request.Path));
            };

            Post["/ok"] = x => {
                return ApiConsumer.Post(CommonString.Append(Application.ServerUrl, Request.Path));
            };

            Get["/checklist"] = x => {
                return ApiConsumer.GetJson(CommonString.Append(Application.ServerUrl, Request.Path));
            };

            Get["/clusterchecklist"] = x => {
                return ApiConsumer.GetJson(CommonString.Append(Application.ServerUrl, Request.Path));
            };
        }
    }
}