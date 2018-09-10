using Nancy;
using System.IO;

namespace AntdUi.Modules {

    public class ContentModule : NancyModule {

        public ContentModule() {
         
            Get["/content"] = x => {
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