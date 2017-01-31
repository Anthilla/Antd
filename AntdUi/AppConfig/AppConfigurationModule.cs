using antdlib.config;
using antdlib.models;
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
                var antdPort = Request.Form.AntdPort;
                var antdUiPort = Request.Form.AntdUiPort;
                var databasePath = Request.Form.DatabasePath;
                var model = new AppConfigurationModel {
                    AntdPort = antdPort,
                    AntdUiPort = antdUiPort,
                    DatabasePath = databasePath
                };
                _appConfiguration.UiSave(model);
                return HttpStatusCode.OK;
            };
        }
    }
}