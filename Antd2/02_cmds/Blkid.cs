using System;
using System.Collections.Generic;
using System.Linq;

namespace Antd2.cmds {
    public class Blkid {

        private const string blkidCommand = "blkid";

        public static IEnumerable<(string Partition, string Uuid, string Type, string Label)> Get() {
            var lines = Bash.Execute(blkidCommand)
                .Select(_ => ParseBlkidLine(_));
            return lines;
        }

        private static (string Partition, string Uuid, string Type, string Label) ParseBlkidLine(string line) {
            var arr = line.Split(new[] { ":" }, StringSplitOptions.RemoveEmptyEntries);
            var partition = arr[0];
            var details = arr[1];
            var uuid = Help.CaptureGroup(details, "UUID=\"([a-zA-Z0-9\\-]+)\"").Trim();
            var type = Help.CaptureGroup(details, "TYPE=\"([a-zA-Z0-9\\-_]+)\"").Trim();
            var label = Help.CaptureGroup(details, "LABEL=\"([a-zA-Z0-9\\-\\/]+)\"").Trim();
            return (partition, uuid, type, label);
        }
    }
}
