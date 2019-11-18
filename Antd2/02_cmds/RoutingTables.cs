using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SSO = System.StringSplitOptions;

namespace Antd2.cmds {
    public class RoutingTables {

        private const string rtTablesFilePath = "/etc/iproute2/rt_tables";

        public static IEnumerable<(string Id, string Name)> Get() {
            if (!File.Exists(rtTablesFilePath)) {
                return Array.Empty<(string Id, string Name)>();
            }
            return File.ReadAllLines(rtTablesFilePath)
                .Where(_ => !_.StartsWith("#", StringComparison.InvariantCulture))
                .Select(_ => ParseRtTablesLine(TrimComment(_)));
        }

        private static string TrimComment(string line) {
            return line.Split(new[] { '#' }, SSO.RemoveEmptyEntries).FirstOrDefault();
        }

        private static (string Id, string Name) ParseRtTablesLine(string line) {
            var arr = line.Split(new[] { ' ' }, SSO.RemoveEmptyEntries);
            return (arr.FirstOrDefault().Trim(), arr.LastOrDefault().Trim());
        }

        public static void Write(IEnumerable<(string Id, string Name)> rtTables) {
            var defaultLines = new[] {
                "#",
                "# reserved values",
                "#",
                "255     local",
                "254     main",
                "253     default",
                "0       unspec",
                "",
                "#",
                "# local",
                "#",
                "",
            };
            File.WriteAllLines(rtTablesFilePath, defaultLines);

            var lines = rtTables.Select(_ => $"{_.Id} {_.Name}");
            File.AppendAllLines(rtTablesFilePath, lines);
        }

    }
}
