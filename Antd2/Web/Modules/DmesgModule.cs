using Antd.cmds;
using Nancy;
using Newtonsoft.Json;

namespace Antd2.Modules {
    public class DmesgModule : NancyModule {

        public DmesgModule() : base("/dmesg") {

            Get["/"] = x => {
                return JsonConvert.SerializeObject(Dmesg.GetLog());
            };
        }
    }
}