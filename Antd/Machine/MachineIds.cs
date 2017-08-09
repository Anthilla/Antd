using System;
using System.IO;
using antdlib.models;
using anthilla.core;
using Newtonsoft.Json;
using Parameter = antdlib.common.Parameter;

namespace Antd.Machine {
    public class MachineIds {

        public static MachineIdsModel Get => GetMachineId();

        private static readonly string IdPath = $"{Parameter.AntdCfg}/machine-id";

        private static MachineIdsModel GetMachineId() {
            if(File.Exists(IdPath)) {
                var checkFile = File.ReadAllText(IdPath);
                if(checkFile == "000000-000000-0000-0000") {
                    File.Delete(IdPath);
                }
                else {
                    return JsonConvert.DeserializeObject<MachineIdsModel>(checkFile);
                }
            }
            else {
                var machineUuid = new MachineIdsModel();
                var json = JsonConvert.SerializeObject(machineUuid, Formatting.Indented);
                FileWithAcl.WriteAllText(IdPath, json, "644", "root", "wheel");
                return machineUuid;
            }
            return new MachineIdsModel();
        }
    }
}
