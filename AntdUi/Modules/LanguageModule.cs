using Nancy;
using System.IO;

namespace AntdUi.Modules {

    public class LanguageModule : NancyModule {

        public LanguageModule() {
            Get["/translate"] = _ => {
                string lang = Request.Query.lang;
                var file = $"/framework/antdui/Translations/{lang}.json";
                if(!File.Exists(file)) {
                    file = "/framework/antdui/Translations/en.json";
                }
                return File.ReadAllText(file);
            };
        }
    }
}