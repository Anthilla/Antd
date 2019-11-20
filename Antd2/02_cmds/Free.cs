using Antd2.models;
using System;
using System.Linq;

namespace Antd2.cmds {
    public class Free {

        private const string freeCommand = "free";
        private const string freeOptions = "-btl";

        public static FreeModel[] Get() {
            var result = Bash.Execute($"{freeCommand} {freeOptions}").Skip(1).Where(_ => !string.IsNullOrEmpty(_)).ToArray();
            var free = new FreeModel[result.Length];
            for (var i = 0; i < result.Length; i++) {
                var currentData = result[i].Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                free[i] = new FreeModel() {
                    Name = currentData[0].Trim(':'),
                    Total = currentData[1],
                    Used = currentData[2],
                    Free = currentData[3],
                    Shared = currentData.Length > 4 ? currentData[4] : string.Empty,
                    BuffCache = currentData.Length > 5 ? currentData[5] : string.Empty,
                    Available = currentData.Length > 6 ? currentData[6] : string.Empty
                };
            }
            return free;
        }
    }
}
