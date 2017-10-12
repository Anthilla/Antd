using System.Timers;
using antdlib.config;

namespace Antd.Timer {
    public class UpdateBindHints {

        public System.Timers.Timer Timer { get; private set; }

        public void Start(int milliseconds) {
            Action();
            Timer = new System.Timers.Timer(milliseconds);
            Timer.Elapsed += Elapsed;
            Timer.Enabled = true;
        }

        public void Stop() {
            Timer?.Dispose();
        }

        private static void Elapsed(object sender, ElapsedEventArgs e) {
            Action();
        }

        private static void Action() {
            BindConfiguration.DownloadRootServerHits();
        }
    }
}
