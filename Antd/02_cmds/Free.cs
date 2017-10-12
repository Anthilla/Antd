using Antd.models;
using anthilla.core;
using System;
using System.Linq;

namespace Antd.cmds {
    public class Free {
  
        private const string freeFileLocation = "/usr/bin/free";
        private const string freeOptions = "-b";

        public static FreeModel[] Get() {
            var result = CommonProcess.Execute(freeFileLocation, freeOptions).Skip(1).Where(_ => !string.IsNullOrEmpty(_)).ToArray();
            var free = new FreeModel[result.Length];
            for(var i = 0; i < result.Length; i++) {
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
