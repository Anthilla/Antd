using System.IO;
using anthilla.core;
using Newtonsoft.Json;
using System;

namespace Antd.cmds {
    public class MachineIds {

        public class Model {

            public Model() {
                PartNumber = Guid.NewGuid();
                SerialNumber = Guid.NewGuid();
                MachineUid = Guid.NewGuid();
            }

            public Guid PartNumber { get; set; }
            public Guid SerialNumber { get; set; }
            public Guid MachineUid { get; set; }
        }

        private static readonly string IdPath = $"{Const.AntdCfg}/machine-id";

        public static Model Get() {
            if(File.Exists(IdPath)) {
                var checkFile = File.ReadAllText(IdPath);
                if(checkFile == "000000-000000-0000-0000") {
                    File.Delete(IdPath);
                }
                else {
                    return JsonConvert.DeserializeObject<Model>(checkFile);
                }
            }
            else {
                var machineUuid = new Model();
                var json = JsonConvert.SerializeObject(machineUuid, Formatting.Indented);
                File.WriteAllText(IdPath, json);
                return machineUuid;
            }
            return new Model();
        }
    }
}
