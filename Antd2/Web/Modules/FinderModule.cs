using Antd2.cmds;
using Nancy;
using Newtonsoft.Json;
using System.Text;

namespace Antd2.Modules {
    public class FinderModule : NancyModule {

        public FinderModule() : base("/finder") {

            Get("/", x => ApiGet());

        }

        private dynamic ApiGet() {
            string src = Request.Query.s;
            string pattern = Request.Query.p;
            var a = Find.File(string.IsNullOrEmpty(src) ? "/" : src, pattern);
            var jsonString = JsonConvert.SerializeObject(a);
            var jsonBytes = Encoding.UTF8.GetBytes(jsonString);
            return new Response {
                ContentType = "application/json",
                Contents = s => s.Write(jsonBytes, 0, jsonBytes.Length)
            };
        }

    }
}