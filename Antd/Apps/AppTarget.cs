using System.Collections.Generic;
using System.IO;
using antdlib.common;

namespace Antd.Apps {
    public class AppTarget {
        public static void Setup() {
            if (IsTargetActive()) return;
            Bash.Execute("mkdir -p /etc/systemd/system/app.target.wants");
            Bash.Execute("mkdir -p /mnt/cdrom/Units/app.target.wants");
            WriteTimerTargetFile();
            WriteTimerServiceFile();
            WriteTimerMountFile();
            Bash.Execute("ln -s ../../../../usr/lib64/systemd/system/app.service app.service", "/etc/systemd/system/multi-user.target.wants");
            Bash.Execute("systemctl daemon-reload");
            Bash.Execute("systemctl start app.service");
            Bash.Execute("systemctl start app.target");
            Bash.Execute("systemctl daemon-reload");
        }

        public static void StartAll() {
            Bash.Execute("systemctl restart app.target");
        }

        #region TT Target
        private static bool IsTargetActive() {
            var result = Bash.Execute("systemctl is-active app.target");
            return result.Trim() == "active";
        }

        private static void WriteTimerTargetFile() {
            const string file = "/usr/lib64/systemd/system/app.target";
            if (File.Exists(file)) {
                File.Delete(file);
            }
            var timerText = new List<string> {
                "[Unit]",
                "Description=Description=Anthilla OS - Antd Managed Application Target",
                "After=etc-systemd-system-app.target.wants.mount",
                "AllowIsolate=yes",
                "",
                "[Install]",
                "WantedBy=multi-user.target",
                ""
            };
            File.WriteAllLines(file, timerText);
        }

        private static void WriteTimerServiceFile() {
            const string file = "/usr/lib64/systemd/system/app.service";
            if (File.Exists(file)) {
                File.Delete(file);
            }
            var timerText = new List<string> {
                "[Unit]",
                "Description=Description=Anthilla OS - Antd Managed Application Target",
                "ConditionDirectoryNotEmpty=/etc/systemd/system/app.target.wants/",
                "After=etc-systemd-system-app.target.wants.mount",
                "Before=app.target",
                "Requires=etc-systemd-system-app.target.wants.mount",
                "",
                "[Service]",
                "ExecStartPre=/usr/bin/systemctl daemon-reload",
                "ExecStart=/usr/bin/systemctl start app.target",
                "",
                "[Install]",
                "WantedBy=multi-user.target",
                ""
            };
            File.WriteAllLines(file, timerText);
        }

        private static void WriteTimerMountFile() {
            const string file = "/usr/lib64/systemd/system/etc-systemd-system-app.target.wants.mount";
            if (File.Exists(file)) {
                File.Delete(file);
            }
            var timerText = new List<string> {
                "[Unit]",
                "Description=Description=Anthilla OS - Antd Managed Application Target Units Binding",
                "After=mnt-cdrom.mount",
                "Before=app.service app.target",
                "ConditionPathExists=/etc/systemd/system/app.target.wants",
                "ConditionPathExists=/mnt/cdrom/Units/app.target.wants",
                "ConditionPathIsDirectory=/etc/systemd/system/app.target.wants",
                "",
                "[Mount]",
                "What=/mnt/cdrom/Units/app.target.wants",
                "Where=/etc/systemd/system/app.target.wants",
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
