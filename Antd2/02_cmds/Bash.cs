using anthilla.core;
using System;
using System.Collections.Generic;

namespace Antd2.cmds {
    public class Bash {

        private const string bashLocation = "/bin/bash";

        public static IEnumerable<string> Execute(string command) {
            var sw = new System.Diagnostics.Stopwatch();
            sw.Start();
            if (!System.IO.File.Exists(bashLocation)) { return Array.Empty<string>(); }
            var result = CommonProcess.Execute(bashLocation, $"-c \"{command}\"");
            ConsoleLogger.Log($"{command} executed in {sw.ElapsedMilliseconds}");
            return result;
        }
        public static IEnumerable<string> Execute(string command, string dir) {
            var sw = new System.Diagnostics.Stopwatch();
            sw.Start();
            if (!System.IO.File.Exists(bashLocation)) { return Array.Empty<string>(); }
            var result = CommonProcess.Execute(bashLocation, $"-c \"{command}\"", dir);
            ConsoleLogger.Log($"{command} executed in {sw.ElapsedMilliseconds}");
            return result;
        }

        public static void Do(string command) {
            var sw = new System.Diagnostics.Stopwatch();
            sw.Start();
            if (!System.IO.File.Exists(bashLocation)) { return; }
            CommonProcess.Do(bashLocation, $"-c \"{command}\"");
            ConsoleLogger.Log($"{command} executed in {sw.ElapsedMilliseconds}");
        }

        public static void Do(string command, string dir) {
            var sw = new System.Diagnostics.Stopwatch();
            sw.Start();
            if (!System.IO.File.Exists(bashLocation)) { return; }
            CommonProcess.Do(bashLocation, $"-c \"{command}\"", dir);
            ConsoleLogger.Log($"{command} executed in {sw.ElapsedMilliseconds}");
        }
    }
}
