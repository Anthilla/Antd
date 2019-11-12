using System;
using System.Collections.Generic;
using System.Linq;

namespace Antd2.cmds {
    public class Lsblk {

        private const string lsblkCommand = "lsblk";

        public static IEnumerable<(string Name, string MajMin, string Rm, string Size, string Ro, string Type, string Mountpoint)> Get() {
            var lines = Bash.Execute($"{lsblkCommand} -banl")
                .Select(_ => ParseLsblkLine(_));
            return lines;
        }

        private static (string Name, string MajMin, string Rm, string Size, string Ro, string Type, string Mountpoint) ParseLsblkLine(string line) {
            var arr = line.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries);
            var mp = arr.Length < 7 ? string.Empty : arr[6];
            return (arr[0], arr[1], arr[2], arr[3], arr[4], arr[5], mp);
        }
    }
}
