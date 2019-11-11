using Nancy;
using Nancy.Bootstrapper;
using Nancy.TinyIoc;

namespace Antd2.Web {

    public class AntdBootstrapper : DefaultNancyBootstrapper {

        public AntdBootstrapper() {
        }

        protected override void ConfigureApplicationContainer(TinyIoCContainer container) {
            base.ConfigureApplicationContainer(container);
        }

        protected override void RequestStartup(TinyIoCContainer requestContainer, IPipelines pipelines, NancyContext context) {
            base.RequestStartup(requestContainer, pipelines, context);

            //pipelines.BeforeRequest += ctx => {
            //    Console.WriteLine(ctx);
            //    return null;
            //};

            //pipelines.AfterRequest += (ctx) => {
            //    Console.WriteLine(ctx);
            //};

            //pipelines.OnError += (ctx, ex) => {
            //    Console.WriteLine(ex);
            //    return null;
            //};
        }
    }
}
