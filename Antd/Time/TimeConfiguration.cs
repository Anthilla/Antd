using System.Collections.Generic;
using antd.commands;
using antdlib.common;
using antdlib.Systemd;
using Antd.Host;

namespace Antd.Time {
    public class TimeConfiguration {

        private readonly CommandLauncher _launcher = new CommandLauncher();
        private readonly HostModel _host;

        public TimeConfiguration() {
            _host = new HostConfiguration().Host;
        }

        public void Start() {

        }

        public void SyncClock(string ntpServer = "") {
            _launcher.Launch("sync-clock");
            var tz = _host.Timezone;
            _launcher.Launch(tz.SetCmd, tz.StoredValues);
            if(IsNtpdActive() == false) {
                _launcher.Launch("ntpdate",
                    string.IsNullOrEmpty(ntpServer)
                        ? new Dictionary<string, string> { { "$server", "ntp1.ien.it" } }
                        : new Dictionary<string, string> { { "$server", ntpServer } });
            }
            _launcher.Launch("set-ntpdate");
            _launcher.Launch("sync-clock");
            ConsoleLogger.Log("clock synced");
        }

        public void SetNtpdServer() {

        }

        public bool IsNtpdActive() {
            return Systemctl.IsActive("ntpd");
        }
    }
}
