using antdlib.common;

namespace Antd {
    public class Rmmod {
        public static void Ex(string module) {
            var bash = new Bash();
            bash.Execute($"rmmod {module}", false);
        }
    }
}
