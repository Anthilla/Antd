using anthilla.core;
using System.Linq;

namespace Antd.cmds {

    public class Hostnamectl {

        private const string hostnamectlFileLocation = "/usr/bin/hostnamectl";
        private const string setChassisArg = "set-chassis";
        private const string setDeploymentArg = "set-deployment";
        private const string setHostnameArg = "set-hostname";
        private const string setLocationArg = "set-location";

        public static Host Get() {
            var result = CommonProcess.Execute(hostnamectlFileLocation);
            if(result.Count() < 1) {
                return new Host();
            }
            var host = new Host();
            host.HostName = result.FirstOrDefault(_ => _.Contains("Static hostname:")).Split(new[] { ':' }).LastOrDefault().Trim();
            host.HostLocation = result.FirstOrDefault(_ => _.Contains("Location:")).Split(new[] { ':' }).LastOrDefault().Trim();
            host.HostChassis = result.FirstOrDefault(_ => _.Contains("Chassis:")).Split(new[] { ':' }).LastOrDefault().Trim();
            host.HostDeployment = result.FirstOrDefault(_ => _.Contains("Deployment:")).Split(new[] { ':' }).LastOrDefault().Trim();
            var machineIds = MachineIds.Get();
            host.PartNumber = machineIds.PartNumber;
            host.SerialNumber = machineIds.SerialNumber;
            host.MachineUid = machineIds.SerialNumber;
            return host;
        }

        public static bool Apply() {
            var current = Application.CurrentConfiguration.Host;
            var running = Application.RunningConfiguration.Host;
            if(CommonString.AreEquals(current.HostName, running.HostName) == false) {
                ConsoleLogger.Log($"[host] name: {current.HostName}");
                SetHostname(current.HostName);
            }
            if(CommonString.AreEquals(current.HostDeployment, running.HostDeployment) == false) {
                ConsoleLogger.Log($"[host] deployment: {current.HostDeployment}");
                SetDeployment(current.HostDeployment);
            }
            if(CommonString.AreEquals(current.HostChassis, running.HostChassis) == false) {
                ConsoleLogger.Log($"[host] chassis: {current.HostChassis}");
                SetChassis(current.HostChassis);
            }
            if(CommonString.AreEquals(current.HostLocation, running.HostLocation) == false) {
                ConsoleLogger.Log($"[host] location: {current.HostLocation}");
                SetLocation(current.HostLocation);
            }
            return true;
        }

        public static bool SetChassis(string chassis) {
            var args = CommonString.Append(setChassisArg, " ", chassis);
            CommonProcess.Do(hostnamectlFileLocation, args);
            return true;
        }

        public static bool SetDeployment(string deployment) {
            var args = CommonString.Append(setDeploymentArg, " ", deployment);
            CommonProcess.Do(hostnamectlFileLocation, args);
            return true;
        }

        public static bool SetHostname(string hostname) {
            var args = CommonString.Append(setHostnameArg, " ", hostname);
            CommonProcess.Do(hostnamectlFileLocation, args);
            return true;
        }

        public static bool SetLocation(string location) {
            var args = CommonString.Append(setLocationArg, " ", location);
            CommonProcess.Do(hostnamectlFileLocation, args);
            return true;
        }
    }
}
