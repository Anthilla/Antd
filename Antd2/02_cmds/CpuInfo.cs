using Antd2.models;
using System;
using System.IO;
using System.Linq;

namespace Antd2.cmds {
    public class CpuInfo {

        private const string procCpuinfoFile = "/proc/cpuinfo";

        public static CpuInfoModel[] Get() {
            if (!File.Exists(procCpuinfoFile)) {
                return new CpuInfoModel[0];
            }
            var result = File.ReadAllLines(procCpuinfoFile).Where(_ => !string.IsNullOrEmpty(_)).ToArray();
            var cpuinfo = new CpuInfoModel[result.Length];
            for (var i = 0; i < result.Length; i++) {
                var currentData = result[i].Split(new[] { ':' }, 2, StringSplitOptions.RemoveEmptyEntries);
                cpuinfo[i] = new CpuInfoModel() {
                    Key = currentData[0],
                    Value = currentData.Length != 2 ? string.Empty : currentData[1]
                };
            }
            return cpuinfo;
        }
    }
}
