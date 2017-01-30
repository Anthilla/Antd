using AntdUi.Auth;
using Nancy;
using Nancy.Authentication.Forms;
using Nancy.Bootstrapper;
using Nancy.Conventions;
using Nancy.Diagnostics;
using Nancy.TinyIoc;

namespace AntdUi {
    public class Bootstrapper : DefaultNancyBootstrapper {

        protected override DiagnosticsConfiguration DiagnosticsConfiguration => new DiagnosticsConfiguration { Password = "Nancy" };

        protected override void ConfigureConventions(NancyConventions conv) {
            base.ConfigureConventions(conv);
            conv.StaticContentsConventions.Clear();
            var staticContentPrefix = "anthilladoc/";
            conv.StaticContentsConventions.Add(StaticContentConventionBuilder.AddDirectory(staticContentPrefix + "Style", @"/Style"));
            conv.StaticContentsConventions.Add(StaticContentConventionBuilder.AddDirectory(staticContentPrefix + "Scripts", @"/Scripts"));
            conv.StaticContentsConventions.Add(StaticContentConventionBuilder.AddDirectory(staticContentPrefix + "Fonts", @"/Fonts"));
            conv.StaticContentsConventions.Add(StaticContentConventionBuilder.AddDirectory(staticContentPrefix + "partials", @"/Views/Partials/"));
            conv.StaticContentsConventions.Add(StaticContentConventionBuilder.AddDirectory(staticContentPrefix + "pages", @"/Views/Pages"));
            conv.StaticContentsConventions.Add(StaticContentConventionBuilder.AddDirectory(staticContentPrefix + "images", @"/Images"));
            conv.StaticContentsConventions.Add(StaticContentConventionBuilder.AddDirectory(staticContentPrefix + "file", @"/Content"));
        }


        protected override void ConfigureRequestContainer(TinyIoCContainer container, NancyContext context) {
            base.ConfigureRequestContainer(container, context);
            container.Register<IUserMapper, UserDatabase>();
        }

        protected override void ApplicationStartup(TinyIoCContainer container, IPipelines pipelines) {
            base.ApplicationStartup(container, pipelines);
            pipelines.RegisterCompressionCheck();
        }

        protected override void RequestStartup(TinyIoCContainer requestContainer, IPipelines pipelines, NancyContext context) {
            var formsAuthConfiguration = new FormsAuthenticationConfiguration {
                RedirectUrl = "/login",
                UserMapper = requestContainer.Resolve<IUserMapper>()
            };
            FormsAuthentication.Enable(pipelines, formsAuthConfiguration);
        }
    }
}