using System.Collections.Generic;
using System.IO;
using antdlib.common;

namespace Antd.Ui {
    public class UiService {

        private const string Service01PreparePath = "/mnt/cdrom/Units/antd.target.wants/app-antdui-01-prepare.service";
        private const string Service02MountPath = "/mnt/cdrom/Units/antd.target.wants/app-antdui-02-mount.service";
        private const string Service03LauncherPath = "/mnt/cdrom/Units/antd.target.wants/app-antdui-03-launcher.service";

        private const string Service01PrepareUnit = "app-antdui-01-prepare.service";
        private const string Service02MountUnit = "app-antdui-02-mount.service";
        private const string Service03LauncherUnit = "app-antdui-03-launcher.service";

        public static void Setup() {
            var edit = false;

            if(!File.Exists(Service01PreparePath)) {
                File.WriteAllLines(Service01PreparePath, Service01Prepare());
                edit = true;
            }

            if(!File.Exists(Service02MountPath)) {
                File.WriteAllLines(Service02MountPath, Service02Mount());
                edit = true;
            }

            if(!File.Exists(Service03LauncherPath)) {
                File.WriteAllLines(Service03LauncherPath, Service03Launcher());
                edit = true;
            }

            if(edit) {
                Systemctl.DaemonReload();

                if(Systemctl.IsActive(Service01PrepareUnit) == false) {
                    Systemctl.Restart(Service01PrepareUnit);
                }

                if(Systemctl.IsActive(Service02MountUnit) == false) {
                    Systemctl.Restart(Service02MountUnit);
                }

                if(Systemctl.IsActive(Service03LauncherUnit) == false) {
                    Systemctl.Restart(Service03LauncherUnit);
                }
            }
        }

        private static IEnumerable<string> Service01Prepare() {
            return new List<string> {
                "[Unit]",
                "Description=ExtUnit, Application: AntdUi 01 Prepare",
                "Before=app-antdui-02-mount.service",
                "",
                "[Service]",
                "ExecStart =/bin/mkdir -p /framework/antdui",
                "SuccessExitStatus=0",
                "RemainAfterExit=yes",
                "",
                "[Install]",
                "WantedBy=antd.target",
                ""
            };
        }

        private static IEnumerable<string> Service02Mount() {
            return new List<string> {
                "[Unit]",
                "Description=ExtUnit, Application: AntdUi 02 Mount Service",
                "Before=app-antdui-03-launcher.service",
                "",
                "[Service]",
                "ExecStart=/bin/mount /mnt/cdrom/Apps/Anthilla_AntdUi/active-version /framework/antdui",
                "SuccessExitStatus=0",
                "RemainAfterExit=yes",
                "Type=oneshot",
                "",
                "[Install]",
                "WantedBy=antd.target",
                ""
            };
        }

        private static IEnumerable<string> Service03Launcher() {
            return new List<string> {
                "[Unit]",
                "Description=ExtUnit, Application: AntdUi 03 Launcher",
                "",
                "[Service]",
                "User=obse",
                "ExecStart=/usr/bin/mono /framework/antdui/AntdUi.exe",
                "Restart=on-failure",
                "SuccessExitStatus=0",
                "RemainAfterExit=no",
                "TasksMax=infinity",
                "LimitNOFILE=1024000",
                "",
                "[Install]",
                "WantedBy=antd.target",
                ""
            };
        }
    }
}
