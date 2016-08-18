using System.Collections.Generic;
using System.IO;
using antdlib.common;
using antdlib.Systemd;

namespace Antd.Apps {
    public class AppsUnits {
        public static void CreatePrepareUnit(string name, string frameworkDir) {
            var fileName = $"{Parameter.AppsUnits}/app-{name}-01-prepare.service";
            if (File.Exists(fileName)) {
                File.Delete(fileName);
            }
            var lines = new List<string> {
                "[Unit]",
                "Description=External Volume Unit, Application: {name} Prepare Service",
                $"Before=app-{name}-02-mount.service",
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
        }

        public static void CreateMountUnit(string name, string sourcePath, string frameworkDir) {
            var fileName = $"{Parameter.AppsUnits}/app-{name}-02-mount.service";
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
        }

        public static void CreateLauncherUnit(string name, string exeName, string exePath) {
            var fileName = $"{Parameter.AppsUnits}/app-{name}-{exeName.Replace(".exe", "")}-launcher.service";
            if (File.Exists(fileName)) {
                File.Delete(fileName);
            }
            var lines = new List<string> {
                "[Unit]",
                $"Description=External Volume Unit, Application: {exeName} Launcher Service",
                $"After=app-{name}-02-mount.service",
                "",
                "[Service]",
                $"ExecStart=/usr/bin/mono {exePath}",
                "Restart=on-failed",
                "SuccessExitStatus=0",
                "RemainAfterExit=no",
                "",
                "[Install]",
                "WantedBy=applicative.target"
            };
            File.WriteAllLines(fileName, lines);
            Systemctl.DaemonReload();
        }
    }
}
