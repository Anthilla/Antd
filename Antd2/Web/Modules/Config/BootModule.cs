using Antd2.cmds;
using Antd2.Configuration;
using Nancy;
using Nancy.ModelBinding;
using Newtonsoft.Json;
using System.Linq;
using System.Text;

namespace Antd2.Modules {

    public class BootModule : NancyModule {

        public BootModule() : base("/boot/config") {

            Get("/sysctl", x => ApiGetSysctl());
            Get("/modules", x => ApiGetModules());
            Get("/services", x => ApiGetServices());

            Post("/sysctl/save", x => ApiPostSaveSysctl());
            Post("/modules/save", x => ApiPostSaveModules());
            Post("/services/save", x => ApiPostSaveServices());

            Post("/sysctl/apply", x => ApiPostApplySysctl());
            Post("/modules/apply", x => ApiPostApplyModules());
            Post("/services/apply", x => ApiPostApplyServices());
        }

        private dynamic ApiGetSysctl() {
            var sysctl = ConfigManager.Config.Saved.Boot.Sysctl;
            var sysctlTxt = string.Join("\n", sysctl);
            var jsonString = JsonConvert.SerializeObject(sysctlTxt);
            var jsonBytes = Encoding.UTF8.GetBytes(jsonString);
            return new Response {
                ContentType = "application/json",
                Contents = s => s.Write(jsonBytes, 0, jsonBytes.Length)
            };
        }

        private dynamic ApiGetModules() {
            var a = new ModulesView();
            a.ActiveModulesTxt = string.Join("\n", ConfigManager.Config.Saved.Boot.ActiveModules);
            a.InactiveModulesTxt = string.Join("\n", ConfigManager.Config.Saved.Boot.InactiveModules);
            var jsonString = JsonConvert.SerializeObject(a);
            var jsonBytes = Encoding.UTF8.GetBytes(jsonString);
            return new Response {
                ContentType = "application/json",
                Contents = s => s.Write(jsonBytes, 0, jsonBytes.Length)
            };
        }

        private dynamic ApiGetServices() {
            var a = new ServicesView();
            a.ActiveServicesTxt = string.Join("\n", ConfigManager.Config.Saved.Boot.ActiveServices);
            a.InactiveServicesTxt = string.Join("\n", ConfigManager.Config.Saved.Boot.InactiveModules);
            a.DisabledServicesTxt = string.Join("\n", ConfigManager.Config.Saved.Boot.DisabledServices);
            a.BlockedServicesTxt = string.Join("\n", ConfigManager.Config.Saved.Boot.BlockedServices);
            var jsonString = JsonConvert.SerializeObject(a);
            var jsonBytes = Encoding.UTF8.GetBytes(jsonString);
            return new Response {
                ContentType = "application/json",
                Contents = s => s.Write(jsonBytes, 0, jsonBytes.Length)
            };
        }

        private dynamic ApiPostSaveSysctl() {
            string json = Request.Form.Data;
            var sysctlTxt = JsonConvert.DeserializeObject<string>(json);
            var model = sysctlTxt.Split('\n');
            ConfigManager.Config.Saved.Boot.Sysctl = model;
            ConfigManager.Config.Dump();
            return HttpStatusCode.OK;
        }

        private dynamic ApiPostSaveModules() {
            var model = this.Bind<string[]>();
            ConfigManager.Config.Saved.Boot.ActiveModules = model;
            ConfigManager.Config.Dump();
            return HttpStatusCode.OK;
        }
        private dynamic ApiPostSaveServices() {
            var model = this.Bind<string[]>();
            ConfigManager.Config.Saved.Boot.ActiveServices = model;
            ConfigManager.Config.Dump();
            return HttpStatusCode.OK;
        }

        private dynamic ApiPostApplySysctl() {
            foreach (var sysctl in ConfigManager.Config.Saved.Boot.Sysctl) {
                Sysctl.Set(sysctl);
            }
            return HttpStatusCode.OK;
        }
        private dynamic ApiPostApplyModules() {
            var loadedModules = Mod.Get();
            foreach (var module in ConfigManager.Config.Saved.Boot.ActiveModules) {
                var (Module, UsedBy) = loadedModules.FirstOrDefault(_ => _.Module == module);
                if (!string.IsNullOrEmpty(Module)) {
                    continue;
                }
                Mod.Add(module);
            }
            return HttpStatusCode.OK;
        }
        private dynamic ApiPostApplyServices() {
            foreach (var service in ConfigManager.Config.Saved.Boot.ActiveServices) {
                if (Systemctl.IsEnabled(service) == false)
                    Systemctl.Enable(service);
                if (Systemctl.IsActive(service) == false)
                    Systemctl.Start(service);
            }
            return HttpStatusCode.OK;
        }
    }

    public class ModulesView {
        public string ActiveModulesTxt { get; set; }
        public string InactiveModulesTxt { get; set; }
    }

    public class ServicesView {

        public string ActiveServicesTxt { get; set; }
        public string InactiveServicesTxt { get; set; }
        public string DisabledServicesTxt { get; set; }
        public string BlockedServicesTxt { get; set; }
    }
}