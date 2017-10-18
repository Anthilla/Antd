using System.Linq;
using anthilla.core;

namespace Antd.cmds {
    public class Dmesg {

        private const string dmesgFileLocation = "/bin/dmesg";

        public static string[] GetLog() {
            return CommonProcess.Execute(dmesgFileLocation).ToArray();
        }
    }
}