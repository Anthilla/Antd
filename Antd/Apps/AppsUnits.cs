using System.Collections.Generic;
using System.IO;
using antdlib.common;
using antdlib.Systemd;

namespace Antd.Apps {
    public class AppsUnits {
        public static string CreatePrepareUnit(string name, string frameworkDir) {
            var unitName = $"app-{name.ToLower()}-01-prepare.service".Replace(" ", "");
            var fileName = $"{Parameter.AppsUnits}/{unitName}";
            if (File.Exists(fileName)) {
                File.Delete(fileName);
            }
            var lines = new List<string> {
                "[Unit]",
                "Description=External Volume Unit, Application: {name} Prepare Service",
                $"Before=app-{name.ToLower()}-02-mount.service".Replace(" ", ""),
                "",
                "[Service]",
                $"ExecStart=/bin/mkdir -p {frameworkDir}",
                "SuccessExitStatus=0",
                "RemainAfterExit=yes",
                "",
                "[Install]",
                "WantedBy=applicative.target"
            };
            File.WriteAllLines(fileName, lines);
            Systemctl.DaemonReload();
            return unitName;
        }

        public static string CreateMountUnit(string name, string sourcePath, string frameworkDir) {
            var unitName = $"app-{name.ToLower()}-02-mount.service".Replace(" ", "");
            var fileName = $"{Parameter.AppsUnits}/{unitName}";
            if (File.Exists(fileName)) {
                File.Delete(fileName);
            }
            var lines = new List<string> {
                "[Unit]",
                $"Description=External Volume Unit, Application: {frameworkDir} Mount",
                "",
                "[Service]",
                $"ExecStart=/bin/mount {sourcePath} {frameworkDir}",
                "SuccessExitStatus=0",
                "RemainAfterExit=yes",
                "",
                "[Install]",
                "WantedBy=applicative.target"
            };
            File.WriteAllLines(fileName, lines);
            Systemctl.DaemonReload();
            return unitName;
        }

        public static string CreateLauncherUnit(string name, string exeName, string exePath) {
            var unitName = $"app-{name.ToLower()}-{exeName.ToLower().Replace(".exe", "")}-launcher.service";
            var fileName = $"{Parameter.AppsUnits}/{unitName}";
            if (File.Exists(fileName)) {
                File.Delete(fileName);
            }
            var lines = new List<string> {
                "[Unit]",
                $"Description=External Volume Unit, Application: {exeName} Launcher Service",
                $"After=app-{name.ToLower()}-02-mount.service".Replace(" ", ""),
                "",
                "[Service]",
                $"ExecStart=/usr/bin/mono {exePath}",
                "Restart=on-failure",
                "RemainAfterExit=no",
                "TasksMax=infinity",
                "LimitNOFILE=1024000",
                "",
                "[Install]",
                "WantedBy=applicative.target"
            };
            File.WriteAllLines(fileName, lines);
            Systemctl.DaemonReload();
            return unitName;
        }
    }
}
