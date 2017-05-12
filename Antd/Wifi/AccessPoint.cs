using antdlib.common;
using System.Linq;

namespace Antd.Wifi {
    public class AccessPoint {

        public void Set() {

        }

        public bool IsApConfigurable() {
            var devInfo = Bash.Execute("iw list");
            if(string.IsNullOrEmpty(devInfo)) {
                return false;
            }
            var i = devInfo.SplitBash().Grep("* AP");
            return i.Any();
        }
    }
}
