using System.Collections.Generic;
using System.IO;
using anthilla.core;

namespace Antd.Apps {
    public class AppTarget {

        public static void Setup() {
            if(IsTargetActive())
                return;
            ConsoleLogger.Log("[apptarget] setup");
            if(!Directory.Exists("/usr/lib64/systemd/system/")) { return; }
            DirectoryWithAcl.CreateDirectory("/etc/systemd/system/", "755", "root", "wheel");
            DirectoryWithAcl.CreateDirectory("/etc/systemd/system/applicative.target.wants", "755", "root", "wheel");
            DirectoryWithAcl.CreateDirectory("/mnt/cdrom/Units/applicative.target.wants", "755", "root", "wheel");
            WriteTimerTargetFile();
            WriteTimerServiceFile();
            WriteTimerMountFile();
            Bash.Execute("ln -s ../../../../usr/lib64/systemd/system/applicative.service applicative.service", "/etc/systemd/system/multi-user.target.wants", false);
            Bash.Execute("systemctl daemon-reload", false);
            Bash.Execute("systemctl start applicative.service", false);
            Bash.Execute("systemctl start applicative.target", false);
            Bash.Execute("systemctl daemon-reload", false);
        }

        public static void StartAll() {
            Bash.Execute("systemctl start applicative.target", false);
        }

        #region TT Target
        private static  bool IsTargetActive() {
            var result = Bash.Execute("systemctl is-active applicative.target");
            return result.Trim() == "active";
        }

        private static  void WriteTimerTargetFile() {
            const string file = "/usr/lib64/systemd/system/applicative.target";
            if(File.Exists(file)) {
                File.Delete(file);
            }
            var timerText = new List<string> {
                "[Unit]",
                "Description=Description=Anthilla OS - Antd Managed Application Target",
                "After=etc-systemd-system-applicative.target.wants.mount",
                "AllowIsolate=yes",
                "",
                "[Install]",
                "WantedBy=multi-user.target",
                ""
            };
            FileWithAcl.WriteAllLines(file, timerText, "644", "root", "wheel");
        }

        private static  void WriteTimerServiceFile() {
            const string file = "/usr/lib64/systemd/system/applicative.service";
            if(File.Exists(file)) {
                File.Delete(file);
            }
            var timerText = new List<string> {
                "[Unit]",
                "Description=Description=Anthilla OS - Antd Managed Application Target",
                "After=etc-systemd-system-applicative.target.wants.mount",
                "Before=applicative.target",
                "Requires=etc-systemd-system-applicative.target.wants.mount",
                "",
                "[Service]",
                "ExecStartPre=/usr/bin/systemctl daemon-reload",
                "ExecStart=/usr/bin/systemctl start applicative.target",
                "",
                "[Install]",
                "WantedBy=multi-user.target",
                ""
            };
            FileWithAcl.WriteAllLines(file, timerText, "644", "root", "wheel");
        }

        private static  void WriteTimerMountFile() {
            const string file = "/usr/lib64/systemd/system/etc-systemd-system-applicative.target.wants.mount";
            if(File.Exists(file)) {
                File.Delete(file);
            }
            var timerText = new List<string> {
                "[Unit]",
                "Description=Description=Anthilla OS - Antd Managed Application Target Units Binding",
                "After=mnt-cdrom.mount",
                "Before=applicative.service applicative.target",
                "",
                "[Mount]",
                "What=/mnt/cdrom/Units/applicative.target.wants",
                "Where=/etc/systemd/system/applicative.target.wants",
                "Type=bind",
                "Options=bind",
                "",
                "[Install]",
                "WantedBy=multi-user.target",
                ""
            };
            FileWithAcl.WriteAllLines(file, timerText, "644", "root", "wheel");
        }
        #endregion TT Target
    }
}
