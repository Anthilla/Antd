using Antd2.cmds;
using anthilla.core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Antd2 {
    public class NsswitchCommand {
        public static readonly Dictionary<string, Action<string[]>> Options =
            new Dictionary<string, Action<string[]>> {
                { "check", CheckFunc },
                { "apply", ApplyFunc },
            };

        private static (string Key, string Value)[] RequiredSysctl = new (string Key, string Value)[] {
            ("passwd", "files winbind"),
            ("group", "files winbind"),
            ("shadow", "compat"),
            ("hosts", "files mdns_minimal [NOTFOUND=return] resolve dns"),
            ("networks", "files dns"),
            ("services", "db files"),
            ("protocols", "db files"),
            ("rpc", "db files"),
            ("ethers", "db files"),
            ("netmasks", "files"),
            ("netgroup", "files"),
            ("bootparams", "files"),
            ("automount", "files"),
            ("aliases", "files"),
        };

        public static void CheckFunc(string[] args) {
            var currentNsswitch = Nsswitch.Get();
            foreach (var nsswitch in RequiredSysctl) {
                var current = currentNsswitch.FirstOrDefault(_ => _.Key == nsswitch.Key);
                var isConfigured = current.Value == nsswitch.Value;
                if (isConfigured) {
                    CheckFunc_PrintInstalled(nsswitch.Key);
                }
                else {
                    CheckFunc_PrintNotInstalled(nsswitch.Key);
                }
            }
        }

        private static void CheckFunc_PrintInstalled(string param) {
            Console.Write($"  {param}: ");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("configured");
            Console.ForegroundColor = ConsoleColor.White;
        }

        private static void CheckFunc_PrintNotInstalled(string param) {
            Console.Write($"  {param}: ");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("not configured");
            Console.ForegroundColor = ConsoleColor.White;
        }

        public static void ApplyFunc(string[] args) {
            Console.WriteLine("  Write nsswitch file");
            Nsswitch.Write(RequiredSysctl);
        }
    }
}