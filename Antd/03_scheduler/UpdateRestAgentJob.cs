using anthilla.scheduler;
using anthilla.core;

namespace Antd {
    public class UpdateRestAgentJob : Job {

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

        private int _repetitionIntervalTime = 1000 * 60 * 60;

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
            if(string.IsNullOrEmpty(Application.Agent)) {
                Application.Agent = CommonRandom.CrcGuid();
            }
        }
    }
}
