using antdlib.common;
using System;
using System.IO;
using System.Linq;
using antdlib.models;
using Newtonsoft.Json;

namespace Antd.Machine {
    public class MachineId {

        public static string Get => GetMachineId();

        private static string VirtualUid() {
            var g1 = Guid.NewGuid().ToString().Substring(0, 6);
            var g2 = Guid.NewGuid().ToString().Substring(0, 4);
            var id = $"000000-{g1}-{g2}-0000";
            return id;
        }

        private static readonly string IdPath = $"{Parameter.AntdCfg}/machine-id";

        private static string GetMachineId() {
            if(File.Exists(IdPath)) {
                var checkFile = File.ReadAllText(IdPath);
                if(checkFile == "000000-000000-0000-0000") {
                    File.Delete(IdPath);
                }
                else {
                    try {
                        var x = JsonConvert.DeserializeObject<MachineIdModel>(checkFile);
                        return x.Value;
                    }
                    catch(Exception) {
                        File.Delete(IdPath);
                    }
                }
            }
            else {
                var machineUuid = VirtualUid();
                try {
                    var dmidecode = Bash.Execute("dmidecode -t processor").SplitBash().ToList();
                    var cpuId = dmidecode.GrepIgnore("UUID").Grep("ID").FirstOrDefault();
                    cpuId = cpuId?.Split(':').LastOrDefault()?.Trim().Replace(" ", "");
                    if(cpuId != null) {
                        var uid1 = string.Join("", cpuId.ToCharArray().Take(6).Select(_ => _.ToString()));
                        var uid2 = string.Join("", cpuId.ToCharArray().Reverse().Take(6).Select(_ => _.ToString()));
                        cpuId = $"{uid1}-{uid2}";
                    }
                    dmidecode = Bash.Execute("dmidecode -t memory").SplitBash().ToList();
                    var mem = dmidecode.GrepIgnore("Array").Grep("Part Number").FirstOrDefault();
                    mem = mem?.Split(':').LastOrDefault()?.Trim().Replace(" ", "").Replace("/", "");
                    if(mem != null) {
                        var uid1 = string.Join("", mem.ToCharArray().Take(4).Select(_ => _.ToString()));
                        var uid2 = string.Join("", mem.ToCharArray().Reverse().Take(4).Select(_ => _.ToString()));
                        mem = $"{uid1}-{uid2}";
                    }
                    if(string.IsNullOrEmpty(cpuId) || string.IsNullOrEmpty(mem)) {
                        throw new Exception();
                    }
                    machineUuid = cpuId + "-" + mem;
                    if(machineUuid.EndsWith("-")) {
                        machineUuid = machineUuid.TrimEnd('-');
                    }
                }
                catch(Exception) {
                    var txt = JsonConvert.SerializeObject(new MachineIdModel(machineUuid), Formatting.Indented);
                    FileWithAcl.WriteAllText(IdPath, txt, "644", "root", "wheel");
                    return machineUuid;
                }
                var txt2 = JsonConvert.SerializeObject(new MachineIdModel(machineUuid), Formatting.Indented);
                FileWithAcl.WriteAllText(IdPath, txt2, "644", "root", "wheel");
                return machineUuid;
            }
            return string.Empty;
        }
    }
}
