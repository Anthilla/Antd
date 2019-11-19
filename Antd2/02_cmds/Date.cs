using System.Linq;

namespace Antd2.cmds {
    public class Date {

        private const string dateCommand = "date";

        public static string Get() {
            return Bash.Execute(dateCommand).FirstOrDefault();
        }
    }
}
