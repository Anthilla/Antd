using anthilla.core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SSO = System.StringSplitOptions;

namespace Antd2.cmds {
    public class Sysctl {

        private const string sysctlCommand = "sysctl";
        private const string sysctlFile = "/etc/sysctl.conf";

        public static IEnumerable<(string Key, string Value)> Get() {
            return Bash.Execute($"{sysctlCommand} -a")
                .Select(_ => ParseSysctlLine(_))
                .Where(_ => !string.IsNullOrEmpty(_.Key));
        }

        public static (string Key, string Value) ParseSysctlLine(string line) {
            var arr = line.Split(new[] { '=' }, SSO.RemoveEmptyEntries);
            if (arr.Length != 2) {
                return (string.Empty, string.Empty);
            }
            return (arr.FirstOrDefault().Trim(), arr.LastOrDefault().Trim());
        }

        public static (string Key, string Value) Get(string key) {
            var result = Bash.Execute($"{sysctlCommand} {key}").FirstOrDefault();
            return ParseSysctlLine(result);
        }

        public static void Set(string keyValue) {
            var (Key, Value) = ParseSysctlLine(keyValue);
            Set(Key, Value);
        }
        public static void Set(string key, string value) {
            Bash.Do($"{sysctlCommand} {key}=\\\"{value}\\\"");
        }

        public static void Write((string Key, string Value)[] param) {
            File.WriteAllLines(sysctlFile, param.Select(_ => $"{_.Key}={_.Value}"));
        }

        public static void Apply() {
            Bash.Do($"{sysctlCommand} -f {sysctlFile}");
        }
    }
}
