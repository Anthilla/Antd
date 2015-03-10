using System.Collections.Generic;

namespace Antd {

    public class SystemDataRepo {
        private static string cfgAlias = "antdControlSet";

        private static string[] paths = new string[] {
                cfgAlias + "Current",
                cfgAlias + "001",
                cfgAlias + "002"
            };

        private static Anth_ParamWriter config = new Anth_ParamWriter(paths, "antdSystemDataRepo");

        private static string[] baseScriptArgs = new string[] {
                "qemu-nbd --connect=/dev/nbd1 /System/SystemDataRepo.qed",
                "kpartx -a /dev/nbd1",
                "sleep 1",
                "mkdir -p  /Data/SystemDataRepo",
                "mount -o rw,noatime,discard /dev/mapper/nbd1p1 /Data/SystemDataRepo/",
                "mount -o bind /Data/SystemDataRepo/DIR_framework/ /framework/",
                "mount -o bind /Data/SystemDataRepo/DIR_home/ /home/",
                "mount -o bind /Data/SystemDataRepo/DIR_home_SYS/ /home/SYS/",
                "mount -o bind /Data/SystemDataRepo/DIR_var_account/ /var/account/",
                "mount -o bind /Data/SystemDataRepo/DIR_var_amavis/ /var/amavis/",
                "mount -o bind /Data/SystemDataRepo/DIR_var_bind/ /var/bind/",
                "mount -o bind /Data/SystemDataRepo/DIR_var_cache/ /var/cache/",
                "mount -o bind /Data/SystemDataRepo/DIR_var_db_pkg/ /var/db/pkg/",
                "mount -o bind /Data/SystemDataRepo/DIR_var_db_sudo/ /var/db/sudo/",
                "mount -o bind /Data/SystemDataRepo/DIR_var_db_iscsi/ /var/db/iscsi/",
                "mount -o bind /Data/SystemDataRepo/DIR_var_heimdal/ /var/heimdal/",
                "mount -o bind /Data/SystemDataRepo/DIR_var_imap/ /var/imap/",
                "mount -o bind /Data/SystemDataRepo/DIR_var_spool/ /var/spool/",
                "mount -o bind /Data/SystemDataRepo/DIR_var_target/ /var/target/",
                "mount -o bind /Data/SystemDataRepo/DIR_var_www/ /var/www/"
            };

        private static void WriteDefaults() {
            for (int i = 0; i < baseScriptArgs.Length; i++) {
                var n = i.ToString();
                if (config.CheckValue(n) == false) {
                    config.Write(n, baseScriptArgs[i]);
                }
            }
        }

        public static CommandModel[] LaunchDefaults() {
            WriteDefaults();
            List<CommandModel> actionList = new List<CommandModel>() { };
            for (int i = 0; i < baseScriptArgs.Length; i++) {
                var row = config.ReadValue(i.ToString());
                var array = row.Split(new[] { ' ' }, 2);
                string file = array[0];
                string args = array[1];
                var command = Command.Launch(file, args);
                actionList.Add(command);
            }
            return actionList.ToArray();
        }
    }
}