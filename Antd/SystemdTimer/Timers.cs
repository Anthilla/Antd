using antdlib.common;
using antdlib.config;
using antdlib.models;
using System.Collections.Generic;
using System.IO;
using System.Linq;

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

        public void Setup() {
            if(IsTargetActive())
                return;
            var bash = new Bash();
            bash.Execute("mkdir -p /etc/systemd/system/tt.target.wants", false);
            bash.Execute("mkdir -p /mnt/cdrom/Units/tt.target.wants", false);
            WriteTimerTargetFile();
            WriteTimerServiceFile();
            WriteTimerMountFile();
            bash.Execute("ln -s ../../../../usr/lib64/systemd/system/tt.service tt.service", "/etc/systemd/system/multi-user.target.wants", false);
            bash.Execute("systemctl daemon-reload", false);
            bash.Execute("systemctl start tt.service", false);
            bash.Execute("systemctl start tt.target", false);
            bash.Execute("systemctl daemon-reload", false);
        }

        public void StartAll() {
            var bash = new Bash();
            bash.Execute("systemctl restart tt.target", false);
        }

        #region TT Target
        private static bool IsTargetActive() {
            var bash = new Bash();
            var result = bash.Execute("systemctl is-active tt.target");
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
            if(File.Exists(file)) {
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

        private static readonly string TargetDirectory = Parameter.TimerUnits;

        public void Create(string name, string time, string command) {
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
            var bash = new Bash();
            bash.Execute($"chown root:wheel {timerFile}", false);
            bash.Execute($"chown root:wheel {serviceFile}", false);
            bash.Execute("systemctl daemon-reload", false);
        }

        public void Remove(string name) {
            var bash = new Bash();
            bash.Execute($"systemctl stop {name}.target", false);
            var timerFile = $"{TargetDirectory}/{name}.target";
            if(File.Exists(timerFile)) {
                File.Delete(timerFile);
            }

            bash.Execute($"systemctl stop {name}.service", false);
            var serviceFile = $"{TargetDirectory}/{name}.service";
            if(File.Exists(serviceFile)) {
                File.Delete(serviceFile);
            }

            var schedulerConfiguration = new TimerConfiguration();
            var tryget = schedulerConfiguration.Get().Timers.FirstOrDefault(_ => _.Alias == name);
            if(tryget != null) {
                schedulerConfiguration.RemoveTimer(tryget.Guid);
            }

            bash.Execute("systemctl daemon-reload", false);
        }

        public void Import() {
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

        public void Export() {
            var schedulerConfiguration = new TimerConfiguration();
            var all = schedulerConfiguration.Get().Timers;
            foreach(var tt in all) {
                Create(tt.Alias, tt.Time, tt.Command);
            }
        }

        public void Enable(string ttName) {
            var bash = new Bash();
            bash.Execute($"systemctl restart {ttName}.target", false);
        }

        public void Disable(string ttName) {
            var bash = new Bash();
            bash.Execute($"systemctl stop {ttName}.target", false);
        }

        public bool IsActive(string ttName) {
            var bash = new Bash();
            var result = bash.Execute($"systemctl is-active {ttName}.target");
            return result.Trim() == "active";
        }
    }
}
