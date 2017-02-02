using System.Collections.Generic;

namespace antdlib.models {
    public class PageVmModel {
        public bool VmListAny { get; set; }
        public IEnumerable<VirtualMachineInfo> VmList { get; set; }
    }
}
