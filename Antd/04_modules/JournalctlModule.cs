using Antd.cmds;
using Nancy;
using Newtonsoft.Json;

namespace Antd.Modules {
    public class JournalctlModule : NancyModule {

        public JournalctlModule() : base("/journalctl") {

            Get["/"] = x => {
                return JsonConvert.SerializeObject(Journalctl.GetLog());
            };

            Get["/unit/{unitname}"] = x => {
                return JsonConvert.SerializeObject(Journalctl.GetUnitLog((string)x.unitname));
            };

            Get["/unit/antd"] = x => {
                return JsonConvert.SerializeObject(Journalctl.GetAntdLog());
            };

            Get["/unit/antdui"] = x => {
                return JsonConvert.SerializeObject(Journalctl.GetAntdUiLog());
            };

            Get["/last/{hours}"] = x => {
                return JsonConvert.SerializeObject(Journalctl.GetLastHours((int)x.hours));
            };
        }
    }
}