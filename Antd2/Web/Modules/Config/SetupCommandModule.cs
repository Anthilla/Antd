using Antd2.cmds;
using Antd2.Configuration;
using Nancy;
using Nancy.ModelBinding;
using Newtonsoft.Json;
using System;
using System.Text;

namespace Antd2.Modules {
    public class SetupCommandModule : NancyModule {

        public SetupCommandModule() : base("/setupcmd/config") {

            Get("/", x => ApiGet());

            Post("/save", x => ApiPostSave());

            Post("/apply", x => ApiPostApply());
        }

        private dynamic ApiGet() {
            var a = ConfigManager.Config.Saved.Commands.Run;
            var aTxt = string.Join("\n", a);
            var jsonString = JsonConvert.SerializeObject(aTxt);
            var jsonBytes = Encoding.UTF8.GetBytes(jsonString);
            return new Response {
                ContentType = "application/json",
                Contents = s => s.Write(jsonBytes, 0, jsonBytes.Length)
            };
        }

        private dynamic ApiPostSave() {
            string json = Request.Form.Data;
            var aTxt = JsonConvert.DeserializeObject<string>(json);
            var model = aTxt.Split('\n');
            ConfigManager.Config.Saved.Commands.Run = model;
            ConfigManager.Config.Dump();
            return HttpStatusCode.OK;
        }

        private dynamic ApiPostApply() {
            foreach (var command in ConfigManager.Config.Saved.Commands.Run) {
                Console.WriteLine($"[cmd] {command}");
                Bash.Do(command);
            }
            return HttpStatusCode.OK;
        }
    }
}