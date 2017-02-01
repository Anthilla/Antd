using System.Collections.Generic;

namespace antdlib.models {
    public class PageInfoModel {
        public string VersionOs { get; set; }
        public IEnumerable<AosReleaseModel> AosInfo { get; set; }
        public UptimeModel Uptime { get; set; }
        public string GentooRelease { get; set; }
        public string LsbRelease { get; set; }
        public string OsRelease { get; set; }
    }
}
