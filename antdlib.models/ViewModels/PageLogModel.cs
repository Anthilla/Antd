using System.Collections.Generic;

namespace antdlib.models {
    public class PageLogModel {
        public IEnumerable<string> Logs { get; set; }
        public IEnumerable<string> LogReports { get; set; }
    }
}
