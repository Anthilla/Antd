using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Nancy.Owin;

namespace Antd2.Web {
    public class Startup {
        private readonly IConfiguration config;

        public Startup(IWebHostEnvironment env) {
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
