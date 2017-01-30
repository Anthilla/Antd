using Nancy;

namespace AntdUi.Modules {

    public class AppModule : NancyModule {

        public AppModule() {

            Get["/"] = _ => {
                return View["home"];
            };

            Get["/anthilladoc"] = _ => {
                return View["home"];
            };

            Get["/print"] = _ => {
                return View["print"];
            };

            Get["/anthilladoc/print"] = _ => {
                return View["print"];
            };
        }
    }
}