using System.Collections.Generic;

namespace antdlib.models {
    public class PageAssetSettingModel {
        public string SettingsSubnet { get; set; }
        public string SettingsSubnetLabel { get; set; }
        public IEnumerable<NetscanLabelModel> Settings { get; set; }
    }
}
