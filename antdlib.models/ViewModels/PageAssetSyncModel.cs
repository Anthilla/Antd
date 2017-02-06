using System.Collections.Generic;

namespace antdlib.models {
    public class PageAssetSyncMachineModel {
        public bool IsActive { get; set; }
        public IEnumerable<SyncMachineModel> SyncedMachines { get; set; }
    }
}
