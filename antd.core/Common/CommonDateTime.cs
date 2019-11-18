using System;

namespace antd.core {
    public class CommonDateTime {
        public static bool Compare(DateTime d1, DateTime d2) {
            return d1 > d2;
        }

        public static bool Compare(DateTime d1, DateTime d2, long l) {
            return Math.Abs((d1 - d2).Ticks) > l;
        }

        public static DayOfWeek DayOfWeek(DateTime date) {
            return (DayOfWeek)((((date.Ticks >> 14) / 52734375L) + 1) % 7);
        }

        public static DateTime[] GetDatesInMonth(int year, int month) {
            var dates = new System.Collections.Generic.List<DateTime>();
            for(var date = new DateTime(year, month, 1); date.Month == month; date = date.AddDays(1)) {
                dates.Add(date);
            }
            return dates.ToArray();
        }

        public static long Timestamp(DateTime date) {
            var dateString = date.ToString("yyyyMMddHHmmss");
            if(long.TryParse(dateString, out long timestamp) == false) {
                return 0;
            }
            return timestamp;
        }

        public static double DateTimeToEpoch(DateTime dateTime) {
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            var unixDateTime = (dateTime.ToUniversalTime() - epoch).TotalSeconds;
            return unixDateTime;
        }

        public static DateTime EpochToDateTime(long unixDateTime) {
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            var timeSpan = TimeSpan.FromSeconds(unixDateTime);
            var localDateTime = epoch.Add(timeSpan).ToLocalTime();
            return localDateTime;
        }


        public static DateTime UnixTimestampToDateTime(double unixTime) {
            DateTime unixStart = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            long unixTimeStampInTicks = (long)(unixTime * TimeSpan.TicksPerSecond);
            return new DateTime(unixStart.Ticks + unixTimeStampInTicks, DateTimeKind.Utc);
        }

        public static double DateTimeToUnixTimestamp(DateTime dateTime) {
            DateTime unixStart = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            long unixTimeStampInTicks = (dateTime.ToUniversalTime() - unixStart).Ticks;
            return (double)unixTimeStampInTicks / TimeSpan.TicksPerSecond;
        }
    }
}
