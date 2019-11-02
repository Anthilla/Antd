using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SSO = System.StringSplitOptions;

namespace Antd2.cmds {
    public class Nsswitch {

        private const string nsswitchFile = "/etc/nsswitch.conf";

        public static IEnumerable<(string Key, string Value)> Get() {
            return File.ReadAllLines(nsswitchFile)
                .Where(_ => !string.IsNullOrEmpty(_))
                .Where(_ => !_.Trim().StartsWith("#", StringComparison.InvariantCulture))
                .Select(_ => ParseNsswitchLine(_))
                .Where(_ => !string.IsNullOrEmpty(_.Key));
        }

        private static (string Key, string Value) ParseNsswitchLine(string line) {
            var arr = line.Split(new[] { ':' }, SSO.RemoveEmptyEntries);
            if (arr.Length != 2) {
                return (string.Empty, string.Empty);
            }
            return (arr.FirstOrDefault().Trim(), arr.LastOrDefault().Trim());
        }

        public static void Write((string Key, string Value)[] param) {
            File.WriteAllLines(nsswitchFile, param.Select(_ => $"{_.Key}:{_.Value}"));
        }
    }
}
