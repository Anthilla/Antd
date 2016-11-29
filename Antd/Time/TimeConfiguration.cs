using System.Collections.Generic;
using System.IO;
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

        public void SetNtpConfiguration() {
            if(!File.Exists("/etc/ntp.conf") || string.IsNullOrEmpty(File.ReadAllText("/etc/ntp.conf"))) {
                var lines = new List<string> {
                    "restrict 10.11.0.0 mask 255.255.0.0 nomodify",
                    "restrict 192.168.0.0 mask 255.255.255.0 nomodify",
                    "server 0.it.pool.ntp.org",
                    "server 1.it.pool.ntp.org",
                    "server 2.it.pool.ntp.org",
                    "server 3.it.pool.ntp.org",
                    "server ntp1.ien.it",
                    "server ntp2.ien.it",
                    "interface ignore wildcard",
                    "interface listen 10.11.19.111",
                    "driftfile /var/lib/ntp/ntp.drift",
                    "logfile /var/log/ntp/ntpd.log",
                    "statistics loopstats",
                    "statsdir /var/log/ntp/",
                    "filegen peerstats file peers type day link enable",
                    "filegen loopstats file loops type day link enable"
                };
                File.WriteAllLines("/etc/ntp.conf", lines);
            }
        }

        public bool IsNtpdActive() {
            return Systemctl.IsActive("ntpd");
        }
    }
}
