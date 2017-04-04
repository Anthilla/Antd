using antdlib.common;
using antdlib.models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using IoDir = System.IO.Directory;

namespace antdlib.config {
    public class AclConfiguration {

        private readonly AclConfigurationModel _serviceModel;

        private readonly string _storeDir = $"{Parameter.AntdCfgServices}/acls";
        private readonly string _storeDirTemplate = $"{Parameter.AntdCfgServices}/acls/template";
        private readonly string _cfgFile = $"{Parameter.AntdCfgServices}/acl.conf";
        private readonly string _cfgFileBackup = $"{Parameter.AntdCfgServices}/acl.conf.bck";
        public Timer Timer { get; private set; }

        public AclConfiguration() {
            IoDir.CreateDirectory(Parameter.AntdCfgServices);
            IoDir.CreateDirectory(_storeDir);
            IoDir.CreateDirectory(_storeDirTemplate);
            if(!File.Exists(_cfgFile)) {
                _serviceModel = new AclConfigurationModel();
            }
            else {
                try {
                    var text = File.ReadAllText(_cfgFile);
                    var obj = JsonConvert.DeserializeObject<AclConfigurationModel>(text);
                    _serviceModel = obj;
                }
                catch(Exception) {
                    _serviceModel = new AclConfigurationModel();
                }

            }
        }

        public void Save(AclConfigurationModel model) {
            var text = JsonConvert.SerializeObject(model, Formatting.Indented);
            if(File.Exists(_cfgFile)) {
                File.Copy(_cfgFile, _cfgFileBackup, true);
            }
            File.WriteAllText(_cfgFile, text);
            ConsoleLogger.Log("[acl] configuration saved");
        }

        public void Set() {
            Enable();
            Stop();
            Start();
        }

        public bool IsActive() {
            if(!File.Exists(_cfgFile)) {
                return false;
            }
            return _serviceModel != null && _serviceModel.IsActive;
        }

        public AclConfigurationModel Get() {
            return _serviceModel;
        }

        public void Enable() {
            _serviceModel.IsActive = true;
            Save(_serviceModel);
            ConsoleLogger.Log("[acl] enabled");
        }

        public void Disable() {
            _serviceModel.IsActive = false;
            Save(_serviceModel);
            ConsoleLogger.Log("[acl] disabled");
        }

        public void Stop() {
            Timer?.Dispose();
            ConsoleLogger.Log("[acl] stop");
        }

        public void Start() {
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

        private readonly Bash _bash = new Bash();

        private string SetAclBackupFilePath(string directory) {
            return string.IsNullOrEmpty(directory) ? $"{_storeDir}/{Guid.NewGuid()}.acl" : $"{_storeDir}/ACL{directory.Replace("/", "_")}.acl";
        }

        public void Backup(string dir) {
            var acls = _bash.Execute($"getfacl -R {dir}").SplitBash();
            var destination = SetAclBackupFilePath(dir);
            File.WriteAllLines(destination, acls);
        }

        public void Restore() {
            var acls = IoDir.EnumerateFiles(_storeDir);
            foreach(var acl in acls) {
                Restore(acl);
            }
        }

        public string Restore(string acl) {
            return _bash.Execute($"setfacl --restore {acl}", false);
        }

        public Dictionary<string, string[]> GetTemplates() {
            var files = IoDir.EnumerateFiles(_storeDirTemplate);
            var dict = new Dictionary<string, string[]>();
            foreach(var file in files) {
                dict[file] = File.ReadAllLines(file);
            }
            return dict;
        }

        public void SaveTemplate(string name, string[] rules) {
            var file = $"{_storeDirTemplate}/{name}";
            File.WriteAllLines(file, rules);
        }

        public string[] GetDefaultTemplate() {
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

        public void AddAcl(string dir) {
            var backupFile = SetAclBackupFilePath(dir);
            File.WriteAllLines(backupFile, GetDefaultTemplate());
            var acls = _serviceModel.Settings;
            if(acls.Any(_ => _.Path == dir)) {
                return;
            }
            var model = new AclPersistentSettingModel {
                Path = dir,
                Acl = backupFile
            };
            acls.Add(model);
            _serviceModel.Settings = acls;
            Save(_serviceModel);
        }

        public void SetAcl(string guid, string[] rules) {
            var acls = _serviceModel.Settings;
            var model = acls.FirstOrDefault(_ => _.Guid == guid);
            if(model == null) {
                return;
            }
            File.WriteAllLines(model.Acl, rules);
        }

        public void RemoveAcl(string guid) {
            var acls = _serviceModel.Settings;
            var model = acls.FirstOrDefault(_ => _.Guid == guid);
            if(model == null) {
                return;
            }
            acls.Remove(model);
            _serviceModel.Settings = acls;
            Save(_serviceModel);
        }

        public string[] GetAcl(string guid) {
            var acls = _serviceModel.Settings;
            var model = acls.FirstOrDefault(_ => _.Guid == guid);
            if(model == null) {
                return new[] { "" };
            }
            var result = File.ReadAllLines(model.Acl);
            return result;
        }

        public string ApplyAcl(string guid) {
            var acls = _serviceModel.Settings;
            var model = acls.FirstOrDefault(_ => _.Guid == guid);
            return model == null ? "Error" : Restore(model.Acl);
        }

        #region [    Script    ]
        public void ScriptSetup() {
            File.Copy("/framework/antd/Resources/.010_Home_SKEL.acl", $"{Parameter.AntdCfgServices}/acls/.010_Home_SKEL.acl", true);
            File.Copy("/framework/antd/Resources/.011_Shared_SKEL.acl", $"{Parameter.AntdCfgServices}/acls/.011_Shared_SKEL.acl", true);
            File.Copy("/framework/antd/Resources/.000_define_user_acl.sh", $"{Parameter.AntdCfgServices}/acls/.000_define_user_acl.sh", true);
        }

        public void ApplyAclScript(string user) {
            var bash = new Bash();
            bash.Execute($"./.000_define_user_acl.sh {user}", $"{Parameter.AntdCfgServices}/acls");
        }
        #endregion

        public class AclAuto {

            public static void ApplyAcl(string user) {
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
            }

            public string[] HomeSkel(string value) {
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

            public string[] SharedSkel(string value) {
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
