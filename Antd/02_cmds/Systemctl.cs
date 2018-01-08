using Antd.models;
using anthilla.core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Antd.cmds {

    public class Systemctl {

        private const string systemctlFileLocation = "/bin/systemctl";
        private const string daemonReloadArg = "daemon-reload";
        private const string startArg = "start";
        private const string stopArg = "stop";
        private const string restartArg = "restart";
        private const string reloadArg = "reload";
        private const string enableArg = "enable";
        private const string disableArg = "disable";
        private const string maskArg = "mask";
        private const string statusArg = "status";
        private const string isActiveArg = "is-active";
        private const string isEnabledArg = "is-enabled";

        public static SystemService[] Get(SystemctlType type = SystemctlType.none) {
            string filter = "";
            switch(type) {
                case SystemctlType.none:
                    break;
                case SystemctlType.Service:
                    filter = "--type=service";
                    break;
                case SystemctlType.Mount:
                    filter = "--type=mount";
                    break;
                case SystemctlType.Timer:
                    filter = "--type=timer";
                    break;
                case SystemctlType.Target:
                    filter = "--type=target";
                    break;
                default:
                    break;
            }

            var args = CommonString.Append("--no-pager --no-legend ", filter);
            var result = CommonProcess.Execute(systemctlFileLocation, args).ToArray();
            var status = new SystemService[result.Length];
            for(var i = 0; i < result.Length; i++) {
                var currentLine = result[i];
                var currentLineData = currentLine.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                status[i] = new SystemService() {
                    Type = type,
                    Service = currentLineData[0],
                    Active = IsEnabled(currentLineData[0]) ? true : false,
                    Start = IsActive(currentLineData[0]) ? true : false,
                    ForceRestart = false,
                    Masking = false
                };
            }
            return status;
        }

        public static SystemService[] GetServices() {
            return Get(SystemctlType.Service);
        }

        public static SystemService[] GetMounts() {
            return Get(SystemctlType.Mount);
        }

        public static SystemService[] GetTimers() {
            return Get(SystemctlType.Timer);
        }

        public static SystemService[] GetTargets() {
            return Get(SystemctlType.Target);
        }

        public static SystemService[] GetAll() {
            return GetServices();
            //return GetServices().Concat(GetMounts()).Concat(GetTimers()).Concat(GetTargets()).ToArray();
        }

        public static bool Set() {
            var current = Application.CurrentConfiguration.Boot.Services;
            for(var i = 0; i < current.Length; i++) {
                var currentService = current[i];
                if(currentService.Masking == true) {
                    Stop(currentService.Service);
                    Disable(currentService.Service);
                    Mask(currentService.Service);
                    continue;
                }
                if(currentService.Active == true) {
                    Enable(currentService.Service);
                }
                else if(currentService.Active == false) {
                    Disable(currentService.Service);
                }
                if(currentService.Start == true) {
                    if(currentService.ForceRestart == true) {
                        Restart(currentService.Service);
                    }
                    else {
                        Start(currentService.Service);
                    }
                }
                else if(currentService.Start == false) {
                    Stop(currentService.Service);

                }
            }
            return true;
        }

        public static bool DaemonReload() {
            CommonProcess.Do(systemctlFileLocation, daemonReloadArg);
            return true;
        }

        public static bool Start(string unit) {
            var args = CommonString.Append(startArg, " ", unit);
            CommonProcess.Do(systemctlFileLocation, args);
            ConsoleLogger.Log($"[systemctl] start {unit}");
            return true;
        }

        public static bool Stop(string unit) {
            var args = CommonString.Append(stopArg, " ", unit);
            CommonProcess.Do(systemctlFileLocation, args);
            ConsoleLogger.Log($"[systemctl] stop {unit}");
            return true;
        }

        public static bool Restart(string unit) {
            var args = CommonString.Append(restartArg, " ", unit);
            CommonProcess.Do(systemctlFileLocation, args);
            ConsoleLogger.Log($"[systemctl] restart {unit}");
            return true;
        }

        public static bool Reload(string unit) {
            var args = CommonString.Append(reloadArg, " ", unit);
            CommonProcess.Do(systemctlFileLocation, args);
            return true;
        }

        public static IEnumerable<string> Status(string unit) {
            var args = CommonString.Append(statusArg, " ", unit);
            return CommonProcess.Execute(systemctlFileLocation, args);
        }

        public static bool IsActive(string unit) {
            var args = CommonString.Append(isActiveArg, " ", unit);
            var status = CommonProcess.Execute(systemctlFileLocation, args);
            if(status == null) {
                return false;
            }
            if(string.IsNullOrEmpty(status.FirstOrDefault())) {
                return false;
            }
            return !status.FirstOrDefault().Contains("inactive");
        }

        public static bool IsEnabled(string unit) {
            var args = CommonString.Append(isEnabledArg, " ", unit);
            var status = CommonProcess.Execute(systemctlFileLocation, args);
            if(status == null) {
                return false;
            }
            if(string.IsNullOrEmpty(status.FirstOrDefault())) {
                return false;
            }
            return !status.FirstOrDefault().Contains("disabled");
        }

        public static bool Enable(string unit) {
            var args = CommonString.Append(enableArg, " ", unit);
            CommonProcess.Do(systemctlFileLocation, args);
            ConsoleLogger.Log($"[systemctl] enable {unit}");
            return true;
        }

        public static bool Disable(string unit) {
            var args = CommonString.Append(disableArg, " ", unit);
            CommonProcess.Do(systemctlFileLocation, args);
            ConsoleLogger.Log($"[systemctl] disable {unit}");
            return true;
        }

        public static bool Mask(string unit) {
            var args = CommonString.Append(maskArg, " ", unit);
            CommonProcess.Do(systemctlFileLocation, args);
            ConsoleLogger.Log($"[systemctl] mask {unit}");
            return true;
        }

        public static string[] GetList() {
            var args = CommonString.Append("--no-pager --no-legend list-units");
            var result = CommonProcess.Execute(systemctlFileLocation, args).Where(_ => !_.Contains("device")).ToArray();
            var status = new string[result.Length];
            for(var i = 0; i < result.Length; i++) {
                var currentLineData = result[i].Split(new[] { ' ' }, 2, StringSplitOptions.RemoveEmptyEntries);
                status[i] = currentLineData[0];
            }
            return status;
        }
    }
}
