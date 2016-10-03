using System.Collections.Generic;
using System.IO;
using antdlib.common;

namespace antdsh {
    public class Units {
        #region Private Vars
        private const string Antdsh01RemountService = "tt-antdsh-01-remount.service";
        private static readonly string Antdsh01RemountServicePath = $"{Parameter.TimerUnits}/{Antdsh01RemountService}";
        private const string Antdsh01RemountTimer = "tt-antdsh-01-remount.timer";
        private static readonly string Antdsh01RemountTimerPath = $"{Parameter.TimerUnits}/{Antdsh01RemountTimer}";
        private const string Antdsh02UmountService = "tt-antdsh-02-umount.service";
        private static readonly string Antdsh02UmountServicePath = $"{Parameter.TimerUnits}/{Antdsh02UmountService}";
        private const string Antdsh03MountService = "tt-antdsh-03-mount.service";
        private static readonly string Antdsh03MountServicePath = $"{Parameter.TimerUnits}/{Antdsh03MountService}";
        #endregion

        public static void CreateRemountUnits() {
            if (!File.Exists(Antdsh01RemountServicePath)) {
                CreateRemountServiceFile();
            }
            if (!File.Exists(Antdsh01RemountTimerPath)) {
                CreateRemountTimerFile();
            }
            if (!File.Exists(Antdsh02UmountServicePath)) {
                CreateUmountServiceFile();
            }
            if (!File.Exists(Antdsh03MountServicePath)) {
                CreateMountServiceFile();
            }
            Bash.Execute($"chown -R root:wheel {Parameter.TimerUnits}/");
            Bash.Execute($"chmod -R 664 {Parameter.TimerUnits}/");
            Bash.Execute("systemctl daemon-reload");
        }

        private static void CreateRemountServiceFile() {
            var lines = new List<string> {
                "[Unit]",
                "Description=ExtUnit, Application: TT Antdsh 01 remount service",
                "ConditionFileNotEmpty=/mnt/cdrom/Units/tt.target.wants/tt-antdsh-02-umount.service",
                "ConditionFileNotEmpty=/mnt/cdrom/Units/tt.target.wants/tt-antdsh-03-mount.service",
                "",
                "[Service]",
                "ExecStart=/usr/bin/systemctl restart tt-antdsh-02-umount.service",
                "ExecStart=/usr/bin/systemctl restart tt-antdsh-03-mount.service",
                "SuccessExitStatus=1 2 3 4 5 6 7 8 9 0",
                "RemainAfterExit=no",
                "Type=oneshot",
                "           ",
                "[Install]",
                "WantedBy=tt.target"
            };
            File.WriteAllLines(Antdsh01RemountServicePath, lines);
        }

        private static void CreateRemountTimerFile() {
            var lines = new List<string> {
                "[Unit]",
                "Description=ExtUnit, Application: TT Antdsh 01 remount timer",
                "",
                "[Timer]",
                "OnActiveSec=10s",
                "Persistent=false",
                "",
                "[Install]",
                "WantedBy=tt.target"
            };
            File.WriteAllLines(Antdsh01RemountTimerPath, lines);
        }

        private static void CreateUmountServiceFile() {
            var lines = new List<string> {
                "[Unit]",
                "Description=ExtUnit, Application: TT Antdsh 02 umount service",
                "ConditionDirectoryNotEmpty=/framework/antdsh",
                "",
                "[Service]",
                "ExecStart=/bin/umount /framework/antdsh",
                "SuccessExitStatus=1 2 3 4 5 6 7 8 9 0",
                "RemainAfterExit=no",
                "Type=oneshot",
                "",
                "[Install]",
                "WantedBy=tt.target",
            };
            File.WriteAllLines(Antdsh02UmountServicePath, lines);
        }

        private static void CreateMountServiceFile() {
            var lines = new List<string> {
                "[Unit]",
                "Description=ExtUnit, Application: TT Antdsh 03 mount service",
                "ConditionDirectoryEmpty=/framework/antdsh",
                "",
                "[Service]",
                "ExecStart=/bin/mount /mnt/cdrom/Apps/Anthilla_antdsh/active-version /framework/antdsh",
                "SuccessExitStatus=1 2 3 4 5 6 7 8 9 0",
                "RemainAfterExit=no",
                "Type=oneshot",
                "",
                "[Install]",
                "WantedBy=tt.target"
            };
            File.WriteAllLines(Antdsh03MountServicePath, lines);
        }
    }
}
