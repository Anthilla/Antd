using anthilla.core;

namespace Antd.cmds {
    public class Parted {

        private const string partedLocation = "/usr/sbin/parted";

        public static bool MakeLabel(string diskPath, string labelType) {
            var args = CommonString.Append(diskPath, " -a optimal mklabel ", labelType);
            CommonProcess.Do(partedLocation, args);
            return true;
        }

        public static bool MakePartition(string diskPath, string partitionName, string partitionType, string partitionStart, string partitionEnd) {
            var args = CommonString.Append(diskPath, " -a optimal mkpart ", partitionName, " ", partitionType, " ", partitionStart, " ", partitionEnd);
            CommonProcess.Do(partedLocation, args);
            return true;
        }
    }
}
