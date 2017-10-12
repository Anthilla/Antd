using anthilla.core;
using System.IO;

namespace Antd.cmds {

    /// <summary>
    /// TODO
    /// Converti Bash in CommonProcess
    /// </summary>
    public class Applicative {

        public const string applicativeService = "applicative.service";
        public const string applicativeTarget = "applicative.target";

        public static void Setup() {
            if(IsTargetActive())
                return;
            ConsoleLogger.Log($"[{applicativeTarget}] setup");
            if(!Directory.Exists("/usr/lib64/systemd/system/")) { return; }
            DirectoryWithAcl.CreateDirectory("/etc/systemd/system/", "755", "root", "wheel");
            DirectoryWithAcl.CreateDirectory("/etc/systemd/system/applicative.target.wants", "755", "root", "wheel");
            DirectoryWithAcl.CreateDirectory("/mnt/cdrom/Units/applicative.target.wants", "755", "root", "wheel");
            WriteTimerTargetFile();
            WriteTimerServiceFile();
            WriteTimerMountFile();
            Bash.Execute("ln -s ../../../../usr/lib64/systemd/system/applicative.service applicative.service", "/etc/systemd/system/multi-user.target.wants", false);
            Systemctl.Start(applicativeService);
            Systemctl.Start(applicativeTarget);
            Systemctl.DaemonReload();
        }

        public static void Start() {
            Systemctl.Start(applicativeTarget);
        }

        #region [    Target   ]
        private static bool IsTargetActive() {
            var result = Bash.Execute("systemctl is-active applicative.target");
            return result.Trim() == "active";
        }

        private static void WriteTimerTargetFile() {
            const string file = "/usr/lib64/systemd/system/applicative.target";
            if(File.Exists(file)) {
                File.Delete(file);
            }
            var timerText = new string[] {
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

        private static void WriteTimerServiceFile() {
            const string file = "/usr/lib64/systemd/system/applicative.service";
            if(File.Exists(file)) {
                File.Delete(file);
            }
            var timerText = new string[] {
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

        private static void WriteTimerMountFile() {
            const string file = "/usr/lib64/systemd/system/etc-systemd-system-applicative.target.wants.mount";
            if(File.Exists(file)) {
                File.Delete(file);
            }
            var timerText = new string[] {
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
