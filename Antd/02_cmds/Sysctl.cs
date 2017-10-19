using anthilla.core;
using System;
using System.IO;
using System.Linq;

namespace Antd.cmds {

    public class Sysctl {

        private const string sysctlFileLocation = "/usr/sbin/sysctl";
        private const string sysctlConfFile = "/cfg/antd/conf/sysctl.conf";
        private const string sysctlEtcConfFile = "/etc/sysctl-default.conf";

        /// <summary>
        /// Controllo la configurazione salvata e controllo, per ogni Key in Boot.Parameters, il valore effettivo
        /// </summary>
        /// <returns></returns>
        public static SystemParameter[] Get() {
            var current = Application.CurrentConfiguration.Boot.Parameters;
            var running = new SystemParameter[current.Length];
            var all = GetAll();
            for(var i = 0; i < current.Length; i++) {
                var param = current[i];
                var value = all.FirstOrDefault(_ => _.Key == param.Key) == null ? string.Empty : all.FirstOrDefault(_ => _.Key == param.Key).Value;
                running[i] = new SystemParameter() {
                    Key = param.Key,
                    Value = value
                };
            }
            Array.Sort(running, (a, b) => (a.Key.CompareTo(b.Key)));
            return running;
        }

        /// <summary>
        /// Ottiene tutte le coppie Key-Value da Sysctl
        /// </summary>
        /// <returns></returns>
        public static SystemParameter[] GetAll() {
            var args = CommonString.Append("--all");
            var result = CommonProcess.Execute(sysctlFileLocation).ToArray();
            var status = new SystemParameter[result.Length];
            for(var i = 0; i < result.Length; i++) {
                var currentLine = result[i];
                var currentLineData = currentLine.Split(new[] { '=' }, StringSplitOptions.RemoveEmptyEntries);
                status[i] = new SystemParameter() {
                    Key = GetKeyFilePath(currentLineData[0].Trim()),
                    Value = currentLineData[1].Trim()
                };
            }
            Array.Sort(status, (a, b) => (a.Key.CompareTo(b.Key)));
            return status;
        }

        /// <summary>
        /// Prende la configurazione salvata
        /// Prende tutte le coppie Key-Value da Sysctl -> GetAll()
        /// Importa da GetAll tutte le coppie mancanti
        /// </summary>
        public static void ImportKeys() {
            throw new NotImplementedException();
        }

        public static bool Set() {
            var current = Application.CurrentConfiguration.Boot.Parameters;
            for(var i = 0; i < current.Length; i++) {
                var currentParameter = current[i];
                Apply(currentParameter.Key, currentParameter.Value);
            }
            return true;
        }

        public static void Apply(string key, string value) {
            if(!File.Exists(key)) {
                return;
            }
            var runningValue = File.ReadAllText(key).Trim();
            if(CommonString.AreEquals(value.Trim(), runningValue) == true) {
                return;
            }
            ConsoleLogger.Log($"[sysctl] {key} = {value}");
            File.WriteAllText(key, value);
        }

        private const string keyFileFolder = "/proc/sys";

        private static string GetKeyFilePath(string key) {
            return CommonString.Append(keyFileFolder, "/", key.Replace(".", "/"));
        }

        public static void SaveDefaultValues() {
            if(File.Exists(sysctlEtcConfFile)) {
                return;
            }
            CommonProcess.Do(sysctlFileLocation, "--system");
            var result = CommonProcess.Execute(sysctlFileLocation, "-a");
            File.WriteAllLines(sysctlEtcConfFile, result);
            File.Copy(sysctlEtcConfFile, sysctlConfFile, true);
            ConsoleLogger.Log("[sysctl] save default values");
        }

        public static void RestoreDefaultValues() {
            if(!File.Exists(sysctlConfFile)) {
                return;
            }
            var arg = CommonString.Append("-p ", sysctlConfFile);
            CommonProcess.Do(sysctlFileLocation, arg);
            ConsoleLogger.Log("[sysctl] restore default values");
        }
    }
}
