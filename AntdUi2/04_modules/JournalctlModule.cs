using anthilla.core;
using Nancy;
using Nancy.Security;

namespace AntdUi.Modules {
    public class JournalctlModule : NancyModule {

        public JournalctlModule() : base("/journalctl") {
            this.RequiresAuthentication();

            Get["/"] = x => {
                return ApiConsumer.GetJson(CommonString.Append(Application.ServerUrl, Request.Path));
            };

            Get["/unit/{unitname}"] = x => {
                return ApiConsumer.GetJson(CommonString.Append(Application.ServerUrl, Request.Path));
            };

            Get["/unit/antd"] = x => {
                return ApiConsumer.GetJson(CommonString.Append(Application.ServerUrl, Request.Path));
            };

            Get["/unit/antdui"] = x => {
                return ApiConsumer.GetJson(CommonString.Append(Application.ServerUrl, Request.Path));
            };

            Get["/last/{hours}"] = x => {
                return ApiConsumer.GetJson(CommonString.Append(Application.ServerUrl, Request.Path));
            };
        }
    }
}