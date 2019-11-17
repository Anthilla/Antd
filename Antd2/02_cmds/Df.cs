using System;
using System.Collections.Generic;
using System.Linq;

namespace Antd2.cmds {
    public class Df {

        private const string dfCommand = "df";

        public static IEnumerable<(string FS, string Type, string Blocks, string Used, string Avail, string Mountpoint)> Get() {
            var lines = Bash.Execute($"{dfCommand} -kTh")
                .Skip(1)
                .Select(_ => ParseDfLine(_));
            return lines;
        }

        //  Filesystem     Type      Size  Used Avail Use% Mounted on
        private static (string FS, string Type, string Blocks, string Used, string Avail, string Mountpoint) ParseDfLine(string line) {
            var arr = line.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries);
            return (arr[0], arr[1], arr[2], arr[3], arr[4], arr[6]);
        }
    }
}
