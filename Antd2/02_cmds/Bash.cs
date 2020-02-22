using antd.core;
using System;
using System.Collections.Generic;

namespace Antd2.cmds {
    /// <summary>
    /// TODO
    /// se avvio l'applicazione, su linux, da riga di comando interattivamente
    /// i comandi ci impiegano > 2000 ms ad essere eseguiti
    /// invece, se avvio l'applicazione direttamente (es. 'dotnet antd.dll start')
    /// i comandi ci impiegano < 10 ms (per cui un tempo sensato)
    /// </summary>
    public class Bash {

        private const string bashLocation = "/bin/bash";

        public static IEnumerable<string> Execute(string command) {
            if (!System.IO.File.Exists(bashLocation)) { return Array.Empty<string>(); }
            var result = CommonProcess.Execute(bashLocation, $"-c \"{command}\"");
            return result;
        }
        public static IEnumerable<string> Execute(string command, string dir) {
            if (!System.IO.File.Exists(bashLocation)) { return Array.Empty<string>(); }
            var result = CommonProcess.Execute(bashLocation, $"-c \"{command}\"", dir);
            return result;
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
