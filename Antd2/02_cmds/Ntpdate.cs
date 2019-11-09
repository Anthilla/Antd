using anthilla.core;

namespace Antd2.cmds {
    public class Ntpdate {

        private const string ntpdateCommand = "ntpdate";

        public static void SyncFromRemoteServer(string remoteServer) {
            Console.WriteLine($"[ntpdate] sync time with {remoteServer}");
            CommonProcess.Execute($"{ntpdateCommand} {remoteServer}");
        }
    }
}
