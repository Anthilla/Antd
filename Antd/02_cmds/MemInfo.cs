using Antd.models;
using System;
using System.IO;
using System.Linq;

namespace Antd.cmds {
    public class MemInfo {

        private const string procMeminfoFile = "/proc/meminfo";

        public static MemInfoModel[] Get() {
            if(!File.Exists(procMeminfoFile)) {
                return new MemInfoModel[0];
            }
            var result = File.ReadAllLines(procMeminfoFile).Where(_ => !string.IsNullOrEmpty(_)).ToArray();
            var meminfo = new MemInfoModel[result.Length];
            for(var i = 0; i < result.Length; i++) {
                var currentData = result[i].Replace("kB", "").Trim().Split(new[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
                meminfo[i] = new MemInfoModel() {
                    Key = currentData[0],
                    Value = currentData.Length == 1 ? string.Empty : currentData[1]
                };
            }
            return meminfo;
        }
    }
}
