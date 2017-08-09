using anthilla.core;
using Nancy;
using Nancy.Security;

namespace AntdUi.Modules {
    public class MainModule : NancyModule {

        public MainModule() {
            this.RequiresAuthentication();

            Before += ctx => {
                var req = Request.Headers.UserAgent;
                ConsoleLogger.Log(req);
                if(Request.Path == "/wizard") {
                    return null;
                }
                var isConfigured = ApiConsumer.Get<bool>($"http://127.0.0.1:{Application.ServerPort}/configured");
                return !isConfigured ? Response.AsRedirect("/wizard") : null;
            };

            Get["/"] = _ => Response.AsRedirect("/antd");

            Get["/antd"] = _ => View["home"];

            Get["/wizard"] = _ => View["home-wizard"];
        }
    }
}