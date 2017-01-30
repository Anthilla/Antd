using System.IO;
using Nancy;

namespace AntdUi.Modules {

    public class ContentModule : NancyModule {

        public ContentModule() {
            Get["/content"] = _ => {
                string content = Request.Query.q;
                var file = $"/framework/antdui/Content/{content}";
                if(!File.Exists(file)) {
                    file = string.Empty;
                }
                return File.ReadAllText(file);
            };
        }
    }
}