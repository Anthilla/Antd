using Antd2.cmds;
using Antd2.Configuration;
using System;

namespace Antd2.Jobs {
    public class LogRotateJob : Job {

        private int _repetitionIntervalTime = (int)TimeSpan.FromHours(1).TotalMilliseconds;

        #region [    Core Parameter    ]
        private bool _isRepeatable = true;

        public override bool IsRepeatable {
            get {
                return _isRepeatable;
            }
            set {
                value = _isRepeatable;
            }
        }

        public override int RepetitionIntervalTime {
            get {
                return _repetitionIntervalTime;
            }

            set {
                value = _repetitionIntervalTime;
            }
        }

        public override string Name {
            get {
                return GetType().Name;
            }

            set {
                value = GetType().Name;
            }
        }
        #endregion

        public override void DoJob() {
            LogRotation.Rotate(ConfigManager.Config.Saved.LogRotation);
        }
    }
}
