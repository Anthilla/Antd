using antd.core;
using System.Collections.Generic;

namespace Antd2.cmds {
    public class Journalctl {

        private const string journalctlCommand = "journalctl";
        private const string journalctlOptions = "--no-pager --quiet";

        public static IEnumerable<string> GetLog() {
            return Bash.Execute($"{journalctlCommand} {journalctlOptions}");
        }

        public static IEnumerable<string> GetUnitLog(string unitName) {
            var args = CommonString.Append(journalctlOptions, " -u ", unitName);
            return Bash.Execute($"{journalctlCommand} {args}");
        }

        public static IEnumerable<string> GetLastHours(int hours) {
            var args = CommonString.Append(journalctlOptions, " --since='", hours.ToString(), "h ago'");
            return Bash.Execute($"{journalctlCommand} {args}");
        }
    }
}