using System.Collections.Generic;

namespace Antd {

    public class ZfsMount {
        private static string cfgAlias = "antdControlSet";

        private static string[] paths = new string[] {
                cfgAlias + "Current",
                cfgAlias + "001",
                cfgAlias + "002"
            };

        private static Anth_ParamWriter config = new Anth_ParamWriter(paths, "antdZfsMount");

        private static string[] baseScriptArgs = new string[] {
                "zpool import Data01",
                "zpool import Data02",
                "zfs mount -a",
                "rsync -aHAz --delete-during /tmp/ /Data/Data01Vol01/tmp/",
                "rm -fR /tmp/* ",
                "mount -o bind /Data/Data01Vol01/tmp /tmp"
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