using antdlib.config.shared;
using antdlib.models;
using Nancy;
using Newtonsoft.Json;

namespace AntdUi.Modules {

    public class AntdAppConfigurationModule : NancyModule {

        private readonly AppConfiguration _appConfiguration = new AppConfiguration();

        public AntdAppConfigurationModule() {

            Get["/config"] = _ => {
                var list = _appConfiguration.UiGet();
                return JsonConvert.SerializeObject(list);
            };

            Post["/config"] = _ => {
                int antdPort = Request.Form.AntdPort;
                int antdUiPort = Request.Form.AntdUiPort;
                string databasePath = Request.Form.DatabasePath;
                string cloudAddress = Request.Form.CloudAddress;
                var model = new AppConfigurationModel {
                    AntdPort = antdPort,
                    AntdUiPort = antdUiPort,
                    DatabasePath = databasePath,
                    CloudAddress = cloudAddress
                };
                _appConfiguration.UiSave(model);
                return HttpStatusCode.OK;
            };
        }
    }
}