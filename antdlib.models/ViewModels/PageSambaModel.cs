using System.Collections.Generic;

namespace antdlib.models {
    public class PageSambaModel {
        public bool SambaIsActive { get; set; }
        public SambaConfigurationModel SambaOptions { get; set; }
        public IEnumerable<SambaConfigurationResourceModel> SambaResources { get; set; }
    }
}
