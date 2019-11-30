﻿using antd.core;
using Antd2.cmds;
using Antd2.Configuration;
using Nancy;
using Nancy.ModelBinding;
using Newtonsoft.Json;
using System.Collections.Generic;
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
            var a = new SysctlView();
            var sysctl = ConfigManager.Config.Saved.Boot.Sysctl;
            a.SysctlTxt = string.Join("\n", sysctl);
            a.RunningSysctl = Sysctl.Get().Select(_ => $"{_.Key}={_.Value}").OrderBy(_ => _).ToArray();
            var jsonString = JsonConvert.SerializeObject(a);
            var jsonBytes = Encoding.UTF8.GetBytes(jsonString);
            return new Response {
                ContentType = "application/json",
                Contents = s => s.Write(jsonBytes, 0, jsonBytes.Length)
            };
        }

        private dynamic ApiGetModules() {
            //var confAct = ConfigManager.Config.Saved.Boot.ActiveModules;
            //var confIna = ConfigManager.Config.Saved.Boot.ActiveModules;
            //var running = Mod.Get();

            //var keysAct = confAct.Select(_ => _).ToArray();
            //var keysIna = confIna.Select(_ => _).ToArray();
            //var keysRun = running.Select(_ => _.Module).ToArray();

            //var keys = CommonArray.Merge(keysAct, keysIna, keysRun).ToHashSet()
            //    .OrderBy(_ => _).ToArray();

            //var mode = new List<ModuleElement>();
            //foreach (var k in keys) {
            //    var m = new ModuleElement();
            //    m.Key = k;
            //    m.ValueR = running.Any(_ => _.Module == k) ? true : (bool?)null;
            //    if (confAct.Contains(k)) {
            //        m.ValueC = true;
            //    }
            //    else if (confIna.Contains(k)) {
            //        m.ValueC = false;
            //    }
            //    else {
            //        m.ValueC = null;
            //    }
            //}

            var a = new ModulesView();
            a.ActiveModulesTxt = string.Join("\n", ConfigManager.Config.Saved.Boot.ActiveModules);
            a.InactiveModulesTxt = string.Join("\n", ConfigManager.Config.Saved.Boot.InactiveModules);
            a.RunningModules = Mod.Get().Select(_ => _.Module).OrderBy(_ => _).ToArray();
            //a.Modules = mode;
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
            a.InactiveServicesTxt = string.Join("\n", ConfigManager.Config.Saved.Boot.InactiveServices);
            a.DisabledServicesTxt = string.Join("\n", ConfigManager.Config.Saved.Boot.DisabledServices);
            a.BlockedServicesTxt = string.Join("\n", ConfigManager.Config.Saved.Boot.BlockedServices);
            a.RunningServices = Systemctl.GetServices().Where(_ => _.Active).Select(_ => _.Service).OrderBy(_ => _).ToArray();
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
            string json = Request.Form.Data;
            var modv = JsonConvert.DeserializeObject<ModulesView>(json);
            ConfigManager.Config.Saved.Boot.ActiveModules = modv.ActiveModulesTxt.Split('\n');
            ConfigManager.Config.Saved.Boot.InactiveModules = modv.InactiveModulesTxt.Split('\n');
            ConfigManager.Config.Dump();
            return HttpStatusCode.OK;
        }
        private dynamic ApiPostSaveServices() {
            string json = Request.Form.Data;
            var modv = JsonConvert.DeserializeObject<ServicesView>(json);
            ConfigManager.Config.Saved.Boot.ActiveServices = modv.ActiveServicesTxt.Split('\n');
            ConfigManager.Config.Saved.Boot.InactiveServices = modv.InactiveServicesTxt.Split('\n');
            ConfigManager.Config.Saved.Boot.DisabledServices = modv.DisabledServicesTxt.Split('\n');
            ConfigManager.Config.Saved.Boot.BlockedServices = modv.BlockedServicesTxt.Split('\n');
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

        public string[] RunningModules { get; set; }



        public List<ModuleElement> Modules { get; set; }

    }

    public class ModuleElement {
        public string Key { get; set; }
        public bool? ValueC { get; set; }
        public bool? ValueR { get; set; }
    }

    public class ServicesView {

        public string ActiveServicesTxt { get; set; }
        public string InactiveServicesTxt { get; set; }
        public string DisabledServicesTxt { get; set; }
        public string BlockedServicesTxt { get; set; }

        public string[] RunningServices { get; set; }

    }

    public class SysctlView {

        public string SysctlTxt { get; set; }

        public string[] RunningSysctl { get; set; }

    }
}