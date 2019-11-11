using Antd2.cmds;
using Antd2.Configuration;
using Nancy;
using Nancy.ModelBinding;

namespace Antd2.Modules {
    public class HostModule : NancyModule {

        public HostModule() : base("/host/config") {

            Get("/", x => Response.AsJson((object)ConfigManager.Config.Saved.Host));

            Get("/save", x => ApiPostSave());

            Get("/apply", x => ApiPostApply());
        }

        private dynamic ApiPostSave() {
            var model = this.Bind<HostParameters>();
            ConfigManager.Config.Saved.Host = model;
            ConfigManager.Config.Dump();
            return HttpStatusCode.OK;
        }

        private dynamic ApiPostApply() {
            if (!string.IsNullOrEmpty(ConfigManager.Config.Saved.Host.Name)) {
                Hostnamectl.SetHostname(ConfigManager.Config.Saved.Host.Name);
            }
            if (!string.IsNullOrEmpty(ConfigManager.Config.Saved.Host.Chassis)) {
                Hostnamectl.SetChassis(ConfigManager.Config.Saved.Host.Chassis);
            }
            if (!string.IsNullOrEmpty(ConfigManager.Config.Saved.Host.Deployment)) {
                Hostnamectl.SetDeployment(ConfigManager.Config.Saved.Host.Deployment);
            }
            if (!string.IsNullOrEmpty(ConfigManager.Config.Saved.Host.Location)) {
                Hostnamectl.SetLocation(ConfigManager.Config.Saved.Host.Location);
            }
            return HttpStatusCode.OK;
        }
    }
}