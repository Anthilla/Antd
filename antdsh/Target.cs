using System.Collections.Generic;
using System.IO;
using anthilla.core;

namespace antdsh {
    public class Target {
        public void Check() {
        }

        public static void Setup() {
            if(IsTargetActive())
                return;
            if(!Directory.Exists("/usr/lib64/systemd/system/")) { return; }
            Directory.CreateDirectory("/etc/systemd/system/");
            Directory.CreateDirectory("/etc/systemd/system/tt.target.wants");
            Directory.CreateDirectory("/mnt/cdrom/Units/tt.target.wants");
            Directory.CreateDirectory("/mnt/cdrom/Units/ttUnits");
            WriteTimerTargetFile();
            WriteTimerServiceFile();
            WriteTimerMountFile();
            Bash.Execute("ln -s ../../../../usr/lib64/systemd/system/tt.service tt.service", "/etc/systemd/system/multi-user.target.wants", false);
            Bash.Execute("systemctl daemon-reload", false);
            Bash.Execute("systemctl start tt.service", false);
            Bash.Execute("systemctl start tt.target", false);
            Bash.Execute("systemctl daemon-reload", false);
        }

        public static void StartAll() {
            Bash.Execute("systemctl restart tt.target", false);
        }

        #region TT Target
        private static bool IsTargetActive() {
            var result = Bash.Execute("systemctl is-active tt.target");
            return result.Trim() == "active";
        }

        private static void WriteTimerTargetFile() {
            const string file = "/usr/lib64/systemd/system/tt.target";
            if(File.Exists(file)) {
                File.Delete(file);
            }
            var timerText = new List<string> {
                "[Unit]",
                "Description=Description=Anthilla OS - Triggers and Timers Target",
                "After=etc-systemd-system-tt.target.wants.mount",
                "AllowIsolate=yes",
                "",
                "[Install]",
                "WantedBy=multi-user.target",
                ""
            };
            File.WriteAllLines(file, timerText);
        }

        private static void WriteTimerServiceFile() {
            const string file = "/usr/lib64/systemd/system/tt.service";
            if(File.Exists(file)) {
                File.Delete(file);
            }
            var timerText = new List<string> {
                "[Unit]",
                "Description=Description=Anthilla OS - Triggers and Timers Target",
                "After=etc-systemd-system-tt.target.wants.mount",
                "Before=tt.target",
                "Requires=etc-systemd-system-tt.target.wants.mount",
                "",
                "[Service]",
                "ExecStartPre=/usr/bin/systemctl daemon-reload",
                "ExecStart=/usr/bin/systemctl start tt.target",
                "",
                "[Install]",
                "WantedBy=multi-user.target",
                ""
            };
            File.WriteAllLines(file, timerText);
        }

        private static void WriteTimerMountFile() {
            const string file = "/usr/lib64/systemd/system/etc-systemd-system-tt.target.wants.mount";
            if(File.Exists(file)) {
                File.Delete(file);
            }
            var timerText = new List<string> {
                "[Unit]",
                "Description=Description=Anthilla OS - Triggers and Timers Target Units Binding",
                "After=mnt-cdrom.mount",
                "Before=tt.service tt.target",
                "",
                "[Mount]",
                "What=/mnt/cdrom/Units/tt.target.wants",
                "Where=/etc/systemd/system/tt.target.wants",
                "Type=bind",
                "Options=bind",
                "",
                "[Install]",
                "WantedBy=multi-user.target",
                ""
            };
            File.WriteAllLines(file, timerText);
        }
        #endregion TT Target
    }
}
