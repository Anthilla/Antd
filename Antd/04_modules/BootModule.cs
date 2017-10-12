using Antd.cmds;
using Nancy;
using Newtonsoft.Json;
using System.Linq;

namespace Antd.Modules {
    public class BootModule : NancyModule {

        public class BootParameterJoin {
            public string Key { get; set; } = string.Empty;
            public string Value { get; set; } = string.Empty;
            public string Running { get; set; } = string.Empty;
        }

        public BootModule() : base("/boot") {

            Get["/parameters"] = x => {
                var current = Application.CurrentConfiguration.Boot.Parameters;
                var running = Application.RunningConfiguration.Boot.Parameters;
                var result = new BootParameterJoin[current.Length];
                for(var i = 0;  i < current.Length; i++) {
                    var param = current[i];
                    var value = running.FirstOrDefault(_ => _.Key == param.Key) == null ? string.Empty : running.FirstOrDefault(_ => _.Key == param.Key).Value;
                    result[i] = new BootParameterJoin() {
                        Key = param.Key,
                        Value = param.Value,
                        Running = value
                    };
                }
                return JsonConvert.SerializeObject(result);
            };

            Get["/modules"] = x => {
                return JsonConvert.SerializeObject(Application.CurrentConfiguration.Boot.Modules);
            };

            Get["/modules/list"] = x => {
                return JsonConvert.SerializeObject(Mod.GetList());
            };

            Get["/services"] = x => {
                return JsonConvert.SerializeObject(Application.CurrentConfiguration.Boot.Services);
            };

            Get["/services/list"] = x => {
                return JsonConvert.SerializeObject(Systemctl.GetList());
            };

            Get["/commands"] = x => {
                return JsonConvert.SerializeObject(Application.CurrentConfiguration.SetupCommands);
            };

            Post["/save/parameters"] = x => {
                string data = Request.Form.Data;
                var objects = JsonConvert.DeserializeObject<SystemParameter[]>(data);
                Application.CurrentConfiguration.Boot.Parameters = objects;
                ConfigRepo.Save();
                return HttpStatusCode.OK;
            };

            Post["/save/modules"] = x => {
                string data = Request.Form.Data;
                var objects = JsonConvert.DeserializeObject<SystemModule[]>(data);
                Application.CurrentConfiguration.Boot.Modules = objects;
                ConfigRepo.Save();
                return HttpStatusCode.OK;
            };

            Post["/save/services"] = x => {
                string data = Request.Form.Data;
                var objects = JsonConvert.DeserializeObject<SystemService[]>(data);
                Application.CurrentConfiguration.Boot.Services = objects;
                ConfigRepo.Save();
                return HttpStatusCode.OK;
            };

            Post["/save/commands"] = x => {
                string data = Request.Form.Data;
                var objects = JsonConvert.DeserializeObject<Command[]>(data);
                Application.CurrentConfiguration.SetupCommands = objects;
                ConfigRepo.Save();
                return HttpStatusCode.OK;
            };

            Post["/apply/parameters"] = x => {
                Sysctl.Set();
                return HttpStatusCode.OK;
            };

            Post["/apply/modules"] = x => {
                Mod.Set();
                return HttpStatusCode.OK;
            };

            Post["/apply/services"] = x => {
                Systemctl.Set();
                return HttpStatusCode.OK;
            };

            Post["/apply/commands"] = x => {
                SetupCommands.Set();
                return HttpStatusCode.OK;
            };
        }
    }
}