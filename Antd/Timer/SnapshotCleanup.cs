using System;
using System.Threading;
using Antd.Storage;

namespace Antd.Timer {
    public class SnapshotCleanup {

        public System.Threading.Timer Timer { get; private set; }

        public void Start(TimeSpan alertTime) {
            Timer = new System.Threading.Timer(x => {
                Action();
            }, null, alertTime, Timeout.InfiniteTimeSpan);
        }

        public void Stop() {
            Timer?.Dispose();
        }

        private static void Action() {
            new BackupClean().Launch();
        }
    }
}
