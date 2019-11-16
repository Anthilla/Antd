using Nancy;

namespace AntdUi2.Modules {
    public class BaseModule : NancyModule {

        public BaseModule() {
            Before += ctx => {
                if (this.Context.CurrentUser == null) {
                    return HttpStatusCode.Unauthorized;
                }
                return null;
            };
        }

        public BaseModule(string modulePath) : base(modulePath) {
            Before += ctx => {
                if (this.Context.CurrentUser == null) {
                    return HttpStatusCode.Unauthorized;
                }
                return null;
            };
        }
    }
}