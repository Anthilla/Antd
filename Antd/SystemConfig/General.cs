using Antd.Common;

namespace Antd.SystemConfig {
    public class SystemSet {
        public class General {
            public static void NewHostname(string hostname) {
                var oldHostname = Command.Launch("hostname", "");
                Command.Launch("hostnamectl", "set-hostname " + hostname);
                ConsoleLogger.Success("You changed the hostname from {0} to {1}", oldHostname, hostname);
            }

            public static void NewDomainname(string domainname) {
                var oldDomainname = Command.Launch("domainname", "");
                Command.Launch("domainname", domainname);
                ConsoleLogger.Success("You changed the domainname from {0} to {1}", oldDomainname, domainname);
            }

            public static void NewTimezone(string timezone) {
                var oldTimezone = Command.Launch("date", "+%Z,%t%:z");
                Command.Launch("timedatectl", "set-timezone " + timezone);
                ConsoleLogger.Success("You changed the timezone from {0} to {1}", oldTimezone, timezone);
            }

            public static void NewTimeserver(string timeserver) {
                //todo: check linux command?
                var oldTimeserver = "";
                Command.Launch("", timeserver);
                ConsoleLogger.Success("You changed the timeserver from {0} to {1}", oldTimeserver, timeserver);
            }
        }
    }
}
