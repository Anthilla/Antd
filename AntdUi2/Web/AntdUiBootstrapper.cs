using Nancy;
using Nancy.Bootstrapper;
using Nancy.TinyIoc;

namespace AntdUi2.Web {

    public class AntdUiBootstrapper : DefaultNancyBootstrapper {

        public AntdUiBootstrapper() {
        }

        protected override void ConfigureApplicationContainer(TinyIoCContainer container) {
            base.ConfigureApplicationContainer(container);

        }

        protected override void RequestStartup(TinyIoCContainer requestContainer, IPipelines pipelines, NancyContext context) {
            base.RequestStartup(requestContainer, pipelines, context);
        }
    }
}
