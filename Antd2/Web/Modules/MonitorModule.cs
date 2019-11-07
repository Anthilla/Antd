using Antd.models;
using anthilla.core;
using Nancy;
using Newtonsoft.Json;
using System.Linq;

namespace Antd2.Modules {
    public class MonitorModule : NancyModule {

        private static string localDisk = "/mnt/cdrom";

        /// <summary>
        /// Ottiene le informazioni base sullo stato macchina: 
        /// - hostname
        /// - uptime
        /// - load average
        /// - memory usage
        /// - disk usage
        /// </summary>
        public MonitorModule() : base("/monitor") {
            Get["/"] = x => {
                var hostName = Application.RunningConfiguration.Host.HostName;
                var uptime = Application.RunningConfiguration.Info.Uptime.Uptime;
                var loadAverage = Application.RunningConfiguration.Info.Uptime.LoadAverage;
                var free = Application.RunningConfiguration.Info.Free[0];
                var memoryUsage = GetPercentage(long.Parse(free.Total), long.Parse(free.Used)).ToString();
                var diskUsage = Application.RunningConfiguration.Info.DiskUsage.FirstOrDefault(_ => _.MountedOn == localDisk).UsePercentage;
                var model = new MonitorModel {
                    Hostname = hostName,
                    Uptime = uptime,
                    LoadAverage = loadAverage,
                    MemoryUsage = memoryUsage,
                    DiskUsage = diskUsage
                };
                return JsonConvert.SerializeObject(model);
            };
        }

        private static int GetPercentage(long tot, long part) {
            if(tot == 0 || part == 0) {
                return 0;
            }
            var p = part * 100 / tot;
            return p <= 100 ? (int)p : 0;
        }
    }
}