using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using antdlib.common;
using antdlib.common.Tool;

namespace Antd.Machine {
    public class MachineId {

        public static string Get => GetMachineId();

        private static string EmptyUid() {
            var list = new List<string> {
                string.Join("", new byte[]{ 0, 0, 0, 0, 0, 0 }.Select(_=>_.ToString("D1"))),
                string.Join("", new byte[]{ 0, 0, 0, 0, 0, 0 }.Select(_=>_.ToString("D1"))),
                string.Join("", new byte[]{ 0, 0, 0, 0 }.Select(_=>_.ToString("D1"))),
                string.Join("", new byte[]{ 0, 0, 0, 0 }.Select(_=>_.ToString("D1")))
            };
            var emptyid = string.Join("-", list);
            return emptyid;
        }

        private static readonly string IdPath = $"{Parameter.AntdCfg}/machine-id";

        private static string GetMachineId() {
            if(!File.Exists(IdPath)) {
                File.WriteAllText(IdPath, "");
            }
            var machineUuid = EmptyUid();
            var exe = new Bash();
            var dmidecode = exe.Execute("dmidecode -t processor").SplitBash().ToList();
            var cpuId = dmidecode.GrepIgnore("UUID").Grep("ID").FirstOrDefault();
            cpuId = cpuId?.Split(':').LastOrDefault()?.Trim().Replace(" ", "");
            if(cpuId != null) {
                var uid1 = string.Join("", cpuId.ToCharArray().Take(6).Select(_ => _.ToString()));
                var uid2 = string.Join("", cpuId.ToCharArray().Reverse().Take(6).Select(_ => _.ToString()));
                cpuId = $"{uid1}-{uid2}";
            }
            dmidecode = exe.Execute("dmidecode -t memory").SplitBash().ToList();
            var mem = dmidecode.GrepIgnore("Array").Grep("Part Number").FirstOrDefault();
            mem = mem?.Split(':').LastOrDefault()?.Trim().Replace(" ", "").Replace("/", "");
            if(mem != null) {
                var uid1 = string.Join("", mem.ToCharArray().Take(4).Select(_ => _.ToString()));
                var uid2 = string.Join("", mem.ToCharArray().Reverse().Take(4).Select(_ => _.ToString()));
                mem = $"{uid1}-{uid2}";
            }
            if(string.IsNullOrEmpty(cpuId) || string.IsNullOrEmpty(mem)) {
                return machineUuid;
            }
            machineUuid = cpuId + "-" + mem;
            File.WriteAllText(IdPath, machineUuid);
            return machineUuid;
        }
    }
}
