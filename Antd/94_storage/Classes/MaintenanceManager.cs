using System;

namespace Kvpbase {
    public class MaintenanceManager {
        #region Public-Members

        #endregion

        #region Private-Members

        private bool Enabled { get; set; }

        #endregion

        #region Constructors-and-Factories

        public MaintenanceManager() {
            Enabled = false;
        }

        #endregion

        #region Public-Methods

        public bool IsEnabled() {
            return Enabled;
        }

        public void Set() {
            Enabled = true;
            return;
        }

        public void Stop() {
            Enabled = false;
            return;
        }

        #endregion

        #region Public-Static-Methods

        #endregion
    }
}
