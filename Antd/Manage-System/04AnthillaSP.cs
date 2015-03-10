using System.Collections.Generic;

namespace Antd {

    public class SetAnthillaSP {
        private static string cfgAlias = "antdControlSet";

        private static string[] paths = new string[] {
                cfgAlias + "Current",
                cfgAlias + "001",
                cfgAlias + "002"
            };

        private static Anth_ParamWriter config = new Anth_ParamWriter(paths, "antdAnthillaSP");

        private static string[] baseScriptArgs = new string[] {
                "qemu-nbd --connect=/dev/nbd2 /System/anthillasp.qed",
                "kpartx -a /dev/nbd2",
                "sleep 1",
                "mkdir -p  /framework/Anthilla/AnthillaSP/version",
                "mount -o rw,noatime,discard /dev/mapper/nbd2p1 /framework/Anthilla/AnthillaSP/version"
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