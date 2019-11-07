using Nancy;
using Nancy.Bootstrapper;
using Nancy.ModelBinding;
using Nancy.Serialization.JsonNet;
using Nancy.TinyIoc;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Antd2.Web {

    public class CustomJsonSerializer : JsonSerializer {
        public CustomJsonSerializer() {
            this.ContractResolver = new DefaultContractResolver();
            this.Formatting = Formatting.None;
            this.CheckAdditionalContent = true;
        }
    }

    public class AntdBootstrapper : DefaultNancyBootstrapper {
        private readonly IAppConfiguration appConfig;

        public AntdBootstrapper() {
        }

        public AntdBootstrapper(IAppConfiguration appConfig) {
            this.appConfig = appConfig;
        }

        protected override void ConfigureApplicationContainer(TinyIoCContainer container) {
            base.ConfigureApplicationContainer(container);

            container.Register<ISerializer, JsonNetSerializer>();
            container.Register<IBodyDeserializer, JsonNetBodyDeserializer>();
            container.Register<JsonSerializer, CustomJsonSerializer>();

            if (appConfig != null) {
                container.Register<IAppConfiguration>(appConfig);
            }
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
