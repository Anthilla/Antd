using anthilla.scheduler;
using System.Threading.Tasks;
using System;

namespace Antd.Mqtt {
    public class MqttTestJob : Job {

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

        private int _repetitionIntervalTime = 1000;

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
            DoJobAsync().GetAwaiter().GetResult();
        }

        static async Task DoJobAsync() {
            await SyncController.TestCommand(DateTime.Now.ToString());
        }
    }
}
