using Antd.cmds;
using Nancy;
using Newtonsoft.Json;
using System.Linq;

namespace Antd2.Modules {
    public class InfoModule : NancyModule {

        private static Info CURRENT_INFO = new Info();

        public InfoModule() : base("/info") {

            Get["/"] = x => {
                CURRENT_INFO.Uptime = Uptime.Get();
                CURRENT_INFO.Free = Free.Get();
                CURRENT_INFO.DiskUsage = DiskUsage.Get()
                    .Where(_ => _.Type != "squashfs" && _.Type != "tmpfs" && _.Type != "devtmpfs")
                    .OrderBy(_ => _.MountedOn)
                    .ToArray();

                return JsonConvert.SerializeObject(CURRENT_INFO);
            };

            Get["/memory"] = x => {
                return JsonConvert.SerializeObject(Application.RunningConfiguration.Info.MemInfo);
            };

            Get["/free"] = x => {
                return JsonConvert.SerializeObject(Application.RunningConfiguration.Info.Free);
            };

            Get["/cpu"] = x => {
                return JsonConvert.SerializeObject(Application.RunningConfiguration.Info.CpuInfo);
            };
        }
    }
}