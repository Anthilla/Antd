using Nancy;
using System.IO;

namespace AntdUi2.Modules {

    public class LanguageModule : NancyModule {

        public LanguageModule() {

            Get("/translate", x => ApiGet());
        }

        private dynamic ApiGet() {
            string lang = Request.Query.lang;
            var file = $"/framework/antdui/Translations/{lang}.json";
            if (!File.Exists(file)) {
                file = "/framework/antdui/Translations/en.json";
            }
            return File.ReadAllText(file);
        }
    }
}