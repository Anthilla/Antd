using antd.core;
using Antd2.models;
using System;
using System.Linq;

namespace Antd2.cmds {
    public class Virsh {

        private const string virshEtcDirectory = "/etc/libvirt/qemu";
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
            if (result.Length - 1 <= 0) {
                return new VirshDomainModel[0];
            }
            var list = new VirshDomainModel[result.Length - 1];
            for (var i = 0; i < result.Length - 1; i++) {
                var currentLineData = result[i].Split(new[] { ' ', '\t' }, 3, StringSplitOptions.RemoveEmptyEntries);
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

        //public static void StartAll() {
        //    var current = Application.RunningConfiguration.Services.Virsh.Domains;
        //    if(current.Length <= 0) {
        //        return;
        //    }
        //    for(var i = 0; i < current.Length; i++) {
        //        Start(current[i].Name);
        //    }
        //}

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

        //  /var/cache/libvirt
        //  /var/log/libvirt
        //  /var/lib/libvirt
        //public static void PrepareDirectory() {
        //    if(!Directory.Exists(virshEtcDirectory)) { return; }
        //    var data = Directory.EnumerateFiles(virshEtcDirectory, "*.xml", SearchOption.TopDirectoryOnly);
        //    if(data.Any()) {
        //        if(MountHelper.IsAlreadyMounted(virshEtcDirectory) == false) {
        //            Mount.AutoMountDirectory(virshEtcDirectory);
        //        }
        //    }
        //}

        /// <summary>
        /// Test di migrazione vm
        /// step 1) sync dischi
        /// step 2) virsh migrate
        ///  
        /// rsync --progress vhd001.qed root@192.168.111.102:/Data/Data01/002_NET_HWK_Debian/vhd001.qed
        /// virsh migrate 002_NET_HWK_Debian qemu+ssh://root@box02/system --live --unsafe --verbose
        /// </summary>
        public static void MigrateVm(string domain) {

        }
    }
}
