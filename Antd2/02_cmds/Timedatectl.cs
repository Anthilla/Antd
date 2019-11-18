using antd.core;
using System.Collections.Generic;
using System.Linq;

namespace Antd2.cmds {

    public class Timedatectl {

        public class Status {
            public string Timezone { get; set; }
        }

        private const string timedatectlCommand = "timedatectl";
        private const string ntpdateCommand = "ntpdate";
        private const string hwclockCommand = "hwclock";
        private const string setTimezoneArg = "set-timezone";
        private const string setNtpdateArg = "set-ntp yes";
        private const string getTimezoneArg = "list-timezones --no-pager";
        private const string systohcArg = "-s";
        private const string hctosysArg = "-w";

        public static Status Get() {
            var result = Bash.Execute(timedatectlCommand);
            if (result.Count() < 1) {
                return new Status();
            }
            var status = new Status {
                Timezone = result.FirstOrDefault(_ => _.Contains("Time zone:")).Split(new[] { ':' }).LastOrDefault().Trim()
            };
            return status;
        }

        //public static bool Apply() {
        //    var current = Application.CurrentConfiguration.TimeDate;
        //    SetTimezone(current.Timezone);
        //    return true;
        //}

        public static IEnumerable<string> Timezones() {
            return Bash.Execute($"{timedatectlCommand} {getTimezoneArg}");
        }

        public static bool SetTimezone(string timezone) {
            var args = CommonString.Append(setTimezoneArg, " ", timezone);
            Bash.Do($"{timedatectlCommand} {getTimezoneArg}");
            return true;
        }

        public static bool SetNtpdate(string ntpServer) {
            Bash.Do($"{ntpdateCommand} {ntpServer}");
            Bash.Do($"{timedatectlCommand} {setNtpdateArg}");
            return true;
        }

        public static bool SyncClock() {
            Bash.Do($"{hwclockCommand} {systohcArg}");
            Bash.Do($"{hwclockCommand} {hctosysArg}");
            return true;
        }
    }
}
