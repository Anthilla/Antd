using System.Collections.Generic;

namespace Antd2.cmds {

    /// <summary>
    /// https://linux.die.net/man/8/e2fsck
    /// </summary>
    public class E2fsck {

        private const string e2fsckCommand = "e2fsck";

        public static IEnumerable<string> CheckPartition(string partition) {
            return Bash.Execute($"{e2fsckCommand} -f -y -v -C 0 {partition}");
        }
    }
}
