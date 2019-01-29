using anthilla.core;
namespace Antd.cmds {
    public class Ntpdate {

        private const string ntpdateFileLocation = "/usr/sbin/ntpdate";

        public static void SyncFromRemoteServer(string remoteServer) {
            ConsoleLogger.Log($"[ntpdate] sync time with {remoteServer}");
            CommonProcess.Execute(ntpdateFileLocation, remoteServer);
        }
    }
}
