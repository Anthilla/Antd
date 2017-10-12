using anthilla.core;
using System.Linq;

namespace Antd.cmds {
    public class Uname {

        private const string unameFileLocation = "/usr/bin/uname";
        private const string argument = "-a";
        private const string kernelArgument = "-r";

        public static string Get() {
            var result = CommonProcess.Execute(unameFileLocation, argument).ToArray();
            if(result == null) {
                return string.Empty;
            }
            if(result.Length < 1) {
                return string.Empty;
            }
            return result[0].Trim();
        }

        public static string GetKernel() {
            var result = CommonProcess.Execute(unameFileLocation, kernelArgument).ToArray();
            if(result == null) {
                return string.Empty;
            }
            if(result.Length < 1) {
                return string.Empty;
            }
            return result[0].Trim();
        }
    }
}
