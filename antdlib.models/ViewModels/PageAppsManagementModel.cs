using System.Collections.Generic;
using antdlib.views;

namespace antdlib.models {
    public class PageAppsManagementModel {
        public IEnumerable<ApplicationModel> AppList { get; set; }
    }
}
