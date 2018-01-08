using anthilla.core;
using System.Linq;

namespace Antd.cmds {
    public class Date {
  
        private const string dateFileLocation = "/bin/date";

        public static string Get() {
            return CommonProcess.Execute(dateFileLocation).ToArray()[0];
        }
    }
}
