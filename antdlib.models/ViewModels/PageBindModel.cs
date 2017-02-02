using System.Collections.Generic;

namespace antdlib.models {
    public class PageBindModel {
        public bool BindIsActive { get; set; }
        public BindConfigurationModel BindOptions { get; set; }
        public IEnumerable<BindConfigurationZoneModel> BindZones { get; set; }
    }
}
