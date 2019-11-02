using System.Collections.Generic;
using System.Linq;

namespace Antd2.cmds {
    public class Whereis {

        private const string whereisCommand = "whereis";

        public static IEnumerable<string> Find(string package) {
            return Bash.Execute($"{whereisCommand} {package}");
        }

        public static bool IsInstalled(string package) {
            var whereis = Find(package).FirstOrDefault();
            if (whereis == null) { return false; }
            var data = whereis.Split(new[] { ":" }, System.StringSplitOptions.RemoveEmptyEntries);
            return data.Length > 1;
        }
    }
}
