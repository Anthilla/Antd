using Antd2.cmds;
using Antd2.Configuration;
using Nancy;
using Nancy.ModelBinding;
using System;

namespace Antd2.Modules {
    public class SetupCommandModule : NancyModule {

        public SetupCommandModule() : base("/setupcmd/config") {

            Get("/", x => Response.AsJson((object)ConfigManager.Config.Saved.Commands));

            Get("/save", x => ApiPostSave());

            Get("/apply", x => ApiPostApply());
        }

        private dynamic ApiPostSave() {
            var model = this.Bind<SetupCommandParameters>();
            ConfigManager.Config.Saved.Commands = model;
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