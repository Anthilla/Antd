using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using antdlib.common;
using antdlib.models;
using Newtonsoft.Json;
using IoDir = System.IO.Directory;

namespace Antd.Acl {
    public class AclConfiguration {

        private readonly AclConfigurationModel _serviceModel;

        private readonly string _storeDir = $"{Parameter.AntdCfgServices}/acls";
        private readonly string _storeDirTemplate = $"{Parameter.AntdCfgServices}/acls/template";
        private readonly string _cfgFile = $"{Parameter.AntdCfgServices}/acl.conf";
        private readonly string _cfgFileBackup = $"{Parameter.AntdCfgServices}/acl.conf.bck";
        public System.Threading.Timer Timer { get; private set; }

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
            Timer = new System.Threading.Timer(x => {
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
    }
}
