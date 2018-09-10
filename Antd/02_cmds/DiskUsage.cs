using Antd.models;
using anthilla.core;
using System;
using System.Linq;

namespace Antd.cmds {
    public class DiskUsage {

        private const string dfFileLocation = "/bin/df";
        private const string dfOptions = "-HTP";

        public static DiskUsageModel[] Get() {
            var result = CommonProcess.Execute(dfFileLocation, dfOptions).Skip(1).ToArray();
            var free = new DiskUsageModel[result.Length];
            for(var i = 0; i < result.Length; i++) {
                var currentData = result[i].Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                free[i] = new DiskUsageModel() {
                    Filesystem = currentData[0],
                    Type = currentData[1],
                    Size = currentData[2],
                    Used = currentData[3],
                    Avail = currentData[4],
                    UsePercentage = currentData[5],
                    MountedOn = currentData[6]
                };
            }
            return free;
        }
    }
}
