using anthilla.core;
using System.Linq;

namespace Antd.cmds {
    public class Versioning {

        private const string fileLocation = "/usr/bin/file";

        private const string systemFolder = "/mnt/cdrom/System/";
        private const string kernelFolder = "/mnt/cdrom/Kernel/";
        private const string antdFolder = "/mnt/cdrom/Apps/Anthilla_Antd/";
        private const string antdguiFolder = "/mnt/cdrom/Apps/Anthilla_AntdUi/";
        private const string antdshFolder = "/mnt/cdrom/Apps/Anthilla_antdsh/";

        private const string activeSystem = "active-system";

        private const string activeSystemmap = "active-System.map";
        private const string activeFirmware = "active-firmware";
        private const string activeInitrd = "active-initrd";
        private const string activeKernel = "active-kernel";
        private const string activeModules = "active-modules";

        private const string activeVersion = "active-version";

        private static string GetVersion(string file) {
            var result = CommonProcess.Execute(fileLocation, file).ToArray();
            if(result.Length == 0) {
                return string.Empty;
            }
            var version = Help.CaptureGroup(result[0], "([0-9]{8,12})");
            return version;
        }

        public static string System() {
            return GetVersion(CommonString.Append(systemFolder, activeSystem));
        }

        public static string SystemMap() {
            return GetVersion(CommonString.Append(kernelFolder, activeSystemmap));
        }

        public static string Firmware() {
            return GetVersion(CommonString.Append(kernelFolder, activeFirmware));
        }

        public static string Initrd() {
            return GetVersion(CommonString.Append(kernelFolder, activeInitrd));
        }

        public static string Kernel() {
            return GetVersion(CommonString.Append(kernelFolder, activeKernel));
        }

        public static string Modules() {
            return GetVersion(CommonString.Append(kernelFolder, activeModules));
        }

        public static string Antd() {
            return GetVersion(CommonString.Append(antdFolder, activeVersion));
        }

        public static string AntdGui() {
            return GetVersion(CommonString.Append(antdguiFolder, activeVersion));
        }

        public static string Antdsh() {
            return GetVersion(CommonString.Append(antdshFolder, activeVersion));
        }

        public static Versions Get() {
            return new Versions() {
                System = new VersionElement() { Ver = System() },
                SystemMap = new VersionElement() { Ver = SystemMap() },
                Kernel = new VersionElement() { Ver = Kernel() },
                Firmware = new VersionElement() { Ver = Firmware() },
                Initrd = new VersionElement() { Ver = Initrd() },
                Modules = new VersionElement() { Ver = Modules() },
                Antd = new VersionElement() { Ver = Antd() },
                AntdGui = new VersionElement() { Ver = AntdGui() },
                Antdsh = new VersionElement() { Ver = Antdsh() }
            };
        }
    }
}
