using Antd2.cmds;
using System.Linq;

namespace Antd2.Jobs {
    public class ModulesControllerJob : Job {

        private readonly int _repetitionIntervalTime = 1000 * 60 * 5;

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
            var loadedModules = Mod.Get();
            foreach (var module in StartCommand.CONF.Boot.InactiveModules) {
                if (loadedModules.Any(_ => _.Module.Trim().ToUpperInvariant() == module.Trim().ToUpperInvariant())) {
                    Mod.Remove(loadedModules.FirstOrDefault(_ => _.Module.Trim().ToUpperInvariant() == module.Trim().ToUpperInvariant()));
                }
            }
            foreach (var module in StartCommand.CONF.Boot.ActiveModules) {
                Mod.Add(module);
            }
        }
    }
}
