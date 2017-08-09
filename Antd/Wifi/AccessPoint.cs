using System.Linq;
using anthilla.core;

namespace Antd.Wifi {
    public class AccessPoint {

        public void Set() {

        }

        public bool IsApConfigurable() {
            var devInfo = Bash.Execute("iw list");
            if(string.IsNullOrEmpty(devInfo)) {
                return false;
            }
            var i = devInfo.Split().Grep("* AP");
            return i.Any();
        }
    }
}
