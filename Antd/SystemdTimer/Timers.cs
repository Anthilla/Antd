using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using antdlib.common;
using antdlib.views;
using Antd.Database;

namespace Antd.SystemdTimer {
    public class Timers {
        public class Model {
            public string Next { get; set; }
            public string Left { get; set; }
            public string Last { get; set; }
            public string Passed { get; set; }
            public string Unit { get; set; }
            public string Activates { get; set; }
        }

        public static void Setup() {
            if (IsTargetActive()) return;
            Terminal.Execute("mkdir -p /etc/systemd/system/tt.target.wants");
            Terminal.Execute("mkdir -p /mnt/cdrom/Units/tt.target.wants");
            WriteTimerTargetFile();
            WriteTimerServiceFile();
            WriteTimerMountFile();
            Terminal.Execute("ln -s ../../../../usr/lib64/systemd/system/tt.service tt.service", "/etc/systemd/system/multi-user.target.wants");
            Terminal.Execute("systemctl daemon-reload");
            Terminal.Execute("systemctl start tt.service");
            Terminal.Execute("systemctl start tt.target");
            Terminal.Execute("systemctl daemon-reload");
        }

        public static void StartAll() {
            Terminal.Execute("systemctl restart tt.target");
        }

        #region TT Target
        private static bool IsTargetActive() {
            var result = Terminal.Execute("systemctl is-active tt.target");
            return result.Trim() == "active";
        }

        private static void WriteTimerTargetFile() {
            const string file = "/usr/lib64/systemd/system/tt.target";
            if (File.Exists(file)) {
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
            if (File.Exists(file)) {
                File.Delete(file);
            }
            var timerText = new List<string> {
                "[Unit]",
                "Description=Description=Anthilla OS - Triggers and Timers Target",
                "ConditionDirectoryNotEmpty=/etc/systemd/system/tt.target.wants/",
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
            if (File.Exists(file)) {
                File.Delete(file);
            }
            var timerText = new List<string> {
                "[Unit]",
                "Description=Description=Anthilla OS - Triggers and Timers Target Units Binding",
                "After=mnt-cdrom.mount",
                "Before=tt.service tt.target",
                "ConditionPathExists=/etc/systemd/system/tt.target.wants",
                "ConditionPathExists=/mnt/cdrom/Units/tt.target.wants",
                "ConditionPathIsDirectory=/etc/systemd/system/tt.target.wants",
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

        public static IEnumerable<TimerSchema> GetAll() {
            var list = TimerRepository.GetAll().ToList();
            foreach (var el in list) {
                el.IsEnabled = IsActive(el.Alias);
            }
            return list;
        }

        private static readonly string TargetDirectory = Parameter.TimerUnits;
        private static readonly TimerRepository TimerRepository = new TimerRepository();

        public static void Create(string name, string time, string command) {
            var timerFile = $"{TargetDirectory}/{name}.timer";
            if (File.Exists(timerFile)) {
                File.Delete(timerFile);
            }
            var timerText = new List<string> {
                "[Unit]",
                $"Description={name} Timer",
                "",
                "[Timer]",
                $"OnCalendar={time}",
                "Persistent=true",
                "",
                "[Install]",
                "WantedBy=tt.target",
                ""
            };
            File.WriteAllLines(timerFile, timerText);

            var serviceFile = $"{TargetDirectory}/{name}.service";
            if (File.Exists(serviceFile)) {
                File.Delete(serviceFile);
            }
            var serviceText = new List<string> {
                "[Unit]",
                $"Description={name} Service",
                "",
                "[Service]",
                "Type=oneshot",
                "ExecStartPre=/bin/bash -c \"/usr/bin/systemctl set-environment TTDATE=$(/bin/date +'%%Y%%m%%d-%%H%%M%%S')\"",
                $"ExecStart={command}",
                "",
                "[Install]",
                "WantedBy=tt.target",
                ""
            };
            File.WriteAllLines(serviceFile, serviceText);

            var tryget = TimerRepository.GetByName(name);
            if (tryget == null) {
                TimerRepository.Create(new Dictionary<string, string> {
                    {"Guid", Guid.NewGuid().ToString()},
                    {"Alias", name},
                    {"Time", time},
                    {"Command", command}
                });
            }

            Terminal.Execute($"chown root:wheel {timerFile}");
            Terminal.Execute($"chown root:wheel {serviceFile}");
            Terminal.Execute("systemctl daemon-reload");
        }

        public static void Remove(string name) {
            Terminal.Execute($"systemctl stop {name}.target");
            var timerFile = $"{TargetDirectory}/{name}.target";
            if (File.Exists(timerFile)) {
                File.Delete(timerFile);
            }

            Terminal.Execute($"systemctl stop {name}.service");
            var serviceFile = $"{TargetDirectory}/{name}.service";
            if (File.Exists(serviceFile)) {
                File.Delete(serviceFile);
            }

            var tryget = TimerRepository.GetByName(name);
            if (tryget != null) {
                TimerRepository.Delete(tryget.Id);
            }

            Terminal.Execute("systemctl daemon-reload");
        }

        public static void Import() {
            var files = Directory.EnumerateFiles(TargetDirectory).ToList();
            foreach (var file in files.Where(_ => _.EndsWith(".target"))) {
                var name = Path.GetFileName(file);
                if (name != null) {
                    var coreName = name.Replace(".timer", "");
                    var tryget = TimerRepository.GetByName(coreName);
                    var time = "";
                    if (!File.Exists(file)) continue;
                    var lines = File.ReadAllLines(file);
                    var t = lines.FirstOrDefault(_ => _.Contains("OnCalendar"));
                    if (t != null) {
                        time = t.SplitToList("=").Last();
                    }

                    var command = "";
                    var eqPath = files.FirstOrDefault(_ => _.Contains(coreName) && _.EndsWith(".service"));
                    if (eqPath != null) {
                        if (!File.Exists(eqPath)) continue;
                        var clines = File.ReadAllLines(eqPath);
                        var c = clines.FirstOrDefault(_ => _.Contains("ExecStart"));
                        if (c != null) {
                            command = c.SplitToList("=").Last();
                        }
                    }

                    if (tryget == null) {
                        TimerRepository.Create(new Dictionary<string, string> {
                            {"Guid", Guid.NewGuid().ToString()},
                            {"Alias", coreName},
                            {"Time", time},
                            {"Command", command}
                        });
                    }
                }
            }
        }

        public static void Export() {
            var all = TimerRepository.GetAll();
            foreach (var tt in all) {
                Create(tt.Alias, tt.Time, tt.Command);
            }
        }

        public static void Enable(string ttName) {
            Terminal.Execute($"systemctl restart {ttName}.target");
        }

        public static void Disable(string ttName) {
            Terminal.Execute($"systemctl stop {ttName}.target");
        }

        public static bool IsActive(string ttName) {
            var result = Terminal.Execute($"systemctl is-active {ttName}.target");
            return result.Trim() == "active";
        }
    }
}
