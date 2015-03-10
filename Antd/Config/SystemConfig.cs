namespace Antd {

    public class SystemConfig {
        private static string cfgAlias = "antdControlSet";

        private static string[] paths = new string[] {
                cfgAlias + "Current",
                cfgAlias + "001",
                cfgAlias + "002"
            };

        private static Anth_ParamWriter config = new Anth_ParamWriter(paths, "antdSystem");

        public static string[] baseScriptArgs = new string[] {
                "mount /mnt/cdrom -o remount,rw,noatime,discard",
                "mount -o bind /mnt/cdrom /System",
                "mount -o bind /mnt/cdrom /boot",
                "mount -t tmpfs none /Data",
                "mount -t tmpfs none /lib64/modules",
                "mkdir -p /lib64/modules/3.16.7-aufs",
                "mount /System/DIR_lib64_modules_3.16.7-aufs.squash.xz /lib64/modules/3.16.7-aufs",
                "mount /System/DIR_lib64_firmware.squash.xz /lib64/firmware"
            };

        public static void WriteDefaults() {
            for (int i = 0; i < baseScriptArgs.Length; i++) {
                var n = i.ToString();
                if (config.CheckValue(n) == false) {
                    config.Write(n, baseScriptArgs[i]);
                }
            }
        }

        public static void LaunchDefaults() {
            for (int i = 0; i < baseScriptArgs.Length; i++) {
                var row = config.ReadValue(i.ToString());
                var array = row.Split(new[] { ' ' }, 2);
                string file = array[0];
                string args = array[1];
                Command.Launch(file, args);
            }
        }

        public static void FirstLaunchDefaults() {
            for (int i = 0; i < baseScriptArgs.Length; i++) {
                var row = baseScriptArgs[i];
                var array = row.Split(new[] { ' ' }, 2);
                string file = array[0];
                string args = array[1];
                Command.Launch(file, args);
            }
        }
    }
}