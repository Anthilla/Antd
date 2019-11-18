using Antd2.cmds;
using Antd2.Configuration;
using System;
using System.Collections.Generic;

namespace Antd2 {
    public class SysctlCommand {
        public static readonly Dictionary<string, Action<string[]>> Options =
            new Dictionary<string, Action<string[]>> {
                { "check", CheckFunc },
                { "set", SetFunc },
            };

        public static void CheckFunc(string[] args) {
            foreach (var sysctl in ConfigManager.Config.Saved.Boot.Sysctl) {
                var parsed = Sysctl.ParseSysctlLine(sysctl);
                var running = Sysctl.Get(parsed.Key);
                var isConfigured = Help.RemoveWhiteSpace(running.Value) == Help.RemoveWhiteSpace(parsed.Value);
                if (isConfigured) {
                    CheckFunc_PrintInstalled(parsed.Key);
                }
                else {
                    CheckFunc_PrintNotInstalled(parsed.Key);
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

        public static void SetFunc(string[] args) {
            foreach (var sysctl in ConfigManager.Config.Saved.Boot.Sysctl) {
                Sysctl.Set(sysctl);
            }
        }
    }
}