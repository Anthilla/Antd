using antdlib.common;
using System.Linq;

namespace Antd.Wifi {
    public class AccessPoint {

        private readonly Bash _bash = new Bash();

        public void Set() {

        }

        public bool IsApConfigurable() {
            var devInfo = _bash.Execute("iw list");
            if(string.IsNullOrEmpty(devInfo)) {
                return false;
            }
            var i = devInfo.SplitBash().Grep("* AP");
            return i.Any();
        }
    }
}
