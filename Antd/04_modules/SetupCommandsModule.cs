using Antd.cmds;
using Nancy;
using Newtonsoft.Json;

namespace Antd.Modules {
    public class SetupCommandsModule : NancyModule {

        public SetupCommandsModule() : base("/setupcommands") {

            Post["/apply"] = x => {
                SetupCommands.Set();
                return HttpStatusCode.OK;
            };

            Post["/"] = x => {
                string data = Request.Form.Data;
                var objects = JsonConvert.DeserializeObject<Command[]>(data);
                Application.CurrentConfiguration.SetupCommands = objects;
                ConfigRepo.Save();
                return HttpStatusCode.OK;
            };
        }
    }
}