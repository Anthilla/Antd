namespace AntdUi2.Modules {
    public class MainModule : BaseModule {

        public MainModule() {
            Get("/", x => View["home.min.html"]);
        }
    }
}