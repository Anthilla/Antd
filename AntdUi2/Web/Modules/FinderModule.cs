using antd.core;
using Nancy;
using System.Text;

namespace AntdUi2.Modules {
    public class FinderModule : NancyModule {

        public FinderModule() : base("/finder") {

            Get("/", x => ApiGet());

        }

        private dynamic ApiGet() {
            var p = $"?p={Request.Query.p}";
            var jsonString = ApiConsumer.GetString($"{Application.ServerUrl}{Request.Path}{p}");
            var jsonBytes = Encoding.UTF8.GetBytes(jsonString);
            return new Response {
                ContentType = "application/json",
                Contents = s => s.Write(jsonBytes, 0, jsonBytes.Length)
            };
        }
    }
}