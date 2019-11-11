using Antd2.cmds;
using System.Linq;

namespace Antd2.Jobs {
    public class ServicesControllerJob : Job {

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
            foreach (var service in StartCommand.CONF.Boot.ActiveServices) {
                if (Systemctl.IsEnabled(service) == false)
                    Systemctl.Enable(service);
                if (Systemctl.IsActive(service) == false)
                    Systemctl.Start(service);
            }
            foreach (var service in StartCommand.CONF.Boot.InactiveServices) {
                if (Systemctl.IsActive(service))
                    Systemctl.Stop(service);
            }
            foreach (var service in StartCommand.CONF.Boot.DisabledServices) {
                if (Systemctl.IsActive(service))
                    Systemctl.Stop(service);
                if (Systemctl.IsEnabled(service))
                    Systemctl.Disable(service);
            }
            foreach (var service in StartCommand.CONF.Boot.BlockedServices) {
                if (Systemctl.IsActive(service))
                    Systemctl.Stop(service);
                if (Systemctl.IsEnabled(service))
                    Systemctl.Disable(service);
                Systemctl.Mask(service);
            }
        }
    }
}
