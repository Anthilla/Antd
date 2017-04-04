using antdlib.common;
using antdlib.common.Helpers;
using antdlib.models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using IoDir = System.IO.Directory;

namespace antdlib.config {
    public class JournaldConfiguration {

        private readonly JournaldConfigurationModel _serviceModel;

        private readonly string _cfgFile = $"{Parameter.AntdCfgServices}/journald.conf";
        private readonly string _cfgFileBackup = $"{Parameter.AntdCfgServices}/journald.conf.bck";
        private const string ServiceName = "systemd-journald.service";
        private const string MainFilePath = "/etc/systemd/journald.conf";
        private const string MainFilePathBackup = "/etc/systemd/.journald.conf";
        private const string DirsDirectoryPath = "/mnt/cdrom/DIRS/FILE_etc_systemd_journald.conf";

        public JournaldConfiguration() {
            IoDir.CreateDirectory(Parameter.AntdCfgServices);
            if(!File.Exists(_cfgFile)) {
                _serviceModel = new JournaldConfigurationModel();
            }
            else {
                try {
                    var text = File.ReadAllText(_cfgFile);
                    var obj = JsonConvert.DeserializeObject<JournaldConfigurationModel>(text);
                    _serviceModel = obj;
                }
                catch(Exception) {
                    _serviceModel = new JournaldConfigurationModel();
                }

            }
        }

        public void Save(JournaldConfigurationModel model) {
            var text = JsonConvert.SerializeObject(model, Formatting.Indented);
            if(File.Exists(_cfgFile)) {
                File.Copy(_cfgFile, _cfgFileBackup, true);
            }
            File.WriteAllText(_cfgFile, text);
            ConsoleLogger.Log("[journald] configuration saved");
        }

        public void Set() {
            Enable();
            Stop();

            if(File.Exists(MainFilePath)) {
                if(File.Exists(MainFilePathBackup)) {
                    File.Delete(MainFilePathBackup);
                }
                File.Copy(MainFilePath, MainFilePathBackup);
            }

            #region [    fake journald.conf generation    ]
            var fakeOptions = _serviceModel;
            fakeOptions.Storage = "volatile";
            var fakeLines = new List<string> {
                "[Journal]",
                fakeOptions.Storage == "#" ? "#Storage=" : $"Storage={fakeOptions.Storage}",
                fakeOptions.Compress == "#" ? "#Compress=" : $"Compress={fakeOptions.Compress}",
                fakeOptions.Seal == "#" ? "#Seal=" : $"Seal={fakeOptions.Seal}",
                fakeOptions.SplitMode == "#" ? "#SplitMode=" : $"SplitMode={fakeOptions.SplitMode}",
                fakeOptions.SyncIntervalSec == "#"
                    ? "#SyncIntervalSec="
                    : $"SyncIntervalSec={fakeOptions.SyncIntervalSec}",
                fakeOptions.RateLimitInterval == "#"
                    ? "#RateLimitInterval="
                    : $"RateLimitInterval={fakeOptions.RateLimitInterval}",
                fakeOptions.RateLimitBurst == "#" ? "#RateLimitBurst=" : $"RateLimitBurst={fakeOptions.RateLimitBurst}",
                fakeOptions.SystemMaxUse == "#" ? "#SystemMaxUse=" : $"SystemMaxUse={fakeOptions.SystemMaxUse}",
                fakeOptions.SystemKeepFree == "#" ? "#SystemKeepFree=" : $"SystemKeepFree={fakeOptions.SystemKeepFree}",
                fakeOptions.SystemMaxFileSize == "#"
                    ? "#SystemMaxFileSize="
                    : $"SystemMaxFileSize={fakeOptions.SystemMaxFileSize}",
                fakeOptions.RuntimeMaxUse == "#" ? "#RuntimeMaxUse=" : $"RuntimeMaxUse={fakeOptions.RuntimeMaxUse}",
                fakeOptions.RuntimeKeepFree == "#"
                    ? "#RuntimeKeepFree="
                    : $"RuntimeKeepFree={fakeOptions.RuntimeKeepFree}",
                fakeOptions.RuntimeMaxFileSize == "#"
                    ? "#RuntimeMaxFileSize="
                    : $"RuntimeMaxFileSize={fakeOptions.RuntimeMaxFileSize}",
                fakeOptions.MaxRetentionSec == "#"
                    ? "#MaxRetentionSec="
                    : $"MaxRetentionSec={fakeOptions.MaxRetentionSec}",
                fakeOptions.MaxFileSec == "#" ? "#MaxFileSec=" : $"MaxFileSec={fakeOptions.MaxFileSec}",
                fakeOptions.ForwardToSyslog == "#"
                    ? "#ForwardToSyslog="
                    : $"ForwardToSyslog={fakeOptions.ForwardToSyslog}",
                fakeOptions.ForwardToKMsg == "#" ? "#ForwardToKMsg=" : $"ForwardToKMsg={fakeOptions.ForwardToKMsg}",
                fakeOptions.ForwardToConsole == "#"
                    ? "#ForwardToConsole="
                    : $"ForwardToConsole={fakeOptions.ForwardToConsole}",
                fakeOptions.ForwardToWall == "#" ? "#ForwardToWall=" : $"ForwardToWall={fakeOptions.ForwardToWall}",
                fakeOptions.TtyPath == "#" ? "#TTYPath=" : $"TTYPath={fakeOptions.TtyPath}",
                fakeOptions.MaxLevelStore == "#" ? "#MaxLevelStore=" : $"MaxLevelStore={fakeOptions.MaxLevelStore}",
                fakeOptions.MaxLevelSyslog == "#" ? "#MaxLevelSyslog=" : $"MaxLevelSyslog={fakeOptions.MaxLevelSyslog}",
                fakeOptions.MaxLevelKMsg == "#" ? "#MaxLevelKMsg=" : $"MaxLevelKMsg={fakeOptions.MaxLevelKMsg}",
                fakeOptions.MaxLevelConsole == "#"
                    ? "#MaxLevelConsole="
                    : $"MaxLevelConsole={fakeOptions.MaxLevelConsole}",
                fakeOptions.MaxLevelWall == "#" ? "#MaxLevelWall=" : $"MaxLevelWall={fakeOptions.MaxLevelWall}"
            };
            File.WriteAllLines(MainFilePath, fakeLines);
            #endregion
            Start();
            Remount();
            #region [    journald.conf generation    ]
            var lines = new List<string> {
                "[Journal]"
            };
            var options = _serviceModel;
            lines.Add(options.Storage == "#" ? "#Storage=" : $"Storage={options.Storage}");
            lines.Add(options.Compress == "#" ? "#Compress=" : $"Compress={options.Compress}");
            lines.Add(options.Seal == "#" ? "#Seal=" : $"Seal={options.Seal}");
            lines.Add(options.SplitMode == "#" ? "#SplitMode=" : $"SplitMode={options.SplitMode}");
            lines.Add(options.SyncIntervalSec == "#" ? "#SyncIntervalSec=" : $"SyncIntervalSec={options.SyncIntervalSec}");
            lines.Add(options.RateLimitInterval == "#" ? "#RateLimitInterval=" : $"RateLimitInterval={options.RateLimitInterval}");
            lines.Add(options.RateLimitBurst == "#" ? "#RateLimitBurst=" : $"RateLimitBurst={options.RateLimitBurst}");
            lines.Add(options.SystemMaxUse == "#" ? "#SystemMaxUse=" : $"SystemMaxUse={options.SystemMaxUse}");
            lines.Add(options.SystemKeepFree == "#" ? "#SystemKeepFree=" : $"SystemKeepFree={options.SystemKeepFree}");
            lines.Add(options.SystemMaxFileSize == "#" ? "#SystemMaxFileSize=" : $"SystemMaxFileSize={options.SystemMaxFileSize}");
            lines.Add(options.RuntimeMaxUse == "#" ? "#RuntimeMaxUse=" : $"RuntimeMaxUse={options.RuntimeMaxUse}");
            lines.Add(options.RuntimeKeepFree == "#" ? "#RuntimeKeepFree=" : $"RuntimeKeepFree={options.RuntimeKeepFree}");
            lines.Add(options.RuntimeMaxFileSize == "#" ? "#RuntimeMaxFileSize=" : $"RuntimeMaxFileSize={options.RuntimeMaxFileSize}");
            lines.Add(options.MaxRetentionSec == "#" ? "#MaxRetentionSec=" : $"MaxRetentionSec={options.MaxRetentionSec}");
            lines.Add(options.MaxFileSec == "#" ? "#MaxFileSec=" : $"MaxFileSec={options.MaxFileSec}");
            lines.Add(options.ForwardToSyslog == "#" ? "#ForwardToSyslog=" : $"ForwardToSyslog={options.ForwardToSyslog}");
            lines.Add(options.ForwardToKMsg == "#" ? "#ForwardToKMsg=" : $"ForwardToKMsg={options.ForwardToKMsg}");
            lines.Add(options.ForwardToConsole == "#" ? "#ForwardToConsole=" : $"ForwardToConsole={options.ForwardToConsole}");
            lines.Add(options.ForwardToWall == "#" ? "#ForwardToWall=" : $"ForwardToWall={options.ForwardToWall}");
            lines.Add(options.TtyPath == "#" ? "#TTYPath=" : $"TTYPath={options.TtyPath}");
            lines.Add(options.MaxLevelStore == "#" ? "#MaxLevelStore=" : $"MaxLevelStore={options.MaxLevelStore}");
            lines.Add(options.MaxLevelSyslog == "#" ? "#MaxLevelSyslog=" : $"MaxLevelSyslog={options.MaxLevelSyslog}");
            lines.Add(options.MaxLevelKMsg == "#" ? "#MaxLevelKMsg=" : $"MaxLevelKMsg={options.MaxLevelKMsg}");
            lines.Add(options.MaxLevelConsole == "#" ? "#MaxLevelConsole=" : $"MaxLevelConsole={options.MaxLevelConsole}");
            lines.Add(options.MaxLevelWall == "#" ? "#MaxLevelWall=" : $"MaxLevelWall={options.MaxLevelWall}");
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

        public JournaldConfigurationModel Get() {
            return _serviceModel;
        }

        public void Enable() {
            _serviceModel.IsActive = true;
            Save(_serviceModel);
            ConsoleLogger.Log("[journald] enabled");
        }

        public void Disable() {
            _serviceModel.IsActive = false;
            Save(_serviceModel);
            ConsoleLogger.Log("[journald] disabled");
        }

        public void Stop() {
            Systemctl.Stop(ServiceName);
            ConsoleLogger.Log("[journald] stop");
        }

        public void Start() {
            if(Systemctl.IsEnabled(ServiceName) == false) {
                Systemctl.Enable(ServiceName);
            }
            if(Systemctl.IsActive(ServiceName) == false) {
                Systemctl.Restart(ServiceName);
            }
            ConsoleLogger.Log("[journald] start");
        }

        public void Remount() {
            while(MountHelper.IsAlreadyMounted(MainFilePath, DirsDirectoryPath)) {
                MountHelper.Umount(MainFilePath);
            }
            var mount = new MountManagement();
            mount.File(MainFilePath);
        }
    }
}
