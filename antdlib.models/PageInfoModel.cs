using System.Collections.Generic;

namespace antdlib.models {
    public class PageInfoModel {
        public string VersionOs { get; set; }
        public IEnumerable<AosReleaseModel> AosInfo { get; set; }
        public UptimeModel Uptime { get; set; }
        public IEnumerable<string> GentooRelease { get; set; }
        public IEnumerable<string> LsbRelease { get; set; }
        public IEnumerable<string> OsRelease { get; set; }
    }
}
