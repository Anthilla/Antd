using antd.core;
using Antd2.Configuration;
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

        public static void ApplyFile() {
            Bash.Do($"{sysctlCommand} -f {sysctlFile}");
        }

        public static void Apply(BootParameters boot) {
            if (boot is null)
                throw new ArgumentNullException(nameof(boot));

            if (boot.UseSysctlFile && File.Exists(boot.SysctlFile)) {
                var lines = File.ReadAllLines(boot.SysctlFile);

                foreach (var line in lines) {
                    var (Key, Value) = ParseSysctlLine(line);

                    if (Value.EndsWith(" off")) {
                        //var offValue = d.Value.TrimEnd(" off").Trim();

                    }
                    else {
                        var onValue = Value.EndsWith(" on") ? Value.TrimEnd(" on").Trim() : Value.Trim();
                        Set(Key, onValue);
                    }
                }

            }
            else {
                foreach (var sysctl in boot.Sysctl) {
                    Sysctl.Set(sysctl);
                }
            }

        }

    }
}
