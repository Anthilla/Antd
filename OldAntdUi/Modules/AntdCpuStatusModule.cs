using antdlib.models;
using anthilla.core;
using Nancy;
using Newtonsoft.Json;

namespace AntdUi.Modules {
    public class AntdCpuStatusModule : NancyModule {

        public AntdCpuStatusModule() {
            Get["/cpustatus"] = x => {
                var model = ApiConsumer.Get<PageCpuStatusModel>($"http://127.0.0.1:{Application.ServerPort}/cpustatus");
                var json = JsonConvert.SerializeObject(model);
                return json;
            };
        }
    }
}
