using Antd2.cmds;
using System;
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
                var loadedModule = loadedModules.FirstOrDefault(_ => _.Module == module);
                if (string.IsNullOrEmpty(loadedModule.Module)) {
                    continue;
                }

                Console.Write($"[mod] removing module '{module}'");
                if (loadedModule.UsedBy.Length > 0)
                    Console.WriteLine($" and its {loadedModule.UsedBy.Length} dependecies");
                else
                    Console.WriteLine();

                Mod.Remove(loadedModule);
            }
            foreach (var module in StartCommand.CONF.Boot.ActiveModules) {
                var (Module, UsedBy) = loadedModules.FirstOrDefault(_ => _.Module == module);
                if (!string.IsNullOrEmpty(Module)) {
                    continue;
                }
                Mod.Add(module);
            }
        }
    }
}
