using Antd2.models;
using System;
using System.Linq;

namespace Antd2.cmds {
    public class Losetup {

        private const string losetupCommand = "losetup";
        private const string losetupOptions = "--list -n";

        public static LosetupModel[] Get() {
            var result = Bash.Execute($"{losetupCommand} {losetupOptions}").Where(_ => !string.IsNullOrEmpty(_)).ToArray();
            var loetup = new LosetupModel[result.Length];
            for (var i = 0; i < result.Length; i++) {
                var currentData = result[i].Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                loetup[i] = new LosetupModel() {
                    Name = currentData[0],
                    Sizelimit = currentData[1],
                    Offset = currentData[2],
                    Autoclear = currentData[3],
                    Readonly = currentData[4],
                    Backfile = currentData[5],
                    Dio = currentData[6]
                    //Hash = currentData[7]
                };
            }
            return loetup;
        }
    }
}
