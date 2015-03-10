using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Antd.UnitFiles {
    public class Systemctl {
        public static CommandModel Start(string unit) {
            return Command.Launch("systemctl", "start " + unit);
        }

        public static CommandModel Stop(string unit) {
            return Command.Launch("systemctl", "stop " + unit);
        }

        public static CommandModel Restart(string unit) {
            return Command.Launch("systemctl", "restart " + unit);
        }

        public static CommandModel Reload(string unit) {
            return Command.Launch("systemctl", "reload " + unit);
        }

        public static CommandModel Status(string unit) {
            return Command.Launch("systemctl", "status " + unit);
        }

        public static CommandModel IsEnabled(string unit) {
            return Command.Launch("systemctl", "is-enabled " + unit);
        }

        public static CommandModel Enable(string unit) {
            return Command.Launch("systemctl", "enable " + unit);
        }

        public static CommandModel Disable(string unit) {
            return Command.Launch("systemctl", "disable " + unit);
        }
    }
}
