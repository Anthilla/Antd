using antdlib.common;

namespace Antd {
    public class Rmmod {
        public static void Ex(string module) {
            Bash.Execute($"rmmod {module}");
        }
    }
}
