using System;
using System.Collections.Generic;
using System.Linq;

namespace Antd2.cmds {
    public class Mkfs {

        /// <summary>
        /// https://linux.die.net/man/8/mkfs.ext4
        /// </summary>
        public class Ext4 {

            private const string mkfsExt4Command = "mkfs.ext4";

            public static void AddLabel(string partition, string label) {
                Bash.Do($"{mkfsExt4Command} -F -L {label} {partition}");
            }
        }
    }
}
