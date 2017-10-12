using anthilla.core;

namespace Antd.cmds {
    public class Rsync {

        private const string rsyncFileLocation = "/usr/bin/rsync";
        private const string optionaHA = "-aHA ";
        private const string optionDeleteAfter = "-aHA --delete-after ";
        private const string optionDeleteDuring = "-aHA --delete-during ";

        public static bool Sync(string source, string destination) {
            var args = CommonString.Append(optionaHA, source, " ", destination);
            CommonProcess.Do(rsyncFileLocation, args);
            return true;
        }

        public static bool SyncDeleteAfter(string source, string destination) {
            var args = CommonString.Append(optionDeleteAfter, source, " ", destination);
            CommonProcess.Do(rsyncFileLocation, args);
            return true;
        }

        public static bool SyncDeleteDuring(string source, string destination) {
            var args = CommonString.Append(optionDeleteDuring, source, " ", destination);
            CommonProcess.Do(rsyncFileLocation, args);
            return true;
        }
    }
}
