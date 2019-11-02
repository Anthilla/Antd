using anthilla.core;
using System;
using System.Collections.Generic;

namespace Antd2.cmds {
    public class Bash {

        private const string bashLocation = "/bin/bash";

        public static IEnumerable<string> Execute(string command) {
            if (!System.IO.File.Exists(bashLocation)) { return Array.Empty<string>(); }
            return CommonProcess.Execute(bashLocation, $"-c \"{command}\"");
        }
        public static IEnumerable<string> Execute(string command, string dir) {
            if (!System.IO.File.Exists(bashLocation)) { return Array.Empty<string>(); }
            return CommonProcess.Execute(bashLocation, $"-c \"{command}\"", dir);
        }

        public static void Do(string command) {
            if (!System.IO.File.Exists(bashLocation)) { return; }
            CommonProcess.Do(bashLocation, $"-c \"{command}\"");
        }

        public static void Do(string command, string dir) {
            if (!System.IO.File.Exists(bashLocation)) { return; }
            CommonProcess.Do(bashLocation, $"-c \"{command}\"", dir);
        }
    }
}
