using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Nancy.Owin;

namespace Antd2.Web {
    public class Startup {
        private readonly IConfiguration config;

        /// <summary>
        /// Verificare se aggiornando a .netcore 3 e utilizzando IHostingEnvironment
        /// invece che IWebHostEnvironment, come suggerito, 
        /// funziona correttamente
        /// Microsoft.AspNetCore.Owin pacchetto tenere a versione 2.2.0 e non aggiornare a 3.0.0 
        /// finchè i problemi di Nancy non verranno risolti
        /// </summary>
        /// <param name="env"></param>
        public Startup(IHostingEnvironment env) {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath);

            config = builder.Build();
        }

        public void Configure(IApplicationBuilder app) {
            //var appConfig = new AppConfiguration();
            //ConfigurationBinder.Bind(config, appConfig);
            //app.UseOwin(x => x.UseNancy(opt => opt.Bootstrapper = new AntdBootstrapper(appConfig)));

            app.UseOwin(x => x.UseNancy(opt => opt.Bootstrapper = new AntdBootstrapper()));
        }
    }
}
