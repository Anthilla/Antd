using antd.core;
using Antd2.models;
using System.Linq;

namespace Antd2.cmds {

    public class Hostnamectl {

        private const string hostnamectlCommand = "hostnamectl";
        private const string setChassisArg = "set-chassis";
        private const string setDeploymentArg = "set-deployment";
        private const string setHostnameArg = "set-hostname";
        private const string setLocationArg = "set-location";

        public static Host Get() {
            var result = Bash.Execute(hostnamectlCommand);
            if (result.Count() < 1) {
                return new Host();
            }
            var host = new Host();
            host.HostName = result.FirstOrDefault(_ => _.Contains("Static hostname:")).Split(new[] { ':' }).LastOrDefault().Trim();
            host.HostLocation = result.FirstOrDefault(_ => _.Contains("Location:")).Split(new[] { ':' }).LastOrDefault().Trim();
            host.HostChassis = result.FirstOrDefault(_ => _.Contains("Chassis:")).Split(new[] { ':' }).LastOrDefault().Trim();
            host.HostDeployment = result.FirstOrDefault(_ => _.Contains("Deployment:")).Split(new[] { ':' }).LastOrDefault().Trim();
            return host;
        }

        public static bool SetChassis(string chassis) {
            var args = CommonString.Append(setChassisArg, " ", chassis);
            Bash.Do($"{hostnamectlCommand} {args}");
            return true;
        }

        public static bool SetDeployment(string deployment) {
            var args = CommonString.Append(setDeploymentArg, " ", deployment);
            Bash.Do($"{hostnamectlCommand} {args}");
            return true;
        }

        public static bool SetHostname(string hostname) {
            var args = CommonString.Append(setHostnameArg, " ", hostname);
            Bash.Do($"{hostnamectlCommand} {args}");
            return true;
        }

        public static bool SetLocation(string location) {
            var args = CommonString.Append(setLocationArg, " ", location);
            Bash.Do($"{hostnamectlCommand} {args}");
            return true;
        }
    }
}
