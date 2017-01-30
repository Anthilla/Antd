using System.IO;
using Nancy;

namespace AntdUi.Modules {

    public class LanguageModule : NancyModule {

        public LanguageModule() {
            Get["/translate"] = _ => {
                string lang = Request.Query.lang;
                var file = $"/framework/plugin/anthilladoc/Translations/{lang}.json";
                if(!File.Exists(file)) {
                    file = "/framework/plugin/anthilladoc/Translations/en.json";
                }
                return File.ReadAllText(file);
            };
        }
    }
}