using Antd2.cmds;
using Antd2.Configuration;
using Nancy;
using Nancy.ModelBinding;
using Newtonsoft.Json;
using System.Text;

namespace Antd2.Modules {
    public class HostModule : NancyModule {

        public HostModule() : base("/host/config") {

            Get("/", x => ApiGet());

            Post("/save", x => ApiPostSave());

            Post("/apply", x => ApiPostApply());
        }

        private dynamic ApiGet() {
            var host = ConfigManager.Config.Saved.Host;
            var jsonString = JsonConvert.SerializeObject(host);
            var jsonBytes = Encoding.UTF8.GetBytes(jsonString);
            return new Response {
                ContentType = "application/json",
                Contents = s => s.Write(jsonBytes, 0, jsonBytes.Length)
            };
        }

        private dynamic ApiPostSave() {
            string json = Request.Form.Data;
            var model = JsonConvert.DeserializeObject<HostParameters>(json);
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