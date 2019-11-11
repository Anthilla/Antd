using Nancy;
using Nancy.Bootstrapper;
using Nancy.Conventions;
using Nancy.TinyIoc;

namespace AntdUi2.Web {

    public class AntdUiBootstrapper : DefaultNancyBootstrapper {

        public AntdUiBootstrapper() {
        }

        protected override void ConfigureConventions(NancyConventions conv) {
            base.ConfigureConventions(conv);
            conv.StaticContentsConventions.Clear();
            conv.StaticContentsConventions.AddDirectory("css", @"/Style");
            conv.StaticContentsConventions.AddDirectory("js", @"/Scripts");
            conv.StaticContentsConventions.AddDirectory("fonts", @"/Fonts");
            conv.StaticContentsConventions.AddDirectory("webfonts", @"/FontsWeb");
            conv.StaticContentsConventions.AddDirectory("img", @"/Images");
            conv.StaticContentsConventions.AddDirectory("part", @"/Views/partials");
            conv.StaticContentsConventions.AddDirectory("shared", @"/Views/shared");
            conv.StaticContentsConventions.AddDirectory("static", @"/Views/static");
            conv.StaticContentsConventions.AddDirectory("pg", @"/Views/pages");
            conv.StaticContentsConventions.AddDirectory("lo", @"/Views/layouts");
            conv.StaticContentsConventions.AddDirectory("pt", @"/Views/partial");
            conv.StaticContentsConventions.AddDirectory("tpl", @"/Views/templates");
            conv.StaticContentsConventions.AddDirectory("prnt", @"/Views/print");
            conv.StaticContentsConventions.AddDirectory("novnc", @"/novnc");
        }


        protected override void ConfigureApplicationContainer(TinyIoCContainer container) {
            base.ConfigureApplicationContainer(container);

        }

        protected override void RequestStartup(TinyIoCContainer requestContainer, IPipelines pipelines, NancyContext context) {
            base.RequestStartup(requestContainer, pipelines, context);
        }
    }
}
