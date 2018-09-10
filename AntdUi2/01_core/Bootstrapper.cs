using AntdUi.Auth;
using Nancy;
using Nancy.Authentication.Forms;
using Nancy.Bootstrapper;
using Nancy.Conventions;
using Nancy.Diagnostics;
using Nancy.ErrorHandling;
using Nancy.TinyIoc;
using Nancy.ViewEngines;

namespace AntdUi {

    public class Bootstrapper : DefaultNancyBootstrapper {

        protected override DiagnosticsConfiguration DiagnosticsConfiguration => new DiagnosticsConfiguration { Password = "Nancy" };

        protected override void ConfigureConventions(NancyConventions conv) {
            base.ConfigureConventions(conv);
            conv.StaticContentsConventions.Clear();
            conv.StaticContentsConventions.AddDirectory("style", @"/Style");
            conv.StaticContentsConventions.AddDirectory("scripts", @"/Scripts");
            conv.StaticContentsConventions.AddDirectory("fonts", @"/Fonts");
            conv.StaticContentsConventions.AddDirectory("pages", @"/Views");
            conv.StaticContentsConventions.AddDirectory("images", @"/Images");
            conv.StaticContentsConventions.AddDirectory("file", @"/Content");
            //conv.StaticContentsConventions.AddDirectory("vnc", @"/novnc");

            conv.StaticContentsConventions.AddDirectory("vendors", @"/node_modules/gentelella/vendors");
            conv.StaticContentsConventions.AddDirectory("build", @"/node_modules/gentelella/build");

        }

        protected override void ConfigureRequestContainer(TinyIoCContainer container, NancyContext context) {
            base.ConfigureRequestContainer(container, context);
            container.Register<IUserMapper, UserDatabase>();
        }

        protected override void ApplicationStartup(TinyIoCContainer container, IPipelines pipelines) {
            base.ApplicationStartup(container, pipelines);
            StaticConfiguration.EnableRequestTracing = true;
            StaticConfiguration.DisableErrorTraces = false;
        }

        protected override void RequestStartup(TinyIoCContainer requestContainer, IPipelines pipelines, NancyContext context) {
            base.RequestStartup(requestContainer, pipelines, context);
            FormsAuthentication.Enable(pipelines, new FormsAuthenticationConfiguration {
                RedirectUrl = "~/login",
                UserMapper = requestContainer.Resolve<IUserMapper>(),
            });
        }
    }

    public class Error403Handler : DefaultViewRenderer, IStatusCodeHandler {
        public Error403Handler(IViewFactory factory) : base(factory) {
        }

        public bool HandlesStatusCode(HttpStatusCode statusCode, NancyContext context) {
            return statusCode == HttpStatusCode.NotFound ||
                statusCode == HttpStatusCode.Forbidden ||
                statusCode == HttpStatusCode.InternalServerError
            ;
        }

        public void Handle(HttpStatusCode statusCode, NancyContext context) {
            Response response;
            switch(statusCode) {
                case HttpStatusCode.InternalServerError:
                    response = RenderView(context, "page_500");
                    response.StatusCode = HttpStatusCode.InternalServerError;
                    context.Response = response;
                    return;
                case HttpStatusCode.Forbidden:
                    response = RenderView(context, "page_403");
                    response.StatusCode = HttpStatusCode.Forbidden;
                    context.Response = response;
                    return;
                case HttpStatusCode.NotFound:
                    response = RenderView(context, "page_404");
                    response.StatusCode = HttpStatusCode.NotFound;
                    context.Response = response;
                    return;
                default:
                    response = RenderView(context, "page_500");
                    response.StatusCode = HttpStatusCode.InternalServerError;
                    context.Response = response;
                    return;
            }

        }
    }
}