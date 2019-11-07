using Antd2.cmds;
using Nancy;
using Newtonsoft.Json;

namespace Antd2.Modules {
    public class JournalctlModule : NancyModule {

        public JournalctlModule() : base("/journalctl") {

            Get("/", x => ApiGet());

            Get("/unit/{unitname}", x => ApiGetUnitName(x));

        }

        private dynamic ApiGet() {
            return Response.AsJson((object)Journalctl.GetLog());
        }

        private dynamic ApiGetUnitName(dynamic x) {
            return JsonConvert.SerializeObject(Journalctl.GetUnitLog((string)x.unitname));
        }
    }
}