using anthilla.core;
using System.Collections.Generic;
using System.Linq;

namespace Antd.cmds {

    public class Timedatectl {

        public class Status {
            public string Timezone { get; set; }
        }

        private const string timedatectlFileLocation = "/usr/bin/timedatectl";
        private const string ntpdateFileLocation = "/usr/sbin/ntpdate";
        private const string hwclockFileLocation = "/sbin/hwclock";
        private const string setTimezoneArg = "--no-pager --no-ask-password --adjust-system-clock set-timezone";
        private const string setNtpdateArg = "--no-pager --no-ask-password --adjust-system-clock set-ntp yes";
        private const string getTimezoneArg = "list-timezones --no-pager";
        private const string systohcArg = "-s";
        private const string hctosysArg = "-w";

        public static Status Get() {
            var result = CommonProcess.Execute(timedatectlFileLocation);
            if(result.Count() < 1) {
                return new Status();
            }
            var status = new Status {
                Timezone = result.FirstOrDefault(_ => _.Contains("Time zone:")).Split(new[] { ':' }).LastOrDefault().Trim()
            };
            return status;
        }

        public static bool Apply() {
            var current = Application.CurrentConfiguration.TimeDate;
            var running = Application.RunningConfiguration.TimeDate;
            if(CommonString.AreEquals(current.Timezone, running.Timezone) == false) {
                SetTimezone(current.Timezone);
            }
            return true;
        }

        public static IEnumerable<string> Timezones() {
            return CommonProcess.Execute(timedatectlFileLocation, getTimezoneArg);
        }

        public static bool SetTimezone(string timezone) {
            var args = CommonString.Append(setTimezoneArg, " ", timezone);
            CommonProcess.Do(timedatectlFileLocation, args);
            return true;
        }

        public static bool SetNtpdate(string ntpServer) {
            CommonProcess.Do(ntpdateFileLocation, ntpServer);
            CommonProcess.Do(timedatectlFileLocation, setNtpdateArg);
            return true;
        }

        public static bool SyncClock() {
            CommonProcess.Do(hwclockFileLocation, systohcArg);
            CommonProcess.Do(hwclockFileLocation, hctosysArg);
            return true;
        }
    }
}
