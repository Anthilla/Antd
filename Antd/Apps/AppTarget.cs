using antdlib.common;
using System.Collections.Generic;
using System.IO;

namespace Antd.Apps {
    public class AppTarget {

        private static readonly Bash Bash = new Bash();

        public static void Setup() {
            if(IsTargetActive())
                return;
            if(!Directory.Exists("/usr/lib64/systemd/system/")) { return; }
            Directory.CreateDirectory("/etc/systemd/system/");
            Directory.CreateDirectory("/etc/systemd/system/app.target.wants");
            Directory.CreateDirectory("/mnt/cdrom/Units/app.target.wants");
            WriteTimerTargetFile();
            WriteTimerServiceFile();
            WriteTimerMountFile();
            Bash.Execute("ln -s ../../../../usr/lib64/systemd/system/app.service app.service", "/etc/systemd/system/multi-user.target.wants", false);
            Bash.Execute("systemctl daemon-reload", false);
            Bash.Execute("systemctl start app.service", false);
            Bash.Execute("systemctl start app.target", false);
            Bash.Execute("systemctl daemon-reload", false);
        }

        public static void StartAll() {
            Bash.Execute("systemctl restart app.target", false);
        }

        #region TT Target
        private static  bool IsTargetActive() {
            var result = Bash.Execute("systemctl is-active app.target", false);
            return result.Trim() == "active";
        }

        private static  void WriteTimerTargetFile() {
            const string file = "/usr/lib64/systemd/system/app.target";
            if(File.Exists(file)) {
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

        private static  void WriteTimerServiceFile() {
            const string file = "/usr/lib64/systemd/system/app.service";
            if(File.Exists(file)) {
                File.Delete(file);
            }
            var timerText = new List<string> {
                "[Unit]",
                "Description=Description=Anthilla OS - Antd Managed Application Target",
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

        private static  void WriteTimerMountFile() {
            const string file = "/usr/lib64/systemd/system/etc-systemd-system-app.target.wants.mount";
            if(File.Exists(file)) {
                File.Delete(file);
            }
            var timerText = new List<string> {
                "[Unit]",
                "Description=Description=Anthilla OS - Antd Managed Application Target Units Binding",
                "After=mnt-cdrom.mount",
                "Before=app.service app.target",
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
