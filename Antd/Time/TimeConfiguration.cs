using System.Collections.Generic;
using antd.commands;
using antdlib.common;
using antdlib.Systemd;

namespace Antd.Time {
    public class TimeConfiguration {

        private readonly CommandLauncher _launcher = new CommandLauncher();

        public void Start() {

        }

        public void SyncClock(string ntpServer = "") {
            _launcher.Launch("sync-clock");
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

        public bool IsNtpdActive() {
            return Systemctl.IsActive("ntpd");
        }
    }
}
