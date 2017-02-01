using System.Collections.Generic;

namespace antdlib.models {
    public class PageMemoryStatusModel {
        public IEnumerable<MeminfoModel> Meminfo { get; set; }
        public IEnumerable<FreeModel> Free { get; set; }
    }
}
