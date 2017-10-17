using Antd.models;
using anthilla.core;
using Nancy;
using Newtonsoft.Json;
using System.Linq;

namespace Antd.Modules {
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
                var memoryUsage = CommonInt32.GetPercentage(int.Parse(free.Total), int.Parse(free.Used)).ToString();
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

            Get["/antduptime"] = x => {
                return JsonConvert.SerializeObject(Application.STOPWATCH);
            };
        }
    }
}