using System.Collections.Generic;

namespace Antd2.cmds {
    public class Ping {

        private const string pingCommand = "ping";

        public static IEnumerable<string> Get(string destination, int count = 4) {
            var result = Bash.Execute($"{pingCommand} -c {count} {destination}");
            return result;
        }
    }
}
