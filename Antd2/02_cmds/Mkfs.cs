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

            public static void AddLabel(string device, string label) {
                Bash.Do($"{mkfsExt4Command} -L {label} {device}");
            }

            private static (string FS, string Type, string Blocks, string Used, string Avail, string Mountpoint) ParseDfLine(string line) {
                var arr = line.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                return (arr[0], arr[1], arr[2], arr[3], arr[4], arr[6]);
            }
        }

 
    }
}
