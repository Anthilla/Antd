using Antd2.cmds;
using Antd2.Configuration;
using Nancy;
using Nancy.ModelBinding;
using System;
using System.Linq;

namespace Antd2.Modules {
    public class BootModule : NancyModule {

        public BootModule() : base("/boot/config") {

            Get("/", x => Response.AsJson((object)ConfigManager.Config.Saved.Boot));

            Get("/sysctl/save", x => ApiPostSaveSysctl());
            Get("/modules/active/save", x => ApiPostSaveActiveModules());
            Get("/modules/inactive/save", x => ApiPostSaveInactiveModules());
            Get("/services/active/save", x => ApiPostSaveActiveServices());
            Get("/services/inactive/save", x => ApiPostSaveInactiveServices());
            Get("/services/disabled/save", x => ApiPostSaveDisabledServices());
            Get("/services/blocked/save", x => ApiPostSaveBlockedServices());

            Get("/sysctl/apply", x => ApiPostApplySysctl());
            Get("/modules/active/apply", x => ApiPostApplyActiveModules());
            Get("/modules/inactive/apply", x => ApiPostApplyInactiveModules());
            Get("/services/active/apply", x => ApiPostApplyActiveServices());
            Get("/services/inactive/apply", x => ApiPostApplyInactiveServices());
            Get("/services/disabled/apply", x => ApiPostApplyDisabledServices());
            Get("/services/blocked/apply", x => ApiPostApplyBlockedServices());
        }

        private dynamic ApiPostSaveSysctl() {
            var model = this.Bind<string[]>();
            ConfigManager.Config.Saved.Boot.Sysctl = model;
            ConfigManager.Config.Dump();
            return HttpStatusCode.OK;
        }

        private dynamic ApiPostSaveActiveModules() {
            var model = this.Bind<string[]>();
            ConfigManager.Config.Saved.Boot.ActiveModules = model;
            ConfigManager.Config.Dump();
            return HttpStatusCode.OK;
        }
        private dynamic ApiPostSaveInactiveModules() {
            var model = this.Bind<string[]>();
            ConfigManager.Config.Saved.Boot.InactiveModules = model;
            ConfigManager.Config.Dump();
            return HttpStatusCode.OK;
        }
        private dynamic ApiPostSaveActiveServices() {
            var model = this.Bind<string[]>();
            ConfigManager.Config.Saved.Boot.ActiveServices = model;
            ConfigManager.Config.Dump();
            return HttpStatusCode.OK;
        }
        private dynamic ApiPostSaveInactiveServices() {
            var model = this.Bind<string[]>();
            ConfigManager.Config.Saved.Boot.InactiveServices = model;
            ConfigManager.Config.Dump();
            return HttpStatusCode.OK;
        }
        private dynamic ApiPostSaveDisabledServices() {
            var model = this.Bind<string[]>();
            ConfigManager.Config.Saved.Boot.DisabledServices = model;
            ConfigManager.Config.Dump();
            return HttpStatusCode.OK;
        }
        private dynamic ApiPostSaveBlockedServices() {
            var model = this.Bind<string[]>();
            ConfigManager.Config.Saved.Boot.BlockedServices = model;
            ConfigManager.Config.Dump();
            return HttpStatusCode.OK;
        }

        private dynamic ApiPostApplySysctl() {
            foreach (var sysctl in ConfigManager.Config.Saved.Boot.Sysctl) {
                Sysctl.Set(sysctl);
            }
            return HttpStatusCode.OK;
        }
        private dynamic ApiPostApplyActiveModules() {
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
        private dynamic ApiPostApplyInactiveModules() {
            var loadedModules = Mod.Get();
            foreach (var module in ConfigManager.Config.Saved.Boot.InactiveModules) {
                var loadedModule = loadedModules.FirstOrDefault(_ => _.Module == module);
                if (string.IsNullOrEmpty(loadedModule.Module)) {
                    continue;
                }

                Console.Write($"[mod] removing module '{module}'");
                if (loadedModule.UsedBy.Length > 0)
                    Console.WriteLine($" and its {loadedModule.UsedBy.Length} dependecies");
                else
                    Console.WriteLine();

                Mod.Remove(loadedModule);
            }
            return HttpStatusCode.OK;
        }
        private dynamic ApiPostApplyActiveServices() {
            foreach (var service in ConfigManager.Config.Saved.Boot.ActiveServices) {
                if (Systemctl.IsEnabled(service) == false)
                    Systemctl.Enable(service);
                if (Systemctl.IsActive(service) == false)
                    Systemctl.Start(service);
            }
            return HttpStatusCode.OK;
        }
        private dynamic ApiPostApplyInactiveServices() {
            foreach (var service in ConfigManager.Config.Saved.Boot.InactiveServices) {
                if (Systemctl.IsActive(service))
                    Systemctl.Stop(service);
            }
            return HttpStatusCode.OK;
        }
        private dynamic ApiPostApplyDisabledServices() {
            foreach (var service in ConfigManager.Config.Saved.Boot.DisabledServices) {
                if (Systemctl.IsActive(service))
                    Systemctl.Stop(service);
                if (Systemctl.IsEnabled(service))
                    Systemctl.Disable(service);
            }
            return HttpStatusCode.OK;
        }
        private dynamic ApiPostApplyBlockedServices() {
            foreach (var service in ConfigManager.Config.Saved.Boot.BlockedServices) {
                if (Systemctl.IsActive(service))
                    Systemctl.Stop(service);
                if (Systemctl.IsEnabled(service))
                    Systemctl.Disable(service);
                Systemctl.Mask(service);
            }
            return HttpStatusCode.OK;
        }
    }
}