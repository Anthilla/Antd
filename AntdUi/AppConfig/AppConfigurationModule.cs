using Nancy;
using Newtonsoft.Json;

namespace AntdUi.AppConfig {

    public class AppConfigurationModule : NancyModule {

        private readonly AppConfiguration _appConfiguration = new AppConfiguration();

        public AppConfigurationModule() {

            Get["/config"] = _ => {
                var list = _appConfiguration.Get();
                return JsonConvert.SerializeObject(list);
            };

            Post["/config"] = _ => {
                var port = Request.Form.Port;
                var model = new AppConfigurationModel {
                    Port = port,
                };
                _appConfiguration.Save(model);
                return HttpStatusCode.OK;
            };
        }
    }
}