using System;
using System.IO;

namespace Antd2.Configuration {

    public class ConfigManager : IConfManager, IDisposable {

        public MachineConfiguration Saved { get; set; }
        public string TomlPath { get; set; }

        private bool loaded = false;

        private ConfigManager() {
        }

        private void PLoad() {
            if (Saved == null) {
                if (!string.IsNullOrEmpty(TomlPath)) {
                    if (File.Exists(TomlPath)) {
                        Saved = Nett.Toml.ReadFile<MachineConfiguration>(TomlPath);
                    }
                }
            }
            if (Saved != null) {
                Saved.Host.Uid = Antd2.cmds.Device.LocalId;
            }
            loaded = Saved != null;
        }

        public void Load() {
            if (loaded == false) {
                PLoad();
            }
        }

        public void Reload() {
            if (!string.IsNullOrEmpty(TomlPath)) {
                if (File.Exists(TomlPath)) {
                    Saved = Nett.Toml.ReadFile<MachineConfiguration>(TomlPath);
                }
            }
            if (Saved != null) {
                Saved.Host.Uid = Antd2.cmds.Device.LocalId;
            }
            loaded = Saved != null;
        }

        public void Dump() {
            if (loaded == true) {
                Nett.Toml.WriteFile<MachineConfiguration>(Saved, TomlPath);
            }
        }

        #region Do not modify!

        /// <summary>
        /// The volatile keyword ensures that the instantiation is complete
        /// before it can be accessed further helping with thread safety.
        /// </summary>
        private static volatile ConfigManager _instance = null;
        private static readonly object _syncLock = new object();
        private readonly bool _disposed = false;

        /// <summary>
        /// Pattern 'double check locking'
        /// </summary>
        public static ConfigManager Config {
            get {
                if (_instance != null)
                    return _instance;
                lock (_syncLock) {
                    if (_instance == null) {
                        _instance = new ConfigManager();
                    }
                    return _instance;
                }
            }
        }

        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private protected virtual void Dispose(bool disposing) {
            if (_disposed)
                return;
            if (disposing) {
                lock (_syncLock) {
                    Saved = null;
                    _instance = null;
                }
            }
        }
        #endregion
    }


    public interface IConfManager {
        void Dispose();
    }
}
