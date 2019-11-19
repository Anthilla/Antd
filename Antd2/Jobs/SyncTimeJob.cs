using Antd2.cmds;
using Antd2.Configuration;

namespace Antd2.Jobs {
    public class SyncTimeJob : Job {

        private int _repetitionIntervalTime = 1000 * 60 * 5;

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
            if (ConfigManager.Config.Saved.Time.EnableNtpSync && ConfigManager.Config.Saved.Time.NtpServer.Length > 0) {
                Ntpdate.SyncFromRemoteServer(ConfigManager.Config.Saved.Time.NtpServer[0]);
            }
            //Timedatectl.SyncClock();
        }
    }
}
