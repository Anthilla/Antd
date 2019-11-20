using System.Collections.Generic;

namespace Antd2.cmds {
    public class Dmesg {

        private const string dmesgCommand = "dmesg";

        public static IEnumerable<string> GetLog() {
            return Bash.Execute(dmesgCommand);
        }
    }
}