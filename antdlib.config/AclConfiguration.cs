using antdlib.common;
using antdlib.models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

namespace antdlib.config {
    public class AclConfiguration {

        private static AclConfigurationModel ServiceModel => Load();

        private static readonly string StoreDir = $"{Parameter.AntdCfgServices}/acls";
        private static readonly string StoreDirTemplate = $"{Parameter.AntdCfgServices}/acls/template";
        private static readonly string CfgFile = $"{Parameter.AntdCfgServices}/acl.conf";
        private static readonly string CfgFileBackup = $"{Parameter.AntdCfgServices}/acl.conf.bck";
        public static Timer Timer { get; private set; }

        private static AclConfigurationModel Load() {
            if(!File.Exists(CfgFile)) {
                return new AclConfigurationModel();
            }
            try {
                var text = File.ReadAllText(CfgFile);
                var obj = JsonConvert.DeserializeObject<AclConfigurationModel>(text);
                return obj;
            }
            catch(Exception) {
                return new AclConfigurationModel();
            }
        }

        public static void Save(AclConfigurationModel model) {
            var text = JsonConvert.SerializeObject(model, Formatting.Indented);
            FileWithAcl.WriteAllText(CfgFile, text, "644", "root", "wheel");
            ConsoleLogger.Log("[acl] configuration saved");
        }

        public static void Set() {
            Enable();
            Stop();
            Start();
        }

        public static bool IsActive() {
            if(!File.Exists(CfgFile)) {
                return false;
            }
            return ServiceModel != null && ServiceModel.IsActive;
        }

        public static AclConfigurationModel Get() {
            return ServiceModel;
        }

        public static void Enable() {
            ServiceModel.IsActive = true;
            Save(ServiceModel);
            ConsoleLogger.Log("[acl] enabled");
        }

        public static void Disable() {
            ServiceModel.IsActive = false;
            Save(ServiceModel);
            ConsoleLogger.Log("[acl] disabled");
        }

        public static void Stop() {
            Timer?.Dispose();
            ConsoleLogger.Log("[acl] stop");
        }

        public static void Start() {
            ConsoleLogger.Log("[acl] start");
            var alertTime = new TimeSpan(0, 40, 0);
            var current = DateTime.Now;
            var timeToGo = alertTime - current.TimeOfDay;
            if(timeToGo < TimeSpan.Zero) {
                return;
            }
            Timer = new Timer(x => {
                Restore();
            }, null, timeToGo, Timeout.InfiniteTimeSpan);
        }

        private static string SetAclBackupFilePath(string directory) {
            return string.IsNullOrEmpty(directory) ? $"{StoreDir}/{Guid.NewGuid()}.acl" : $"{StoreDir}/ACL{directory.Replace("/", "_")}.acl";
        }

        public static void Backup(string dir) {
            var acls = Bash.Execute($"getfacl -R {dir}").SplitBash();
            var destination = SetAclBackupFilePath(dir);
            FileWithAcl.WriteAllLines(destination, acls, "644", "root", "wheel");
        }

        public static void Restore() {
            var acls = Directory.EnumerateFiles(StoreDir);
            foreach(var acl in acls) {
                Restore(acl);
            }
        }

        public static string Restore(string acl) {
            return Bash.Execute($"setfacl --restore {acl}", false);
        }

        public static Dictionary<string, string[]> GetTemplates() {
            var files = Directory.EnumerateFiles(StoreDirTemplate);
            var dict = new Dictionary<string, string[]>();
            foreach(var file in files) {
                dict[file] = File.ReadAllLines(file);
            }
            return dict;
        }

        public static void SaveTemplate(string name, string[] rules) {
            var file = $"{StoreDirTemplate}/{name}";
            FileWithAcl.WriteAllLines(file, rules, "644", "root", "wheel");
        }

        public static string[] GetDefaultTemplate() {
            return new[] {
                "# file: REPLACE_WITH_PATH",
                "# owner: root",
                "# group: root",
                "user::rwx",
                "user:root:rwx",
                "user:1018:rwx",
                "user:VALUETOREPLACE:rwx",
                "group::---",
                "group:root:---",
                "group:1018:rwx",
                "group:VALUETOREPLACE:rwx",
                "mask::rwx",
                "other::--x",
                "default:user::rwx",
                "default:user:root:rwx",
                "default:user:1018:rwx",
                "default:user:VALUETOREPLACE:rwx",
                "default:group::---",
                "default:group:root:---",
                "default:group:1018:rwx",
                "default:group:VALUETOREPLACE:rwx",
                "default:mask::rwx",
                "default:other::---"
            };
        }

        public static void AddAcl(string dir) {
            var backupFile = SetAclBackupFilePath(dir);
            FileWithAcl.WriteAllLines(backupFile, GetDefaultTemplate(), "644", "root", "wheel");
            var acls = ServiceModel.Settings;
            if(acls.Any(_ => _.Path == dir)) {
                return;
            }
            var model = new AclPersistentSettingModel {
                Path = dir,
                Acl = backupFile
            };
            acls.Add(model);
            ServiceModel.Settings = acls;
            Save(ServiceModel);
        }

        public static void SetAcl(string guid, string[] rules) {
            var acls = ServiceModel.Settings;
            var model = acls.FirstOrDefault(_ => _.Guid == guid);
            if(model == null) {
                return;
            }
            FileWithAcl.WriteAllLines(model.Acl, rules, "644", "root", "wheel");
        }

        public static void RemoveAcl(string guid) {
            var acls = ServiceModel.Settings;
            var model = acls.FirstOrDefault(_ => _.Guid == guid);
            if(model == null) {
                return;
            }
            acls.Remove(model);
            ServiceModel.Settings = acls;
            Save(ServiceModel);
        }

        public static string[] GetAcl(string guid) {
            var acls = ServiceModel.Settings;
            var model = acls.FirstOrDefault(_ => _.Guid == guid);
            if(model == null) {
                return new[] { "" };
            }
            var result = File.ReadAllLines(model.Acl);
            return result;
        }

        public static string ApplyAcl(string guid) {
            var acls = ServiceModel.Settings;
            var model = acls.FirstOrDefault(_ => _.Guid == guid);
            return model == null ? "Error" : Restore(model.Acl);
        }

        #region [    Script    ]
        public static void ScriptSetup() {
            const string file1 = "/framework/antd/Resources/.010_Home_SKEL.acl";
            if(File.Exists(file1)) {
                File.Copy(file1, $"{Parameter.AntdCfgServices}/acls/.010_Home_SKEL.acl", true);
            }

            const string file2 = "/framework/antd/Resources/.011_Shared_SKEL.acl";
            if(File.Exists(file2)) {
                File.Copy(file2, $"{Parameter.AntdCfgServices}/acls/.011_Shared_SKEL.acl", true);
            }

            const string file3 = "/framework/antd/Resources/.000_define_user_acl.sh";
            if(File.Exists(file3)) {
                File.Copy(file3, $"{Parameter.AntdCfgServices}/acls/.000_define_user_acl.sh", true);
            }
        }

        public static void ApplyAclScript(string user) {
            const string file3 = "/framework/antd/Resources/.000_define_user_acl.sh";
            if(!File.Exists(file3))
                return;
            Bash.Execute($"./.000_define_user_acl.sh {user}", $"{Parameter.AntdCfgServices}/acls");
        }
        #endregion

        public static class AclAuto {

            //public static void ApplyAcl(string user) {
                //IFS=$'\n'
                //T=$'\t'

                //USERTOGET=$1
                //#echo $USERTOGET

                //### VARS START ###
                //USERHOMEPATH=/Data/UserData/Home
                //USERRELHOMEPATH=../Home
                //USERSCAMBIOPATH=/Data/UserData/Scambio
                //SELECTEDNAME=$USERTOGET
                //### VARS STOP ###

                //COMMAND=`wbinfo -u|grep $USERTOGET`
                //#echo $COMMAND
                //for OUTPUT in $COMMAND
                //do
                //#echo $OUTPUT
                //CYCLEONE=`wbinfo -n $OUTPUT |awk '{print $1}'`
                //        for OUTPUT in $CYCLEONE
                //        do
                //        #echo $OUTPUT
                //        CYCLETWO=`wbinfo -S $CYCLEONE |awk '{print $1}'`
                //        SELECTEDUID=$CYCLETWO
                //        echo $CYCLETWO
                //        done
                //done

                //### VARS RECAP START###
                //echo USERHOMEPATH=$USERHOMEPATH
                //echo USERRELHOMEPATH=$USERRELHOMEPATH
                //echo USERSCAMBIOPATH=$USERSCAMBIOPATH
                //echo SELECTEDNAME=$SELECTEDNAME
                //echo SELECTEDUID=$SELECTEDUID
                //### VARS RECAP STOP ###

                //### PRE EXEC START ###
                //echo mkdir -p $USERHOMEPATH/$SELECTEDNAME/Shared
                //echo mkdir -p $USERHOMEPATH/$SELECTEDNAME/Saved\ Games
                //echo mkdir -p $USERHOMEPATH/$SELECTEDNAME/Music
                //echo mkdir -p $USERHOMEPATH/$SELECTEDNAME/Links
                //echo mkdir -p $USERHOMEPATH/$SELECTEDNAME/Start\ Menu
                //echo mkdir -p $USERHOMEPATH/$SELECTEDNAME/Desktop
                //echo mkdir -p $USERHOMEPATH/$SELECTEDNAME/My\ Pictures
                //echo mkdir -p $USERHOMEPATH/$SELECTEDNAME/Contacts
                //echo mkdir -p $USERHOMEPATH/$SELECTEDNAME/My\ Videos
                //echo mkdir -p $USERHOMEPATH/$SELECTEDNAME/My\ Music
                //echo mkdir -p $USERHOMEPATH/$SELECTEDNAME/Searches
                //echo mkdir -p $USERHOMEPATH/$SELECTEDNAME/My\ Documents
                //echo mkdir -p $USERHOMEPATH/$SELECTEDNAME/Videos
                //echo mkdir -p $USERHOMEPATH/$SELECTEDNAME/Favorites
                //echo cp -f .010_Home_SKEL.acl 010_Home_$SELECTEDNAME.acl
                //echo cp -f .011_Shared_SKEL.acl 011_Shared_$SELECTEDNAME.acl
                //echo replace VALUETOREPLACE $SELECTEDUID -- 010_Home_$SELECTEDNAME.acl
                //echo replace VALUETOREPLACE $SELECTEDUID -- 011_Shared_$SELECTEDNAME.acl
                //echo setfacl --set-file=010_Home_$SELECTEDNAME.acl -R /Data/UserData/Home/$SELECTEDNAME
                //echo setfacl --set-file=011_Shared_$SELECTEDNAME.acl -R /Data/UserData/Home/$SELECTEDNAME/Shared
                //echo ln -s $USERRELHOMEPATH/$SELECTEDNAME/Shared $USERSCAMBIOPATH/$SELECTEDNAME"_Shared"
                //### PRE EXEC STOP ###
                //### EXEC START ###
                //mkdir -p $USERHOMEPATH/$SELECTEDNAME/Shared
                //mkdir -p $USERHOMEPATH/$SELECTEDNAME/Saved\ Games
                //mkdir -p $USERHOMEPATH/$SELECTEDNAME/Music
                //mkdir -p $USERHOMEPATH/$SELECTEDNAME/Links
                //mkdir -p $USERHOMEPATH/$SELECTEDNAME/Start\ Menu
                //mkdir -p $USERHOMEPATH/$SELECTEDNAME/Desktop
                //mkdir -p $USERHOMEPATH/$SELECTEDNAME/My\ Pictures
                //mkdir -p $USERHOMEPATH/$SELECTEDNAME/Contacts
                //mkdir -p $USERHOMEPATH/$SELECTEDNAME/My\ Videos
                //mkdir -p $USERHOMEPATH/$SELECTEDNAME/My\ Music
                //mkdir -p $USERHOMEPATH/$SELECTEDNAME/Searches
                //mkdir -p $USERHOMEPATH/$SELECTEDNAME/My\ Documents
                //mkdir -p $USERHOMEPATH/$SELECTEDNAME/Videos
                //mkdir -p $USERHOMEPATH/$SELECTEDNAME/Favorites
                //cp -f .010_Home_SKEL.acl 010_Home_$SELECTEDNAME.acl
                //cp -f .011_Shared_SKEL.acl 011_Shared_$SELECTEDNAME.acl
                //replace VALUETOREPLACE $SELECTEDUID -- 010_Home_$SELECTEDNAME.acl
                //replace VALUETOREPLACE $SELECTEDUID -- 011_Shared_$SELECTEDNAME.acl
                //setfacl --set-file=010_Home_$SELECTEDNAME.acl -R /Data/UserData/Home/$SELECTEDNAME
                //setfacl --set-file=011_Shared_$SELECTEDNAME.acl -R /Data/UserData/Home/$SELECTEDNAME/Shared
                //ln -s $USERRELHOMEPATH/$SELECTEDNAME/Shared $USERSCAMBIOPATH/$SELECTEDNAME"_Shared"
                //### FOR EMPTY CLEANUP ###
                //rmdir $USERHOMEPATH/* 2> /dev/null
                //### EXEC STOP ###

                //#STARTPATH="/Data/UserData/Home/"
                //#wbinfo -n m.zafferana
                //#wbinfo -S S-1-5-21-1191849564-1695385468-1789397799-1773
            //}

            public static string[] HomeSkel(string value) {
                return new[] {
                "# file: Data/UserData/Home/user",
                "# owner: root",
                "# group: root",
                "user::rwx",
                "user:root:rwx",
                "user:1018:rwx",
                "user:domain_admins:rwx",
                $"user:{value}:rwx",
                "group::---",
                "group:root:---",
                "group:1018:rwx",
                "group:domain_admins:rwx",
                $"group:{value}:rwx",
                "mask::rwx",
                "other::--x",
                "default:user::rwx",
                "default:user:root:rwx",
                "default:user:1018:rwx",
                "default:user:domain_admins:rwx",
                $"default:user:{value}:rwx",
                "default:group::---",
                "default:group:root:---",
                "default:group:1018:rwx",
                "default:group:domain_admins:rwx",
                $"default:group:{value}:rwx",
                "default:mask::rwx",
                "default:other::---"
            };
            }

            public static string[] SharedSkel(string value) {
                return new[] {
                "# file: Data/UserData/Home/user/Shared",
                "# owner: root",
                "# group: root",
                "user::rwx",
                "user:root:rwx",
                "user:1018:rwx",
                "user:domain_admins:rwx",
                "user:operativi:r-x",
                $"user:{value}:rwx",
                "group::---",
                "group:root:---",
                "group:1018:rwx",
                "group:domain_admins:rwx",
                "group:operativi:r-x",
                $"group:{value}:rwx",
                "mask::rwx",
                "other::---",
                "default:user::rwx",
                "default:user:root:rwx",
                "default:user:1018:rwx",
                "default:user:domain_admins:rwx",
                "default:user:operativi:r-x",
                $"default:user:{value}:rwx",
                "default:group::---",
                "default:group:root:---",
                "default:group:1018:rwx",
                "default:group:domain_admins:rwx",
                "default:group:operativi:r-x",
                $"default:group:{value}:rwx",
                "default:mask::rwx",
                "default:other::---"
            };
            }
        }
    }
}
