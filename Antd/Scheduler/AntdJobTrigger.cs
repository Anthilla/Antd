using System;
using System.Collections;
using System.IO;
using System.Timers;

namespace Antd.Scheduler {
    public class AntdJobTrigger {

        public enum DayOfTheWeek : byte {
            Sunday = 0,
            Monday = 1,
            Tuesday = 2,
            Wednesday = 3,
            Thursday = 4,
            Friday = 5,
            Saturday = 6,
        }

        public enum MonthOfTheYear : byte {
            January = 0,
            February = 1,
            March = 2,
            April = 3,
            May = 4,
            June = 5,
            July = 6,
            August = 7,
            September = 8,
            October = 9,
            November = 10,
            December = 11
        }

        public enum TriggerPeriod : byte {
            IsOneTimeOnly = 0,
            IsDaily = 1,
            IsWeekly = 2,
            IsMonthly = 3
        }

        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }

        public DateTime Hour { get; set; }

        public dynamic TriggerSetting { get; set; }

        public class TriggerSettingIsOneTimeOnly {
            public bool Active { get; set; }

            public DateTime Date { get; set; }
        }

        public class TriggerSettingIsDaily {
            public int TimeSpan { get; set; }
        }

        public class TriggerSettingIsWeekly {
            public int DayOfTheWeek { get; set; }
        }

        //public class TriggerSettingIsMonthly {
        //    public int MonthOfTheYear { get; set; }
        //    //questo
        //    public int DayOfTheMonth { get; set; }
        //    //oppure
        //    public int DayOfTheWeek { get; set; }
        //}
    }
}
