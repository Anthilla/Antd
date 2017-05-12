using antdlib.common;
using antdlib.config;
using antdlib.models;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Antd.SystemdTimer {
    public static class Timers {
        public static class Model {
            public static string Next { get; set; }
            public static string Left { get; set; }
            public static string Last { get; set; }
            public static string Passed { get; set; }
            public static string Unit { get; set; }
            public static string Activates { get; set; }
        }

        public static void MoveExistingTimers() {
            Directory.CreateDirectory(Parameter.TimerUnits);
            Directory.CreateDirectory(Parameter.TimerUnitsLinks);
            var ttunitsFiles = Directory.EnumerateFiles(Parameter.TimerUnits).ToList();
            var ttunitsLinks = Directory.EnumerateFiles(Parameter.TimerUnitsLinks).ToList();
            if(ttunitsLinks.Any() && !ttunitsFiles.Any()) {
                foreach(var link in ttunitsLinks) {
                    var name = link.Split('/').LastOrDefault();
                    if(string.IsNullOrEmpty(name)) {
                        continue;
                    }
                    if(ttunitsFiles.Any(_ => _.Contains(name))) {
                        continue;
                    }
                    var file = $"{Parameter.TimerUnits}/{name}";
                    if(!File.Exists(file)) {
                        File.Copy(link, file);

                    }
                }
            }
            foreach(var file in ttunitsFiles) {
                var name = file.Split('/').LastOrDefault();
                if(string.IsNullOrEmpty(name)) {
                    continue;
                }
                if(ttunitsLinks.Any(_ => _.Contains(name))) {
                    continue;
                }
                var link = $"{Parameter.TimerUnitsLinks}/{name}";
                ConsoleLogger.Log($"[units] create link for {name}");
                Bash.Execute($"ln -s {file} {link}");
            }
            Bash.Execute("systemctl daemon-reload", false);
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

        private static readonly string TargetDirectory = Parameter.TimerUnits;

        public static void Create(string name, string time, string command) {
            var timerFile = $"{TargetDirectory}/{name}.timer";
            if(File.Exists(timerFile)) {
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
            if(File.Exists(serviceFile)) {
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

            var schedulerConfiguration = new TimerConfiguration();
            var tryget = schedulerConfiguration.Get().Timers.FirstOrDefault(_ => _.Alias == name);
            if(tryget == null) {
                schedulerConfiguration.AddTimer(new TimerModel {
                    Alias = name,
                    Command = command,
                    Time = time,
                    IsEnabled = true
                });
            }
            Bash.Execute($"chown root:wheel {timerFile}", false);
            Bash.Execute($"chown root:wheel {serviceFile}", false);

            Bash.Execute($"ln -s {timerFile} {Parameter.TimerUnitsLinks}/{name}.timer");
            Bash.Execute($"ln -s {serviceFile} {Parameter.TimerUnitsLinks}/{name}.service");

            Bash.Execute("systemctl daemon-reload", false);
        }

        public static void Remove(string name) {
            Bash.Execute($"systemctl stop {name}.target", false);
            var timerFile = $"{Parameter.TimerUnitsLinks}/{name}.target";
            if(File.Exists(timerFile)) {
                File.Delete(timerFile);
            }
            var timerFile2 = $"{Parameter.TimerUnits}/{name}.target";
            if(File.Exists(timerFile2)) {
                File.Delete(timerFile2);
            }

            Bash.Execute($"systemctl stop {name}.service", false);
            var serviceFile = $"{Parameter.TimerUnitsLinks}/{name}.service";
            if(File.Exists(serviceFile)) {
                File.Delete(serviceFile);
            }
            var serviceFile2 = $"{Parameter.TimerUnits}/{name}.service";
            if(File.Exists(serviceFile2)) {
                File.Delete(serviceFile2);
            }

            var schedulerConfiguration = new TimerConfiguration();
            var tryget = schedulerConfiguration.Get().Timers.FirstOrDefault(_ => _.Alias == name);
            if(tryget != null) {
                schedulerConfiguration.RemoveTimer(tryget.Guid);
            }

            Bash.Execute("systemctl daemon-reload", false);
        }

        public static void Import() {
            var files = Directory.EnumerateFiles(TargetDirectory).ToList();
            foreach(var file in files.Where(_ => _.EndsWith(".target"))) {
                var name = Path.GetFileName(file);
                if(name != null) {
                    var coreName = name.Replace(".timer", "");
                    var schedulerConfiguration = new TimerConfiguration();
                    var tryget = schedulerConfiguration.Get().Timers.FirstOrDefault(_ => _.Alias == coreName);
                    var time = "";
                    if(!File.Exists(file))
                        continue;
                    var lines = File.ReadAllLines(file);
                    var t = lines.FirstOrDefault(_ => _.Contains("OnCalendar"));
                    if(t != null) {
                        time = t.SplitToList("=").Last();
                    }

                    var command = "";
                    var eqPath = files.FirstOrDefault(_ => _.Contains(coreName) && _.EndsWith(".service"));
                    if(eqPath != null) {
                        if(!File.Exists(eqPath))
                            continue;
                        var clines = File.ReadAllLines(eqPath);
                        var c = clines.FirstOrDefault(_ => _.Contains("ExecStart"));
                        if(c != null) {
                            command = c.SplitToList("=").Last();
                        }
                    }

                    if(tryget == null) {
                        schedulerConfiguration.AddTimer(new TimerModel {
                            Alias = coreName,
                            Command = command,
                            Time = time,
                            IsEnabled = true
                        });
                    }
                }
            }
        }

        public static void Export() {
            var schedulerConfiguration = new TimerConfiguration();
            var all = schedulerConfiguration.Get().Timers;
            foreach(var tt in all) {
                Create(tt.Alias, tt.Time, tt.Command);
            }
        }

        public static void Enable(string ttName) {
            Bash.Execute($"systemctl restart {ttName}.target", false);
        }

        public static void Disable(string ttName) {
            Bash.Execute($"systemctl stop {ttName}.target", false);
        }

        public static bool IsActive(string ttName) {
            var result = Bash.Execute($"systemctl is-active {ttName}.target");
            return result.Trim() == "active";
        }
    }
}
