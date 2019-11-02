using System.Linq;

namespace Antd2.cmds {
    public class Uname {

        private const string unameCommand = "uname";
        private const string argument = "-a";
        private const string kernelArgument = "-r";

        public static string Get() {
            var result = Bash.Execute($"{unameCommand} {argument}");
            if (result == null) {
                return string.Empty;
            }
            if (result.Count() < 1) {
                return string.Empty;
            }
            return result.FirstOrDefault().Trim();
        }

        public static string GetKernel() {
            var result = Bash.Execute($"{unameCommand} {kernelArgument}");
            if (result == null) {
                return string.Empty;
            }
            if (result.Count() < 1) {
                return string.Empty;
            }
            return result.FirstOrDefault().Trim();
        }
    }
}
