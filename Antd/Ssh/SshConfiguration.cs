using System;
using System.Collections.Generic;
using System.IO;
using antdlib.common;
using antdlib.Systemd;
using Newtonsoft.Json;
using IoDir = System.IO.Directory;

namespace Antd.Ssh {
    public class SshConfiguration {

        private readonly SshConfigurationModel _serviceModel;

        private readonly string _cfgFile = $"{Parameter.AntdCfgServices}/ssh.conf";
        private readonly string _cfgFileBackup = $"{Parameter.AntdCfgServices}/ssh.conf.bck";
        private const string ServiceName = "named.service";
        private const string MainFilePath = "/etc/ssh/ssss.conf";
        private const string MainFilePathBackup = "/etc/ssh/.ssss.conf";

        public SshConfiguration() {
            IoDir.CreateDirectory(Parameter.AntdCfgServices);
            if(!File.Exists(_cfgFile)) {
                _serviceModel = new SshConfigurationModel();
            }
            else {
                try {
                    var text = File.ReadAllText(_cfgFile);
                    var obj = JsonConvert.DeserializeObject<SshConfigurationModel>(text);
                    _serviceModel = obj;
                }
                catch(Exception) {
                    _serviceModel = new SshConfigurationModel();
                }

            }
        }

        public void Save(SshConfigurationModel model) {
            var text = JsonConvert.SerializeObject(model, Formatting.Indented);
            if(File.Exists(_cfgFile)) {
                File.Copy(_cfgFile, _cfgFileBackup, true);
            }
            File.WriteAllText(_cfgFile, text);
            ConsoleLogger.Log("[ssh] configuration saved");
        }

        public void Set() {
            Enable();
            Stop();
            #region [    named.conf generation    ]
            if(File.Exists(MainFilePath)) {
                if(File.Exists(MainFilePathBackup)) {
                    File.Delete(MainFilePathBackup);
                }
                File.Copy(MainFilePath, MainFilePathBackup);
            }
            var lines = new List<string> {
                "options {"
            };
            File.WriteAllLines(MainFilePath, lines);
            #endregion
            Start();
        }

        public bool IsActive() {
            if(!File.Exists(_cfgFile)) {
                return false;
            }
            return _serviceModel != null && _serviceModel.IsActive;
        }

        public SshConfigurationModel Get() {
            return _serviceModel;
        }

        public void Enable() {
            _serviceModel.IsActive = true;
            Save(_serviceModel);
            ConsoleLogger.Log("[ssh] enabled");
        }

        public void Disable() {
            _serviceModel.IsActive = false;
            Save(_serviceModel);
            ConsoleLogger.Log("[ssh] disabled");
        }

        public void Stop() {
            Systemctl.Stop(ServiceName);
            ConsoleLogger.Log("[ssh] stop");
        }

        public void Start() {
            if(Systemctl.IsEnabled(ServiceName) == false) {
                Systemctl.Enable(ServiceName);
            }
            if(Systemctl.IsActive(ServiceName) == false) {
                Systemctl.Restart(ServiceName);
            }
            ConsoleLogger.Log("[ssh] start");
        }
    }
}
