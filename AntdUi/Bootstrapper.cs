using AntdUi.Auth;
using Nancy;
using Nancy.Authentication.Forms;
using Nancy.Bootstrapper;
using Nancy.Conventions;
using Nancy.Diagnostics;
using Nancy.TinyIoc;

namespace AntdUi {

    public class CustomRootPathProvider : IRootPathProvider {
        public string GetRootPath() {
            return "/framework";
        }
    }

    public class Bootstrapper : DefaultNancyBootstrapper {

        protected override DiagnosticsConfiguration DiagnosticsConfiguration => new DiagnosticsConfiguration { Password = "Nancy" };

        protected override IRootPathProvider RootPathProvider => new CustomRootPathProvider();

        protected override void ConfigureConventions(NancyConventions conv) {
            base.ConfigureConventions(conv);
            conv.StaticContentsConventions.Clear();
            conv.StaticContentsConventions.AddDirectory("Style", @"/antdui/Style");
            conv.StaticContentsConventions.AddDirectory("Scripts", @"/antdui/Scripts");
            conv.StaticContentsConventions.AddDirectory("Fonts", @"/antdui/Fonts");
            conv.StaticContentsConventions.AddDirectory("partials", @"/antdui/Views/Partials");
            conv.StaticContentsConventions.AddDirectory("pages", @"/antdui/Views/Pages");
            conv.StaticContentsConventions.AddDirectory("images", @"/antdui/Images");
            conv.StaticContentsConventions.AddDirectory("file", @"/antdui/Content");

            conv.ViewLocationConventions.Add((viewName, model, context) => {
                var path = string.Concat("antdui", "/Views/", viewName);
                return path;
            });

            conv.ViewLocationConventions.Add((viewName, model, context) => {
                var path = string.Concat("/framework/antdui", "/Views/", viewName);
                return path;
            });
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
}