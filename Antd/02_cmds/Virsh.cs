using Antd.models;
using anthilla.core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Antd.cmds {
    public class Virsh {
        private const string virshFileLocation = "/usr/bin/virsh";
        private const string destroyArg = "destroy";
        private const string rebootArg = "reboot";
        private const string resetArg = "reset";
        private const string restoreArg = "restore";
        private const string resumeArg = "resume";
        private const string shutdownArg = "shutdown";
        private const string startArg = "start";
        private const string suspendArg = "suspend";
        private const string dompmsuspendArg = "dompmsuspend";
        private const string dompmwakeupArg = "dompmwakeup";
        private const string listArg = "list --all";
        private const string dominfoArg = "dominfo";

        public static VirshDomainModel[] GetDomains() {
            var result = CommonProcess.Execute(virshFileLocation, listArg).Skip(2).ToArray();
            var list = new VirshDomainModel[result.Length - 1];
            for(var i = 0; i < result.Length - 1; i++) {
                var currentLineData = result[i].Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
                list[i] = new VirshDomainModel() {
                    Id = currentLineData[0],
                    Name = currentLineData[1],
                    State = currentLineData[2]
                };
            }
            return list;
        }

        public static VirshDomainModel GetDomainInfo(string domain) {
            //todo map to model
            var arg = CommonString.Append(dominfoArg, " ", domain);
            var result = CommonProcess.Execute(virshFileLocation, arg).Skip(2).ToArray();
            var info = new VirshDomainModel();
            return info;
        }

        public static bool Destroy(string domain) {
            var args = CommonString.Append(destroyArg, " ", domain);
            CommonProcess.Do(virshFileLocation, args);
            return true;
        }

        public static bool Reboot(string domain) {
            var args = CommonString.Append(rebootArg, " ", domain);
            CommonProcess.Do(virshFileLocation, args);
            return true;
        }

        public static bool Reset(string domain) {
            var args = CommonString.Append(resetArg, " ", domain);
            CommonProcess.Do(virshFileLocation, args);
            return true;
        }

        public static bool Restore(string domain) {
            var args = CommonString.Append(restoreArg, " ", domain);
            CommonProcess.Do(virshFileLocation, args);
            return true;
        }

        public static bool Resume(string domain) {
            var args = CommonString.Append(resumeArg, " ", domain);
            CommonProcess.Do(virshFileLocation, args);
            return true;
        }

        public static bool Shutdown(string domain) {
            var args = CommonString.Append(shutdownArg, " ", domain);
            CommonProcess.Do(virshFileLocation, args);
            return true;
        }

        public static bool Start(string domain) {
            var args = CommonString.Append(startArg, " ", domain);
            CommonProcess.Do(virshFileLocation, args);
            return true;
        }

        public static bool Suspend(string domain) {
            var args = CommonString.Append(suspendArg, " ", domain);
            CommonProcess.Do(virshFileLocation, args);
            return true;
        }

        public static bool Dompmsuspend(string domain) {
            var args = CommonString.Append(dompmsuspendArg, " ", domain);
            CommonProcess.Do(virshFileLocation, args);
            return true;
        }

        public static bool Dompmwakeup(string domain) {
            var args = CommonString.Append(dompmwakeupArg, " ", domain);
            CommonProcess.Do(virshFileLocation, args);
            return true;
        }
    }
}
