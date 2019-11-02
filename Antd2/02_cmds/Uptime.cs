using Antd.models;
using System;
using System.IO;
using System.Linq;

namespace Antd2.cmds {
    public class Uptime {

        private const string uptimeCommand = "uptime";
        private const string since = "-p";
        private const string loadaverageFile = "/proc/loadavg";

        public static UptimeModel Get() {
            if (!File.Exists(loadaverageFile)) {
                return new UptimeModel();
            }
            var result = Bash.Execute($"{uptimeCommand} {since}").FirstOrDefault();
            var ldavg = File.Exists(loadaverageFile) ? File.ReadAllText(loadaverageFile) : string.Empty;
            var uptime = new UptimeModel() {
                Uptime = result.ToLowerInvariant().Replace("up", "").Trim(),
                Users = string.Empty,
                LoadAverage = ldavg.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)[0]
            };
            return uptime;
        }
    }
}
