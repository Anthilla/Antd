using Antd2.cmds;

namespace Antd.Jobs {
    public class ModulesRemoverJob : Job {

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
            foreach (var module in StartCommand.CONF.Boot.InactiveModules) {
                Mod.Remove(module);
            }
        }
    }
}
