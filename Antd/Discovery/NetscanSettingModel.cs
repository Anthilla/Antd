using System.Collections.Generic;

namespace Antd.Discovery {
    public class NetscanSettingModel {
        public IEnumerable<NetscanSettingObject> Objects { get; set; } = new List<NetscanSettingObject>();
    }

    public class NetscanSettingObject {
        public string Id { get; set; }
        public Range Range { get; set; }
        public string Label { get; set; } = string.Empty;
    }

    public class Range {
        public string Start { get; set; }
        public string End { get; set; } = string.Empty;
    }

    public class NewNetscanSettingModel {
        public IEnumerable<NewNetscanSettingObject> Objects { get; set; } = new List<NewNetscanSettingObject>();
    }

    public class NewNetscanSettingObject {
        public string Subnet { get; set; } = "10.1.";
        public string Label { get; set; } = string.Empty;
    }   
}
