using Antd2.cmds;
using Nancy;
using System.Linq;
using System.Text;

namespace Antd2.Modules {
    public class TerminalModule : NancyModule {

        private readonly string[] ForbiddenCommands = new[] {
            "poweroff",
            "sudo",
            "su",
            "reboot",
        };

        public TerminalModule() : base("/terminal") {

            Post("/", x => ApiPost());
        }

        private dynamic ApiPost() {
            string command = Request.Form.Command;

            var arr = command.Split(new[] { " " }, System.StringSplitOptions.RemoveEmptyEntries);

            string[] commandResult;
            if (ForbiddenCommands.Contains(arr[0]))
                commandResult = new[] { "Forbidden!" };
            else
                commandResult = Bash.Execute(command).ToArray();
            var jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(commandResult);
            var jsonBytes = Encoding.UTF8.GetBytes(jsonString);
            return new Response {
                ContentType = "application/json",
                Contents = s => s.Write(jsonBytes, 0, jsonBytes.Length)
            };
        }
    }
}