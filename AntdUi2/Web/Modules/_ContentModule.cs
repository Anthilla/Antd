using Nancy;
using System.IO;

namespace AntdUi2.Modules {

    public class ContentModule : NancyModule {

        public ContentModule() {

            Get("/content", x => ApiGet());
        }

        private dynamic ApiGet() {
            string content = Request.Query.q;
            var file = $"/framework/antdui/Content/{content}";
            if (!File.Exists(file)) {
                file = string.Empty;
            }
            return File.ReadAllText(file);
        }
    }
}