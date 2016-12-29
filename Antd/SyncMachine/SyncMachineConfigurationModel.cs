using System.Collections.Generic;

namespace Antd.SyncMachine {
    public class SyncMachineConfigurationModel {
        public bool IsActive { get; set; }
        public List<SyncMachineModel> Machines { get; set; } = new List<SyncMachineModel>();
    }

    public class SyncMachineModel {
        public string Guid { get; set; } = System.Guid.NewGuid().ToString();
        public string MachineAddress { get; set; }
    }
}