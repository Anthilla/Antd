using System.Collections.Generic;

namespace antdlib.models {
    public class NetscanSettingModel {
        public string Subnet { get; set; } = "10.1.";
        public string SubnetLabel { get; set; } = "primary";
        public List<NetscanLabelModel> Values { get; set; }
    }

    public class NetscanLabelModel {
        public string Number { get; set; }
        public string Letter { get; set; }
        public string Label { get; set; }
    }

    public class ScanModel {
        public string Name { get; set; }
        public string Subnet { get; set; }
    }
}
