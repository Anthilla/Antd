using System;
using System.Threading;
using Antd.Storage;

namespace Antd.Timer {
    public class SnapshotCleanup {

        public System.Threading.Timer Timer { get; private set; } = null;

        public void Start(TimeSpan alertTime) {
            var current = DateTime.Now;
            var timeToGo = alertTime - current.TimeOfDay;
            if(timeToGo < TimeSpan.Zero) {
                return;
            }
            Timer = new System.Threading.Timer(x => {
                Action();
            }, null, timeToGo, Timeout.InfiniteTimeSpan);
        }

        public void Stop() {
            Timer?.Dispose();
        }

        private static void Action() {
            new BackupClean().Launch();
        }
    }
}
