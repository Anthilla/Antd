using anthilla.core;
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
            //var sw = new System.Diagnostics.Stopwatch();
            //sw.Start();
            if (!System.IO.File.Exists(bashLocation)) { return Array.Empty<string>(); }
            var result = CommonProcess.Execute(bashLocation, $"-c \"{command}\"");
            //ConsoleLogger.Log($"{command} executed in {sw.ElapsedMilliseconds}");
            return result;
        }
        public static IEnumerable<string> Execute(string command, string dir) {
            //var sw = new System.Diagnostics.Stopwatch();
            //sw.Start();
            if (!System.IO.File.Exists(bashLocation)) { return Array.Empty<string>(); }
            var result = CommonProcess.Execute(bashLocation, $"-c \"{command}\"", dir);
            //ConsoleLogger.Log($"{command} executed in {sw.ElapsedMilliseconds}");
            return result;
        }

        public static void Do(string command) {
            //var sw = new System.Diagnostics.Stopwatch();
            //sw.Start();
            if (!System.IO.File.Exists(bashLocation)) { return; }
            CommonProcess.Do(bashLocation, $"-c \"{command}\"");
            //ConsoleLogger.Log($"{command} executed in {sw.ElapsedMilliseconds}");
        }

        public static void Do(string command, string dir) {
            //var sw = new System.Diagnostics.Stopwatch();
            //sw.Start();
            if (!System.IO.File.Exists(bashLocation)) { return; }
            CommonProcess.Do(bashLocation, $"-c \"{command}\"", dir);
            //ConsoleLogger.Log($"{command} executed in {sw.ElapsedMilliseconds}");
        }
    }
}
